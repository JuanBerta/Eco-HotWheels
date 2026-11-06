namespace CavRnMods.HotWheelsRework
{
    using System.Collections.Generic;
    using Eco.Core.Ecopedia;
    using Eco.Gameplay.EcopediaRoot;
    using Eco.Gameplay.Players;
    using Eco.Gameplay.Systems.TextLinks;
    using Eco.Shared.Localization;
    using Eco.Shared.Utils;

    /// <summary>Adds a "How to use" section to the Multi-Row Wheel Hoe's Ecopedia page (Crafted Objects, Vehicles).</summary>
    public class MultiRowWheelHoeEcopedia : IEcopediaGeneratedData
    {
        // Same page the [Ecopedia("Crafted Objects", "Vehicles", createAsSubPage: true)] attribute on the item creates.
        public IEnumerable<EcopediaPageReference> PagesWeSupplyDataFor() =>
            new EcopediaPageReference(nameof(MultiRowWheelHoeItem), "Crafted Objects", "Vehicles", EcopediaUtils.GetSubPageName(typeof(MultiRowWheelHoeItem))).SingleItemAsEnumerable();

        public LocString GetEcopediaData(Player player, EcopediaPage page)
        {
            var sb = new LocStringBuilder();
            sb.AppendLine(TextLoc.HeaderLocStr("How to use"));
            sb.AppendLineLocStr("Place the Multi-Row Wheel Hoe on the ground and grab its handles to push it.");
            sb.AppendLineLocStr("While you push it, it tills the row of three blocks in front of it, so each pass plows a strip three blocks wide. It works on dirt and other tillable ground, and costs no calories.");
            sb.AppendLineLocStr("To move it without plowing, click it while on foot to switch plowing off. Click it again to switch plowing back on.");
            sb.AppendLine();
            sb.AppendLine(TextLoc.HeaderLocStr("Crafting"));
            sb.AppendLineLocStr("Crafted at the Wainwright Table with Basic Engineering level 3.");
            return sb.ToLocString();
        }
    }
}
