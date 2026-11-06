# Hot Wheels for Eco

Hot Wheels brings new vehicles and boats to [Eco](https://play.eco/): ride a scooter, bicycle, moto, school bus, steam paddle craft, Tesla Model 3 or jet ski, and charge electric vehicles at home or at a Tesla Supercharger.

- Original mod: [mod.io – Hot Wheels](https://mod.io/g/eco/m/hot-wheels) (by Zangdar)
- This repository: port of v3.2.0 to **Eco 0.14.2.1** (public branch, release-1106), version **3.3.0**

## Contents

| Folder | What's in it |
|---|---|
| `Vehicles/` | Bicycle, Moto, Scooter, JetSki, SchoolBus, SteamPaddleCraft, TeslaModel3 |
| `Items/` | BicycleFrame, IronChain (vehicle parts) |
| `Components/` | ElectricCarComponent, ChargerComponent |
| `WorldObjects/` | TeslaSupercharger, WallCharger |
| `Assets/` | `HotWheels-v3.2.0.unity3d` – prefab / texture bundle for the vehicles |
| `HotWheels.cs` | Mod registration (`IModInit`) |

## Installing

1. Build the project (see below), or grab `HotWheels.dll` from a release.
2. Copy `HotWheels.dll` and `Assets\HotWheels-v3.2.0.unity3d` into a `Mods\HotWheels` folder of your Eco server, next to the other mods (for example `Eco_Data\Server\Mods\HotWheels\`).
3. Start the server. The mod is registered as **HotWheels**.

## Building

Requirements: .NET 10 SDK.

```powershell
dotnet build HotWheels.csproj -c Release
```

The project references `Eco.ReferenceAssemblies 0.14.2.1-beta-release-1106` from NuGet and uses Fody (`StrangeLoopGames.PropertyChanged`) for property change notifications. The output is `bin\Release\net10.0\HotWheels.dll`.

## Porting notes (3.2.0 → 3.3.0, Eco 0.14.2.1)

- The C# sources compile against Eco 0.14.2.1 with no warnings.
- `ChargerComponent`: removed `[Notify]` from the getter-only `HasConnectedVehicle` property, which made Fody fail to inject its event invoker. The property is still exposed as an env var with `[EnvVar]`.
- Added `FodyWeavers.xml` so the PropertyChanged weaver is enabled.
- The asset bundle was built with Unity 6000.3.6f1, the same editor version as Eco 0.14.2.1, so it was kept unchanged.

## Status

- [x] Compiles against Eco 0.14.2.1 (0 warnings, 0 errors)
- [ ] Verified in-game on a 0.14.2.1 server (vehicles spawn, drive, charge, and the Tesla doors work)

## Credits

- Original Hot Wheels mod by **Zangdar**.
- Port and maintenance by **Beinded (JuanBerta)**.
