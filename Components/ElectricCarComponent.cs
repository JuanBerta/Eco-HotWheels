using System.ComponentModel;
using Eco.Core.Utils;
using Eco.Gameplay.Items;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.NewTooltip;
using Eco.Shared.Items;

namespace CavRnMods.HotWheels
{
    using Eco.Core.Controller;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Objects;
    using Eco.Shared.Localization;
    using Eco.Shared.Serialization;
    using System;

    [Serialized]
    public class ElectricCarItemData : IController, INotifyPropertyChanged, IClearRequestHandler
    {
        #region IController
#pragma warning disable CS0067 //Raised by the controller system, never invoked directly.
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067
        int            controllerID;
        public ref int ControllerID => ref this.controllerID;
        #endregion

        [Serialized, SyncToView] public float CurrentWatts { get; set; }

        public bool HasDataThatCanBeCleared => false;

        public ElectricCarComponent? Parent { get; set; }

        public Result TryHandleClearRequest(Player player)
        {
            this.CurrentWatts = 0;
            return Result.Succeeded;
        }
    }

    [Serialized]
    [RequireComponent(typeof(VehicleComponent))]
    [RequireComponent(typeof(StatusComponent))]
    [NoIcon]
    public class ElectricCarComponent : WorldObjectComponent, IPersistentData, INotifyPropertyChanged
    {
        public override WorldObjectComponentClientAvailability Availability => WorldObjectComponentClientAvailability.Always;
        private StatusElement? status;
        private VehicleComponent? vehicle;
        [Serialized, SyncToView, NewTooltipChildren(CacheAs.Instance)] public ElectricCarItemData ElectricCarItemData { get; set; } = new();

        public object PersistentData { get => this.ElectricCarItemData; set => this.ElectricCarItemData = value as ElectricCarItemData ?? new ElectricCarItemData(); }

        [Notify] public bool IsCharging { get; set; }
        public float CurrentChargeSpeed { get; set; }
        public float BatteryPercent => (float)Math.Round(this.ElectricCarItemData.CurrentWatts / this.MaxWatts * 100, 2);
        private float ConsumptionPerSecond { get; set; }
        public float MaxWatts { get; private set; }

        public void Initialize(float maxWatts, float joulesPerSecond)
        {
            this.ElectricCarItemData ??= new ElectricCarItemData();
            this.ElectricCarItemData.Parent = this;

            this.MaxWatts = maxWatts;
            this.ConsumptionPerSecond = joulesPerSecond;

            this.status = this.Parent.GetComponent<StatusComponent>().CreateStatusElement();
            this.vehicle = this.Parent.GetComponent<VehicleComponent>();
            this.vehicle.SetAdditionalDrivableCheck(() => !(this.IsCharging || this.ElectricCarItemData.CurrentWatts == 0));
            this.Subscribe(nameof(this.ElectricCarItemData.CurrentWatts), this.NotifyDrivingChange);
            this.Subscribe(nameof(this.IsCharging), this.NotifyDrivingChange);
        }

        void NotifyDrivingChange() => this.vehicle?.Changed(nameof(VehicleComponent.Drivable));

        public override void Tick() => this.Tick(this.Parent.SimTickDelta());
        public void Tick(float deltaTime)
        {
            base.Tick();

            if (!this.IsCharging)
            {
                if (!this.vehicle!.IsMoving)
                {
                    this.ElectricCarItemData.CurrentWatts -= this.ConsumptionPerSecond * deltaTime / 432;
                }
                else
                {
                    this.ElectricCarItemData.CurrentWatts -= this.ConsumptionPerSecond * deltaTime;
                }

                if (this.ElectricCarItemData.CurrentWatts <= 0) this.ElectricCarItemData.CurrentWatts = 0;
            }
        }

        public override void LateTick()
        {
            if (this.IsCharging)
            {
                this.vehicle!.OutOfFuel = true;
            }
            else
            {
                this.vehicle!.OutOfFuel = this.ElectricCarItemData.CurrentWatts == 0;
            }

            this.Parent.SetAnimatedState("BatteryPercent", $"{Math.Round(this.BatteryPercent)}%");
            this.Parent.SetAnimatedState("CurrentChargeSpeed", this.CurrentChargeSpeed > 0 ? $"{Math.Round(this.CurrentChargeSpeed / 1000, 1)}kW" : "");
            this.Parent.SetAnimatedState("IsCharging", this.IsCharging);
            this.status?.SetStatusMessage(
                !(this.IsCharging || this.ElectricCarItemData.CurrentWatts == 0),
                Localizer.DoStr($"Battery is ({this.BatteryPercent}%)"),
                this.IsCharging
                    ? Localizer.DoStr($"Battery is charging... ({this.BatteryPercent}%)")
                    : Localizer.DoStr("Battery is empty!")
                );
        }
    }
}
