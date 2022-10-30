using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System;

namespace CrunchyDuck.MechanoidHibernation {
	// TODO: make "zzz" pop up when they're hibernating.
	// TODO: Update hover tooltip to display how many mechs are hibernating.
	[StaticConstructorOnStartup]
	public class Main {
		public static MentalStateDef hibernationDef = DefDatabase<MentalStateDef>.GetNamed("CDHibernatingMechanoid");
		static Main() {
			PerformPatches();
		}

		private static void PerformPatches() {
			// What can I say, I prefer a manual method of patching.
			var harmony = new Harmony("CrunchyDuck.MechanoidHibernation");
			AddPatch(harmony, typeof(Patch_Mechanitor_UsedBandwidthFromSubjects));
			AddPatch(harmony, typeof(Patch_SelfShutdown));
			AddPatch(harmony, typeof(Patch_MechanitorBandwidthGizmo_GizmoOnGUI));
		}

		private static void AddPatch(Harmony harmony, Type type) {
			// TODO: Sometime make a patch interface.
			var prefix = type.GetMethod("Prefix") != null ? new HarmonyMethod(type, "Prefix") : null;
			var postfix = type.GetMethod("Postfix") != null ? new HarmonyMethod(type, "Postfix") : null;
			var trans = type.GetMethod("Transpiler") != null ? new HarmonyMethod(type, "Transpiler") : null;
			harmony.Patch((MethodBase)type.GetMethod("Target").Invoke(null, null), prefix: prefix, postfix: postfix, transpiler: trans);
		}
	}
}
