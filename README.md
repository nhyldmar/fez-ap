# Fez AP ![thumbnail](icon.png)

## Overview

This is a [HAT](https://github.com/FEZModding/HAT) mod which adds archipelago multiworld randomizer support to FEZ.
It is heavily based on the wonderful [FEZUG](https://github.com/FEZModding/FEZUG).

## Installing

- Follow the instructions to setup HAT.
- Download the latest `FezAP.zip` from the releases tab and place it in your `Mods` folder.
- If all is well, when running `MONOMODDED_FEZ.exe`, you should see the HAT logo and the FEZAP version in the top left.

## Usage

- Create and open a new save (you can backup your old saves by copying their save files from your local files).
  - If you are reconnecting to an existing AP game, just load into it's save file.
- Press \` and use the `connect` command (if you type `help` followed by any command you can get more info).
  - The command to input is of the form: `connect <server> <port_number> <slot_name> <optional: password>`
  - Example: `connect archipelago.gg 12345 My_Fez`
- Everything should be fine if you see `Connected` in the top left of the screen.
- There are no checks until you go outside with the Fez, so you can do the intro sequence before the countdown.
- There are several other handy commands you can use like `ready`, `say`, `missing`, `received` and many quality of life ones ported over from FEZUG.
- There are several utility commands like `collect`, `send`, `warp`, `tp` and `itemcount`, but they should only be used to get out of softlocks or other problems while this mod is not fully stable.
- If you disconnect, don't open any locked doors (both key and cube count doors).
- It is recommended to look up codes for anti-cubes and heart cubes online while the poptracker doesn't exist.
- Handy tools that may help can be found here: <https://jenna1337.github.io/FezTools/>
- Check the [TODO list](TODO.md) for a list of known bugs and future plans.

## Building

- You need clone this repo and have dotnet installed and configured.
- You need to find the folder where you have Fez installed.
- You need to have [HAT](https://github.com/FEZModding/HAT) installed and confirmed to work.
- Go into `FezAP.csproj` and update `FezModDirectory` with the path to your `Mods/` directory in the Fez install folder.
- Get a copy of `Archipelago.MultiClient.Net.dll` and `Newtonsoft.Json.dll` from their respective nugets or repositories.
- Copy over all the files from `dependencies/.gitignore` from your Fez installation, `HATDependencies/` and the previous step.
- Run `dotnet build` from the root directory to build the mod and copy over all the files into your mod directory.
- Run `MONOMODDED_FEZ.exe` to confirm that HAT sees the mod and just test that things seem to work.
- For apworld development, modify the files in `Archipelago/worlds/fez`.

## Thanks

Big thanks to the Fez Modding community especially Krzyhau and Jenna1337 for all the incredible tooling and help.
If you like this mod, please send all the thanks their way.
Big thanks as well to all the playtesters who helped identify lots of bugs and quality of life problems.
