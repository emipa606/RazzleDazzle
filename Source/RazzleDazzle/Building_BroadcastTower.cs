using System;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace RazzleDazzle
{
    // Token: 0x02000004 RID: 4
    public class Building_BroadcastTower : Building
    {
        // Token: 0x04000005 RID: 5
        private const float baseSubscriptionFee = 1000f;

        // Token: 0x04000006 RID: 6
        private const float goodwill_multiplier = 0.2f;

        // Token: 0x0400000A RID: 10
        private Season lastBroadcastSeason;

        // Token: 0x04000008 RID: 8
        public float qualityValue;

        // Token: 0x04000007 RID: 7
        protected float seasonScore;

        // Token: 0x04000009 RID: 9
        public float serviceRegularity = 1f;

        // Token: 0x0600000D RID: 13 RVA: 0x000022C0 File Offset: 0x000004C0
        private string BroadcastQualityString()
        {
            string text = null;
            if (qualityValue > 45f)
            {
                text = "RAZ_BroadcastQuality_Outstanding".Translate();
            }
            else if (qualityValue > 30f)
            {
                text = "RAZ_BroadcastQuality_Great".Translate();
            }
            else if (qualityValue > 16f)
            {
                text = "RAZ_BroadcastQuality_Good".Translate();
            }
            else if (qualityValue > 8f)
            {
                text = "RAZ_BroadcastQuality_Decent".Translate();
            }
            else if (qualityValue >= 0f)
            {
                text = "RAZ_BroadcastQuality_Uninspiring".Translate();
            }

            return text ?? "RAZ_BroadcastQuality_Execrable".Translate();
        }

        // Token: 0x0600000E RID: 14 RVA: 0x00002360 File Offset: 0x00000560
        private string BroadcastRegularityString()
        {
            string text = null;
            if (serviceRegularity == 1f)
            {
                text = "RAZ_BroadcastRegularity_Perfect".Translate();
            }
            else if (serviceRegularity > 0.9f)
            {
                text = "RAZ_BroadcastRegularity_Great".Translate();
            }
            else if (serviceRegularity > 0.75f)
            {
                text = "RAZ_BroadcastRegularity_Good".Translate();
            }
            else if (serviceRegularity > 0.6f)
            {
                text = "RAZ_BroadcastRegularity_Adequate".Translate();
            }
            else if (serviceRegularity > 0.35f)
            {
                text = "RAZ_BroadcastRegularity_Poor".Translate();
            }
            else if (serviceRegularity > 0.15f)
            {
                text = "RAZ_BroadcastRegularity_Terrible".Translate();
            }

            return text ?? "RAZ_BroadcastRegularity_OffAir".Translate();
        }

        // Token: 0x0600000F RID: 15 RVA: 0x0000241C File Offset: 0x0000061C
        public void IncrementGoodwill()
        {
            var num = (int) (seasonScore * serviceRegularity * goodwill_multiplier);
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

        // Token: 0x06000010 RID: 16 RVA: 0x00002528 File Offset: 0x00000728
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
                      (float) settlement.Faction.PlayerGoodwill > 0f &&
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

        // Token: 0x06000011 RID: 17 RVA: 0x0000264C File Offset: 0x0000084C
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

        // Token: 0x06000012 RID: 18 RVA: 0x000026B0 File Offset: 0x000008B0
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref seasonScore, "seasonScore");
            Scribe_Values.Look(ref serviceRegularity, "serviceRegularity");
            Scribe_Values.Look(ref qualityValue, "qualityValue", 1f);
        }

        // Token: 0x06000013 RID: 19 RVA: 0x00002705 File Offset: 0x00000905
        protected void ChangeSeasons(int originTile)
        {
            CallInSubscriptions(originTile);
            seasonScore = 0f;
            serviceRegularity = 1f;
        }

        // Token: 0x06000014 RID: 20 RVA: 0x00002724 File Offset: 0x00000924
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
}