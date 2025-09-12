"""A handy script for generating all the location mapping.

Call from the FezRepacker top directory after unpacking everything.
"""

import json
from typing import Dict, List
from pathlib import Path

trileset_filepaths = list(Path(".").rglob("*.fezts.json"))
trileset_filepaths.remove(Path("Unpacked/trile sets/loading.fezts.json"))  # No idea why this exists and has LIB_INT
level_filepaths = list(Path(".").rglob("*.fezlvl.json"))

def get_idx_for_type(data: Dict, type: str):
    for trile in data["Triles"]:
        if data["Triles"][trile]["Type"] == type:
            return int(trile)
    return None

trileset_data: Dict[str, Dict[str, int|None]] = {}
for filename in trileset_filepaths:
    with open(filename) as file:
        data = json.load(file)
        trileset_data[data['Name']] = {
            "Cube Bit": get_idx_for_type(data, "GoldenCube"),
            # "Chest": get_idx_for_type(data, "TreasureChest"),
            "Cube": get_idx_for_type(data, "CubeShard"),
            "Anti-Cube": get_idx_for_type(data, "SecretCube"),
            "Owl": get_idx_for_type(data, "Owl"),
            "Heart Cube": get_idx_for_type(data, "PieceOfHeart")
        }

to_write = ""

def info_format(type_name: str, level_name: str, emplacement: List[int]) -> str:
    formatted_level_name = level_name.replace("_", " ").title()
    location_name = f"{formatted_level_name} {type_name}"
    # Count is not always reliable, can't be bothered since can just manually edit
    count = to_write.count(f"\"{location_name}") + 1
    location_name += f" {count}"
    return f"new(\"{location_name}\", \"{level_name}\", {emplacement}),\n"

for filename in level_filepaths:
    with open(filename) as file:
        data = json.load(file)
        level_name = data["Name"]
        trileset_name = data["TrileSetName"]
        trileset = trileset_data[trileset_name]
        for trile in data["Triles"]:
            for type_name, id in trileset.items():
                if trile["Id"] == id:
                    emplacement = trile["Emplacement"]
                    to_write += info_format(type_name, level_name, emplacement)

with open("location_data.txt", "w") as file:
    file.write(to_write)
