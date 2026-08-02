import json
import os
from pathlib import Path
from PIL import Image
from rembg import remove

def process_files_in_directory(animal_folder: Path, downloaded_dir: str, processed_dir: str):
    input_path = animal_folder / downloaded_dir
    output_path = animal_folder / processed_dir
    
    output_path.mkdir(parents=True, exist_ok=True)
    
    image_extensions = (".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tif", ".tiff", ".webp")
    
    print(f"Starting background removal using images in: {input_path}")
    
    # Use enumerate() for clean counting without a manual counter variable
    for counter, filename in enumerate(os.listdir(input_path), start=1):
        if not filename.lower().endswith(image_extensions):
            continue
            
        input_full_path = input_path / filename
        if not input_full_path.is_file():
            continue

        # Force output to always be a PNG (.webp might also be good)
        output_file = f"{Path(filename).stem}.png"
        output_full_path = output_path / output_file

        if output_full_path.is_file():
            print(f"Skipping: {filename}")
            continue

        print(f"{counter}: Processing {filename}...")

        try:
            with Image.open(input_full_path) as img:
                result = remove(img)
                result.save(output_full_path)
        except Exception as e:
            print(f"Error processing {filename}: {e}")

    print("Done!")

def main():
    print("Starting background-removal process..")

    # Load configuration
    with open("../pipeline-config.json", "r") as f:
        config = json.load(f)

    # Use Path objects instead of string concatenation
    solution_root = Path(config["solutionRoot"])
    output_dir = config["folders"]["output"]
    assets_folder = config["folders"]["assets"]
    working_dex_output_folder = config["assetsConfig"]["workingDexOutputFolder"]

    downloaded_dir = config["folders"]["downloaded"]
    processed_dir = config["folders"]["processed"]

    # Clean path construction using the / operator
    executing_root = solution_root / output_dir / assets_folder / working_dex_output_folder
    print(f"Executing root is: {executing_root}")

    if not executing_root.exists():
        print(f"Error: The directory {executing_root} does not exist.")
        return

    # Loop through directories and process
    for filename in os.listdir(executing_root):    
        species_folder = executing_root / filename
        if species_folder.is_dir():
            print(f"Looking in species folder: {species_folder}")
            process_files_in_directory(species_folder, downloaded_dir, processed_dir)

if __name__ == "__main__":
    main()
