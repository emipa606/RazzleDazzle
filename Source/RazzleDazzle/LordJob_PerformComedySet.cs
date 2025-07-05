using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RazzleDazzle;

public class LordJob_PerformComedySet : LordJob_Performance
{
    public bool punchline;

    public LordJob_PerformComedySet()
    {
    }

    public LordJob_PerformComedySet(Building_Performance venue)
    {
        this.venue = venue;
    }

    protected override float GetFinalQuality()
    {
        var num = (float)GetQualityModifier(QualityUtility.GenerateQualityCreatedByPawn(Lead, SkillDefOf.Social));
        var num2 = (float)GetQualityModifier(
            QualityUtility.GenerateQualityCreatedByPawn(Lead, SkillDefOf.Artistic));
        return Lead.health.capacities.GetLevel(PawnCapacityDefOf.Talking) * Rand.Range(num, num + num2);
    }

    protected override string GetMessageForQualityLevel(float fQ)
    {
        string text;
        switch (fQ)
        {
            case < 4f:
                text = "RAZ_ComedyFinishedHated";
                break;
            case < 12f:
                text = "RAZ_ComedyFinishedGood";
                break;
            case < 20f:
                text = "RAZ_ComedyFinishedGreat";
                break;
            default:
                text = "RAZ_ComedyFinishedPeerless";
                break;
        }

        return "RAZ_ComedyFinished".Translate(text.Translate());
    }

    protected override ThoughtDef GetThoughtForQualityLevel(float fQ)
    {
        ThoughtDef result;
        switch (fQ)
        {
            case < 4f:
                result = ThoughtDefOfRazzleDazzle.AttendedBadGig;
                break;
            case < 12f:
                result = ThoughtDefOfRazzleDazzle.AttendedGig;
                break;
            case < 20f:
                result = ThoughtDefOfRazzleDazzle.AttendedGoodGig;
                break;
            default:
                result = ThoughtDefOfRazzleDazzle.AttendedAmazingGig;
                break;
        }

        return result;
    }

    protected override LordToil GetPerformanceLordToil()
    {
        return new LordToil_ComedySet(Lead, Venue);
    }
}