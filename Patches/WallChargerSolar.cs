namespace CavRnMods.HotWheels
{
    using Eco.Gameplay.Objects;

    /// <summary>
    /// Hot Wheels Rework: adds solar mode to the original mod's Wall Charger. This is a second part of the original's
    /// partial class; it only adds a component and declares no members, so it compiles alongside the original source.
    /// </summary>
    [RequireComponent(typeof(CavRnMods.HotWheelsRework.SolarChargerComponent))]
    public partial class WallChargerObject { }
}
