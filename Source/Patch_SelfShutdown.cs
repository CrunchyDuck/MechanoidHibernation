using RimWorld;
using Verse.AI;
using Verse;
using HarmonyLib;
using System.Reflection;
using System;

namespace CrunchyDuck.MechanoidHibernation {
	class Patch_SelfShutdown {
		public static MethodInfo Target() {
			return AccessTools.Method(typeof(JobGiver_SelfShutdown), "TryGiveJob");
		}

		// TODO: This doesn't trigger. Find actual hook location. Check how state is sent from the point of changing MechWorkModeDef
		public static bool Prefix(Pawn pawn) {
			if (pawn.GetMechWorkMode() == MechWorkModeDefOf.SelfShutdown) {
				pawn.mindState.mentalStateHandler.TryStartMentalState(Main.hibernationDef, transitionSilently: false);
				return true;
			}
			return true;
		}
	}
}
