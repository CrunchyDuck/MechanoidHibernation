using RimWorld;
using Verse;
using Verse.Sound;
using HarmonyLib;
using System.Reflection;
using System;
using UnityEngine;

namespace CrunchyDuck.MechanoidHibernation {
	public static class ListingExtensions {
        // Stolen heartlessly from https://github.com/PeteTimesSix/ResearchReinvented/blob/main/ResearchReinvented/Source/Utilities/ListingExtensions.cs
        public static void SliderLabeled(this Listing_Standard instance, string label, ref float value, float min, float max, float roundTo = -1, float displayMult = 1, int decimalPlaces = 0, string valueSuffix = "", string tooltip = null, Action onChange = null) {
            if (!string.IsNullOrEmpty(label)) {
				Rect r = instance.GetRect(22f);
				var anchor = Text.Anchor;
                // Min
                Text.Anchor = TextAnchor.MiddleLeft;
				Widgets.Label(r, min * displayMult + valueSuffix);
                // Readout
                Text.Anchor = TextAnchor.MiddleCenter;
                TooltipHandler.TipRegion(r, tooltip);
                Widgets.Label(r, $"{label}: {(value * displayMult).ToString($"F{decimalPlaces}")}{valueSuffix}");
                // Max
                Text.Anchor = TextAnchor.MiddleRight;
                Widgets.Label(r, max * displayMult + valueSuffix);

                Text.Anchor = anchor;
            }
            var valueBefore = value;
            value = instance.FullSlider(value, min, max, roundTo: roundTo);
            if (value != valueBefore) {
                onChange?.Invoke();
            }
        }

        public static float FullSlider(this Listing_Standard instance, float val, float min, float max, float roundTo = -1f, bool middleAlignment = false, string label = null, string leftAlignedLabel = null, string rightAlignedLabel = null) {
            float newVal = Widgets.HorizontalSlider(instance.GetRect(22f), val, min, max, middleAlignment, label, leftAlignedLabel, rightAlignedLabel, roundTo);
            if (newVal != val) {
                SoundDefOf.DragSlider.PlayOneShotOnCamera(null);
            }
            instance.Gap(instance.verticalSpacing);
            return newVal;
        }
    }
}
