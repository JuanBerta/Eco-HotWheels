namespace CavRnMods.HotWheels
{
    using Eco.Core.Controller;
    using Eco.Core.Utils;
    using Eco.Gameplay.Auth;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Interactions.Interactors;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Players;
    using Eco.Gameplay.Systems.EnvVars;
    using Eco.Shared.IoC;
    using Eco.Shared.Items;
    using Eco.Shared.Localization;
    using Eco.Shared.Networking;
    using Eco.Shared.Serialization;
    using Eco.Shared.SharedTypes;
    using Eco.Shared.Utils;
    using Eco.Simulation.Time;
    using System;

    [Serialized]
    [RequireComponent(typeof(PowerGridComponent))]
    [RequireComponent(typeof(PowerConsumptionComponent))]
    [RequireComponent(typeof(OnOffComponent))]
    [RequireComponent(typeof(StatusComponent))]
    [CreateComponentTabLoc("Charger"), HasIcon("ChargerComponent")]
    [Priority(PriorityAttribute.VeryLow)]
    public class ChargerComponent : BoatMooragePostComponent, IOperatingWorldObjectComponent
    {
        private StatusElement? status;
        private PowerConsumptionComponent? powerConsumptionComponent;
        public override WorldObjectComponentClientAvailability Availability => WorldObjectComponentClientAvailability.Always;

        [Serialized] private ThreadSafeList<Guid> attachedVehicleId = new();
        [EnvVar] private bool HasConnectedVehicle => this.attachedVehicleId.Count == 1;
        // In solar mode the charger draws no grid power, so it is not an operating grid load.
        public bool Operating => this.HasConnectedVehicle && !this.solarMode;

        [SyncToView] public string ChargerStatus
        {
            get
            {
                if (this.TryGetCar(out var car))
                {
                    if (this.solarMode)
                    {
                        return $"{car!.Parent.DisplayName} is connected, battery at {car.BatteryPercent}%, solar mode: {(IsDaylight() ? $"charging at {this.maxChargeSpeed}W" : "waiting for daylight")}";
                    }

                    return $"{car!.Parent.DisplayName} is connected, battery at {car.BatteryPercent}%, charging at {this.maxChargeSpeed}W";
                }

                return "No vehicle connected";
            }
        }

        private int connectRadius;
        private float maxChargeSpeed;
        private bool autoDetach;
        private bool solarCapable;
        [Serialized] private bool solarMode;

        /// <summary>True when this charger runs on daylight instead of grid power.</summary>
        public bool SolarMode => this.solarMode;

        public void Initialize(int connectRadiusValue = 3, float maxChargeSpeedValue = 500f, bool autoDetachValue = false, bool solarCapableValue = false)
        {
            this.connectRadius = connectRadiusValue;
            this.maxChargeSpeed = maxChargeSpeedValue;
            this.autoDetach = autoDetachValue;
            this.solarCapable = solarCapableValue;
            this.status = this.Parent.GetComponent<StatusComponent>().CreateStatusElement();
            this.powerConsumptionComponent = this.Parent.GetComponent<PowerConsumptionComponent>();
            this.powerConsumptionComponent.Initialize(maxChargeSpeedValue);

            if (this.TryGetCar(out var car))
            {
                this.ConnectVehicle(car!);
            }

            this.Parent.OnEnableChange.Add(() => {
                if (this.TryGetCar(out var car))
                {
                    if (this.Parent.Enabled)
                    {
                        car!.CurrentChargeSpeed = this.maxChargeSpeed;
                    }
                    else
                    {
                        car!.CurrentChargeSpeed = 0f;
                    }
                }
            });
        }

        public override void Destroy()
        {
            if (this.TryGetCar(out var car))
            {
                this.DetachVehicle(car!);
            }
        }

        private ElectricCarComponent? ObjectIdToCarComponent(Guid objectId)
        {
            var carObject = ServiceHolder<IWorldObjectManager>.Obj.GetFromID(objectId);
            return carObject?.GetComponent<ElectricCarComponent>();
        }

        public void ConnectNearestVehicle(Player player) => this.ConnectNearestVehicleInternal(player, default, default);

        [Interaction(InteractionTrigger.RightClick, "Connect Nearest Vehicle", authRequired: AccessType.FullAccess, RequiredAccess = AccessType.ConsumerAccess)]
        public void ConnectNearestVehicleInternal(Player player, InteractionTriggerInfo triggerInfo, InteractionTarget target)
        {
            if (!this.Parent.Enabled && !this.solarMode)
            {
                player.ErrorLocStr("Charger is not working! Check status.");
                return;
            }

            if (this.attachedVehicleId.Count == 1)
            {
                player.ErrorLocStr("Can't connect more than 1 vehicle!");
                return;
            }

            foreach (var worldObject in ServiceHolder<IWorldObjectManager>.Obj.GetObjectsWithin(this.Parent.WorldPos(), this.connectRadius))
            {
                if (worldObject.TryGetComponent<ElectricCarComponent>(out var car) && ServiceHolder<IAuthManager>.Obj.IsAuthorized(car.Parent, player.User, AccessType.FullAccess, null, out _) && !car.IsCharging)
                {
                    if (this.autoDetach && car.BatteryPercent >= 100)
                    {
                        continue;
                    }

                    this.ConnectVehicle(car);
                    this.attachedVehicleId.Add(car.Parent.ObjectID);
                    player.InfoBoxLocStr($"Connected vehicle {car.Parent.MarkedUpName}!");
                    return;
                }
            }

            player.ErrorLocStr($"Can't find any authorized non-charging {(this.autoDetach ? "non-empty " : "")}electric vehicle within {this.connectRadius} meters!");
        }

        public void ToggleSolarMode(Player player)
        {
            if (!this.solarCapable)
            {
                player.ErrorLocStr("This charger has no solar mode.");
                return;
            }

            // Switching modes with a car attached is fine: the next tick applies the new rate.
            this.solarMode = !this.solarMode;
            player.InfoBoxLocStr(this.solarMode
                ? "Solar mode on: charges only in daylight, using no grid power."
                : "Solar mode off: charges from the grid.");
        }

        public void DisconnectNearestVehicle(Player player) => this.DisconnectVehicleInternal(player, default, default);

        [Interaction(InteractionTrigger.RightClick, "Disconnect Vehicle", modifier: InteractionModifier.Shift, authRequired: AccessType.FullAccess, RequiredAccess = AccessType.ConsumerAccess)]
        public void DisconnectVehicleInternal(Player player, InteractionTriggerInfo triggerInfo, InteractionTarget target)
        {
            if (!this.HasConnectedVehicle)
            {
                player.ErrorLocStr("No car attached!");
                return;
            }

            if (this.TryGetCar(out var car))
            {
                if (!ServiceHolder<IAuthManager>.Obj.IsAuthorized(car!.Parent, player.User, AccessType.FullAccess, null, out _))
                {
                    player.ErrorLocStr("Not authorized to disconnect this vehicle!");
                    return;
                }

                this.DetachVehicle(car);
                player.InfoBoxLocStr($"Disconnected vehicle {car.Parent.MarkedUpName}!");
            }
        }

        private void ConnectVehicle(ElectricCarComponent car)
        {
            car.IsCharging = true;
            car.CurrentChargeSpeed = this.maxChargeSpeed;

            var boatCar = car.Parent.GetComponent<BoatComponent>();
            if (boatCar is not null)
            {
                boatCar.MoorageAttached = this;
            }
        }

        private void DetachVehicle(ElectricCarComponent car)
        {
            car.CurrentChargeSpeed = 0f;
            car.IsCharging = false;
            this.attachedVehicleId.RemoveAt(0);

            var boatCar = car.Parent.GetComponent<BoatComponent>();
            if (boatCar is not null)
            {
                boatCar.MoorageAttached = null;
            }
        }

        private bool TryGetCar(out ElectricCarComponent? car)
        {
            car = null;

            if (!this.HasConnectedVehicle)
            {
                return false;
            }

            car = this.ObjectIdToCarComponent(this.attachedVehicleId[0]);
            if (car == null)
            {
                this.attachedVehicleId.RemoveAt(0);
                return false;
            }

            return true;
        }

        /// <summary>True between 06:00 and 18:00 on the sky clock, the same time of day players see.</summary>
        private static bool IsDaylight()
        {
            var hour = WorldTime.SkyTimeOfDay;
            return hour >= 6f && hour < 18f;
        }

        public override void Tick() => this.Tick(this.Parent.SimTickDelta());
        private void Tick(float deltaTime)
        {
            if (this.TryGetCar(out var car))
            {
                if (this.solarMode)
                {
                    // Solar mode charges at full rate in daylight and pauses at night, without grid power.
                    if (IsDaylight())
                    {
                        car!.CurrentChargeSpeed = this.maxChargeSpeed;
                        car.ElectricCarItemData.CurrentWatts += this.maxChargeSpeed * deltaTime;
                    }
                    else
                    {
                        car!.CurrentChargeSpeed = 0f;
                    }
                }
                else if (this.Parent.Enabled)
                {
                    car!.CurrentChargeSpeed = this.maxChargeSpeed;
                    car.ElectricCarItemData.CurrentWatts += this.maxChargeSpeed * deltaTime;
                }

                if (car!.ElectricCarItemData.CurrentWatts >= car.MaxWatts)
                {
                    car.ElectricCarItemData.CurrentWatts = car.MaxWatts;

                    if (this.autoDetach)
                    {
                        this.DetachVehicle(car);
                    }
                }
            }

            this.Parent.SetAnimatedState("IsCharging", this.Operating);
            this.Parent.SetAnimatedState("IsReady", this.Parent.Enabled);
        }

        public override void LateTick()
        {
            this.status?.SetStatusMessage(
                true,
                this.TryGetCar(out var car)
                    ? Localizer.DoStr($"{car!.Parent.MarkedUpName} is charging, battery is {car.BatteryPercent}%")
                    : Localizer.DoStr("No vehicle connected"), Localizer.DoStr("Never")
            );
        }
    }
}
