using Verse;
using Verse.AI;

namespace RazzleDazzle;

public class JobGiver_PerformMusic : ThinkNode_JobGiver
{
    protected override Job TryGiveJob(Pawn pawn)
    {
        var thing = (Thing)pawn.mindState.duty.focus;
        var result = thing is not Building_GrandPiano
            ? null
            : new Job(JobDefOfRazzleDazzle.JobDef_PerformMusic, thing);

        return result;
    }
}