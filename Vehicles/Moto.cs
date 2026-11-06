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
    [LocDisplayName("Moto")]
    [LocDescription("A motorcycle that runs on steam.")]
    [IconGroup("World Object Minimap")]
    [Weight(10000)]
    [SalvageCost(typeof(BioResidue), 0.4f, typeof(IronScrap), 8.0f, typeof(Textiles), 0.6f, typeof(CopperScrap), 2.0f, typeof(ChemicalWaste), 1.0f)]
    [Ecopedia("Crafted Objects", "Vehicles", createAsSubPage: true)]
    public partial class MotoItem : WorldObjectItem<MotoObject>, IPersistentData
    {
        [Serialized, SyncToView, NewTooltipChildren(CacheAs.Instance, flags: TTFlags.AllowNonControllerTypeForChildren)] public object? PersistentData { get; set; }
    }

    /// <summary>
    /// <para>Server side recipe definition for "Moto".</para>
    /// <para>More information about RecipeFamily objects can be found at https://docs.play.eco/api/server/eco.gameplay/Eco.Gameplay.Items.RecipeFamily.html</para>
    /// </summary>
    /// <remarks>
    /// This is an auto-generated class. Don't modify it! All your changes will be wiped with next update! Use Mods* partial methods instead for customization.
    /// If you wish to modify this class, please create a new partial class or follow the instructions in the "UserCode" folder to override the entire file.
    /// </remarks>
    [RequiresSkill(typeof(MechanicsSkill), 2)]
    [Ecopedia("Crafted Objects", "Vehicles", subPageName: "Moto Item")]
    public partial class MotoRecipe : RecipeFamily
    {
        public MotoRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "Moto",  //noloc
                displayName: Localizer.DoStr("Moto"),

                // Defines the ingredients needed to craft this recipe. An ingredient items takes the following inputs
                // type of the item, the amount of the item, the skill required, and the talent used.
                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(IronPlateItem), 6, typeof(MechanicsSkill)),
                    new IngredientElement(typeof(IronPipeItem), 4, typeof(MechanicsSkill)),
                    new IngredientElement(typeof(ScrewsItem), 6, typeof(MechanicsSkill)),
                    new IngredientElement(typeof(LeatherHideItem), 12, typeof(MechanicsSkill)),
                    new IngredientElement(typeof(PortableSteamEngineItem), 1, true),
                    new IngredientElement(typeof(IronWheelItem), 2, true),
                    new IngredientElement(typeof(IronChainItem), 1, true),
                    new IngredientElement(typeof(LightBulbItem), 1, true),
                    new IngredientElement(typeof(LubricantItem), 2, true),
                },

                // Define our recipe output items.
                // For every output item there needs to be one CraftingElement entry with the type of the final item and the amount
                // to create.
                items: new List<CraftingElement>
                {
                    new CraftingElement<MotoItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 15; // Defines how much experience is gained when crafted.

            // Defines the amount of labor required and the required skill to add labor
            this.LaborInCalories = CreateLaborInCaloriesValue(750, typeof(MechanicsSkill));

            // Defines our crafting time for the recipe
            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(MotoRecipe), start: 5, skillType: typeof(MechanicsSkill));

            // Perform pre/post initialization for user mods and initialize our recipe instance with the display name "Moto"
            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Moto"), recipeType: typeof(MotoRecipe));
            this.ModsPostInitialize();

            // Register our RecipeFamily instance with the crafting system so it can be crafted.
            CraftingComponent.AddRecipe(tableType: typeof(AssemblyLineObject), this);
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
    [RepairRequiresSkill(typeof(MechanicsSkill), 2)]
    [Ecopedia("Crafted Objects", "Vehicles", subPageName: "Moto Item")]
    public partial class MotoObject : PhysicsWorldObject, IRepresentsItem
    {
        static MotoObject()
        {
            WorldObject.AddOccupancy<MotoObject>(new List<BlockOccupancy>(0));
        }
        public override TableTextureMode TableTexture => TableTextureMode.Metal;
        public override bool PlacesBlocks            => false;
        public override LocString DisplayName { get { return Localizer.DoStr("Moto"); } }
        public Type RepresentedItemType { get { return typeof(MotoItem); } }

        private static string[] fuelTagList = new string[]
        {
            "Burnable Fuel",
        };
        private MotoObject() { }
        protected override void Initialize()
        {
            this.ModsPreInitialize();
            base.Initialize();
            this.GetComponent<CustomTextComponent>().Initialize(200);
            this.GetComponent<FuelSupplyComponent>().Initialize(2, fuelTagList);
            this.GetComponent<FuelConsumptionComponent>().Initialize(100);
            this.GetComponent<AirPollutionComponent>().Initialize(0.1f);
            this.GetComponent<StockpileComponent>().Initialize(new Vector3i(2,1,2));
            this.GetComponent<PublicStorageComponent>().Initialize(4, 80000);
            this.GetComponent<PublicStorageComponent>().Storage.AddInvRestriction(new NotCarriedRestriction()); // can't store block or large items
            this.GetComponent<MinimapComponent>().InitAsMovable();
            this.GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Vehicles"));
            this.GetComponent<VehicleComponent>().Initialize(36, 4, 2);
            this.ModsPostInitialize();
                        {
                this.GetComponent<PartsComponent>().Config(() => LocString.Empty, new PartInfo[]
                {
                                        new() { TypeName = nameof(PortableSteamEngineItem), Quantity = 1},
                                        new() { TypeName = nameof(IronWheelItem), Quantity = 2},
                                        new() { TypeName = nameof(IronChainItem), Quantity = 1},
                                        new() { TypeName = nameof(LightBulbItem), Quantity = 1},
                                        new() { TypeName = nameof(LubricantItem), Quantity = 2},
                                    });
            }
        }

        /// <summary>Hook for mods to customize before initialization. You can change housing values here.</summary>
        partial void ModsPreInitialize();
        /// <summary>Hook for mods to customize after initialization.</summary>
        partial void ModsPostInitialize();
    }
}
