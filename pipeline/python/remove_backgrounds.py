from rembg import remove
from PIL import Image
import os

animal_folder = "GL004 - Domestic Cow"
input_path = "C:/code/pipeline/pipeline/src/AnimalAssetPipeline/bin/Debug/net10.0/output/" + animal_folder
output_path = "C:/code/pipeline/background/processed/" + animal_folder
os.makedirs(output_path, exist_ok=True)

image_extensions = (".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tif", ".tiff", ".webp")

for filename in os.listdir(input_path):

    input_full_path = input_path + "/" + filename; #//os.path.join(input_path, filename)    
    output_full_path = output_path + "/" + filename; #os.path.join(output_path, filename)

    if not os.path.isfile(input_full_path):
        continue

    if not filename.lower().endswith(image_extensions):
        continue

    print("Printing: " + input_full_path)

    with Image.open(input_full_path) as img:
        result = remove(img)
        if output_full_path.lower().endswith(".jpg") or output_full_path.lower().endswith(".jpeg"):
            output_full_path = output_full_path.rsplit(".", 1)[0] + ".png"

        result.save(output_full_path)

print("Done!")