# TODO Checklist

## 1.0 TODOs

- Bugs
  - Force you to always be in new game since otherwise can't do the black monolith puzzle
  - While gravity trapped, opening a door will make the door unable to open until you leave the room
  - Custom locked doors look broken
  - When door is unlocked, have to leave and re-enter after first entering
  - Fix "Memory stream is not expandable" caused whenever saving after playing for a while

## 2.0 TODOs

- Traps
  - Extend the timer rather than starting a new one
  - Figure out some way to add a black hole trap
- Bugs
  - Fix door unlocking not updating map locked door icon
  - Fix dot sending the message for collecting stuff every time you collect something, have it removed and then collect another thing and get the message again
  - Fix crash that can happen when drawing the wireframe for invisible triles (currently disabled functionality by commenting out FezugInGameRendering.Draw)
  - Think of performance/approach improvements to avoid freezes on location checking (maybe handle ap interactions on another task)
  - Get the hint dialogue in DialogueManager working
- Qualtiy of life
  - Update world map to indicate levels by colour if they have checks remaining in logic, out of logic, locked and completed
  - Replace chest contents with custom triles representing (progression, non-progression, trap, filler)

## 3.0 TODOs

- Doorsanity
  - Lock different subsets of doors in option (vanilla, default, all hubs, all doors)

## 4.0 TODOs

- Bugs
  - PIVOT_THREE needs to be split like with LIGHTHOUSE
- Entrance randomizer
  - Approach 1: Poll AP server on `LevelManager.LevelChanging` and update `LevelManager.LinkedLevels` on `LevelManager.LevelChanged`
  - Approach 2: Poll AP server at the start (or add in slot data) and update all level data through the custom dot message approach
  - Study this awesome project: <https://github.com/admoore0/fez-randomizer/blob/master/src/mod/LevelChanger.cs>

## 5.0 TODOs

- Multiplayer mod integration
  - <https://github.com/FEZModding/FezMultiplayerMod/tree/main>
