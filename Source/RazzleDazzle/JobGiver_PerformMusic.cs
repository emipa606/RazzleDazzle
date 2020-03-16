using System;
using Verse;
using Verse.AI;

namespace RazzleDazzle
{
	// Token: 0x02000019 RID: 25
	public class JobGiver_PerformMusic : ThinkNode_JobGiver
	{
		// Token: 0x06000076 RID: 118 RVA: 0x00003DE4 File Offset: 0x00001FE4
		protected override Job TryGiveJob(Pawn pawn)
		{
			Thing thing = (Thing)pawn.mindState.duty.focus;
			Job result;
			if (!(thing is Building_GrandPiano))
			{
				result = null;
			}
			else
			{
				result = new Job(JobDefOfRazzleDazzle.JobDef_PerformMusic, thing);
			}
			return result;
		}
	}
}
