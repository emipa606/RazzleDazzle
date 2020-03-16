using System;
using System.Text;
using Verse;

namespace RazzleDazzle
{
	// Token: 0x02000006 RID: 6
	[StaticConstructorOnStartup]
	public class Building_Microphone : Building_Performance
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000018 RID: 24 RVA: 0x0000286C File Offset: 0x00000A6C
		protected override string BasicInspectString
		{
			get
			{
				return Translator.Translate("RAZ_MicrophoneBasicInpectString");
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002878 File Offset: 0x00000A78
		public override bool TryToStartPerformance()
		{
			bool result;
			if (base.TryToStartPerformance())
			{
				new LordJob_PerformComedySet(this).TryStartPerformance();
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000028A0 File Offset: 0x00000AA0
		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.rehearsing)
			{
				if (this.rehearsedFraction < 1f)
				{
					stringBuilder.AppendLine(TranslatorFormattedStringExtensions.Translate("RAZ_Rehearsal_Microphone", base.Lead.Named("PAWN")));
					stringBuilder.AppendLine(TranslatorFormattedStringExtensions.Translate("RAZ_Rehearsal_Progress", this.rehearsedFraction.ToStringPercent()));
				}
				else
				{
					stringBuilder.AppendLine(TranslatorFormattedStringExtensions.Translate("RAZ_PerformancePending_Microphone", base.Lead.Named("PAWN")));
				}
			}
			else
			{
				stringBuilder.AppendLine(this.BasicInspectString);
			}
			return stringBuilder.ToString().TrimEndNewlines();
		}
	}
}
