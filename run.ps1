param()

Write-Host ""
Write-Host "=== Animal Asset Pipeline ==="
Write-Host ""

Write-Host "0. DB Generation"
Write-Host "1. Generate CountryDexBase from DB"
Write-Host "2. Enrich CountryDexBase DB data with OpenAI"
Write-Host "3. Generate full CountryDex from DB with enriched data"
Write-Host "4. Download source images for icon gen"
Write-Host "5. Remove backgrounds from downloaded images"
Write-Host "6. Rank images using VisionAI"
Write-Host "7. Generate icons using GenAI"
Write-Host "8. Run full pipeline"
Write-Host ""

$choice = Read-Host "Select an option"

switch ($choice)
{
    "0"
    {
        dotnet run --project ./src/AnimalAssetsPipeline -- "0"
    }

    "1"
    {
        dotnet run --project ./src/AnimalAssetsPipeline -- "1"
    }

    "2"
    {
        dotnet run --project ./src/AnimalAssetsPipeline -- "2"
    }

    "3"
    {
        dotnet run --project ./src/AnimalAssetsPipeline -- "3"
    }

    "4"
    {
        dotnet run --project ./src/AnimalAssetsPipeline -- "4"
    }

    "5"
    {
        cd ./python_scripts/
        .venv\Scripts\activate
        python ./remove_backgrounds.py
        deactivate
        cd ../
    }

    "6"
    {
        python ./vision/rank_images.py
    }

    "7"
    {
        python ./icons/generate_icons.py
    }

    "8"
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