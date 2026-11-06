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
    using Eco.Shared.Math;
    using Eco.Shared.Serialization;
    using System.Collections.Generic;
    using System;
    using static Eco.Gameplay.Components.PartsComponent;

    [Serialized]
    [LocDisplayName("Bicycle")]
    [LocDescription("A bicycle that runs on leg power.")]
    [IconGroup("World Object Minimap")]
    [Weight(5000)]
    [SalvageCost(typeof(IronScrap), 3.0f, typeof(BioResidue), 0.4f, typeof(ChemicalWaste), 0.1f)]
    [Ecopedia("Crafted Objects", "Vehicles", createAsSubPage: true)]
    public partial class BicycleItem : WorldObjectItem<BicycleObject>, IPersistentData
    {
        [Serialized, SyncToView, NewTooltipChildren(CacheAs.Instance, flags: TTFlags.AllowNonControllerTypeForChildren)] public object? PersistentData { get; set; }
    }

    /// <summary>
    /// <para>Server side recipe definition for "Bicycle".</para>
    /// <para>More information about RecipeFamily objects can be found at https://docs.play.eco/api/server/eco.gameplay/Eco.Gameplay.Items.RecipeFamily.html</para>
    /// </summary>
    /// <remarks>
    /// This is an auto-generated class. Don't modify it! All your changes will be wiped with next update! Use Mods* partial methods instead for customization.
    /// If you wish to modify this class, please create a new partial class or follow the instructions in the "UserCode" folder to override the entire file.
    /// </remarks>
    [RequiresSkill(typeof(BasicEngineeringSkill), 3)]
    [Ecopedia("Crafted Objects", "Vehicles", subPageName: "Bicycle Item")]
    public partial class BicycleRecipe : RecipeFamily
    {
        public BicycleRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "Bicycle",  //noloc
                displayName: Localizer.DoStr("Bicycle"),

                // Defines the ingredients needed to craft this recipe. An ingredient items takes the following inputs
                // type of the item, the amount of the item, the skill required, and the talent used.
                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(IronBarItem), 4, typeof(BasicEngineeringSkill)),
                    new IngredientElement(typeof(LeatherHideItem), 4, typeof(BasicEngineeringSkill)),
                    new IngredientElement(typeof(BicycleFrameItem), 1, true),
                    new IngredientElement(typeof(IronChainItem), 1, true),
                    new IngredientElement(typeof(IronWheelItem), 2, true),
                    new IngredientElement(typeof(LubricantItem), 1, true),
                },

                // Define our recipe output items.
                // For every output item there needs to be one CraftingElement entry with the type of the final item and the amount
                // to create.
                items: new List<CraftingElement>
                {
                    new CraftingElement<BicycleItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 15; // Defines how much experience is gained when crafted.

            // Defines the amount of labor required and the required skill to add labor
            this.LaborInCalories = CreateLaborInCaloriesValue(175, typeof(BasicEngineeringSkill));

            // Defines our crafting time for the recipe
            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(BicycleRecipe), start: 4, skillType: typeof(BasicEngineeringSkill));

            // Perform pre/post initialization for user mods and initialize our recipe instance with the display name "Bicycle"
            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Bicycle"), recipeType: typeof(BicycleRecipe));
            this.ModsPostInitialize();

            // Register our RecipeFamily instance with the crafting system so it can be crafted.
            CraftingComponent.AddRecipe(tableType: typeof(WainwrightTableObject), this);
        }

        /// <summary>Hook for mods to customize RecipeFamily before initialization. You can change recipes, xp, labor, time here.</summary>
        partial void ModsPreInitialize();

        /// <summary>Hook for mods to customize RecipeFamily after initialization, but before registration. You can change skill requirements here.</summary>
        partial void ModsPostInitialize();
    }

    [Serialized]
    [RequireComponent(typeof(StandaloneAuthComponent))]
    [RequireComponent(typeof(PaintableComponent))]
    [RequireComponent(typeof(VehicleComponent))]
    [RequireComponent(typeof(PublicStorageComponent))]
    [RequireComponent(typeof(CustomTextComponent))]
    [RequireComponent(typeof(MinimapComponent))]
    [RequireComponent(typeof(MovableLinkComponent))]
    [RequireComponent(typeof(ModularStockpileComponent))]
    [RequireComponent(typeof(PartsComponent))]
    [RepairRequiresSkill(typeof(BasicEngineeringSkill), 3)]
    [Ecopedia("Crafted Objects", "Vehicles", subPageName: "Bicycle Item")]
    public partial class BicycleObject : PhysicsWorldObject, IRepresentsItem
    {
        static BicycleObject()
        {
            WorldObject.AddOccupancy<BicycleObject>(new List<BlockOccupancy>(0));
        }
        public override TableTextureMode TableTexture => TableTextureMode.Metal;
        public override bool PlacesBlocks            => false;
        public override LocString DisplayName { get { return Localizer.DoStr("Bicycle"); } }
        public Type RepresentedItemType { get { return typeof(BicycleItem); } }

        private BicycleObject() { }
        protected override void Initialize()
        {
            this.ModsPreInitialize();
            base.Initialize();
            this.GetComponent<CustomTextComponent>().Initialize(200);
            this.GetComponent<MinimapComponent>().InitAsMovable();
            this.GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Vehicles"));
            this.GetComponent<StockpileComponent>().Initialize(new Vector3i(2,1,2));
            this.GetComponent<PublicStorageComponent>().Initialize(4, 40000);
            this.GetComponent<PublicStorageComponent>().Storage.AddInvRestriction(new NotCarriedRestriction()); // can't store block or large items
            this.GetComponent<VehicleComponent>().Initialize(26, 3, 1, null, true);
            this.ModsPostInitialize();
                        {
                this.GetComponent<PartsComponent>().Config(() => LocString.Empty, new PartInfo[]
                {
                                        new() { TypeName = nameof(IronChainItem), Quantity = 1},
                                        new() { TypeName = nameof(IronWheelItem), Quantity = 2},
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
