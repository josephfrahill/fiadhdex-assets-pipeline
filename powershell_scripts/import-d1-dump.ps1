param(
    [Parameter(Mandatory = $true)]
    [string]$SqlFile
)

if (-not (Test-Path $SqlFile)) {
    throw "SQL file not found: $SqlFile"
}

$SqlFile = (Resolve-Path $SqlFile).Path

$AccountId = $env:CLOUDFLARE_ACCOUNT_ID
$DatabaseId = $env:CLOUDFLARE_D1_DATABASE_ID
$ApiToken = $env:CLOUDFLARE_D1_API_TOKEN

Write-Host "SQL file: $SqlFile"
Write-Host "File size: $((Get-Item $SqlFile).Length)"

if (-not $AccountId) {
    throw "CLOUDFLARE_ACCOUNT_ID is not set."
}

if (-not $DatabaseId) {
    throw "CLOUDFLARE_D1_DATABASE_ID is not set."
}

if (-not $ApiToken) {
    throw "CLOUDFLARE_API_TOKEN is not set."
}

$ImportUrl = "https://api.cloudflare.com/client/v4/accounts/$AccountId/d1/database/$DatabaseId/import"

$Headers = @{
    Authorization = "Bearer $ApiToken"
    "Content-Type" = "application/json"
}

Write-Host "Calculating MD5 hash..."

$Hash = (Get-FileHash $SqlFile -Algorithm MD5).Hash.ToLower()

Write-Host "Initialising D1 import..."

$InitBody = @{
    action = "init"
    etag = $Hash
} | ConvertTo-Json

$InitResponse = Invoke-RestMethod `
    -Method Post `
    -Uri $ImportUrl `
    -Headers $Headers `
    -Body $InitBody

# Write-Host ($InitResponse | ConvertTo-Json -Depth 10)

if (-not $InitResponse.success) {
    throw "Failed to initialise import."
}

$UploadUrl = $InitResponse.result.upload_url
$Filename = $InitResponse.result.filename

if (-not $UploadUrl) {
    throw "Cloudflare did not return an upload URL."
}

if (-not $Filename) {
    throw "Cloudflare did not return a filename."
}

Write-Host "Upload URL: $UploadUrl"
Write-Host "Filename: $Filename"
Write-Host "Uploading SQL file..."

Invoke-RestMethod `
    -Method Put `
    -Uri $UploadUrl `
    -InFile $SqlFile `
    -ContentType "application/octet-stream"

Write-Host "Starting ingestion..."

$IngestBody = @{
    action = "ingest"
    etag = $Hash
    filename = $Filename
} | ConvertTo-Json

$IngestResponse = Invoke-RestMethod `
    -Method Post `
    -Uri $ImportUrl `
    -Headers $Headers `
    -Body $IngestBody

if (-not $IngestResponse.success) {
    throw "Failed to start import."
}

# Write-Host ($IngestResponse | ConvertTo-Json -Depth 10)

$Bookmark = $IngestResponse.result.at_bookmark

Write-Host "Polling status..."

do {
    Start-Sleep -Seconds 5

    $PollBody = @{
        action = "poll"
        current_bookmark = $Bookmark
    } | ConvertTo-Json

    $PollResponse = Invoke-RestMethod `
        -Method Post `
        -Uri $ImportUrl `
        -Headers $Headers `
        -Body $PollBody

    $Status = $PollResponse.result.status

    Write-Host "Status: $Status"

} while ($Status -ne "complete" -and $Status -ne "error")

if ($Status -eq "error") {
    Write-Host "Full response:"
    Write-Host ($PollResponse | ConvertTo-Json -Depth 10)

    throw "Import failed."
}

Write-Host "Import completed successfully."