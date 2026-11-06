namespace CavRnMods.HotWheelsRework
{
    using System.Linq;
    using Eco.Core.Controller;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.GameActions;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Players;
    using Eco.Shared.Math;
    using Eco.Shared.Serialization;
    using Eco.World;
    using Eco.World.Blocks;

    /// <summary>
    /// Plowing for the Multi-Row Wheel Hoe. Plowing is on by default: while someone is driving it,
    /// every move tills the 3x1 row of blocks in front of the hoe, the same way the Steam Tractor's wide plow does.
    /// </summary>
    [Serialized]
    [RequireComponent(typeof(VehicleComponent))]
    [NoIcon]
    public class MultiRowHoeComponent : WorldObjectComponent
    {
        // One row of three blocks, one block ahead (+Z is forward), at ground level (one below the vehicle).
        static readonly Vector3i[] FrontRow = { new Vector3i(-1, -1, 1), new Vector3i(0, -1, 1), new Vector3i(1, -1, 1) };

        VehicleComponent vehicle;

        // Stored as "off" so plowing is on by default, including for hoes placed before this setting existed.
        [Serialized] public bool PlowingOff { get; private set; }
        public bool Plowing => !this.PlowingOff;

        public override void Initialize()
        {
            this.vehicle = this.Parent.GetComponent<VehicleComponent>();
            this.vehicle.VehicleMovedEvent.AddUnique(this.OnMoved);
            base.Initialize();
        }

        public void TogglePlowing(Player player)
        {
            this.PlowingOff = !this.PlowingOff;
            player?.MsgLocStr(this.Plowing ? "Multi-Row Wheel Hoe: plowing on." : "Multi-Row Wheel Hoe: plowing off.");
            if (this.Plowing) this.OnMoved();   // till the row in front right away
        }

        void OnMoved()
        {
            if (!this.Plowing || this.vehicle.Driver == null) return;

            var area = FrontRow.MoveAndRotate(this.Parent.Position3i, this.Parent.Rotation)
                               .Where(pos => World.GetBlock(pos).Is<Tillable>())
                               .ToList();
            if (area.Count == 0) return;

            AtomicActions.ChangeBlockNow(new MultiblockActionContext
            {
                Player                = this.vehicle.Driver,
                Area                  = area,
                ToolUsed              = this.Parent.CreatingItem as Item,
                ActionDescription     = GameActionDescription.DoStr("plow a block", "plowing a block"),
                GameActionConstructor = () => new PlowField(),
                CaloriesPerAction     = 0f,
            }, typeof(TilledDirtBlock), notify: true, genericNotify: false);
        }
    }
}
