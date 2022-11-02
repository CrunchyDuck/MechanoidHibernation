using RimWorld;
using Verse.AI;
using Verse;
using System.Reflection;
using HarmonyLib;

namespace CrunchyDuck.MechanoidHibernation {
	class MentalState_Hibernation : MentalState {
		protected override bool CanEndBeforeMaxDurationNow => false;

        private Pawn_MechanitorTracker owner;
		private float myPowerDraw = 1;
		private MechWorkModeDef lastWorkMode;

		public override void MentalStateTick() {
			base.MentalStateTick();
			if (owner == null) {
				GetData();
				if (owner == null) {
					Log.Error("Could not find hibernating mechanoid's owner. Removing from hibernation.");
					RecoverFromState();
				}
			}

			// Trying to take mech out of hibernation.
			var work_mode = pawn.GetMechWorkMode();
			if (work_mode != MechWorkModeDefOf.SelfShutdown) {
				// Has enough power to do so.
				var owner_bw = owner.TotalBandwidth - owner.UsedBandwidth + (myPowerDraw / 2);
				if (owner_bw >= myPowerDraw) {
					RecoverFromState();
				}
				// Inform the player they cannot do this :)
				else if (lastWorkMode != work_mode) {
					lastWorkMode = work_mode;
					var s = "CD_MH_NotEnoughBandwidth".Translate(owner.Pawn.LabelShort, pawn.LabelShort);
					Messages.Message(s, pawn, MessageTypeDefOf.NegativeEvent);
					
				}
			}
        }

		public override void PreStart() {
			base.PreStart();
			GetData();
		}

		public void GetData() {
			owner = pawn.relations?.GetFirstDirectRelationPawn(PawnRelationDefOf.Overseer).mechanitor;
			myPowerDraw = pawn.GetStatValue(StatDefOf.BandwidthCost);
			lastWorkMode = owner.GetControlGroup(pawn).WorkMode;
		}
	}
}
