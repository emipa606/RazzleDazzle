using System;
using Verse;

namespace RazzleDazzle
{
	// Token: 0x02000008 RID: 8
	[StaticConstructorOnStartup]
	public class Building_Stage : Building_Performance
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000034 RID: 52 RVA: 0x000032DC File Offset: 0x000014DC
		protected override string BasicInspectString
		{
			get
			{
				return Translator.Translate("RAZ_StageBasicInspectString");
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000032E8 File Offset: 0x000014E8
		public override bool TryToStartPerformance()
		{
			bool result;
			if (base.TryToStartPerformance())
			{
				new LordJob_PerformPlay(this).TryStartPerformance();
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
	}
}
