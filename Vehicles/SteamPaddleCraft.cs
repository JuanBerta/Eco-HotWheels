namespace CavRnMods.HotWheels
{
    using Eco.Gameplay.Components.Storage;
    using Eco.Core.Controller;
    using Eco.Core.Items;
    using Eco.Gameplay.Components.Auth;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Items.Recipes;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Occupancy;
    using Eco.Gameplay.Skills;
    using Eco.Gameplay.Systems.NewTooltip;
    using Eco.Mods.TechTree;
    using Eco.Shared.Items;
    using Eco.Shared.Localization;
    using Eco.Shared.Serialization;
    using System.Collections.Generic;
    using System;
    using static Eco.Gameplay.Components.PartsComponent;

    [Serialized]
    [LocDisplayName("Steam Paddle Craft")]
    [LocDescription("A small paddle boat propelled by a steam engine.")]
    [IconGroup("World Object Minimap")]
    [Weight(20000)]
    [SalvageCost(typeof(IronScrap), 6.0f, typeof(WoodScrap), 0.8f, typeof(BioResidue), 0.2f, typeof(CopperScrap), 1.0f)]
    [WaterPlaceable]
    [Ecopedia("Crafted Objects", "Vehicles", createAsSubPage: true)]
    public partial class SteamPaddleCraftItem : WorldObjectItem<SteamPaddleCraftObject>, IPersistentData
    {
        public float InteractDistance => DefaultInteractDistance.WaterPlacement;
        public bool ShouldHighlight(Type block) => false;
        [Serialized, SyncToView, NewTooltipChildren(CacheAs.Instance, flags: TTFlags.AllowNonControllerTypeForChildren)] public object? PersistentData { get; set; }
    }

    /// <summary>
    /// <para>Server side recipe definition for "SteamPaddleCraft".</para>
    /// <para>More information about RecipeFamily objects can be found at https://docs.play.eco/api/server/eco.gameplay/Eco.Gameplay.Items.RecipeFamily.html</para>
    /// </summary>
    /// <remarks>
    /// This is an auto-generated class. Don't modify it! All your changes will be wiped with next update! Use Mods* partial methods instead for customization.
    /// If you wish to modify this class, please create a new partial class or follow the instructions in the "UserCode" folder to override the entire file.
    /// </remarks>
    [RequiresSkill(typeof(ShipwrightSkill), 5)]
    [Ecopedia("Crafted Objects", "Vehicles", subPageName: "Steam Paddle Craft Item")]
    public partial class SteamPaddleCraftRecipe : RecipeFamily
    {
        public SteamPaddleCraftRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "SteamPaddleCraft",  //noloc
                displayName: Localizer.DoStr("Steam Paddle Craft"),

                // Defines the ingredients needed to craft this recipe. An ingredient items takes the following inputs
                // type of the item, the amount of the item, the skill required, and the talent used.
                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(IronHullSheetItem), 4, typeof(ShipwrightSkill)),
                    new IngredientElement(typeof(WoodenHullPlanksItem), 2, typeof(ShipwrightSkill)),
                    new IngredientElement(typeof(IronBarItem), 4, typeof(ShipwrightSkill)),
                    new IngredientElement(typeof(HempMooringRopeItem), 1, true),
                    new IngredientElement(typeof(WoodenRudderItem), 1, true),
                    new IngredientElement(typeof(PortableSteamEngineItem), 1, true),
                    new IngredientElement(typeof(MediumMetalShipFrameItem), 1, true),
                    new IngredientElement(typeof(LubricantItem), 1, true),
                },

                // Define our recipe output items.
                // For every output item there needs to be one CraftingElement entry with the type of the final item and the amount
                // to create.
                items: new List<CraftingElement>
                {
                    new CraftingElement<SteamPaddleCraftItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 10; // Defines how much experience is gained when crafted.

            // Defines the amount of labor required and the required skill to add labor
            this.LaborInCalories = CreateLaborInCaloriesValue(600, typeof(ShipwrightSkill));

            // Defines our crafting time for the recipe
            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(SteamPaddleCraftRecipe), start: 5, skillType: typeof(ShipwrightSkill));

            // Perform pre/post initialization for user mods and initialize our recipe instance with the display name "Steam Paddle Craft"
            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Steam Paddle Craft"), recipeType: typeof(SteamPaddleCraftRecipe));
            this.ModsPostInitialize();

            // Register our RecipeFamily instance with the crafting system so it can be crafted.
            CraftingComponent.AddRecipe(tableType: typeof(MediumShipyardObject), recipeFamily: this);
        }

        /// <summary>Hook for mods to customize RecipeFamily before initialization. You can change recipes, xp, labor, time here.</summary>
        partial void ModsPreInitialize();

        /// <summary>Hook for mods to customize RecipeFamily after initialization, but before registration. You can change skill requirements here.</summary>
        partial void ModsPostInitialize();
    }

    [Serialized]
    [RequireComponent(typeof(StandaloneAuthComponent))]
    [RequireComponent(typeof(FuelSupplyComponent))]
    [RequireComponent(typeof(FuelConsumptionComponent))]
    [RequireComponent(typeof(PublicStorageComponent))]
    [RequireComponent(typeof(MovableLinkComponent))]
    [RequireComponent(typeof(VehicleComponent))]
    [RequireComponent(typeof(BoatComponent))]
    [RequireComponent(typeof(MinimapComponent))]
    [RequireComponent(typeof(PartsComponent))]
    [RepairRequiresSkill(typeof(ShipwrightSkill), 5)]
    [Ecopedia("Crafted Objects", "Vehicles", subPageName: "Steam Paddle Craft Item")]
    public partial class SteamPaddleCraftObject : PhysicsWorldObject, IRepresentsItem
    {
        static SteamPaddleCraftObject()
        {
            WorldObject.AddOccupancy<SteamPaddleCraftObject>(new List<BlockOccupancy>(0));
        }
        public override float InteractDistance => DefaultInteractDistance.WaterPlacement;
        public override TableTextureMode TableTexture => TableTextureMode.Wood;
        public override bool PlacesBlocks            => false;
        public override LocString DisplayName { get { return Localizer.DoStr("Steam Paddle Craft"); } }
        public Type RepresentedItemType { get { return typeof(SteamPaddleCraftItem); } }

        private static string[] fuelTagList = new string[]
        {
            "Burnable Fuel",
        };
        private SteamPaddleCraftObject() { }
        protected override void Initialize()
        {
            this.ModsPreInitialize();
            base.Initialize();
            this.GetComponent<FuelSupplyComponent>().Initialize(1, fuelTagList);
            this.GetComponent<FuelConsumptionComponent>().Initialize(800);
            this.GetComponent<VehicleComponent>().HumanPowered(0.25f);
            this.GetComponent<PublicStorageComponent>().Initialize(2, 50000);
            this.GetComponent<PublicStorageComponent>().Storage.AddInvRestriction(new NotCarriedRestriction());
            this.GetComponent<MinimapComponent>().InitAsMovable();
            this.GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Vehicles"));
            this.GetComponent<VehicleComponent>().Initialize(16, 2, 1, null, true);
            this.GetComponent<BoatComponent>().Size = BoatComponent.BoatSize.Small;
            this.GetComponent<VehicleComponent>().FailDriveMsg = Localizer.Do($"You are too hungry to drive {this.DisplayName}!");
            this.ModsPostInitialize();
                        {
                this.GetComponent<PartsComponent>().Config(() => LocString.Empty, new PartInfo[]
                {
                                        new() { TypeName = nameof(IronHullSheetItem), Quantity = 2},
                                        new() { TypeName = nameof(PortableSteamEngineItem), Quantity = 1},
                                        new() { TypeName = nameof(LubricantItem), Quantity = 1},
                                    });
            }
        }

        /// <summary>Hook for mods to customize before initialization. You can change housing values here.</summary>
        partial void ModsPreInitialize();
        /// <summary>Hook for mods to customize after initialization.</summary>
        partial void ModsPostInitialize();
    }
}
