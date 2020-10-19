using System;
using System.Collections.Generic;
using Verse.AI;

namespace RazzleDazzle
{
	// Token: 0x02000013 RID: 19
	public class JobDriver_PrepareConcert : JobDriver
	{
		// Token: 0x0600005D RID: 93 RVA: 0x00003828 File Offset: 0x00001A28
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00003857 File Offset: 0x00001A57
		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
			yield break;
		}
	}
}
