using RimWorld;
using Verse.AI;
using Verse;

namespace CrunchyDuck.MechanoidHibernation {
	class MentalState_Hibernation : MentalState {
		protected override bool CanEndBeforeMaxDurationNow => false;

        private Pawn_MechanitorTracker owner;
		private float myPowerDraw = 1;
		private MechWorkModeDef lastWorkMode;

		public override void MentalStateTick() {
			base.MentalStateTick();
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
					var s = "{0} does not have enough bandwidth to wake {1} up from hibernation.".Formatted(owner.Pawn.LabelShort, pawn.LabelShort);
					Messages.Message(s, pawn, MessageTypeDefOf.NegativeEvent);
					
				}
			}
        }

		public override void PreStart() {
			base.PreStart();
			// Get robot owner.
			owner = pawn.relations?.GetFirstDirectRelationPawn(PawnRelationDefOf.Overseer).mechanitor;
			myPowerDraw = pawn.GetStatValue(StatDefOf.BandwidthCost);
			lastWorkMode = owner.GetControlGroup(pawn).WorkMode;
		}
	}
}
