# TODO

## Overview

Cubes and anti-cubes are shuffled into the pool with no specification on which ones come from where. Keys, Owls, Maps and things like 3D vision and the like are optionally shuffled in.
Locations are specific bits, cubes and anti-cubes. Keys, Owls and Maps as well if they are shuffled in.
Filler is TBD.
Traps are "force rotate".
Goal is bound to checking the flags (Finished32 or Finished64).
Deathlink is just when Gomez dies and kills Gomez.

## Archipelago interface

- <https://www.nuget.org/packages/Archipelago.MultiClient.Net>

## Console

- Filter for user message, sent items and received items
- Dot.Say(read_string, true, true)

## Deathlink

```C#
if (PlayerManager.Grounded)
{
    if (thudSound == null)
    {
        // This gives a NullReferenceException if we put this in a constructor so lazily load it here
        thudSound = ContentManagerProvider.Global.Load<SoundEffect>("Sounds/Gomez/CrashLand");
    }
    thudSound.EmitAt(PlayerManager.Position).NoAttenuation = true;
    InputManager.ActiveGamepad.Vibrate(VibrationMotor.RightHigh, 1.0, TimeSpan.FromSeconds(0.5), EasingType.Quadratic);
    InputManager.ActiveGamepad.Vibrate(VibrationMotor.LeftLow, 1.0, TimeSpan.FromSeconds(0.35));
    PlayerManager.Action = FezGame.Structure.ActionType.Dying;
    PlayerManager.Velocity *= Vector3.UnitY;
}
else
{
    PlayerManager.Action = FezGame.Structure.ActionType.FreeFalling;
}
return true;
```

## Items

- GameState.SaveData.CubeShards += 1
- GameState.SaveData.SecretCubes += 1
- GameState.SaveData.CollectedParts += 1
- GameState.SaveData.PiecesOfHeart += 1
- GameState.SaveData.Keys += 1
- GameState.SaveData.CollectedOwls += 1
- GameState.SaveData.Maps.Add(mapName)
- GameState.SaveData.HasFPView = true
- GameState.SaveData.HasStereo3D = true

## Locations

- Gomez.CollectedSplitUpCube
- Gomez.CollectedShard
- Gomez.CollectedAnti
- Gomez.CollectedGlobalAnti
- Gomez.CollectedPieceOfHeart
- Gomez.OpenedTreasure
- Owl.OwlCollected

## Traps

- Rotate trap -> Camera.Rotate(-1) or Camera.Rotate(1)
- Sleep trap -> Gomez.SetCanControl(false) then wait 5 seconds then true

## Goal

- GameState.SaveData.Finished32
- GameState.SaveData.Finished64

## Hints

- (.fezlvl) TrileInstanceActorSettings.SignText
- (.feznpc) NpcMetadata.SoundAction = NpcAction.Talk

## Quality of Life

Photosensitivity mode:

- (.fezlvl) Level.Quantum = false
- (.fezlvl) Level.Rainy = false

## Entrance randomiser

- Gomez.EnteredDoor
- <https://github.com/FEZModding/FEZAP/blob/main/Features/WarpLevel.cs>
- <https://github.com/ArchipelagoMW/Archipelago/blob/main/docs/entrance%20randomization.md>

## Options

Goal: `str: "32 cubes"`

- 32 cubes
- 64 cubes

Shuffle keys: `bool: true`

Shuffle owls: `bool: true`

Shuffle Clock Tower: `bool: false` (avoids having to wait or change system time)

Shuffle Glitch Room: `bool: true` (avoids photo-sensitivity problems)

Deathlink: `bool: false`

Entrance randomiser: `bool: false`

Disable visual pain: `bool: false` (disables quantum and rainy effects to help photosensitivity)
