using System.Collections.Generic;
using RimWorld;
using Verse.AI;

namespace RazzleDazzle;

public class JobDriver_PerformPlay : JobDriver
{
    private Building_Performance Stage => (Building_Performance)job.targetA.Thing;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(Stage, job, 2, 0, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        var list = Stage.Director.RequestPlayToils(GetActor(), Stage);
        foreach (var toil in list)
        {
            toil.AddFailCondition(() =>
                !GatheringsUtility.AcceptableGameConditionsToContinueGathering(GetActor().Map));
            yield return toil;
        }
    }
}