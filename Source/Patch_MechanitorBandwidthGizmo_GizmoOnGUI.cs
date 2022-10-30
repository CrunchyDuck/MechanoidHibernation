using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace CrunchyDuck.MechanoidHibernation {
	// This patch adds in the blue squares when a mech is hibernating.
	class Patch_MechanitorBandwidthGizmo_GizmoOnGUI {
		private static readonly Color EmptyBlockColor = new Color(0.3f, 0.3f, 0.3f, 1f);
		private static readonly Color FilledBlockColor = ColorLibrary.Yellow;
		private static readonly Color ExcessBlockColor = ColorLibrary.Red;
		private static readonly Color HibernatingBlockColor = ColorLibrary.Blue;
		private static Pawn_MechanitorTracker tracker;
		private static FieldInfo getTracker = AccessTools.Field(typeof(MechanitorBandwidthGizmo), "tracker");
		
		// blocks used for each thing
		private static int numGestating;
		private static int numHibernatingInt;
		private static float numHibernating;
		private static int numControlledNotHibernating;

		public static MethodInfo Target() {
			return AccessTools.Method(typeof(MechanitorBandwidthGizmo), "GizmoOnGUI");
		}

		public static void Prefix(MechanitorBandwidthGizmo __instance) {
			tracker = (Pawn_MechanitorTracker)getTracker.GetValue(__instance);
			var ps = tracker.ControlledPawns;
			numGestating = tracker.UsedBandwidthFromGestation;
			numHibernating = ps.Where(p => p.IsHibernating()).Sum(p => p.GetStatValue(StatDefOf.BandwidthCost) * Settings.hibernationUsage);
			numHibernatingInt = Mathf.CeilToInt(numHibernating);
			numControlledNotHibernating = Mathf.CeilToInt(ps.Where(p => !p.IsHibernating()).Sum(p => p.GetStatValue(StatDefOf.BandwidthCost)));
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			//.locals init(
			//  [0] valuetype[UnityEngine.CoreModule]UnityEngine.Rect rect2,
			// [1] int32 totalBandwidth,
			// [2] int32 usedBandwidth,
			// [3] string label,
			// [4] valuetype Verse.TaggedString tip,
			// [5] int32 bandwidthFromSubjects,
			// [6] int32 bandwidthFromGestation1,
			// [7] valuetype[UnityEngine.CoreModule]UnityEngine.Rect rect3,
			//[8] int32 num1,
			//[9] valuetype[UnityEngine.CoreModule]UnityEngine.Rect rect4,
		 //  [10] int32 num2,
		 //  [11] int32 num3,
		 //  [12] int32 num4,
		 //  [13] int32 num5,
		 //  [14] int32 num6,
		 //  [15] int32 num7,
		 //  [16] float32 num8,
		 //  [17] int32 num9,
		 //  [18] int32 bandwidthFromGestation2,
		 //  [19] int32 margin,
		 //  [20] class [mscorlib] System.Collections.Generic.IEnumerable`1<string> V_20,
			//  [21] class [mscorlib] System.Collections.Generic.IEnumerable`1<string> V_21,
			//  [22]
			//	int32 index1,
			//  [23] int32 index2,
			//  [24] valuetype[UnityEngine.CoreModule] UnityEngine.Rect rect5
			//)
			var codes = new List<CodeInstruction>(instructions);
			MethodInfo transpiler_target = AccessTools.Method(typeof(Widgets), "DrawRectFast");
			MethodInfo call_draw = AccessTools.Method(typeof(Patch_MechanitorBandwidthGizmo_GizmoOnGUI), "DrawSquares");

			for (var i = 0; i < codes.Count; i++) {
				CodeInstruction code = codes[i];
				if (code.Calls(transpiler_target)) {
					var pos = i - 6;
					codes[pos-1].Branches(out Label? branch_target);
					
					codes.Insert(pos++, new CodeInstruction(OpCodes.Ldloc, 17));
					codes.Insert(pos++, new CodeInstruction(OpCodes.Ldloc, 19));
					codes.Insert(pos++, new CodeInstruction(OpCodes.Ldloc, 24));
					codes.Insert(pos++, new CodeInstruction(OpCodes.Call, call_draw));
					codes.Insert(pos++, new CodeInstruction(OpCodes.Br, branch_target));
					break;
				}
			}
			return codes.AsEnumerable();
		}


		// This is bad practice. Too bad! I'll fix it if it matters.
		public static void DrawSquares(int index, int margin, Rect rect5) {
			int totalBandwidth = tracker.TotalBandwidth;
			if (index <= numGestating) {
				Widgets.DrawRectFast(rect5, EmptyBlockColor);
				Widgets.DrawRectFast(rect5.ContractedBy(margin), FilledBlockColor);
			}
			else if (index - numGestating <= numHibernatingInt) {
				Widgets.DrawRectFast(rect5, index <= totalBandwidth ? HibernatingBlockColor : ExcessBlockColor);
			}
			else if (index - numGestating - numHibernatingInt <= numControlledNotHibernating) {
				Widgets.DrawRectFast(rect5, index <= totalBandwidth ? FilledBlockColor : ExcessBlockColor);
			}
			else
				Widgets.DrawRectFast(rect5, EmptyBlockColor);
		}
	}
}
