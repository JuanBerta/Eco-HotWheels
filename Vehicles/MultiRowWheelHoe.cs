namespace CavRnMods.HotWheelsRework
{
    using System;
    using System.Collections.Generic;
    using Eco.Core.Controller;
    using Eco.Core.Items;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Components.Auth;
    using Eco.Gameplay.Interactions.Interactors;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Items.Recipes;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Occupancy;
    using Eco.Gameplay.Players;
    using Eco.Gameplay.Skills;
    using Eco.Gameplay.Systems.Exhaustion;
    using Eco.Gameplay.Systems.NewTooltip;
    using Eco.Mods.TechTree;
    using Eco.Shared.Items;
    using Eco.Shared.Localization;
    using Eco.Shared.Serialization;
    using Eco.Shared.SharedTypes;
    using static Eco.Gameplay.Components.PartsComponent;

    [Serialized]
    [LocDisplayName("Multi-Row Wheel Hoe")]
    [LocDescription("A wheel hoe with three rows of tines. Grab the handles and push it to till a strip three blocks wide in front of it. Click it while on foot to switch plowing off or back on.")]
    [IconGroup("World Object Minimap")]
    [Weight(6000)]
    [SalvageCost(typeof(WoodScrap), 6f, typeof(IronScrap), 6f)]
    [Hoer]
    [Ecopedia("Crafted Objects", "Vehicles", createAsSubPage: true)]
    public partial class MultiRowWheelHoeItem : WorldObjectItem<MultiRowWheelHoeObject>, IPersistentData
    {
        [Serialized, SyncToView, NewTooltipChildren(CacheAs.Instance, flags: TTFlags.AllowNonControllerTypeForChildren)] public object? PersistentData { get; set; }
    }

    [RequiresSkill(typeof(BasicEngineeringSkill), 3)]
    [Ecopedia("Crafted Objects", "Vehicles", subPageName: "Multi-Row Wheel Hoe Item")]
    public partial class MultiRowWheelHoeRecipe : RecipeFamily
    {
        public MultiRowWheelHoeRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "MultiRowWheelHoe",  //noloc
                displayName: Localizer.DoStr("Multi-Row Wheel Hoe"),
                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(IronBarItem), 15, typeof(BasicEngineeringSkill)),
                    new IngredientElement("HewnLog", 10, typeof(BasicEngineeringSkill)), //noloc
                    new IngredientElement("WoodBoard", 8, typeof(BasicEngineeringSkill)), //noloc
                    new IngredientElement(typeof(WoodenWheelItem), 1, true),
                    new IngredientElement(typeof(LubricantItem), 1, true),
                },
                items: new List<CraftingElement>
                {
                    new CraftingElement<MultiRowWheelHoeItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 15;
            this.LaborInCalories = CreateLaborInCaloriesValue(150, typeof(BasicEngineeringSkill));
            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(MultiRowWheelHoeRecipe), start: 3, skillType: typeof(BasicEngineeringSkill));
            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Multi-Row Wheel Hoe"), recipeType: typeof(MultiRowWheelHoeRecipe));
            this.ModsPostInitialize();
            CraftingComponent.AddRecipe(tableType: typeof(WainwrightTableObject), recipeFamily: this);
        }

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }

    [Serialized]
    [RequireComponent(typeof(StandaloneAuthComponent))]
    [RequireComponent(typeof(PaintableComponent))]
    [RequireComponent(typeof(VehicleComponent))]
    [RequireComponent(typeof(MultiRowHoeComponent))]
    [RequireComponent(typeof(MinimapComponent))]
    [RequireComponent(typeof(PartsComponent))]
    [RepairRequiresSkill(typeof(BasicEngineeringSkill), 3)]
    [ExhaustableUnlessOverridenVehicle]
    [Ecopedia("Crafted Objects", "Vehicles", subPageName: "Multi-Row Wheel Hoe Item")]
    public partial class MultiRowWheelHoeObject : PhysicsWorldObject, IRepresentsItem
    {
        static MultiRowWheelHoeObject()
        {
            WorldObject.AddOccupancy<MultiRowWheelHoeObject>(new List<BlockOccupancy>(0));
        }
        public override TableTextureMode TableTexture => TableTextureMode.Wood;
        public override bool PlacesBlocks            => false;
        public override LocString DisplayName { get { return Localizer.DoStr("Multi-Row Wheel Hoe"); } }
        public Type RepresentedItemType { get { return typeof(MultiRowWheelHoeItem); } }

        private MultiRowWheelHoeObject() { }
        protected override void Initialize()
        {
            this.ModsPreInitialize();
            base.Initialize();
            // No HumanPowered call: like the other Hot Wheels vehicles, it needs no food to push.
            this.GetComponent<MinimapComponent>().InitAsMovable();
            this.GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Vehicles"));
            this.GetComponent<VehicleComponent>().Initialize(10, 1, 1);
            this.ModsPostInitialize();
            this.GetComponent<PartsComponent>().Config(() => LocString.Empty, new PartInfo[]
            {
                new() { TypeName = nameof(WoodenWheelItem), Quantity = 1 },
                new() { TypeName = nameof(LubricantItem), Quantity = 1 },
            });
        }

        [Interaction(InteractionTrigger.LeftClick, "Toggle Plowing", authRequired: AccessType.ConsumerAccess, MinCaloriesRequired = 0)]
        public void TogglePlowingLeft(Player player, InteractionTriggerInfo trigger, InteractionTarget target) => this.GetComponent<MultiRowHoeComponent>().TogglePlowing(player);

        [Interaction(InteractionTrigger.RightClick, "Toggle Plowing", authRequired: AccessType.ConsumerAccess, MinCaloriesRequired = 0)]
        public void TogglePlowingRight(Player player, InteractionTriggerInfo trigger, InteractionTarget target) => this.GetComponent<MultiRowHoeComponent>().TogglePlowing(player);

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }
}
