namespace CavRnMods.HotWheels
{
    using Eco.Core.Items;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.DynamicValues;
    using Eco.Gameplay.Items.Recipes;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Skills;
    using Eco.Mods.TechTree;
    using Eco.Shared.Localization;
    using Eco.Shared.Serialization;
    using System.Collections.Generic;

    /// <summary>
    /// <para>Server side recipe definition for "IronChain".</para>
    /// <para>More information about RecipeFamily objects can be found at https://docs.play.eco/api/server/eco.gameplay/Eco.Gameplay.Items.RecipeFamily.html</para>
    /// </summary>
    /// <remarks>
    /// This is an auto-generated class. Don't modify it! All your changes will be wiped with next update! Use Mods* partial methods instead for customization.
    /// If you wish to modify this class, please create a new partial class or follow the instructions in the "UserCode" folder to override the entire file.
    /// </remarks>
    [RequiresSkill(typeof(BlacksmithSkill), 2)]
    [Ecopedia("Items", "Products", subPageName: "Iron Chain Item")]
    public partial class IronChainRecipe : RecipeFamily
    {
        public IronChainRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "IronChain",  //noloc
                displayName: Localizer.DoStr("Iron Chain"),

                // Defines the ingredients needed to craft this recipe. An ingredient items takes the following inputs
                // type of the item, the amount of the item, the skill required, and the talent used.
                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(IronBarItem), 4, typeof(BlacksmithSkill)), //noloc
                },

                // Define our recipe output items.
                // For every output item there needs to be one CraftingElement entry with the type of the final item and the amount
                // to create.
                items: new List<CraftingElement>
                {
                    new CraftingElement<IronChainItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 1; // Defines how much experience is gained when crafted.

            // Defines the amount of labor required and the required skill to add labor
            this.LaborInCalories = CreateLaborInCaloriesValue(50, typeof(BlacksmithSkill));

            // Defines our crafting time for the recipe
            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(IronChainRecipe), start: 1, skillType: typeof(BlacksmithSkill));

            // Perform pre/post initialization for user mods and initialize our recipe instance with the display name "Iron Chain"
            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Iron Chain"), recipeType: typeof(IronChainRecipe));
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
    /// <para>Server side item definition for the "IronChain" item.</para>
    /// <para>More information about PartItem objects can be found at https://docs.play.eco/api/server/eco.gameplay/Eco.Gameplay.Items.PartItem.html</para>
    /// </summary>
    /// <remarks>
    /// This is an auto-generated class. Don't modify it! All your changes will be wiped with next update! Use Mods* partial methods instead for customization.
    /// If you wish to modify this class, please create a new partial class or follow the instructions in the "UserCode" folder to override the entire file.
    /// </remarks>
    [Serialized] // Tells the save/load system this object needs to be serialized.
    [LocDisplayName("Iron Chain")] // Defines the localized name of the item.
    [Weight(500)] // Defines how heavy IronChain is.
    [SalvageCost(typeof(IronScrap), 0.8f)]
    [RepairRequiresSkill(typeof(BlacksmithSkill), 0)]
    [Ecopedia("Items", "Products", createAsSubPage: true)]
    [LocDescription("An iron chain used in some two-wheeled vehicles and as a part.")] //The tooltip description for the item.
    public partial class IronChainItem : PartItem    {
        private static SkillModifiedValue skilledRepairCost = new SkillModifiedValue(2, BlacksmithSkill.MultiplicativeStrategy, typeof(BlacksmithSkill), typeof(IronChainItem), Localizer.DoStr("repair cost"), DynamicValueType.Efficiency);

        public override IDynamicValue SkilledRepairCost => skilledRepairCost;
        public override int FullRepairAmount            => 2;
        public float ReduceMaxDurabilityByPercent       => 0.05f;

        // This handles multiple repair elements and how much reduction in cost of the material type
        // meaning 1 = full cost and .1 = 10% of the total cost for 100% repair.
        public override IEnumerable<RepairingItem> RepairItems {get
        {
                    yield return new() { Item = Item.Get("IronBarItem"), MaterialMult = 1 };
        } }
    }
}
