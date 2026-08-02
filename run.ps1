param()

Write-Host ""
Write-Host "=== FiadhDex Asset Pipeline ==="
Write-Host ""

Write-Host "0. Initial DB Generation"
Write-Host "1. Generate selected CountryDexBase from DB"
Write-Host "2. Enrich CountryDexBase DB data with OpenAI"
Write-Host "3. Generate full CountryDex from DB with enriched data"
Write-Host "4. Cloud DB Generation"
Write-Host "5. Download source images for icon gen"
Write-Host "6. Remove backgrounds from downloaded images"
Write-Host "7. Rank images using GeminiAI"
Write-Host "8. Generate icons using GenAI"
Write-Host "9. Run full pipeline based on values set in config.json"
Write-Host ""

# cd $$PSScriptRoot

$choice = Read-Host "Select an option"

switch ($choice)
{
    "0"
    {
        dotnet run --project ./src/FiadhDex.AssetsPipeline -- "0"
    }
    
    "1"
    {
        dotnet run --project ./src/FiadhDex.AssetsPipeline -- "1"
    }

    "2"
    {
        dotnet run --project ./src/FiadhDex.AssetsPipeline -- "2"
    }

    "3"
    {
        dotnet run --project ./src/FiadhDex.AssetsPipeline -- "3"
    }

    "4"
    {
        cd ./powershell_scripts/
        .\create-d1-dump.ps1
        cd ../
        # npx wrangler d1 create lifedex     - DB too large
    }

    "5"
    {
        dotnet run --project ./src/FiadhDex.AssetsPipeline -- "4"
    }

    "6"
    {
        cd ./python_scripts/
        .venv\Scripts\activate
        python ./remove_backgrounds.py
        deactivate
        cd ../
    }

    "7"
    {
        python ./vision/rank_images.py
    }

    "8"
    {
        python ./icons/generate_icons.py
    }

    "9"
    {
        dotnet run --project ./src/FiadhDex.AssetsPipeline -- fetch

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