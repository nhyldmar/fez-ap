dotnet build --configuration Release

mkdir release

# Generate FezAP.zip
mkdir release/fezap
cp LICENSE release/fezap/LICENSE
cp README.md release/fezap/README.md
cp Metadata.xml release/fezap/Metadata.xml
cp bin/Release/FezAP.dll release/fezap/FezAP.dll
cp -r Assets release/fezap/Assets
cp dependencies/Archipelago.MultiClient.Net.dll release/fezap/Archipelago.MultiClient.Net.dll
cp dependencies/Newtonsoft.Json.dll release/fezap/Newtonsoft.Json.dll
# TODO: Confirm this zips it such that it's zipping the files into the zip not the folder
zip -r release/FezAP.zip release/fezap
rm -rf release/fezap

# Generate fez.apworld
# TODO: Swap with using the Generate Apworlds after rebasing the AP repo
# py Launcher.py "Build APWorlds" -- "Fez"
rm -rf Archipelago/worlds/fez/__pycache__
cp -r Archipelago/worlds/fez release/fez
zip -r release/fez.zip release/fez
mv release/fez.zip release/fez.apworld
rm -rf release/fez

# Copy YAML template
cd Archipelago
py -c '
from Options import generate_yaml_templates
import Utils

target = Utils.user_path("Players", "Templates")
generate_yaml_templates(target, False)
'
cd -
cp Archipelago/Players/Templates/Fez.yaml release/Fez.yaml

echo "Make sure Fezap.cs and Metadata.xml have the correct version, tag your commit and double check your zips before uploading them."
