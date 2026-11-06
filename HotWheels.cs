namespace CavRnMods.HotWheelsRework
{
    using Eco.Core.Plugins.Interfaces;

    /// <summary>
    /// Hot Wheels Rework: an add-on to the original Hot Wheels mod by Zangdar (a mod.io dependency).
    /// It adds the Multi-Row Wheel Hoe; the original mod provides every other vehicle and the chargers.
    /// </summary>
    public class HotWheelsReworkMod : IModInit
    {
        public static ModRegistration Register() => new()
        {
            ModName = "HotWheelsRework",
            ModDescription = "Hot Wheels Rework adds the Multi-Row Wheel Hoe to the Hot Wheels mod.",
            ModDisplayName = "Hot Wheels Rework",
        };
    }
}
