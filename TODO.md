# TODO Checklist

## 1.0 Features

- Handle all the TODOs in the code

## 2.0 Features

- Hints
  - Poll AP server at the start and update all the NPC dialogue with hints through the custom dot message approach

## 3.0 Features

- Entrance randomizer
  - Approach 1: Poll AP server on `LevelManager.LevelChanging` and update `LevelManager.LinkedLevels` on `LevelManager.LevelChanged`
  - Approach 2: Poll AP server at the start (or add in slot data) and update all level data through the custom dot message approach
  - Study this awesome project: <https://github.com/admoore0/fez-randomizer/blob/master/src/mod/LevelChanger.cs>

## 4.0 Features

- Multiplayer mod integration
  - <https://github.com/FEZModding/FezMultiplayerMod/tree/main>
