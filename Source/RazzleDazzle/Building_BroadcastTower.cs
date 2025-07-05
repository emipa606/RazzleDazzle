using System;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace RazzleDazzle;

public class Building_BroadcastTower : Building
{
    private const float baseSubscriptionFee = 1000f;

    private const float goodwill_multiplier = 0.2f;

    private Season lastBroadcastSeason;

    public float qualityValue;

    private float seasonScore;

    private float serviceRegularity = 1f;

    private string BroadcastQualityString()
    {
        string text = null;
        switch (qualityValue)
        {
            case > 45f:
                text = "RAZ_BroadcastQuality_Outstanding".Translate();
                break;
            case > 30f:
                text = "RAZ_BroadcastQuality_Great".Translate();
                break;
            case > 16f:
                text = "RAZ_BroadcastQuality_Good".Translate();
                break;
            case > 8f:
                text = "RAZ_BroadcastQuality_Decent".Translate();
                break;
            case >= 0f:
                text = "RAZ_BroadcastQuality_Uninspiring".Translate();
                break;
        }

        return text ?? "RAZ_BroadcastQuality_Execrable".Translate();
    }

    private string BroadcastRegularityString()
    {
        string text = null;
        switch (serviceRegularity)
        {
            case 1f:
                text = "RAZ_BroadcastRegularity_Perfect".Translate();
                break;
            case > 0.9f:
                text = "RAZ_BroadcastRegularity_Great".Translate();
                break;
            case > 0.75f:
                text = "RAZ_BroadcastRegularity_Good".Translate();
                break;
            case > 0.6f:
                text = "RAZ_BroadcastRegularity_Adequate".Translate();
                break;
            case > 0.35f:
                text = "RAZ_BroadcastRegularity_Poor".Translate();
                break;
            case > 0.15f:
                text = "RAZ_BroadcastRegularity_Terrible".Translate();
                break;
        }

        return text ?? "RAZ_BroadcastRegularity_OffAir".Translate();
    }

    private void IncrementGoodwill()
    {
        var num = (int)(seasonScore * serviceRegularity * goodwill_multiplier);
        if (num > 12f)
        {
            Messages.Message("RAZ_MessageSeasonEnd".Translate("RAZ_SeasonPositive".Translate()),
                MessageTypeDefOf.PositiveEvent);
        }
        else if (num >= 2f)
        {
            Messages.Message("RAZ_MessageSeasonEnd".Translate("RAZ_SeasonNeutral".Translate()),
                MessageTypeDefOf.NeutralEvent);
        }
        else if (num < 0f)
        {
            Messages.Message("RAZ_MessageSeasonEnd".Translate("RAZ_SeasonNegative".Translate()),
                MessageTypeDefOf.NegativeEvent);
        }

        foreach (var faction in Find.FactionManager.AllFactionsVisible)
        {
            if (!faction.IsPlayer)
            {
                faction.TryAffectGoodwillWith(Faction.OfPlayer, num, false, false);
            }
        }
    }

    private void CallInSubscriptions(int originTile)
    {
        IncrementGoodwill();
        var num = seasonScore * serviceRegularity * 75f;
        if (!(num >= 50f))
        {
            return;
        }

        var list = (from settlement in Find.WorldObjects.SettlementBases
            where settlement.Faction != Faction.OfPlayer && settlement.Faction.def.CanEverBeNonHostile &&
                  settlement.Faction.PlayerGoodwill > 0f &&
                  Find.WorldGrid.ApproxDistanceInTiles(originTile, settlement.Tile) < 36f &&
                  Find.WorldReachability.CanReach(originTile, settlement.Tile)
            select settlement).ToList();
        if (list.NullOrEmpty())
        {
            return;
        }

        foreach (var settlementBase in list)
        {
            var incidentParms = new IncidentParms
            {
                faction = settlementBase.Faction,
                points = num,
                spawnCenter = Position,
                target = Map
            };
            var qi = new QueuedIncident(
                new FiringIncident(ThingDefOf_RazzleDazzle.RAZSubscription, null, incidentParms),
                Find.TickManager.TicksGame + Rand.RangeInclusive(6000, 120000));
            Find.Storyteller.incidentQueue.Add(qi);
        }
    }

    public override string GetInspectString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(base.GetInspectString());
        if (stringBuilder.Length != 0)
        {
            stringBuilder.AppendLine();
        }

        stringBuilder.AppendLine(
            "RAZ_BroadcasterInspectString".Translate(BroadcastQualityString(), BroadcastRegularityString()));
        return stringBuilder.ToString().TrimEndNewlines();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref seasonScore, "seasonScore");
        Scribe_Values.Look(ref serviceRegularity, "serviceRegularity");
        Scribe_Values.Look(ref qualityValue, "qualityValue", 1f);
    }

    private void ChangeSeasons(int originTile)
    {
        CallInSubscriptions(originTile);
        seasonScore = 0f;
        serviceRegularity = 1f;
    }

    public override void TickRare()
    {
        if (lastBroadcastSeason == Season.Undefined)
        {
            lastBroadcastSeason = GenLocalDate.Season(Map);
        }

        seasonScore += 0.001f * qualityValue;
        if (qualityValue > 0f)
        {
            qualityValue -= Math.Min(qualityValue, (0.0008f * qualityValue) + 0.01f);
        }

        if (qualityValue < 0f)
        {
            qualityValue += Math.Min(-qualityValue, 0.005f);
        }

        if (!GetComp<CompPowerTrader>().PowerOn && serviceRegularity > 0f)
        {
            serviceRegularity -= 0.001f;
        }

        if (GenLocalDate.Season(Map) == lastBroadcastSeason)
        {
            return;
        }

        lastBroadcastSeason = GenLocalDate.Season(Map);
        ChangeSeasons(Map.Tile);
    }
}