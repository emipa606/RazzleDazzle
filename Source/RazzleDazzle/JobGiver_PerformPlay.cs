using System;
using Verse;
using Verse.AI;

namespace RazzleDazzle
{
	// Token: 0x0200001A RID: 26
	public class JobGiver_PerformPlay : ThinkNode_JobGiver
	{
		// Token: 0x06000078 RID: 120 RVA: 0x00003E28 File Offset: 0x00002028
		protected override Job TryGiveJob(Pawn pawn)
		{
			Thing thing = (Thing)pawn.mindState.duty.focus;
			Job result;
			if (!(thing is Building_Stage))
			{
				result = null;
			}
			else
			{
				result = new Job(JobDefOfRazzleDazzle.JobDef_PerformPlay, thing);
			}
			return result;
		}
	}
}
