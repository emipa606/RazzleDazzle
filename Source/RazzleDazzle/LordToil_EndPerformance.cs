using RimWorld;
using Verse.AI.Group;

namespace RazzleDazzle;

public class LordToil_EndPerformance : LordToil
{
    private readonly Building_Performance venue;

    public LordToil_EndPerformance(Building_Performance venue)
    {
        this.venue = venue;
    }

    public override bool ShouldFail => true;

    public void CleanUpStage()
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