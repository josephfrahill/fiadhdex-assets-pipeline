from rembg import remove
from PIL import Image
import os
import argparse
import json


#parser = argparse.ArgumentParser()
# parser.add_argument("--speciesIdFolder", required=True)
# parser.add_argument("--output", required=True)

#args = parser.parse_args()
#speces_id_dir = args.speciesIdFolder
##output_path = args.output

with open("../pipeline-config.json", "r") as f:
    config = json.load(f)

solution_root = config["solutionRoot"]
output_dir = config["folders"]["output"]
assets_folder = config["folders"]["assets"]
working_dex_output_folder = config["assetsConfig"]["workingDexOutputFolder"]
species_id_output_folder = config["assetsConfig"]["speciesIdFolder"]

downloaded_dir = config["folders"]["downloaded"]
processed_dir = config["folders"]["processed"]

#C:\code\lifedex\pipeline\          output\         assets              \global-dex
executing_root = solution_root + "/" + output_dir +  "/" + assets_folder + "/" + working_dex_output_folder + "/" + species_id_output_folder
input_path = executing_root +  "/" + downloaded_dir
output_path = executing_root +  "/" + processed_dir
os.makedirs(output_path, exist_ok=True)
# is webp better to than png?
image_extensions = (".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tif", ".tiff", ".webp")

counter = 1
print("Starting background removal using images in: " + input_path)
for filename in os.listdir(input_path):
    print(f"{counter}: " + filename)
    counter += 1
    input_full_path = input_path + "/" + filename; #//os.path.join(input_path, filename)    

    base_name = os.path.splitext(filename)[0]
    output_file = base_name + ".png"
    output_full_path = os.path.join(output_path, output_file)

    if not os.path.isfile(input_full_path):
        continue

    if not filename.lower().endswith(image_extensions):
        continue

    if os.path.isfile(output_full_path):
        print("Skipping: " + filename)
        continue

    print("Processing " + filename + "...")

    with Image.open(input_full_path) as img:
        result = remove(img)
        if output_full_path.lower().endswith(".jpg") or output_full_path.lower().endswith(".jpeg"):
            output_full_path = output_full_path.rsplit(".", 1)[0] + ".png"

        result.save(output_full_path)

print("Done!")


