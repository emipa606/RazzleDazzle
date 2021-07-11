using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RazzleDazzle
{
    // Token: 0x02000024 RID: 36
    public class LordToil_PerformPlay : LordToil
    {
        // Token: 0x04000034 RID: 52
        public Pawn lead;

        // Token: 0x04000036 RID: 54
        public Thing stage;

        // Token: 0x04000035 RID: 53
        public Pawn support;

        // Token: 0x060000BD RID: 189 RVA: 0x00004EA4 File Offset: 0x000030A4
        public LordToil_PerformPlay(Pawn lead, Pawn support, Thing stage)
        {
            this.lead = lead;
            this.support = support;
            this.stage = stage;
        }

        // Token: 0x060000BE RID: 190 RVA: 0x00004EC1 File Offset: 0x000030C1
        public bool HasPlayFinished()
        {
            return false;
        }

        // Token: 0x060000BF RID: 191 RVA: 0x00004EC4 File Offset: 0x000030C4
        public override void UpdateAllDuties()
        {
            if (lord == null)
            {
                return;
            }

            foreach (var pawn in lord.ownedPawns)
            {
                if (pawn == lead || pawn == support)
                {
                    pawn.mindState.duty = new PawnDuty(DutyDefOfRazzleDazzle.PerformPlay, stage);
                }
                else
                {
                    pawn.mindState.duty = new PawnDuty(DutyDefOfRazzleDazzle.WatchPlayQuietly, lead);
                }
            }
        }

        // Token: 0x060000C0 RID: 192 RVA: 0x00004F6C File Offset: 0x0000316C
        public override ThinkTreeDutyHook VoluntaryJoinDutyHookFor(Pawn p)
        {
            ThinkTreeDutyHook hook;
            if (p == lead || p == support)
            {
                hook = DutyDefOfRazzleDazzle.PerformPlay.hook;
            }
            else
            {
                hook = DutyDefOfRazzleDazzle.WatchPlayQuietly.hook;
            }

            return hook;
        }
    }
}