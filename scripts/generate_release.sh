dotnet build ./FezAP.csproj

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

# Generate fez.apworld (TODO: Confirm that this is all that's needed)
cp -r Archipelago/worlds/fez release/apworld
zip -r release/fez-apworld.zip release/apworld
mv release/fez-apworld.zip release/fez.apworld
rm -rf release/apworld

# Copy YAML template
# TODO: Regenerate the templates before copying
cp Archipelago/Players/Templates/Fez.yaml release/Fez.yaml

echo "Tag your commit and double check your zips before uploading them."
