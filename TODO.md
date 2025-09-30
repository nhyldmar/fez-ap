# TODO Checklist

## 1.0 TODOs

- Handle all the game-breaking TODOs in the code
- Fix any game-breaking bugs
- Bugs
  - While gravity trapped, opening a door will make the door unable to open until you leave the room

## 2.0 TODOs

- Hints
  - Poll AP server at the start and update all the NPC dialogue with hints through the custom dot message approach
- Traps
  - Extend the timer rather than starting a new one
  - Figure out some way to add a black hole trap
- Bugs
  - Fix the early link door unlock edge case for Mausoleum and Sewer Hub
  - Fix crash that can happen when drawing the wireframe for invisible triles
  - Use async sending to avoid freezes on location checking

## 3.0 TODOs

- Entrance randomizer
  - Approach 1: Poll AP server on `LevelManager.LevelChanging` and update `LevelManager.LinkedLevels` on `LevelManager.LevelChanged`
  - Approach 2: Poll AP server at the start (or add in slot data) and update all level data through the custom dot message approach
  - Study this awesome project: <https://github.com/admoore0/fez-randomizer/blob/master/src/mod/LevelChanger.cs>

## 4.0 TODOs

- Multiplayer mod integration
  - <https://github.com/FEZModding/FezMultiplayerMod/tree/main>
