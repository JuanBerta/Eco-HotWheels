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
    using Eco.Shared.Math;
    using Eco.Shared.Serialization;
    using Eco.Shared.SharedTypes;
    using System.Collections.Generic;
    using System;
    using static Eco.Gameplay.Components.PartsComponent;

    [Serialized]
    [LocDisplayName("School Bus")]
    [LocDescription("Modern school bus for public transportation.")]
    [IconGroup("World Object Minimap")]
    [Weight(25000)]
    [SalvageCost(typeof(IronScrap), 16.0f, typeof(Textiles), 2.4f, typeof(CopperScrap), 6.0f, typeof(ChemicalWaste), 4.0f)]
    [AirPollution(0.5f)]
    [Ecopedia("Crafted Objects", "Vehicles", createAsSubPage: true)]
    public partial class SchoolBusItem : WorldObjectItem<SchoolBusObject>, IPersistentData
    {
        [Serialized, SyncToView, NewTooltipChildren(CacheAs.Instance, flags: TTFlags.AllowNonControllerTypeForChildren)] public object? PersistentData { get; set; }
    }

    /// <summary>
    /// <para>Server side recipe definition for "SchoolBus".</para>
    /// <para>More information about RecipeFamily objects can be found at https://docs.play.eco/api/server/eco.gameplay/Eco.Gameplay.Items.RecipeFamily.html</para>
    /// </summary>
    /// <remarks>
    /// This is an auto-generated class. Don't modify it! All your changes will be wiped with next update! Use Mods* partial methods instead for customization.
    /// If you wish to modify this class, please create a new partial class or follow the instructions in the "UserCode" folder to override the entire file.
    /// </remarks>
    [RequiresSkill(typeof(IndustrySkill), 2)]
    [Ecopedia("Crafted Objects", "Vehicles", subPageName: "School Bus Item")]
    public partial class SchoolBusRecipe : RecipeFamily
    {
        public SchoolBusRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "SchoolBus",  //noloc
                displayName: Localizer.DoStr("School Bus"),

                // Defines the ingredients needed to craft this recipe. An ingredient items takes the following inputs
                // type of the item, the amount of the item, the skill required, and the talent used.
                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(GearboxItem), 10, typeof(IndustrySkill)),
                    new IngredientElement(typeof(SteelPlateItem), 50, typeof(IndustrySkill)),
                    new IngredientElement(typeof(NylonFabricItem), 40, typeof(IndustrySkill)),
                    new IngredientElement(typeof(SteelSpringItem), 15, typeof(IndustrySkill)),
                    new IngredientElement(typeof(CombustionEngineItem), 2, true),
                    new IngredientElement(typeof(RubberWheelItem), 8, true),
                    new IngredientElement(typeof(RadiatorItem), 2, true),
                    new IngredientElement(typeof(SteelAxleItem), 4, true),
                    new IngredientElement(typeof(LightBulbItem), 12, true),
                    new IngredientElement(typeof(LubricantItem), 20, true),
                },

                // Define our recipe output items.
                // For every output item there needs to be one CraftingElement entry with the type of the final item and the amount
                // to create.
                items: new List<CraftingElement>
                {
                    new CraftingElement<SchoolBusItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 18; // Defines how much experience is gained when crafted.

            // Defines the amount of labor required and the required skill to add labor
            this.LaborInCalories = CreateLaborInCaloriesValue(2000, typeof(IndustrySkill));

            // Defines our crafting time for the recipe
            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(SchoolBusRecipe), start: 10, skillType: typeof(IndustrySkill));

            // Perform pre/post initialization for user mods and initialize our recipe instance with the display name "SchoolBus"
            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("School Bus"), recipeType: typeof(SchoolBusRecipe));
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
    [RequireComponent(typeof(FuelSupplyComponent))]
    [RequireComponent(typeof(FuelConsumptionComponent))]
    [RequireComponent(typeof(PublicStorageComponent))]
    [RequireComponent(typeof(MovableLinkComponent))]
    [RequireComponent(typeof(AirPollutionComponent))]
    [RequireComponent(typeof(VehicleComponent))]
    [RequireComponent(typeof(CustomTextComponent))]
    [RequireComponent(typeof(ModularStockpileComponent))]
    [RequireComponent(typeof(MinimapComponent))]
    [RequireComponent(typeof(PartsComponent))]
    [RepairRequiresSkill(typeof(IndustrySkill), 2)]
    [Ecopedia("Crafted Objects", "Vehicles", subPageName: "School Bus Item")]
    public partial class SchoolBusObject : PhysicsWorldObject, IRepresentsItem, IHasInteractions
    {
        static SchoolBusObject()
        {
            WorldObject.AddOccupancy<SchoolBusObject>(new List<BlockOccupancy>(0));
        }
        public override TableTextureMode TableTexture => TableTextureMode.Metal;
        public override bool PlacesBlocks            => false;
        public override LocString DisplayName { get { return Localizer.DoStr("School Bus"); } }
        public Type RepresentedItemType { get { return typeof(SchoolBusItem); } }



        private static string[] fuelTagList = new string[]
        {
            "Liquid Fuel",
        };
        private SchoolBusObject() { }
        protected override void Initialize()
        {
            base.Initialize();
            this.GetComponent<CustomTextComponent>().Initialize(200);
            this.GetComponent<FuelSupplyComponent>().Initialize(2, fuelTagList);
            this.GetComponent<FuelSupplyComponent>().Inventory.AddInvRestriction(new StackLimitRestriction(40));
            this.GetComponent<FuelConsumptionComponent>().Initialize(1000);
            this.GetComponent<AirPollutionComponent>().Initialize(2.5f);
            this.GetComponent<StockpileComponent>().Initialize(new Vector3i(2,2,3));
            this.GetComponent<PublicStorageComponent>().Initialize(72, 40000000);
            this.GetComponent<MinimapComponent>().InitAsMovable();
            this.GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Vehicles"));
            this.GetComponent<VehicleComponent>().Initialize(20, 4, 29);
            {
                this.GetComponent<PartsComponent>().Config(() => LocString.Empty, new PartInfo[]
                {
                                        new() { TypeName = nameof(CombustionEngineItem), Quantity = 2},
                                        new() { TypeName = nameof(RubberWheelItem), Quantity = 8},
                                        new() { TypeName = nameof(LightBulbItem), Quantity = 12},
                                        new() { TypeName = nameof(LubricantItem), Quantity = 20},
                                    });
            }
        }

        [Interaction(InteractionTrigger.RightClick, "Door", authRequired: AccessType.ConsumerAccess, requiredEnvVars: new[] { "Door" })]
        public void Door(Player player, InteractionTriggerInfo trigger, InteractionTarget target)
        {
            var isOpened = this.AnimatedStates.GetOrDefault("Door") as bool? ?? false;
            this.SetAnimatedState("Door", !isOpened);
        }
    }
}
