using Verse;
using Verse.AI;

namespace RazzleDazzle;

public class JobGiver_FaceObject : ThinkNode_JobGiver
{
    public int ticks;

    protected override Job TryGiveJob(Pawn pawn)
    {
        return new Job(JobDefOfRazzleDazzle.JobDef_FaceObject, pawn.mindState.duty.focus)
        {
            expiryInterval = ticks
        };
    }
}