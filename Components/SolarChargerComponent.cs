namespace CavRnMods.HotWheelsRework
{
    using System;
    using System.Reflection;
    using CavRnMods.HotWheels;
    using Eco.Core.Controller;
    using Eco.Core.Utils;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Players;
    using Eco.Shared.Logging;
    using Eco.Shared.Networking;
    using Eco.Shared.Serialization;
    using Eco.Simulation.Time;

    /// <summary>
    /// Solar mode for the original Hot Wheels Wall Charger. While it is on, the charger draws no grid power and charges
    /// the connected vehicle only in daylight (06:00 to 18:00 on the sky clock); at night it pauses and the vehicle stays connected.
    /// The original ChargerComponent keeps doing the actual connecting and charging; this component only steers it.
    /// </summary>
    [Serialized]
    [RequireComponent(typeof(ChargerComponent))]
    [RequireComponent(typeof(OnOffComponent))]
    [RequireComponent(typeof(PowerConsumptionComponent))]
    [CreateComponentTabLoc("Solar Charging", true), HasIcon("ChargerComponent")]
    public class SolarChargerComponent : WorldObjectComponent
    {
        // The original keeps these private; read them by name and fall back safely if a future version renames them.
        static readonly FieldInfo MaxChargeSpeedField = typeof(ChargerComponent).GetField("maxChargeSpeed", BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly MethodInfo TryGetCarMethod = typeof(ChargerComponent).GetMethod("TryGetCar", BindingFlags.Instance | BindingFlags.NonPublic);
        static bool warned;

        ChargerComponent charger;
        OnOffComponent onOff;
        PowerConsumptionComponent power;
        string lastStatus;

        [Serialized] public bool SolarMode { get; private set; }
        [Serialized] bool PausedForNight { get; set; }   // true while we switched the charger off for the night

        public override WorldObjectComponentClientAvailability Availability => WorldObjectComponentClientAvailability.Always;

        [SyncToView, Autogen, PropReadOnly, UITypeName("StringDisplay")]
        public string SolarStatus
        {
            get
            {
                if (!this.SolarMode) return "Solar mode is off: the charger uses grid power.";
                return IsDaylight() ? $"Solar mode is on: charging from daylight at {this.ChargeRate:0} W, no grid power used."
                                    : "Solar mode is on: waiting for daylight (06:00). The vehicle stays connected.";
            }
        }

        [Autogen, RPC, UITypeName("BigButton")]
        public void ToggleSolarMode(Player player)
        {
            this.SolarMode = !this.SolarMode;
            this.Apply();
            player?.MsgLocStr(this.SolarMode ? "Solar mode on: charging only in daylight, without grid power." : "Solar mode off: charging from the grid.");
            this.RefreshStatus();
        }

        public override void Initialize()
        {
            this.charger = this.Parent.GetComponent<ChargerComponent>();
            this.onOff = this.Parent.GetComponent<OnOffComponent>();
            this.power = this.Parent.GetComponent<PowerConsumptionComponent>();
            base.Initialize();
        }

        public override void PostInitialize()
        {
            base.PostInitialize();
            this.Apply();
        }

        float ChargeRate => MaxChargeSpeedField?.GetValue(this.charger) is float f ? f : 750f;

        static bool IsDaylight()
        {
            var hour = WorldTime.SkyTimeOfDay;
            return hour >= 6f && hour < 18f;
        }

        /// <summary>Applies the grid draw for the current mode. Called on load, on placement and whenever the mode changes.</summary>
        void Apply()
        {
            if (this.SolarMode) this.power.OverridePowerConsumption(0f);
            else
            {
                this.power.ClearPowerOverride();
                if (this.PausedForNight) { this.onOff.On = true; this.PausedForNight = false; }
            }
        }

        public override void Tick()
        {
            base.Tick();
            if (!this.SolarMode) return;
            this.power.OverridePowerConsumption(0f);   // keep it at zero even if something re-initialised the draw

            if (IsDaylight())
            {
                if (this.PausedForNight) { this.onOff.On = true; this.PausedForNight = false; }

                // The original charger only charges while the object is enabled. If it is switched on but still not enabled
                // (no grid at all, for example), deliver the daylight charge ourselves.
                if (this.onOff.On && !this.Parent.Enabled && this.TryGetCar(out var car))
                {
                    var rate = this.ChargeRate;
                    car.CurrentChargeSpeed = rate;
                    car.ElectricCarItemData.CurrentWatts = Math.Min(car.MaxWatts, car.ElectricCarItemData.CurrentWatts + rate * this.Parent.SimTickDelta());
                }
            }
            else if (this.onOff.On)
            {
                this.onOff.On = false;   // pause for the night; the original detaches nothing, so the vehicle stays connected
                this.PausedForNight = true;
            }
            this.RefreshStatus();
        }

        bool TryGetCar(out ElectricCarComponent car)
        {
            car = null;
            if (TryGetCarMethod == null)
            {
                if (!warned) { warned = true; Log.WriteWarningLineLocStr("[Hot Wheels Rework] Solar mode: could not find ChargerComponent.TryGetCar in Hot Wheels; daylight charging without a grid is disabled."); }
                return false;
            }
            var args = new object[] { null };
            var ok = TryGetCarMethod.Invoke(this.charger, args) is true;
            car = args[0] as ElectricCarComponent;
            return ok && car != null;
        }

        void RefreshStatus()
        {
            var s = this.SolarStatus;
            if (s == this.lastStatus) return;
            this.lastStatus = s;
            this.Changed(nameof(this.SolarStatus));
        }
    }
}
