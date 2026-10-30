# Calculator

## Projects
- `Calculator/`: .NET MAUI Android app
- `Calculator.Core/`: calculation logic library
- `Calculator.Core.Tests/`: xUnit tests
- `web/`: static website (`index.html`, `style.css`, `calculator.js`)

## Website
- https://calculatoor.click, on a Hetzner server (`deploy@2.31.79.19`, folder `/var/www/calculator`, served by Caddy).
- Deploy ONLY with `.\deploy-web.ps1 "message"`. It commits and pushes `web/` only, then uploads the three site files.
- Never upload `README.md` or `design-preview/`.

## GitHub
- `HovhannesMovsisyan/CalculatorNew`, branch `main`.

## Android
- Package `com.hovhannes.calculator`. Target `net10.0-android` only.
- Release APK: `dotnet publish Calculator\Calculator.csproj -f net10.0-android -c Release -p:AndroidPackageFormat=apk`
- adb: `C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe`
- Emulator AVD: `pixel_7_-_api_34_0`

## Rules
- MVVM with CommunityToolkit.Mvvm; no logic in code-behind.
- All calculation lives in `Calculator.Core`. Run `dotnet test` after logic changes.
- Screenshots go in `design-preview/` (git-ignored).
- Never commit or push unless asked.
- Never touch the server except through `deploy-web.ps1`.
- Never print or store passwords, keystores or private keys. `*.keystore` and `*.jks` stay out of Git.

## Goal
Publish the app on Google Play. This needs:
- a signed `.aab`
- a privacy policy page at `/privacy.html` on the site
- a 14-day closed test with 12+ testers
