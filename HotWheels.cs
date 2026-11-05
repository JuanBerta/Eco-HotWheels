namespace CavRnMods.HotWheels
{
    using Eco.Core.Plugins.Interfaces;

    public class HotWheelsMod : IModInit
    {
        public static ModRegistration Register() => new()
        {
            ModName = "HotWheels",
            ModDescription = "HotWheels brings new vehicles to Eco",
            ModDisplayName = "HotWheels",
        };
    }

}
