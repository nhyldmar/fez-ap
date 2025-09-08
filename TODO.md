# TODO

## 1.0 Features

- Location handling
  - Gomez.CollectedSplitUpCube
  - Gomez.CollectedShard
  - Gomez.CollectedAnti
  - Gomez.CollectedGlobalAnti
  - Gomez.CollectedPieceOfHeart
  - Gomez.OpenedTreasure
  - Owl.OwlCollected
- Goal handling
  - GameState.SaveData.Finished32
  - GameState.SaveData.Finished64
- Photosensitivity mode
  - quantum.fezlvl.json -> Quantum = false
  - big_owl.fezlvl.json -> Rainy = false (repeat for grave_cabin, grave_ghost, grave_lesser_gate, grave_treasure_a, graveyard_a, graveyard_gate, industrial_city, mausoleum, owl, skull_b, skull)
  - figure out if you can just change the lightning flashes instead of disabling Rainy value

## 2.0 Features

- Hints
  - gametext.feztxt.json (only english)
- Locations
  - Make each collectible unique and add the corresponding logic
  - <https://github.com/FEZModding/FezMultiplayerMod/blob/main/FezMultiplayerMod/MultiplayerMod/OpenTreasureListener.cs>
  - <https://github.com/FEZModding/FezMultiplayerMod/blob/main/FezMultiplayerMod/MultiplayerMod/SaveDataObserver.cs>

## 3.0 Features

- Entrance randomizer
  - Gomez.EnteredDoor
  - <https://github.com/FEZModding/FEZAP/blob/main/Features/WarpLevel.cs>
  - <https://github.com/ArchipelagoMW/Archipelago/blob/main/docs/entrance%20randomization.md>

## 4.0 Features

- Multiplayer mod integration
  - <https://github.com/FEZModding/FezMultiplayerMod/tree/main>
