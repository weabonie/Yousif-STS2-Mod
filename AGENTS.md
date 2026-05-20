# AGENTS.md

## Project Shape
- This is a Slay the Spire 2 Godot/.NET mod named `YousifMod`.
- C# gameplay code lives under `YousifModCode/`; Godot assets and localization live under `YousifMod/`.
- `MainFile.cs` is the mod initializer. It defines `ModId = "YousifMod"`, `ResPath = "res://YousifMod"`, creates the logger, and runs `Harmony.PatchAll()`.
- `YousifMod.json` is the mod manifest. It declares a gameplay mod with both DLL and PCK output and a `BaseLib` dependency.

## Key Code Areas
- Cards are in `YousifModCode/Cards/`. Existing examples are `DevourCard.cs`, `GluttonCard.cs`, and the image-path base class `YousifModCard.cs`.
- Powers are in `YousifModCode/Powers/`. `DevourPower` heals after unblocked powered attack damage; `DevourNoBlockPower` suppresses block from cards for the combat.
- Character setup is in `YousifModCode/Character/`. `YousifCharacter.cs` defines HP, starter deck, starter relic, and pool models.
- Relic image conventions are centralized in `YousifModCode/Relics/YousifModRelic.cs`.
- Asset path helpers are in `YousifModCode/Extensions/StringExtensions.cs`; use these helpers rather than hand-building resource paths in model classes.

## Localization And Assets
- Localization JSON is under `YousifMod/localization/eng/`; `YousifMod.csproj` includes these files as analyzer inputs.
- Card localization keys currently use forms like `YOUSIFMOD-DEVOUR_CARD.title` and `.description`.
- Power localization keys use forms like `YOUSIFMOD-DEVOUR_POWER.title`, `.description`, and `.smartDescription`.
- Card portraits are under `YousifMod/images/card_portraits/` and `YousifMod/images/card_portraits/big/`.
- Power icons are under `YousifMod/images/powers/` and `YousifMod/images/powers/big/`; relic icons follow the same normal and `big` split.
- The image helpers log fallback messages such as `Could not find card image path:` when `ResourceLoader.Exists(...)` misses a resource.

## Build, Deploy, And Debug
- Build with `dotnet build YousifMod.csproj` from the repo root.
- The project targets `net9.0` with `Godot.NET.Sdk/4.5.1`.
- `Sts2PathDiscovery.props` auto-detects the STS2 install, `Sts2DataDir`, and `ModsPath`; override with MSBuild properties if detection is wrong.
- `CopyToModsFolderOnBuild` copies the DLL, PDB, and manifest JSON to `$(ModsPath)YousifMod/`.
- `dotnet publish YousifMod.csproj` also runs the `GodotPublish` target, using `Directory.Build.props` `GodotPath` and the `BasicExport` preset to export `YousifMod.pck`.
- If copy or publish fails with file lock errors, close `SlayTheSpire2.exe` before rebuilding.

## Editing Rules For Agents
- Work only in files relevant to the requested change; this repo has existing uncommitted and generated Godot files.
- Do not delete `.uid`, `.import`, `.godot`, or `.idea` files unless the user explicitly asks.
- Preserve public parameterless constructors on model classes that BaseLib or STS2 may instantiate through reflection.
- Keep dynamic values synchronized between `CanonicalVars`, card behavior, and localization placeholders.
- Use no em dashes in responses or generated prose for this workspace.
