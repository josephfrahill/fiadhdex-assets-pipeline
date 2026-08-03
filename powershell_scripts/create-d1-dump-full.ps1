param(
    [string]$DatabaseFile = "$PSScriptRoot\..\db\fiadhdex.db",
    [string]$OutputFile = "$PSScriptRoot\..\db\fiadhdex-d1.sql"
)

if (-not (Test-Path -Path $DatabaseFile)) {
    throw "Database file not found: $DatabaseFile"
}

Write-Host "Generating D1 SQL dump..."

$OriginalCount = 0
$RemovedCount = 0

sqlite3 $DatabaseFile ".dump" |
    ForEach-Object {
        $OriginalCount++

        if ($_ -match "^\s*BEGIN TRANSACTION;\s*$" -or
            $_ -match "^\s*COMMIT;\s*$") {
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