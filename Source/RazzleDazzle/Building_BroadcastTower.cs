﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace RazzleDazzle
{
	// Token: 0x02000004 RID: 4
	public class Building_BroadcastTower : Building
	{
		// Token: 0x0600000D RID: 13 RVA: 0x000022C0 File Offset: 0x000004C0
		private string BroadcastQualityString()
		{
			string text = null;
			if (this.qualityValue > 45f)
			{
				text = Translator.Translate("RAZ_BroadcastQuality_Outstanding");
			}
			else if (this.qualityValue > 30f)
			{
				text = Translator.Translate("RAZ_BroadcastQuality_Great");
			}
			else if (this.qualityValue > 16f)
			{
				text = Translator.Translate("RAZ_BroadcastQuality_Good");
			}
			else if (this.qualityValue > 8f)
			{
				text = Translator.Translate("RAZ_BroadcastQuality_Decent");
			}
			else if (this.qualityValue >= 0f)
			{
				text = Translator.Translate("RAZ_BroadcastQuality_Uninspiring");
			}
			return text ?? Translator.Translate("RAZ_BroadcastQuality_Execrable");
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002360 File Offset: 0x00000560
		private string BroadcastRegularityString()
		{
			string text = null;
			if (this.serviceRegularity == 1f)
			{
				text = Translator.Translate("RAZ_BroadcastRegularity_Perfect");
			}
			else if (this.serviceRegularity > 0.9f)
			{
				text = Translator.Translate("RAZ_BroadcastRegularity_Great");
			}
			else if (this.serviceRegularity > 0.75f)
			{
				text = Translator.Translate("RAZ_BroadcastRegularity_Good");
			}
			else if (this.serviceRegularity > 0.6f)
			{
				text = Translator.Translate("RAZ_BroadcastRegularity_Adequate");
			}
			else if (this.serviceRegularity > 0.35f)
			{
				text = Translator.Translate("RAZ_BroadcastRegularity_Poor");
			}
			else if (this.serviceRegularity > 0.15f)
			{
				text = Translator.Translate("RAZ_BroadcastRegularity_Terrible");
			}
			return text ?? Translator.Translate("RAZ_BroadcastRegularity_OffAir");
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000241C File Offset: 0x0000061C
		public void IncrementGoodwill()
		{
			int num = (int)(this.seasonScore * this.serviceRegularity * 0.2f);
			if ((float)num > 12f)
			{
				Messages.Message(TranslatorFormattedStringExtensions.Translate("RAZ_MessageSeasonEnd", Translator.Translate("RAZ_SeasonPositive")), MessageTypeDefOf.PositiveEvent, true);
			}
			else if ((float)num >= 2f)
			{
				Messages.Message(TranslatorFormattedStringExtensions.Translate("RAZ_MessageSeasonEnd", Translator.Translate("RAZ_SeasonNeutral")), MessageTypeDefOf.NeutralEvent, true);
			}
			else if ((float)num < 0f)
			{
				Messages.Message(TranslatorFormattedStringExtensions.Translate("RAZ_MessageSeasonEnd", Translator.Translate("RAZ_SeasonNegative")), MessageTypeDefOf.NegativeEvent, true);
			}
			foreach (Faction faction in Find.FactionManager.AllFactionsVisible)
			{
				if (!faction.IsPlayer)
				{
					faction.TryAffectGoodwillWith(Faction.OfPlayer, num, false, false, null, null);
				}
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002528 File Offset: 0x00000728
		private void CallInSubscriptions(int originTile)
		{
			this.IncrementGoodwill();
			float num = this.seasonScore * this.serviceRegularity * 75f;
			if (num >= 50f)
			{
				List<Settlement> list = (from settlement in Find.WorldObjects.SettlementBases
				where settlement.Faction != Faction.OfPlayer && settlement.Faction.def.CanEverBeNonHostile && (float)settlement.Faction.PlayerGoodwill > 0f && Find.WorldGrid.ApproxDistanceInTiles(originTile, settlement.Tile) < 36f && Find.WorldReachability.CanReach(originTile, settlement.Tile)
				select settlement).ToList<Settlement>();
				if (!list.NullOrEmpty<Settlement>())
				{
					foreach (Settlement settlementBase in list)
					{
						IncidentParms incidentParms = new IncidentParms();
						incidentParms.faction = settlementBase.Faction;
						incidentParms.points = num;
						incidentParms.spawnCenter = base.Position;
						incidentParms.target = base.Map;
						QueuedIncident qi = new QueuedIncident(new FiringIncident(ThingDefOf_RazzleDazzle.RAZSubscription, null, incidentParms), Find.TickManager.TicksGame + Rand.RangeInclusive(6000, 120000), 0);
						Find.Storyteller.incidentQueue.Add(qi);
					}
				}
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x0000264C File Offset: 0x0000084C
		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.GetInspectString());
			if (stringBuilder.Length != 0)
			{
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendLine(TranslatorFormattedStringExtensions.Translate("RAZ_BroadcasterInspectString", this.BroadcastQualityString(), this.BroadcastRegularityString()));
			return stringBuilder.ToString().TrimEndNewlines();
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000026B0 File Offset: 0x000008B0
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<float>(ref this.seasonScore, "seasonScore", 0f, false);
			Scribe_Values.Look<float>(ref this.serviceRegularity, "serviceRegularity", 0f, false);
			Scribe_Values.Look<float>(ref this.qualityValue, "qualityValue", 1f, false);
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002705 File Offset: 0x00000905
		protected void ChangeSeasons(int originTile)
		{
			this.CallInSubscriptions(originTile);
			this.seasonScore = 0f;
			this.serviceRegularity = 1f;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002724 File Offset: 0x00000924
		public override void TickRare()
		{
			if (this.lastBroadcastSeason == Season.Undefined)
			{
				this.lastBroadcastSeason = GenLocalDate.Season(base.Map);
			}
			this.seasonScore += 0.001f * this.qualityValue;
			if (this.qualityValue > 0f)
			{
				this.qualityValue -= Math.Min(this.qualityValue, 0.0008f * this.qualityValue + 0.01f);
			}
			if (this.qualityValue < 0f)
			{
				this.qualityValue += Math.Min(-this.qualityValue, 0.005f);
			}
			if (!base.GetComp<CompPowerTrader>().PowerOn && this.serviceRegularity > 0f)
			{
				this.serviceRegularity -= 0.001f;
			}
			if (GenLocalDate.Season(base.Map) != this.lastBroadcastSeason)
			{
				this.lastBroadcastSeason = GenLocalDate.Season(base.Map);
				this.ChangeSeasons(base.Map.Tile);
			}
		}

		// Token: 0x04000005 RID: 5
		private const float baseSubscriptionFee = 1000f;

		// Token: 0x04000006 RID: 6
		private const float goodwill_multiplier = 0.2f;

		// Token: 0x04000007 RID: 7
		protected float seasonScore;

		// Token: 0x04000008 RID: 8
		public float qualityValue;

		// Token: 0x04000009 RID: 9
		public float serviceRegularity = 1f;

		// Token: 0x0400000A RID: 10
		private Season lastBroadcastSeason;
	}
}