using System.Collections.Generic;
using RimWorld;
using Verse.AI;

namespace RazzleDazzle;

public class JobDriver_StartRehearseDirectly : JobDriver
{
    private Building_Performance Venue => (Building_Performance)job.targetA.Thing;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(Venue, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Goto.GotoCell(Venue.InteractionCell, PathEndMode.OnCell);
        yield return new Toil
        {
            initAction = delegate
            {
                Venue.rehearsing = true;
                Venue.rehearsedFraction = 0f;
                Venue.artistName = GetActor().Name.ToStringShort;
                Venue.artTitle = "Latest Set";
                Venue.artQuality = QualityCategory.Normal;
                Venue.Lead = GetActor();
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
    }
}