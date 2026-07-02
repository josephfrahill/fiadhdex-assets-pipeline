param()

Write-Host ""
Write-Host "=== Animal Asset Pipeline ==="
Write-Host ""

Write-Host "1. Download source images"
Write-Host "2. Remove backgrounds"
Write-Host "3. Rank images"
Write-Host "4. Generate icons"
Write-Host "5. Run full pipeline"
Write-Host ""

$choice = Read-Host "Select an option"

switch ($choice)
{
    "1"
    {
        dotnet run --project ./src/AnimalAssetsPipeline -- "1"
    }

    "2"
    {
        cd ./pipeline/background-removal
        .venv\Scripts\activate
        python ./remove_backgrounds.py --speciesIdFolder "GL001-domestic-dog"
        deactivate
        cd ../../
    }

    "3"
    {
        python ./vision/rank_images.py
    }

    "4"
    {
        python ./icons/generate_icons.py
    }

    "5"
    {
        dotnet run --project ./src/AnimalAssetsPipeline -- fetch

        if ($LASTEXITCODE -ne 0) { exit }

        python ./background/remove_backgrounds.py

        if ($LASTEXITCODE -ne 0) { exit }

        python ./vision/rank_images.py

        if ($LASTEXITCODE -ne 0) { exit }

        python ./icons/generate_icons.py
    }

    default
    {
        Write-Host "Unknown option."
    }
}