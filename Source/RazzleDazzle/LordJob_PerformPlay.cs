using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RazzleDazzle;

public class LordJob_PerformPlay : LordJob_Performance
{
    public LordJob_PerformPlay()
    {
    }

    public LordJob_PerformPlay(Building_Performance venue)
    {
        this.venue = venue;
    }

    protected override float GetStatModifier()
    {
        var level = Lead.health.capacities.GetLevel(PawnCapacityDefOf.Talking);
        var level2 = Lead.health.capacities.GetLevel(PawnCapacityDefOf.Moving);
        var level3 = Support.health.capacities.GetLevel(PawnCapacityDefOf.Talking);
        var level4 = Support.health.capacities.GetLevel(PawnCapacityDefOf.Moving);
        return (0.3f * (level + level2)) + (0.2f * (level3 + level4));
    }

    protected override string GetMessageForQualityLevel(float fQ)
    {
        string text;
        switch (fQ)
        {
            case < 4f:
                text = "RAZ_PlayFinishedHated";
                break;
            case < 12f:
                text = "RAZ_PlayFinishedGood";
                break;
            case < 20f:
                text = "RAZ_PlayFinishedGreat";
                break;
            default:
                text = "RAZ_PlayFinishedPeerless";
                break;
        }

        return "RAZ_PlayFinished".Translate(text.Translate());
    }

    protected override ThoughtDef GetThoughtForQualityLevel(float fQ)
    {
        ThoughtDef result;
        switch (fQ)
        {
            case < 4f:
                result = ThoughtDefOfRazzleDazzle.SawBadPlay;
                break;
            case < 12f:
                result = ThoughtDefOfRazzleDazzle.SawPlay;
                break;
            case < 20f:
                result = ThoughtDefOfRazzleDazzle.SawGoodPlay;
                break;
            default:
                result = ThoughtDefOfRazzleDazzle.SawAmazingPlay;
                break;
        }

        return result;
    }

    protected override LordToil GetPerformanceLordToil()
    {
        return new LordToil_PerformPlay(Lead, Support, Venue);
    }
}