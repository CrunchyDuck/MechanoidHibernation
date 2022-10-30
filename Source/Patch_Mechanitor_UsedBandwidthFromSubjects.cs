using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System;
using System.Linq;

namespace CrunchyDuck.MechanoidHibernation {
	class Patch_Mechanitor_UsedBandwidthFromSubjects {
		public static MethodInfo Target() {
			return AccessTools.PropertyGetter(typeof(Pawn_MechanitorTracker), "UsedBandwidthFromSubjects");
		}

		public static bool Prefix(ref int __result, Pawn_MechanitorTracker __instance) {
			float i = 0;
			foreach (Pawn p in __instance.OverseenPawns) {
				if (p.IsGestating())
					continue;
				// Are they hibernating?
				float val = p.GetStatValue(StatDefOf.BandwidthCost);
				if (p.IsHibernating())
					val *= Settings.hibernationUsage;
				i += val;
			}
			__result = UnityEngine.Mathf.CeilToInt(i);
			return false;
		}
	}
}
