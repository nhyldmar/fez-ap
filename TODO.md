# TODO Checklist

## 1.0 TODOs

- Fix any game-breaking bugs
- Bugs
  - While gravity trapped, opening a door will make the door unable to open until you leave the room
  - Sleep trap doesn't keep you in sleep animation
  - Custom locked doors look broken
- Quality of life
  - Visual indication of sleep/gravity trap duration

## 2.0 TODOs

- Hints
  - Poll AP server at the start and update all the NPC dialogue with hints through the custom dot message approach
- Traps
  - Extend the timer rather than starting a new one
  - Figure out some way to add a black hole trap
- Bugs
  - Fix door unlocking not updating map locked door icon
  - Fix dot sending the message for collecting stuff every time you collect something, have it removed and then collect another thing and get the message again
  - Fix crash that can happen when drawing the wireframe for invisible triles (currently disabled functionality by commenting out FezugInGameRendering.Draw)
  - Think of performance/approach improvements to avoid freezes on location checking (maybe handle ap interactions on another task)

## 3.0 TODOs

- Entrance randomizer
  - Approach 1: Poll AP server on `LevelManager.LevelChanging` and update `LevelManager.LinkedLevels` on `LevelManager.LevelChanged`
  - Approach 2: Poll AP server at the start (or add in slot data) and update all level data through the custom dot message approach
  - Study this awesome project: <https://github.com/admoore0/fez-randomizer/blob/master/src/mod/LevelChanger.cs>

## 4.0 TODOs

- Multiplayer mod integration
  - <https://github.com/FEZModding/FezMultiplayerMod/tree/main>
