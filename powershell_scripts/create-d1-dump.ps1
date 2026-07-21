param(
    [string]$DatabaseFile = "$PSScriptRoot\..\db\lifedex.db",
    [string]$OutputFile = "$PSScriptRoot\..\db\lifedex-d1.sql"
)

if (-not (Test-Path -Path $DatabaseFile)) {
    throw "Database file not found: $DatabaseFile"
}

Write-Host "Generating D1 SQL dump..."

sqlite3 $DatabaseFile ".dump" |
    Where-Object {
        $_ -notmatch "^\s*BEGIN TRANSACTION;\s*$" -and
        $_ -notmatch "^\s*COMMIT;\s*$"
    } |
    Set-Content $OutputFile -Encoding utf8

if ($LASTEXITCODE -ne 0) {
    throw "SQLite dump failed."
}

Write-Host "Output file created, verifying stripped lines are correct..."

$OriginalCount = (sqlite3 $DatabaseFile ".dump" | Measure-Object -Line).Lines
$OutputCount = (Get-Content $OutputFile | Measure-Object -Line).Lines

$RemovedLines = $OriginalCount - $OutputCount

if ($RemovedLines -ne 2) {
    throw "Expected to remove exactly 2 lines, but removed $RemovedLines."
}

Write-Host "Validated: exactly 2 transaction wrapper lines were removed."
Write-Host "D1 SQL dump created successfully."


