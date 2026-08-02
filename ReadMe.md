# FiadhDex Asset Pipeline

A .NET-based pipeline for generating FiadhDex's country-level animal encyclopaedia data and species icon assets.

## Overview

The pipeline has two core functions:

1. **Country Dex Generation** — builds a full species dex for a given country (Mammalia, Aves, Amphibia, Reptilia), sourced from Catalogue of Life (COL) and the Global Biodiversity Information Facility (GBIF), parsed into a local SQLite database. AI enrichment of species data (via OpenAI) is a planned/in-progress step in this flow.

2. **Icon Generation** — generates species icon assets for a given dex. Source images are pulled via the Wikimedia API, backgrounds are removed using a Python-based background removal step, and (planned) images are ranked using Gemini Vision before final icon generation via a Flux-based image model.

## Architecture

- **`src/FiadhDex.AssetsPipeline`** — .NET project handling DB generation, dex building, and enrichment. Invoked with numeric stage arguments (e.g. `-- "1"`) or a `fetch` mode for the full pipeline run.
- **`python_scripts/`** — background removal (`remove_backgrounds.py`), run inside a local `.venv`.
- **`vision/`** — image ranking via Gemini AI (`rank_images.py`) *(planned)*.
- **`icons/`** — final icon generation via GenAI/Flux (`generate_icons.py`) *(planned)*.
- **`powershell_scripts/`** — Cloudflare D1 dump/upload helpers (`create-d1-dump.ps1`).
- **`run.ps1`** — interactive entry point exposing all pipeline stages as a menu, plus a full end-to-end run (option 9).

Data flows: **GBIF / COL → SQLite → CountryDexBase → (AI enrichment) → full CountryDex JSON → D1/R2**, with icon generation as a parallel asset track keyed off a generated dex.

## Usage

Run the interactive menu:

```powershell
.\run.ps1
```

### Pipeline stages

| Option | Stage | Description | Status |
|--------|-------|-------------|--------|
| 0 | Initial DB Generation | Parses COL/GBIF source downloads into local SQLite DB | ✅ |
| 1 | Generate CountryDexBase | Builds a base dex for a selected country from the DB | ✅ |
| 2 | Enrich CountryDexBase | Enriches base dex data using OpenAI | 🔜 |
| 3 | Generate full CountryDex | Produces the final enriched CountryDex from DB data | 🔜 |
| 4 | Cloud DB Generation | Dumps/pushes DB data to Cloudflare D1 | 🟡 Partial |
| 5 | Download source images | Fetches source images via Wikimedia API for icon generation | ✅ |
| 6 | Remove backgrounds | Runs Python background removal on downloaded images | ✅ |
| 7 | Rank images (planned) | Ranks images using Gemini Vision | 🔜 |
| 8 | Generate icons (planned) | Generates final icons using a GenAI/Flux model | 🔜 |
| 9 | Full pipeline | Runs fetch → background removal → ranking → icon generation end-to-end, per `config.json` | 🔜 |

### Direct invocation

Stages 0–3 and 5 can also be run directly via the .NET project:

```powershell
dotnet run --project ./src/FiadhDex.AssetsPipeline -- "<stage>"
```

Python stages require the local virtual environment:

```powershell
cd ./python_scripts/
.venv\Scripts\activate
python ./remove_backgrounds.py
deactivate
```

## Configuration

Pipeline behaviour for the full run (option 9) is driven by `config.json` *(location/schema TBD — document once finalised)*.

## Tech Stack

**.NET 10** console application (`src/FiadhDex.AssetsPipeline`)

- `AWSSDK.S3` — asset storage
- `Microsoft.EntityFrameworkCore` (+ `.Sqlite`, `.Relational`, `.Design`, `.Tools`) — SQLite data layer
- `SQLitePCLRaw.lib.e_sqlite3` — SQLite native provider
- `Microsoft.Extensions.Hosting` / `.DependencyInjection` / `.Http` / `.Options` — app host, DI, HTTP client, config binding
- `OpenAI` — species data enrichment

**Python 3.14.3** for image processing stages (background removal, planned vision ranking, planned icon generation)

**Wrangler CLI** for Cloudflare D1 operations

> Package list will keep growing as enrichment, vision ranking, and icon generation stages are built out.

## Requirements

- .NET 10 SDK
- Python 3.14.3 with a local `.venv` for background removal / vision / icon stages
- Cloudflare `wrangler` CLI for D1 operations
- AWS S3 credentials configured for asset storage
- OpenAI API key for enrichment stage