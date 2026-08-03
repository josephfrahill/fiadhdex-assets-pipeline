param(
    [Parameter(Mandatory = $true)]
    [string]$TableName,

    [string]$DatabaseFile = "$PSScriptRoot\..\db\fiadhdex.db",

    [string]$OutputFile
)

if (-not (Test-Path $DatabaseFile)) {
    throw "Database file not found: $DatabaseFile"
}

if (-not $OutputFile) {
    $OutputFile = "$PSScriptRoot\..\db\$TableName.sql"
}

Write-Host "Database: $DatabaseFile"
Write-Host "Table: $TableName"
Write-Host "Output file: $OutputFile"

Write-Host "Validating table..."

$TableExists = sqlite3 $DatabaseFile `
    "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='$TableName';"

if ($LASTEXITCODE -ne 0) {
    throw "Failed to query database."
}

if ([int]$TableExists -eq 0) {
    throw "Table '$TableName' does not exist."
}

Write-Host "Generating D1 SQL dump..."

$OriginalCount = 0
$RemovedCount = 0

sqlite3 $DatabaseFile ".dump $TableName" |
    ForEach-Object {
        $OriginalCount++

        if ($_ -match '^\s*BEGIN TRANSACTION;\s*$' -or
            $_ -match '^\s*COMMIT;\s*$') {
            $RemovedCount++
        }
        else {
            $_
        }
    } |
    Set-Content $OutputFile -Encoding utf8

if ($LASTEXITCODE -ne 0) {
    throw "SQLite dump failed."
}

if ($RemovedCount -ne 2) {
    throw "Expected to remove exactly 2 lines, but removed $RemovedCount."
}

Write-Host "Validated: $OriginalCount input lines, $RemovedCount removed."
Write-Host "D1 SQL dump created successfully."