dotnet build --configuration Release

mkdir release

# Generate FezAP.zip
mkdir release/fezap
cp LICENSE release/fezap/LICENSE
cp README.md release/fezap/README.md
cp bin/Debug/FezAP.dll release/fezap/FezAP.dll
cp -r Assets release/fezap/Assets
cp dependencies/Archipelago.MultiClient.Net.dll release/fezap/Archipelago.MultiClient.Net.dll
cp dependencies/Newtonsoft.Json.dll release/fezap/Newtonsoft.Json.dll
zip -r release/FezAP.zip release/fezap
rm -rf release/fezap

# Generate fez.apworld
# TODO: Swap with using the Generate Apworlds after rebasing the AP repo
rm -rf Archipelago/worlds/fez/__pycache__
cp -r Archipelago/worlds/fez release/apworld
zip -r release/fez-apworld.zip release/apworld
mv release/fez-apworld.zip release/fez.apworld
rm -rf release/apworld

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

echo "Tag your commit and double check your zips before uploading them."
