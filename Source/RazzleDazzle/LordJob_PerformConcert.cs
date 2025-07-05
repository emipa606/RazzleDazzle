using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RazzleDazzle;

public class LordJob_PerformConcert : LordJob_Performance
{
    public LordJob_PerformConcert()
    {
    }

    public LordJob_PerformConcert(Building_Performance venue)
    {
        this.venue = venue;
    }

    protected override float GetStatModifier()
    {
        return Lead.health.capacities.GetLevel(PawnCapacityDefOf.Manipulation);
    }

    protected override string GetMessageForQualityLevel(float fQ)
    {
        string text;
        switch (fQ)
        {
            case < 4f:
                text = "RAZ_ConcertFinishedHated";
                break;
            case < 12f:
                text = "RAZ_ConcertFinishedGood";
                break;
            case < 20f:
                text = "RAZ_ConcertFinishedGreat";
                break;
            default:
                text = "RAZ_ConcertFinishedPeerless";
                break;
        }

        return "RAZ_ConcertFinished".Translate(text.Translate());
    }

    protected override ThoughtDef GetThoughtForQualityLevel(float fQ)
    {
        ThoughtDef result;
        switch (fQ)
        {
            case < 4f:
                result = ThoughtDefOfRazzleDazzle.AttendedBadConcert;
                break;
            case < 12f:
                result = ThoughtDefOfRazzleDazzle.AttendedConcert;
                break;
            case < 20f:
                result = ThoughtDefOfRazzleDazzle.AttendedGoodConcert;
                break;
            default:
                result = ThoughtDefOfRazzleDazzle.AttendedAmazingConcert;
                break;
        }

        return result;
    }

    protected override LordToil GetPerformanceLordToil()
    {
        return new LordToil_Concert(Lead, Venue);
    }
}