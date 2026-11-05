namespace CavRnMods.HotWheels
{
    using Eco.Core.Items;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Items.Recipes;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Skills;
    using Eco.Mods.TechTree;
    using Eco.Shared.Localization;
    using Eco.Shared.Serialization;
    using System.Collections.Generic;

    /// <summary>
    /// <para>Server side recipe definition for "BicycleFrame".</para>
    /// <para>More information about RecipeFamily objects can be found at https://docs.play.eco/api/server/eco.gameplay/Eco.Gameplay.Items.RecipeFamily.html</para>
    /// </summary>
    /// <remarks>
    /// This is an auto-generated class. Don't modify it! All your changes will be wiped with next update! Use Mods* partial methods instead for customization.
    /// If you wish to modify this class, please create a new partial class or follow the instructions in the "UserCode" folder to override the entire file.
    /// </remarks>
    [RequiresSkill(typeof(BlacksmithSkill), 2)]
    [Ecopedia("Items", "Products", subPageName: "Bicycle Frame Item")]
    public partial class BicycleFrameRecipe : RecipeFamily
    {
        public BicycleFrameRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "BicycleFrame",  //noloc
                displayName: Localizer.DoStr("Bicycle Frame"),

                // Defines the ingredients needed to craft this recipe. An ingredient items takes the following inputs
                // type of the item, the amount of the item, the skill required, and the talent used.
                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(IronBarItem), 6, typeof(BlacksmithSkill)),
                    new IngredientElement(typeof(IronPipeItem), 4, typeof(BlacksmithSkill)),
                },

                // Define our recipe output items.
                // For every output item there needs to be one CraftingElement entry with the type of the final item and the amount
                // to create.
                items: new List<CraftingElement>
                {
                    new CraftingElement<BicycleFrameItem>(1)
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 5; // Defines how much experience is gained when crafted.

            // Defines the amount of labor required and the required skill to add labor
            this.LaborInCalories = CreateLaborInCaloriesValue(150, typeof(BlacksmithSkill));

            // Defines our crafting time for the recipe
            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(BicycleFrameRecipe), start: 3f, skillType: typeof(BlacksmithSkill));

            // Perform pre/post initialization for user mods and initialize our recipe instance with the display name "BicycleFrame"
            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("BicycleFrame"), recipeType: typeof(BicycleFrameRecipe));
            this.ModsPostInitialize();

            // Register our RecipeFamily instance with the crafting system so it can be crafted.
            CraftingComponent.AddRecipe(tableType: typeof(BlacksmithTableObject), this);
        }

        /// <summary>Hook for mods to customize RecipeFamily before initialization. You can change recipes, xp, labor, time here.</summary>
        partial void ModsPreInitialize();

        /// <summary>Hook for mods to customize RecipeFamily after initialization, but before registration. You can change skill requirements here.</summary>
        partial void ModsPostInitialize();
    }

    /// <summary>
    /// <para>Server side item definition for the "BicycleFrame" item.</para>
    /// <para>More information about Item objects can be found at https://docs.play.eco/api/server/eco.gameplay/Eco.Gameplay.Items.Item.html</para>
    /// </summary>
    /// <remarks>
    /// This is an auto-generated class. Don't modify it! All your changes will be wiped with next update! Use Mods* partial methods instead for customization.
    /// If you wish to modify this class, please create a new partial class or follow the instructions in the "UserCode" folder to override the entire file.
    /// </remarks>
    [Serialized] // Tells the save/load system this object needs to be serialized.
    [LocDisplayName("Bicycle Frame")] // Defines the localized name of the item.
    [Weight(100)] // Defines how heavy BicycleFrame is.
    [SalvageCost(typeof(IronScrap), 3.0f)]
    [Ecopedia("Items", "Products", createAsSubPage: true)]
    [LocDescription("Bicycle Frame is the core of a bicycle.")] //The tooltip description for the item.
    public partial class BicycleFrameItem : Item
    {
    }
}
