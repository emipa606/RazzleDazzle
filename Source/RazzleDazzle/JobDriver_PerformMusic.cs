using System;
using System.Collections.Generic;
using RimWorld;
using Verse.AI;

namespace RazzleDazzle
{
	// Token: 0x02000011 RID: 17
	public class JobDriver_PerformMusic : JobDriver
	{
		// Token: 0x06000052 RID: 82 RVA: 0x0000371C File Offset: 0x0000191C
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, errorOnFailed);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x0000374B File Offset: 0x0000194B
		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
			yield return new Toil
			{
				defaultCompleteMode = ToilCompleteMode.Delay,
				initAction = delegate()
				{
					base.GetActor().jobs.curDriver.ticksLeftThisToil = 250;
				},
				tickAction = delegate()
				{
					if (this.ticksLeftThisToil % 200 == 0)
					{
						MoteMaker.ThrowMetaIcon(base.GetActor().Position, base.GetActor().Map, ThingDefOf_RazzleDazzle.Mote_Music);
					}
					base.GetActor().skills.GetSkill(SkillDefOf.Social).Learn(1f, false);
				}
			};
			yield break;
		}
	}
}
