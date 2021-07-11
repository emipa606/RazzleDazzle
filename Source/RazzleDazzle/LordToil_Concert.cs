using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RazzleDazzle
{
    // Token: 0x02000022 RID: 34
    public class LordToil_Concert : LordToil
    {
        // Token: 0x04000030 RID: 48
        public Pawn performer;

        // Token: 0x04000031 RID: 49
        public Thing performThing;

        // Token: 0x04000032 RID: 50
        public CellRect spectateRect;

        // Token: 0x060000B5 RID: 181 RVA: 0x00004CB7 File Offset: 0x00002EB7
        public LordToil_Concert(Pawn performer, Thing performThing)
        {
            this.performer = performer;
            this.performThing = performThing;
        }

        // Token: 0x060000B6 RID: 182 RVA: 0x00004CCD File Offset: 0x00002ECD
        private CellRect CalculateSpectateRect()
        {
            return CellRect.CenteredOn(performThing.Position, 8);
        }

        // Token: 0x060000B7 RID: 183 RVA: 0x00004CE0 File Offset: 0x00002EE0
        public override void UpdateAllDuties()
        {
            foreach (var pawn in lord.ownedPawns)
            {
                if (pawn == performer)
                {
                    pawn.mindState.duty.focus = performThing;
                    pawn.mindState.duty = new PawnDuty(DutyDefOfRazzleDazzle.PerformConcert, performThing);
                }
                else
                {
                    var duty = new PawnDuty(DutyDefOfRazzleDazzle.WatchPlayQuietly, performer);
                    pawn.mindState.duty = duty;
                }
            }
        }

        // Token: 0x060000B8 RID: 184 RVA: 0x00004D94 File Offset: 0x00002F94
        public override ThinkTreeDutyHook VoluntaryJoinDutyHookFor(Pawn p)
        {
            var hook = p == performer ? DutyDefOfRazzleDazzle.PerformConcert.hook : DutyDefOf.Spectate.hook;

            return hook;
        }
    }
}