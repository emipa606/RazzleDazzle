using System.Collections.Generic;
using Verse.AI;

namespace RazzleDazzle;

public class JobDriver_PrepareConcert : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
    }
}