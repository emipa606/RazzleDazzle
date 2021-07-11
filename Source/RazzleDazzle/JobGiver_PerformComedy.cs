using Verse;
using Verse.AI;

namespace RazzleDazzle
{
    // Token: 0x02000018 RID: 24
    public class JobGiver_PerformComedy : ThinkNode_JobGiver
    {
        // Token: 0x06000074 RID: 116 RVA: 0x00003DA0 File Offset: 0x00001FA0
        protected override Job TryGiveJob(Pawn pawn)
        {
            var thing = (Thing) pawn.mindState.duty.focus;
            var result = !(thing is Building_Microphone)
                ? null
                : new Job(JobDefOfRazzleDazzle.JobDef_PerformComedy, thing);

            return result;
        }
    }
}