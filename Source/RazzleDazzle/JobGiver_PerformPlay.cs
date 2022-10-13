using Verse;
using Verse.AI;

namespace RazzleDazzle;

public class JobGiver_PerformPlay : ThinkNode_JobGiver
{
    protected override Job TryGiveJob(Pawn pawn)
    {
        var thing = (Thing)pawn.mindState.duty.focus;
        var result = thing is not Building_Stage ? null : new Job(JobDefOfRazzleDazzle.JobDef_PerformPlay, thing);

        return result;
    }
}