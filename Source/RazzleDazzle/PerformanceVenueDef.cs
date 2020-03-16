using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RazzleDazzle
{
	// Token: 0x02000028 RID: 40
	public class PerformanceVenueDef : ThingDef
	{
		// Token: 0x0400003D RID: 61
		public int ticksBetweenPerformances = 600;

		// Token: 0x0400003E RID: 62
		public int performersNeeded = 1;

		// Token: 0x0400003F RID: 63
		public float entertainmentWeight = 1f;

		// Token: 0x04000040 RID: 64
		public int minTicksInPerformance = 2500;

		// Token: 0x04000041 RID: 65
		public int numTicksToRehearse = 1;

		// Token: 0x04000042 RID: 66
		public List<ThingDef> rehearsalMotes;

		// Token: 0x04000043 RID: 67
		public ThingDef artDef;

		// Token: 0x04000044 RID: 68
		public TaleDef taleDef;
	}
}
