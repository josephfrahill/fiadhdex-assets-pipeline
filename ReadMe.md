## remember to store rare / legendary images on a separate server

// Planning this console app      
1. Run option 1: - Source Image fetch
  - pass json path as argument
  - wikimedia api stack
  - saves downloaded images in local folder

2. Run option 2: - handled in python
    - pass local folder of images as arg
    - run background removal, // vision ranking is separate
    - save processed images to new folder
    
3. Run option 3:
    - pass local folder of processed images as arg
    - run icon generation
    - save icons to new folder



    ps1 plan

    
On start, load config-json & display props there like dexName & dexCloudPath.

find executing directory, should be "C:/code/pipeline", + append "pipeline". or get this from configFile

Display list of options to user, including 
"1. Download images for dex animals", 
"2. process images by removing bacgrounds"
"3. Rank processed images and discard both downloaded & processed versions that don't meet certain criteria"
"4. Icon gen"

Step 0; Not priority. It fetches sthe source json from the cloud if not existing locally. 
It would be good to include metadata in that source dex json so we can see when it was last updated
and auto-pull the latets everytime. Need to update cli app to have an option to do only this

Step 1> Mostly done. Needs to pull dexName, dexPathRoot, downlaodedDir from config file. 
We write a maifest in animalId/downloaded dir that is just for the wikimedia api and prvenets re-downloading junk images

Step 2> Mostly done. Needs to pull dexPathRoot, downlaodedDir, processedDir from config file. 
DexPathRoot is the important bit.
Ask the user whether they want to process all animals in this dir or just an individual. Wait for user input

- 0 or All -> It needs to find all the folders in that dir, iterate through them, open the downloadedDir, process each image to the outputPath
-1 or individual -> user can provide animal folder name as arg, can just use that to process individually

Step 3> .net app again most likely. Needs to get dexPathRoot, processedDir from config file.
Ask the user whether they want to process all animals in this dir or just an individual. Wait for user input

- 0 or All -> It needs to find all the folders in that dir, iterate through them, open the downloadedDir, process each image to the outputPath
-1 or individual -> user can provide animal folder name as arg, can just use that to process individually

We might be rate limited here when doing all, so maybe better to just build this stage as individual first

This stage also has to do a lot of clean up. After it ranks each image in the animalId/processed dir, 
it should create a metadata.json in that dir for the animal. The background-removal stage doesn't do this.
It should also delete any images here below a certain ranking, which is easy. It should also update the
wikimeida manifest in animalId/downloaded to prevent re-downloading junk images. 

Step 4> Icon gen. Tbd


python background/remove_backgrounds.py `
    --input $Downloads `
    --output $Processed



----------------------------------------

Dex getter

Manual process to get download for now. Future automation
- Apiclient to ChecklistBank, can you captured api as template + gbif account. Queues download
- Need to check when download is available? In emails?

Once have acquired dex:
- write parser in pipeline app
- need to parse all animal classes we support - mammals, reptiles, birds, amphibians, arachnids
- // fish, insects, mollusks, crustaceans, cnidarians, echinoderms, annelids, flatworms, roundworms, sponges
- should produce 5 files, preferably as json, one for each class. These are the master lists.
- Each file should contain a list of animals with their metadata, including:
  - cotId
  - scientificName
  - commonName
  - distribution

  - Dependency on what the distribution data looks like, we should be able to parse it into a list of countries
  - //, or a list of regions, or a list of continents.
  - Then we run some code to extrapolate each entry from each list where the distribution data matches each country
  - This gives us base country dexes
  - We'll likely need AI review / manual cleanup /addition to each dex




Domain (The biggest group, separating things like plants, animals, and bacteria)
Kingdom (Example: Animalia)
Phylum (Example: Chordata or Arthropoda)
Class (Example: Mammalia or Insecta)
Order (Example: Carnivora—meat eaters)
Family (Example: Felidae—all cats)
Genus (Example: Panthera—roaring big cats)
Species (The final specific animal, like Panthera leo—the lion)
 