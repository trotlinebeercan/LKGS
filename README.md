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
- Apply a "Combat Difficulty" setting to lower or increase the number of enemies that can spawn on a planet
- Open the Workbench and Kitchen for crafting from anywhere
- Allow custom World Zoom settings (automatically applied, just open the Settings menu and adjust it)
- Open the in-game development debug menu for l33t h4x1ng

## Installation

### BepInEx

This is a [BepInEx 5](https://github.com/BepInEx/BepInEx) mod and as such requires that you have BepInEx 5 installed and working properly.
If you already know how to get BepInEx working, head over to their [Releases](https://github.com/BepInEx/BepInEx/releases) to grab the latest copy.

To install BepInEx, you should:
- Use the [Little-Known Galaxy BepInEx 5 Pack](https://www.nexusmods.com/littleknowngalaxy/mods/3) mod and extract the archive into the game directory.

**or, to install manually:**

- Download [BepInEx 5.4.22.0](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.22) for your platform.
    - If you do not know, 99% chance it will be the [BepInEx_x64_5.4.22.0.zip](https://github.com/BepInEx/BepInEx/releases/download/v5.4.22/BepInEx_x64_5.4.22.0.zip) file.
- Extract the ZIP archive in the installation directory for Little-Known Galaxy, next to `Little-Known Galaxy.exe`
    - If this worked, you should see a new folder named `BepInEx` and three new files, `winhttp.dll`, `doorstop_config.ini` and `.doorstop_version`.

Be absolutely sure you extract the the contents where the game's exe is. If "winhttp.dll" and "doorstop_config.ini" aren't in the same folder as "Little-Known Galaxy.exe", it won't work.

### Mod Files

- Download the latest [LKGS release](https://github.com/trotlinebeercan/LKGS/releases).
- Extract this ZIP archive into `SteamLibrary\steamapps\common\Little-Known Galaxy\BepInEx\plugins`.
    - If everything is done correctly, you should see the mod installed under `SteamLibrary\steamapps\common\Little-Known Galaxy\BepInEx\plugins\LKGS`.
- Run the game once! You can close out of it at the main menu. This is to allow the mod to create the configuration files.

- Here, you need to enable the patches you would like to have active. There are two ways:
    - NOTE: If you installed the [Little-Known Galaxy BepInEx 5 Pack](https://www.nexusmods.com/littleknowngalaxy/mods/3), you do not need to download it. The pack has it preinstalled.
    - Download [BepInEx Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager) and extract the archive into the `BepInEx/plugins` folder.
        - Hit F1 when the game is loaded and you should see the Configuration Manager UI. Click on LKGS and have fun.
    - Manually edit `BepInEx/config/com.trotlinebeercan.lkgs.cfg` with your favorite text editor. No shame in using Notepad!
- Congrats! You should have LKGS installed and running.

Head over to [Releases](https://github.com/trotlinebeercan/LKGS/releases), download the latest, and extract the archive into your BepInEx plugins folder in the game root.

If everything is done correctly, you should see the mod installed under `SteamLibrary\steamapps\common\Little-Known Galaxy\BepInEx\plugins\LKGS`.

### Running BepInEx 5 on SteamDeck

- Enter Desktop Mode (press the Steam key, go down to Power, and select Switch to Desktop)
- Download everything you need using the instructions above.
- Back in Steam, navigate to the game page for Little-Known Galaxy in your Library.
- Click on the gear icon on the right side, then Manage, then Browse Local Files.
- Extract the archives downloaded previously here in this folder.
- Back in Steam, click the gear icon again, and select Properties.
- Under General, scroll down to Launch Options and input
`WINEDLLOVERRIDES="winhttp=n,b" %command%`
- Exit Properties, and run the game once.

These instructions were copypasta'd from [this Steam Community post](https://steamcommunity.com/sharedfiles/filedetails/?id=3122526585). It is for a different game, but it'll work.

## Recommended

LKGS supports configuration through [BepInEx Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager). I'd recommend it for easier configuration of this mod.

### Building from Source

Once you have a working BepInEx 5 Mono installation running successfully, you can build this repo and the plugins will be automatically staged into your game folder.

You should find the mod installed under `SteamLibrary\steamapps\common\Little-Known Galaxy\BepInEx\plugins\LKGS`.

### License & Attribution

Follow the terms of the [MIT License](https://opensource.org/licenses/MIT) and have fun!

<sub><sup>this readme was greatly inspired by [Maverik](https://github.com/Maverik)</sup></sub>
