param([string]$Message = "")
$ErrorActionPreference = "Stop"

$root   = $PSScriptRoot
$web    = Join-Path $root "web"
$server = "deploy@2.31.79.19"
$remote = "/var/www/calculator/"

if (-not (Test-Path (Join-Path $web "index.html"))) {
    throw "web\index.html not found. Run this script from the solution folder."
}

# Commit and push only the web folder, if a message was given
if ($Message) {
    git -C $root add web
    if ($LASTEXITCODE -ne 0) { throw "git add failed" }

    # Look only at web/, so changes staged elsewhere are neither detected nor committed
    git -C $root diff --cached --quiet -- web
    if ($LASTEXITCODE -eq 1) {
        git -C $root commit -m $Message -- web
        if ($LASTEXITCODE -ne 0) { throw "git commit failed" }
        git -C $root push
        if ($LASTEXITCODE -ne 0) { throw "git push failed" }
    } elseif ($LASTEXITCODE -eq 0) {
        Write-Host "Nothing new to commit in web/."
    } else {
        throw "git diff failed"
    }
}

# Upload site files (README.md is left out on purpose)
$files = Get-ChildItem $web -File | Where-Object { $_.Name -ne "README.md" } | ForEach-Object { $_.FullName }
scp $files "${server}:${remote}"
if ($LASTEXITCODE -ne 0) { throw "scp upload failed (exit code $LASTEXITCODE)" }

# Check the live site
$code = curl.exe -s -o NUL -w "%{http_code}" https://calculatoor.click
Write-Host "Live site returned HTTP $code"
