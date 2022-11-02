using RimWorld;
using Verse.AI;
using Verse;
using System.Reflection;
using HarmonyLib;

namespace CrunchyDuck.MechanoidHibernation {
	class Patch_GetStatDef {
		public static MethodInfo Target() {
			return AccessTools.Method(typeof(StatExtension), "GetStatValue");
		}

		public static void Postfix(ref float __result, Thing thing, StatDef stat) {
			if (stat == StatDefOf.BandwidthCost) {
				Pawn p = (Pawn)thing;
				if (p.IsHibernating())
					__result *= Settings.hibernationUsage;
			}
		}
	}
}
