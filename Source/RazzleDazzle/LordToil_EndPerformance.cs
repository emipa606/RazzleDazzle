using RimWorld;
using Verse.AI.Group;

namespace RazzleDazzle;

public class LordToil_EndPerformance(Building_Performance venue) : LordToil
{
    public override bool ShouldFail => true;

    private void CleanUpStage()
    {
        venue.rehearsing = false;
        venue.rehearsedFraction = 0f;
        venue.ticksIntoThisPerformance = 0;
        venue.artTitle = "";
        venue.artistName = "";
        venue.artQuality = QualityCategory.Normal;
        venue.ClearPerformers();
    }

    public override void UpdateAllDuties()
    {
        foreach (var pawn in lord.ownedPawns)
        {
            pawn.jobs.StopAll();
        }

        CleanUpStage();
    }
}