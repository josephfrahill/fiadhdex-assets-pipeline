from rembg import remove
from PIL import Image
import os
import argparse
import json

parser = argparse.ArgumentParser()
parser.add_argument("--speciesIdFolder", required=True)
# parser.add_argument("--output", required=True)

args = parser.parse_args()
speces_id_dir = args.speciesIdFolder
##output_path = args.output

with open("../../pipeline-config.json", "r") as f:
    config = json.load(f)

pipeline_root = config["pipelineRoot"]
output_dir = config["folders"]["output"]
dex_path_root = config["dexConfig"]["dexPathRoot"]
downloaded_dir = config["folders"]["downloaded"]
processed_dir = config["folders"]["processed"]

executing_root = pipeline_root + "/" + output_dir +  "/" + dex_path_root +  "/" + speces_id_dir
input_path = executing_root +  "/" + downloaded_dir
output_path = executing_root +  "/" + processed_dir
os.makedirs(output_path, exist_ok=True)

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


