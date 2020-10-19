using System;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RazzleDazzle
{
	// Token: 0x0200001F RID: 31
	public class LordJob_PerformPlay : LordJob_Performance
	{
		// Token: 0x060000A9 RID: 169 RVA: 0x0000486C File Offset: 0x00002A6C
		public LordJob_PerformPlay()
		{
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00004874 File Offset: 0x00002A74
		public LordJob_PerformPlay(Building_Performance venue)
		{
			this.venue = venue;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00004A64 File Offset: 0x00002C64
		protected override float GetStatModifier()
		{
			float level = Lead.health.capacities.GetLevel(PawnCapacityDefOf.Talking);
			float level2 = Lead.health.capacities.GetLevel(PawnCapacityDefOf.Moving);
			float level3 = Support.health.capacities.GetLevel(PawnCapacityDefOf.Talking);
			float level4 = Support.health.capacities.GetLevel(PawnCapacityDefOf.Moving);
			return 0.3f * (level + level2) + 0.2f * (level3 + level4);
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00004AF0 File Offset: 0x00002CF0
		public override string GetMessageForQualityLevel(float fQ)
		{
			string text;
			if (fQ < 4f)
			{
				text = "RAZ_PlayFinishedHated";
			}
			else if (fQ < 12f)
			{
				text = "RAZ_PlayFinishedGood";
			}
			else if (fQ < 20f)
			{
				text = "RAZ_PlayFinishedGreat";
			}
			else
			{
				text = "RAZ_PlayFinishedPeerless";
			}
			return TranslatorFormattedStringExtensions.Translate("RAZ_PlayFinished", Translator.Translate(text));
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00004B48 File Offset: 0x00002D48
		public override ThoughtDef GetThoughtForQualityLevel(float fQ)
		{
			ThoughtDef result;
			if (fQ < 4f)
			{
				result = ThoughtDefOfRazzleDazzle.SawBadPlay;
			}
			else if (fQ < 12f)
			{
				result = ThoughtDefOfRazzleDazzle.SawPlay;
			}
			else if (fQ < 20f)
			{
				result = ThoughtDefOfRazzleDazzle.SawGoodPlay;
			}
			else
			{
				result = ThoughtDefOfRazzleDazzle.SawAmazingPlay;
			}
			return result;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00004B8C File Offset: 0x00002D8C
		protected override LordToil GetPerformanceLordToil()
		{
			return new LordToil_PerformPlay(Lead, Support, Venue);
		}
	}
}
