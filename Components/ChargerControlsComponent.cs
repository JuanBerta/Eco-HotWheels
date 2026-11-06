namespace CavRnMods.HotWheels
{
    using Eco.Core.Controller;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Players;
    using Eco.Shared.Networking;
    using Eco.Shared.Serialization;
    using Eco.Shared.SharedTypes;

    /// <summary>
    /// Owns the Charger tab and its buttons for the Wall Charger. It is a plain WorldObjectComponent
    /// (not derived from BoatMooragePostComponent), so the tab and buttons render correctly, and it
    /// forwards each action to ChargerComponent.
    /// </summary>
    [Serialized]
    [RequireComponent(typeof(ChargerComponent))]
    [CreateComponentTabLoc("Charger", true), HasIcon("ChargerComponent")]
    public class ChargerControlsComponent : WorldObjectComponent
    {
        public override WorldObjectComponentClientAvailability Availability => WorldObjectComponentClientAvailability.Always;

        private ChargerComponent Charger => this.Parent.GetComponent<ChargerComponent>();

        [SyncToView, Autogen, PropReadOnly, UITypeName("StringDisplay")] public string ChargerStatus => this.Charger.ChargerStatus;

        [Autogen, RPC, UITypeName("BigButton")] public void ConnectNearestVehicle(Player player) => this.Charger.ConnectNearestVehicle(player);

        [Autogen, RPC, UITypeName("BigButton")] public void DisconnectNearestVehicle(Player player) => this.Charger.DisconnectNearestVehicle(player);

        [Autogen, RPC, UITypeName("BigButton")] public void ToggleSolarMode(Player player) => this.Charger.ToggleSolarMode(player);
    }
}
