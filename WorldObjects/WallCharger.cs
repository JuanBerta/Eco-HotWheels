namespace CavRnMods.HotWheels
{
    using Eco.Core.Items;
    using Eco.Gameplay.Components.Auth;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Items.Recipes;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Occupancy;
    using Eco.Gameplay.Skills;
    using Eco.Mods.TechTree;
    using Eco.Shared.Items;
    using Eco.Shared.Localization;
    using Eco.Shared.Math;
    using Eco.Shared.Serialization;
    using System.Collections.Generic;
    using System;

    [Serialized]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(OccupancyRequirementComponent))]
    [RequireComponent(typeof(ForSaleComponent))]
    [RequireComponent(typeof(PowerGridComponent))]
    [RequireComponent(typeof(ChargerComponent))]
    [RequireComponent(typeof(ChargerControlsComponent))]
    [RequireComponent(typeof(OnOffComponent))]
    [Tag("Usable")]
    public partial class WallChargerObject : WorldObject, IRepresentsItem
    {
        public virtual Type RepresentedItemType => typeof(WallChargerItem);
        public override LocString DisplayName => Localizer.DoStr("Wall Charger");
        public override TableTextureMode TableTexture => TableTextureMode.Metal;

        protected override void Initialize()
        {
            this.GetComponent<PowerGridComponent>().Initialize(10, new ElectricPower());
            this.GetComponent<ChargerComponent>().Initialize(3, 2350f, false, true);
        }
    }

    [Serialized]
    [LocDisplayName("Wall Charger")]
    [LocDescription("An electric vehicle charging station.")]
    [Weight(1000)] // Defines how heavy WallCharger is.
    [SalvageCost(typeof(CopperScrap), 20.0f, typeof(GoldScrap), 0.7f, typeof(IronScrap), 6.0f, typeof(PlasticScrap), 10.0f)]
    public partial class WallChargerItem : WorldObjectItem<WallChargerObject>
    {
        protected override OccupancyContext GetOccupancyContext => new SideAttachedContext( 0  | DirectionAxisFlags.Backward , WorldObject.GetOccupancyInfo(this.WorldObjectType));
    }

    /// <summary>
    /// <para>Server side recipe definition for "WallCharger".</para>
    /// <para>More information about RecipeFamily objects can be found at https://docs.play.eco/api/server/eco.gameplay/Eco.Gameplay.Items.RecipeFamily.html</para>
    /// </summary>
    /// <remarks>
    /// This is an auto-generated class. Don't modify it! All your changes will be wiped with next update! Use Mods* partial methods instead for customization.
    /// If you wish to modify this class, please create a new partial class or follow the instructions in the "UserCode" folder to override the entire file.
    /// </remarks>
    [RequiresSkill(typeof(ElectronicsSkill), 1)]
    public partial class WallChargerRecipe : RecipeFamily
    {
        public WallChargerRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "WallCharger",  //noloc
                displayName: Localizer.DoStr("Wall Charger"),

                // Defines the ingredients needed to craft this recipe. An ingredient items takes the following inputs
                // type of the item, the amount of the item, the skill required, and the talent used.
                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(SteelPlateItem), 4, typeof(ElectronicsSkill)),
                    new IngredientElement(typeof(BasicCircuitItem), 1, typeof(ElectronicsSkill)),
                    new IngredientElement(typeof(PlasticItem), 10, typeof(ElectronicsSkill)),
                    new IngredientElement(typeof(CopperWiringItem), 20, typeof(ElectronicsSkill)),
                    new IngredientElement(typeof(HeatSinkItem), 2, typeof(ElectronicsSkill)),
                    new IngredientElement(typeof(AdvancedCircuitItem), 1, typeof(ElectronicsSkill)),
                },

                // Define our recipe output items.
                // For every output item there needs to be one CraftingElement entry with the type of the final item and the amount
                // to create.
                items: new List<CraftingElement>
                {
                    new CraftingElement<WallChargerItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 1; // Defines how much experience is gained when crafted.

            // Defines the amount of labor required and the required skill to add labor
            this.LaborInCalories = CreateLaborInCaloriesValue(60, typeof(ElectronicsSkill));

            // Defines our crafting time for the recipe
            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(WallChargerRecipe), start: 5, skillType: typeof(ElectronicsSkill));

            // Perform pre/post initialization for user mods and initialize our recipe instance with the display name "Wall Charger"
            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Wall Charger"), recipeType: typeof(WallChargerRecipe));
            this.ModsPostInitialize();

            // Register our RecipeFamily instance with the crafting system so it can be crafted.
            CraftingComponent.AddRecipe(tableType: typeof(ElectricMachinistTableObject), recipeFamily: this);
        }

        /// <summary>Hook for mods to customize RecipeFamily before initialization. You can change recipes, xp, labor, time here.</summary>
        partial void ModsPreInitialize();

        /// <summary>Hook for mods to customize RecipeFamily after initialization, but before registration. You can change skill requirements here.</summary>
        partial void ModsPostInitialize();
    }
}
