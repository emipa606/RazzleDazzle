using Verse;
using Verse.AI;

namespace RazzleDazzle;

public class JobGiver_PerformComedy : ThinkNode_JobGiver
{
    protected override Job TryGiveJob(Pawn pawn)
    {
        var thing = (Thing)pawn.mindState.duty.focus;
        var result = thing is not Building_Microphone
            ? null
            : new Job(JobDefOfRazzleDazzle.JobDef_PerformComedy, thing);

        return result;
    }
}