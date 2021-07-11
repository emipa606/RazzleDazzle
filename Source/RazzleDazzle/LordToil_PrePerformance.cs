using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RazzleDazzle
{
    // Token: 0x02000025 RID: 37
    public class LordToil_PrePerformance : LordToil
    {
        // Token: 0x04000037 RID: 55
        private readonly Thing venue;

        // Token: 0x060000C1 RID: 193 RVA: 0x00004FA4 File Offset: 0x000031A4
        public LordToil_PrePerformance(Thing venue)
        {
            this.venue = venue;
        }

        // Token: 0x060000C2 RID: 194 RVA: 0x00004FB4 File Offset: 0x000031B4
        public override void UpdateAllDuties()
        {
            if (lord == null)
            {
                return;
            }

            foreach (var pawn in lord.ownedPawns)
            {
                pawn.mindState.duty = new PawnDuty(DutyDefOf.Party, venue);
            }
        }

        // Token: 0x060000C3 RID: 195 RVA: 0x00005019 File Offset: 0x00003219
        public override ThinkTreeDutyHook VoluntaryJoinDutyHookFor(Pawn p)
        {
            return DutyDefOf.Party.hook;
        }
    }
}