using Verse;
using Verse.AI;

namespace RazzleDazzle
{
    // Token: 0x0200001B RID: 27
    public class JobGiver_PrepareConcert : ThinkNode_JobGiver
    {
        // Token: 0x0600007A RID: 122 RVA: 0x00003E6C File Offset: 0x0000206C
        protected override Job TryGiveJob(Pawn pawn)
        {
            var thing = (Thing) pawn.mindState.duty.focus;
            var result = !(thing is Building_GrandPiano)
                ? null
                : new Job(JobDefOfRazzleDazzle.JobDef_PrepareConcert, thing);

            return result;
        }
    }
}