[![license](https://img.shields.io/badge/License-MIT-555555?style=flat&labelColor=333&color=yellow)](https://opensource.org/licenses/MIT) [![discord](https://img.shields.io/badge/-trotlinebeercan-555555?style=flat&labelColor=333&logoColor=white&logo=discord)](https://github.com/trotlinebeercan) ![github](https://img.shields.io/badge/-trotlinebeercan-555555?style=flat&labelColor=333&logoColor=white&logo=github) [![kofi](https://img.shields.io/badge/Ko--fi-555555?style=flat&logo=ko-fi&logoColor=white&labelColor=333&link=https%253A%252F%252Fko-fi.com%25%2FZ8Z6T7OWX)](https://ko-fi.com/trotlinebeercan)

# Little-Known Galaxy - Instant Subliminal Learning (LKGS Mod)

This repo holds an all-in-one mod for [Little-Known Galaxy](https://linktr.ee/littleknowngalaxy).

Current features include:
- Increase time length of the day (slow the clock down)
- Pause the clock for the current location
- Infinite Stamina
- Infinite Health
- Move around twice as fast
- Tools Instantly Fully Charge
- Open the Workbench and Kitchen for crafting from anywhere
- Open the in-game development debug menu for l33t h4x1ng

## Installation

### Step-by-step Instructions
- Download [BepInEx 5.4.22.0](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.22) for your platform.
    - If you do not know, 99% chance it will be the [BepInEx_x64_5.4.22.0.zip](https://github.com/BepInEx/BepInEx/releases/download/v5.4.22/BepInEx_x64_5.4.22.0.zip) file.
- Extract the ZIP archive in the installation directory for Little-Known Galaxy, next to `Little-Known Galaxy.exe`
    - If this worked, you should see a new folder named `BepInEx` and three new files, `winhttp.dll`, `doorstop_config.ini` and `.doorstop_version`.
- Download the latest [LKGS release](https://github.com/trotlinebeercan/LKGS/releases).
- Extract this ZIP archive into `BepInEx/plugins`.
    - If this worked, you should see a new folder named `LKGS` in the plugins folder.
- Run the game once! You can close out of it at the main menu. This is to allow the mod to create the configuration files.
- Here, you need to enable the patches you would like to have active. There are two ways:
    - Download [BepInEx Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager) and extract the archive into the `BepInEx/plugins` folder.
        - Hit F1 when the game is loaded and you should see the Configuration Manager UI. Click on LKGS and have fun.
    - Manually edit `BepInEx/config/com.trotlinebeercan.lkgs.cfg` with your favorite text editor. No shame in using Notepad!
- Congrats! You should have LKGS installed and running.

### BepInEx

This is a [BepInEx 5](https://github.com/BepInEx/BepInEx) mod and as such requires that you have BepInEx 5 installed and working properly. If you've not done that yet, please go to their [documentation](https://docs.bepinex.dev/articles/user_guide/installation/index.html) on how to get it installed. 

You want the Stable (5.x - 5.4.21 LTS at the time of writing) **MONO** build when you're presented with choices.

If you already know how to get BepInEx working, head over to their [Releases](https://github.com/BepInEx/BepInEx/releases) to grab the latest copy.

### Mod Files

Head over to [Releases](https://github.com/trotlinebeercan/LKGS/releases), download the latest, and extract the archive into your BepInEx plugins folder in the game root.

If everything is done correctly, you should see the mod installed under `SteamLibrary\steamapps\common\Little-Known Galaxy\BepInEx\plugins\LKGS`.

## Recommended

LKGS supports configuration through [BepInEx Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager). I'd recommend it for easier configuration of this mod.

### Building from Source

Once you have a working BepInEx 5 Mono installation running successfully, you can build this repo and the plugins will be automatically staged into your game folder.

You should find the mod installed under `SteamLibrary\steamapps\common\Little-Known Galaxy\BepInEx\plugins\LKGS`.

### License & Attribution

Follow the terms of the [MIT License](https://opensource.org/licenses/MIT) and have fun!

<sub><sup>this readme was greatly inspired by [Maverik](https://github.com/Maverik)</sup></sub>
