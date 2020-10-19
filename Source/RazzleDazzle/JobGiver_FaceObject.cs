using System;
using Verse;
using Verse.AI;

namespace RazzleDazzle
{
	// Token: 0x02000016 RID: 22
	public class JobGiver_FaceObject : ThinkNode_JobGiver
	{
		// Token: 0x0600006C RID: 108 RVA: 0x00003B25 File Offset: 0x00001D25
		protected override Job TryGiveJob(Pawn pawn)
		{
			return new Job(JobDefOfRazzleDazzle.JobDef_FaceObject, pawn.mindState.duty.focus)
			{
				expiryInterval = ticks
			};
		}

		// Token: 0x04000028 RID: 40
		public int ticks;
	}
}
