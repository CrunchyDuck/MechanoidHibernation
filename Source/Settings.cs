using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System;

namespace CrunchyDuck.MechanoidHibernation {
	class Settings : ModSettings {
		public static float hibernationUsage = 0.5f;

        public override void ExposeData() {
            Scribe_Values.Look(ref hibernationUsage, "hibernationUsage", 0.5f);
            base.ExposeData();
        }
    }

    public class ModMenuThing : Mod {
        public ModMenuThing(ModContentPack content) : base(content) {
            GetSettings<Settings>();
        }

        // TODO: Button to reset values.
        public override void DoSettingsWindowContents(UnityEngine.Rect inRect) {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.Label("CD_MH_Setting_HibernationTooltip".Translate() + " " + Settings.hibernationUsage);
            Settings.hibernationUsage = listingStandard.Slider(Settings.hibernationUsage, 0f, 1f);
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory() {
            return "CD_MH_ModName".Translate();
        }
    }
}
