$ErrorActionPreference = "Stop"

$config = Get-Content roslyn.json | ConvertFrom-Json
if (Test-Path roslyn) {
    Remove-Item roslyn -Recurse -Force
}
git init roslyn
git -C roslyn remote add origin $config.url
git -C roslyn fetch --depth 1 origin $config.point
git -C roslyn checkout FETCH_HEAD
Get-ChildItem vb-ls\patches\*.patch | ForEach-Object { git -C roslyn apply $_.FullName }
Remove-Item roslyn\.git -Recurse -Force
