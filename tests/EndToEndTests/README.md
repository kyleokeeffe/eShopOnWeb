# EndToEndTests

This project contains browser-based end-to-end tests using Playwright for .NET.

## First-time setup

1. Build the project to generate the Playwright install script:

   dotnet build tests/EndToEndTests/EndToEndTests.csproj

2. Install browser binaries:

   pwsh tests/EndToEndTests/bin/Debug/net10.0/playwright.ps1 install

## Run tests

dotnet test tests/EndToEndTests/EndToEndTests.csproj

The smoke test starts the Web app automatically using its normal launch profile (<https://localhost:5001>), loads the home page, and validates seeded catalog data is rendered.
