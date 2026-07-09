#!/usr/bin/env dotnet
// Run this with:
// dotnet ./BuildTestFormat.cs

using System.Diagnostics;
using System.Text.RegularExpressions;

string solutionRoot = FindSolutionRoot();
string solutionFile = Path.Combine(solutionRoot, "eShopOnWeb.slnx");
string testsRoot = Path.Combine(solutionRoot, "tests");

var overall = Stopwatch.StartNew();
try
{
    Console.WriteLine("Restoring...");
    CommandResult restoreResult = await RunAsync("dotnet", $"restore \"{solutionFile}\"", solutionRoot);
    if (restoreResult.ExitCode != 0)
    {
        WriteFailureDetails(restoreResult);
        return restoreResult.ExitCode;
    }

    Console.WriteLine($"Restoring... completed in {FormatElapsed(restoreResult.Elapsed)}");

    Console.WriteLine("Building...");
    CommandResult buildResult = await RunAsync("dotnet", $"build \"{solutionFile}\" --no-restore", solutionRoot);
    if (buildResult.ExitCode != 0)
    {
        WriteFailureDetails(buildResult);
        return buildResult.ExitCode;
    }

    Console.WriteLine($"Building... completed in {FormatElapsed(buildResult.Elapsed)}");

    Console.WriteLine("Executing tests...");
    foreach (string projectPath in GetTestProjects(testsRoot))
    {
        string projectName = GetProjectName(projectPath);
        string projectDirectory = Path.GetDirectoryName(projectPath)!;

        CommandResult testResult = await RunAsync("dotnet", $"test --no-build --logger \"console;verbosity=minimal\" \"{projectPath}\"", projectDirectory);

        if (testResult.ExitCode != 0)
        {
            if (TryParseTestSummary(testResult.StdOut + Environment.NewLine + testResult.StdErr, out int passed, out int failed, out int total))
            {
                Console.WriteLine($"  {projectName}... completed in {FormatElapsed(testResult.Elapsed)}: ❌ {passed}/{total} passing, {failed} failed");
            }
            else
            {
                Console.WriteLine($"  {projectName}... completed in {FormatElapsed(testResult.Elapsed)}: ❌ failed");
            }

            WriteFailureDetails(testResult);
            return testResult.ExitCode;
        }

        if (TryParseTestSummary(testResult.StdOut + Environment.NewLine + testResult.StdErr, out int passedTests, out _, out int totalTests))
        {
            Console.WriteLine($"  {projectName}... completed in {FormatElapsed(testResult.Elapsed)}: ✅ {passedTests}/{totalTests} passing");
        }
        else
        {
            Console.WriteLine($"  {projectName}... completed in {FormatElapsed(testResult.Elapsed)}: ✅ passed");
        }
    }

    CommandResult formatResult = await RunAsync("dotnet", $"format \"{solutionFile}\" --verify-no-changes", solutionRoot);
    if (formatResult.ExitCode != 0)
    {
        WriteFailureDetails(formatResult);
        return formatResult.ExitCode;
    }

    return 0;
}
finally
{
    overall.Stop();
    Console.WriteLine($"Completed in {FormatElapsed(overall.Elapsed)}");
}

static async Task<CommandResult> RunAsync(string file, string args, string workingDirectory)
{
    var start = Stopwatch.StartNew();
    using var process = Process.Start(
      new ProcessStartInfo(file, args)
      {
          UseShellExecute = false,
          RedirectStandardOutput = true,
          RedirectStandardError = true,
          WorkingDirectory = workingDirectory
      })!;

    Task<string> stdoutTask = process.StandardOutput.ReadToEndAsync();
    Task<string> stderrTask = process.StandardError.ReadToEndAsync();

    await process.WaitForExitAsync();

    string stdout = await stdoutTask;
    string stderr = await stderrTask;

    start.Stop();
    return new CommandResult(process.ExitCode, start.Elapsed, stdout, stderr);
}

static string FormatElapsed(TimeSpan elapsed)
    => elapsed.TotalMinutes >= 1
        ? $"{(int)elapsed.TotalMinutes}m {elapsed.Seconds}s"
        : $"{elapsed.Seconds}s";

static bool TryParseTestSummary(string output, out int passed, out int failed, out int total)
{
    Match match = Regex.Match(
        output,
        @"Failed:\s*(\d+),\s*Passed:\s*(\d+),\s*Skipped:\s*(\d+),\s*Total:\s*(\d+)",
        RegexOptions.Multiline);

    if (!match.Success)
    {
        passed = 0;
        failed = 0;
        total = 0;
        return false;
    }

    failed = int.Parse(match.Groups[1].Value);
    passed = int.Parse(match.Groups[2].Value);
    total = int.Parse(match.Groups[4].Value);
    return true;
}

static void WriteFailureDetails(CommandResult result)
{
    Console.WriteLine();
    Console.WriteLine("Test failure details:");

    string combinedOutput = string.Join(Environment.NewLine, new[] { result.StdOut, result.StdErr }
        .Where(text => !string.IsNullOrWhiteSpace(text)));

    Console.WriteLine(string.IsNullOrWhiteSpace(combinedOutput)
        ? "  (no output captured)"
        : string.Join(Environment.NewLine, combinedOutput.Split(Environment.NewLine, StringSplitOptions.None)
            .Select(line => $"  {line}")));
}

static string GetProjectName(string projectPath)
    => Path.GetFileNameWithoutExtension(projectPath);

static IEnumerable<string> GetTestProjects(string testsRoot)
    => Directory.EnumerateFiles(testsRoot, "*Tests.csproj", SearchOption.AllDirectories)
        .OrderBy(path => path, StringComparer.OrdinalIgnoreCase);

static string FindSolutionRoot()
{
    string? current = Environment.CurrentDirectory;
    while (!string.IsNullOrWhiteSpace(current))
    {
        if (File.Exists(Path.Combine(current, "eShopOnWeb.slnx")))
        {
            return current;
        }

        current = Directory.GetParent(current)?.FullName;
    }

    throw new DirectoryNotFoundException("Could not locate repository root containing eShopOnWeb.slnx.");
}

record CommandResult(int ExitCode, TimeSpan Elapsed, string StdOut, string StdErr);
