# Hot Wheels Rework for Eco

An add-on to the [Hot Wheels](https://mod.io/g/eco/m/hot-wheels) mod by Zangdar, for [Eco](https://play.eco/) 0.14.2.1. Hot Wheels is a required dependency: it provides the scooter, bicycle, moto, school bus, steam paddle craft, Tesla Model 3, jet ski and the chargers. This add-on adds:

- **Multi-Row Wheel Hoe**: a pushable wheel hoe with its own 3D model. Grab the handles and push it, and it tills the row of three blocks in front of it. Click it on foot to switch plowing off or back on. Crafted at the Wainwright Table (Basic Engineering 3). Its Ecopedia page has a How to use section.
- **Solar mode for the Wall Charger**: a Solar Charging tab with a Toggle Solar Mode button. With solar mode on, the charger uses no grid power and charges only in daylight (06:00 to 18:00); at night it pauses and the vehicle stays connected.

Version **4.1.0**. mod.io: [HotWheels Rework](https://mod.io/g/eco/m/hotwheels-rework).

## How it fits with the original

The original Hot Wheels ships as C# source in `Mods/UserCode/CavRnMods/HotWheels`. Rework ships as source too, in `Mods/UserCode/HotWheelsRework`, and Eco compiles both into one assembly. That lets Rework extend the original's partial `WallChargerObject` with a component (`Patches/WallChargerSolar.cs`) without redefining any of its classes. Rework's own types live in the `CavRnMods.HotWheelsRework` namespace.

The repository also keeps the earlier full port of the original vehicles, chargers and parts (`Vehicles/`, `Items/`, `WorldObjects/`, `Components/ChargerComponent.cs`, ...) for reference. Those files are excluded from the build and not packaged, since the original mod provides them.

## Contents

| Path | What's in it |
|---|---|
| `HotWheels.cs` | Mod registration (`HotWheelsRework`), packaged as `HotWheelsRework.cs` |
| `Vehicles/MultiRowWheelHoe.cs` | Multi-Row Wheel Hoe item, recipe and object |
| `Components/MultiRowHoeComponent.cs` | Tills the 3x1 row in front of the hoe as it moves |
| `Components/SolarChargerComponent.cs` | Solar mode for the original Wall Charger |
| `Patches/WallChargerSolar.cs` | Adds the solar component to the original `WallChargerObject` |
| `Ecopedia/MultiRowWheelHoeEcopedia.cs` | How to use section on the hoe's Ecopedia page |
| `Unity/` | Unity side of the hoe: bundle builder, stand-in client scripts, FBX model |
| `modio/` | mod.io changelog and descriptions |

## Installing

Subscribe on mod.io; Hot Wheels is installed with it. For a manual install, install Hot Wheels first, then extract `HotWheelsRework-4.1.0.zip` into `Eco_Data/Server/` (it contains `Mods/UserCode/HotWheelsRework/`), and restart the server. Remove any older `HotWheelsRework.dll` from the Mods folder.

## Building

Requirements: .NET 10 SDK.

```powershell
dotnet build HotWheels.csproj -c Release
```

The build is a compile check: the mod ships as source. It references `Eco.ReferenceAssemblies 0.14.2.1-beta-release-1106` and, when it exists, compiles the original Hot Wheels source from the local server (`OriginalHotWheelsSource` in the project file) alongside Rework, so the partial-class extension is checked too.

## The hoe's client bundle

`MultiRowWheelHoe.unity3d` is built in a Unity 6000.3.6f1 project with the Eco ModKit 0.14.2.1 imported:

- `Unity/ClientStubs` and `Unity/ThirdPartyStubs` are stand-ins for the game's client vehicle scripts (`Vehicle`, `Mountable`, `MountSpot`, `Wheel`, `RCCCarControllerV2`, ...), which the ModKit doesn't include. They declare only the serialized fields the prefab sets; the game binds its real classes by assembly, namespace and class name. Their fields and values mirror the Hot Wheels Scooter prefab.
- `Unity/Editor/HotWheelsHoeBundle.cs` adds a **Hot Wheels > Build Multi-Row Wheel Hoe Bundle** menu that builds the vehicle prefab from `Unity/Models/MultiRowWheelHoe.fbx` and writes the bundle.

Put the folders under `Assets/HotWheels/` in the Unity project (`Models/`, `ClientStubs/`, `ThirdPartyStubs/`, `Editor/`).

## Credits

- Hot Wheels by **Zangdar**.
- Hot Wheels Rework, the Multi-Row Wheel Hoe and solar mode by **Beinded (JuanBerta)**.
