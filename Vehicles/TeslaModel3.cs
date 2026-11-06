namespace CavRnMods.HotWheels
{
    using Eco.Gameplay.Components.Storage;
    using Eco.Core.Controller;
    using Eco.Core.Items;
    using Eco.Gameplay.Components.Auth;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Interactions.Interactors;
    using Eco.Gameplay.Items.Recipes;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Occupancy;
    using Eco.Gameplay.Players;
    using Eco.Gameplay.Skills;
    using Eco.Gameplay.Systems.NewTooltip;
    using Eco.Mods.TechTree;
    using Eco.Shared.Items;
    using Eco.Shared.Localization;
    using Eco.Shared.Serialization;
    using Eco.Shared.SharedTypes;
    using System.Collections.Generic;
    using System;
    using static Eco.Gameplay.Components.PartsComponent;

    [Serialized]
    [LocDisplayName("Tesla Model 3")]
    [LocDescription("Tesla Model 3 for green transportation.")]
    [IconGroup("World Object Minimap")]
    [Weight(15000)]
    [SalvageCost(typeof(IronScrap), 8.0f, typeof(PlasticScrap), 12.0f, typeof(CopperScrap), 6.0f, typeof(Textiles), 1.2f, typeof(ElectronicScrap), 0.5f, typeof(ChemicalWaste), 1.5f)]
    [Ecopedia("Crafted Objects", "Vehicles", createAsSubPage: true)]
    public partial class TeslaModel3Item : WorldObjectItem<TeslaModel3Object>, IPersistentData
    {
        [Serialized, SyncToView, NewTooltipChildren(CacheAs.Instance, flags: TTFlags.AllowNonControllerTypeForChildren)] public object? PersistentData { get; set; }
    }

    /// <summary>
    /// <para>Server side recipe definition for "TeslaModel3".</para>
    /// <para>More information about RecipeFamily objects can be found at https://docs.play.eco/api/server/eco.gameplay/Eco.Gameplay.Items.RecipeFamily.html</para>
    /// </summary>
    /// <remarks>
    /// This is an auto-generated class. Don't modify it! All your changes will be wiped with next update! Use Mods* partial methods instead for customization.
    /// If you wish to modify this class, please create a new partial class or follow the instructions in the "UserCode" folder to override the entire file.
    /// </remarks>
    [RequiresSkill(typeof(ElectronicsSkill), 2)]
    [Ecopedia("Crafted Objects", "Vehicles", subPageName: "Tesla Model 3 Item")]
    public partial class TeslaModel3Recipe : RecipeFamily
    {
        public TeslaModel3Recipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "TeslaModel3",  //noloc
                displayName: Localizer.DoStr("Tesla Model 3"),

                // Defines the ingredients needed to craft this recipe. An ingredient items takes the following inputs
                // type of the item, the amount of the item, the skill required, and the talent used.
                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(SteelPlateItem), 8, typeof(ElectronicsSkill)),
                    new IngredientElement(typeof(NylonFabricItem), 20, typeof(ElectronicsSkill)),
                    new IngredientElement(typeof(SteelSpringItem), 8, typeof(ElectronicsSkill)),
                    new IngredientElement(typeof(CopperWiringItem), 60, typeof(ElectronicsSkill)),
                    new IngredientElement(typeof(BasicCircuitItem), 10, typeof(ElectronicsSkill)),
                    new IngredientElement(typeof(AdvancedCircuitItem), 2, typeof(ElectronicsSkill)),
                    new IngredientElement(typeof(FramedGlassItem), 12, typeof(ElectronicsSkill)),
                    new IngredientElement(typeof(PlasticItem), 120, typeof(ElectronicsSkill)),
                    new IngredientElement(typeof(ElectricMotorItem), 1, true),
                    new IngredientElement(typeof(RubberWheelItem), 4, true),
                    new IngredientElement(typeof(RadiatorItem), 1, true),
                    new IngredientElement(typeof(SteelAxleItem), 2, true),
                    new IngredientElement(typeof(LightBulbItem), 8, true),
                    new IngredientElement(typeof(LubricantItem), 1, true),
                },

                // Define our recipe output items.
                // For every output item there needs to be one CraftingElement entry with the type of the final item and the amount
                // to create.
                items: new List<CraftingElement>
                {
                    new CraftingElement<TeslaModel3Item>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 18; // Defines how much experience is gained when crafted.

            // Defines the amount of labor required and the required skill to add labor
            this.LaborInCalories = CreateLaborInCaloriesValue(3000, typeof(ElectronicsSkill));

            // Defines our crafting time for the recipe
            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(TeslaModel3Recipe), start: 20, skillType: typeof(ElectronicsSkill));

            // Perform pre/post initialization for user mods and initialize our recipe instance with the display name "TeslaModel3"
            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Tesla Model 3"), recipeType: typeof(TeslaModel3Recipe));
            this.ModsPostInitialize();

            // Register our RecipeFamily instance with the crafting system so it can be crafted.
            CraftingComponent.AddRecipe(tableType: typeof(RoboticAssemblyLineObject), recipeFamily: this);
        }

        /// <summary>Hook for mods to customize RecipeFamily before initialization. You can change recipes, xp, labor, time here.</summary>
        partial void ModsPreInitialize();

        /// <summary>Hook for mods to customize RecipeFamily after initialization, but before registration. You can change skill requirements here.</summary>
        partial void ModsPostInitialize();
    }

    [Serialized]
    [RequireComponent(typeof(StandaloneAuthComponent))]
    [RequireComponent(typeof(PaintableComponent))]
    [RequireComponent(typeof(PublicStorageComponent))]
    [RequireComponent(typeof(MovableLinkComponent))]
    [RequireComponent(typeof(VehicleComponent))]
    [RequireComponent(typeof(CustomTextComponent))]
    [RequireComponent(typeof(ElectricCarComponent))]
    [RequireComponent(typeof(MinimapComponent))]
    [RequireComponent(typeof(PartsComponent))]
    [RepairRequiresSkill(typeof(ElectronicsSkill), 2)]
    [Ecopedia("Crafted Objects", "Vehicles", subPageName: "Tesla Model 3 Item")]
    public partial class TeslaModel3Object : PhysicsWorldObject, IRepresentsItem, IHasInteractions
    {
        static TeslaModel3Object()
        {
            WorldObject.AddOccupancy<TeslaModel3Object>(new List<BlockOccupancy>(0));
        }
        public override TableTextureMode TableTexture => TableTextureMode.Metal;
        public override bool PlacesBlocks            => false;
        public override LocString DisplayName { get { return Localizer.DoStr("Tesla Model 3"); } }
        public Type RepresentedItemType { get { return typeof(TeslaModel3Item); } }

        private TeslaModel3Object() { }
        protected override void Initialize()
        {
            base.Initialize();
            this.GetComponent<CustomTextComponent>().Initialize(200);
            this.GetComponent<PublicStorageComponent>().Initialize(16, 2000000);
            this.GetComponent<MinimapComponent>().InitAsMovable();
            this.GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Vehicles"));
            this.GetComponent<VehicleComponent>().Initialize(48, 5, 5);
            this.GetComponent<ElectricCarComponent>().Initialize(5250000, 5000);
            {
                this.GetComponent<PartsComponent>().Config(() => LocString.Empty, new PartInfo[]
                {
                                        new() { TypeName = nameof(ElectricMotorItem), Quantity = 1},
                                        new() { TypeName = nameof(RubberWheelItem), Quantity = 4},
                                        new() { TypeName = nameof(LightBulbItem), Quantity = 8},
                                    });
            }
        }

        [Interaction(InteractionTrigger.RightClick, "DriverDoor", authRequired: AccessType.OwnerAccess, requiredEnvVars: new[] { "DriverDoor" }, MinCaloriesRequired = 0)]
        public void DriverDoor(Player player, InteractionTriggerInfo trigger, InteractionTarget target)
        {
            var isOpened = this.AnimatedStates.GetOrDefault("DriverDoor") as bool? ?? false;
            this.SetAnimatedState("DriverDoor", !isOpened);
        }

        [Interaction(InteractionTrigger.RightClick, "PassengerDoor", authRequired: AccessType.ConsumerAccess, requiredEnvVars: new[] { "PassengerDoor" }, MinCaloriesRequired = 0)]
        public void PassengerDoor(Player player, InteractionTriggerInfo trigger, InteractionTarget target)
        {
            var isOpened = this.AnimatedStates.GetOrDefault("PassengerDoor") as bool? ?? false;
            this.SetAnimatedState("PassengerDoor", !isOpened);
        }

        [Interaction(InteractionTrigger.RightClick, "RearRightDoor", authRequired: AccessType.ConsumerAccess, requiredEnvVars: new[] { "RearRightDoor" }, MinCaloriesRequired = 0)]
        public void RearRightDoor(Player player, InteractionTriggerInfo trigger, InteractionTarget target)
        {
            var isOpened = this.AnimatedStates.GetOrDefault("RearRightDoor") as bool? ?? false;
            this.SetAnimatedState("RearRightDoor", !isOpened);
        }

        [Interaction(InteractionTrigger.RightClick, "RearLeftDoor", authRequired: AccessType.ConsumerAccess, requiredEnvVars: new[] { "RearLeftDoor" }, MinCaloriesRequired = 0)]
        public void RearLeftDoor(Player player, InteractionTriggerInfo trigger, InteractionTarget target)
        {
            var isOpened = this.AnimatedStates.GetOrDefault("RearLeftDoor") as bool? ?? false;
            this.SetAnimatedState("RearLeftDoor", !isOpened);
        }
    }
}
