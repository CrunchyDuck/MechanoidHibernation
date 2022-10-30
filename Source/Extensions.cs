using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System;
using System.Linq;

namespace CrunchyDuck.MechanoidHibernation {
	static class Extensions {
		public static bool IsHibernating(this Pawn pawn) {
			return pawn.MentalStateDef == Main.hibernationDef;
		}
	}
}
