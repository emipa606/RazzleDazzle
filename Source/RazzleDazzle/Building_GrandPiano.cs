using System;
using Verse;

namespace RazzleDazzle
{
	// Token: 0x02000005 RID: 5
	[StaticConstructorOnStartup]
	public class Building_GrandPiano : Building_Performance
	{
		// Token: 0x06000016 RID: 22 RVA: 0x0000283C File Offset: 0x00000A3C
		public override bool TryToStartPerformance()
		{
			bool result;
			if (base.TryToStartPerformance())
			{
				new LordJob_PerformConcert(this).TryStartPerformance();
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
