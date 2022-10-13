using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace RazzleDazzle;

public class JobDriver_DoRehearsePlayBill : JobDriver
{
    private Building_Performance Venue => (Building_Performance)job.targetA.Thing;

    private Thing Art => job.targetB.Thing;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(Venue, job, 1, -1, null, errorOnFailed) &&
               pawn.Reserve(Art, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch);
        yield return Toils_Haul.StartCarryThing(TargetIndex.B);
        yield return Toils_Haul.CarryHauledThingToCell(TargetIndex.A);
        yield return new Toil
        {
            initAction = delegate
            {
                Venue.rehearsing = true;
                var compArt = Art.TryGetComp<CompArt>();
                var compQuality = Art.TryGetComp<CompQuality>();
                if (compArt != null && compQuality != null)
                {
                    Venue.artistName = compArt.AuthorName;
                    Venue.artTitle = compArt.Title;
                    Venue.artQuality = compQuality.Quality;
                }

                Art.Destroy();
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
    }
}