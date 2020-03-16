using System;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RazzleDazzle
{
	// Token: 0x0200001E RID: 30
	public class LordJob_PerformConcert : LordJob_Performance
	{
		// Token: 0x060000A3 RID: 163 RVA: 0x0000486C File Offset: 0x00002A6C
		public LordJob_PerformConcert()
		{
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00004874 File Offset: 0x00002A74
		public LordJob_PerformConcert(Building_Performance venue)
		{
			this.venue = venue;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00004997 File Offset: 0x00002B97
		protected override float GetStatModifier()
		{
			return base.Lead.health.capacities.GetLevel(PawnCapacityDefOf.Manipulation);
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x000049B4 File Offset: 0x00002BB4
		public override string GetMessageForQualityLevel(float fQ)
		{
			string text;
			if (fQ < 4f)
			{
				text = "RAZ_ConcertFinishedHated";
			}
			else if (fQ < 12f)
			{
				text = "RAZ_ConcertFinishedGood";
			}
			else if (fQ < 20f)
			{
				text = "RAZ_ConcertFinishedGreat";
			}
			else
			{
				text = "RAZ_ConcertFinishedPeerless";
			}
			return TranslatorFormattedStringExtensions.Translate("RAZ_ConcertFinished", Translator.Translate(text));
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00004A0C File Offset: 0x00002C0C
		public override ThoughtDef GetThoughtForQualityLevel(float fQ)
		{
			ThoughtDef result;
			if (fQ < 4f)
			{
				result = ThoughtDefOfRazzleDazzle.AttendedBadConcert;
			}
			else if (fQ < 12f)
			{
				result = ThoughtDefOfRazzleDazzle.AttendedConcert;
			}
			else if (fQ < 20f)
			{
				result = ThoughtDefOfRazzleDazzle.AttendedGoodConcert;
			}
			else
			{
				result = ThoughtDefOfRazzleDazzle.AttendedAmazingConcert;
			}
			return result;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00004A50 File Offset: 0x00002C50
		protected override LordToil GetPerformanceLordToil()
		{
			return new LordToil_Concert(base.Lead, base.Venue);
		}
	}
}
