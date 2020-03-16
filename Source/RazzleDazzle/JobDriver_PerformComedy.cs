using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RazzleDazzle
{
	// Token: 0x02000010 RID: 16
	public class JobDriver_PerformComedy : JobDriver
	{
		// Token: 0x0600004E RID: 78 RVA: 0x000036DC File Offset: 0x000018DC
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, errorOnFailed);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x0000370B File Offset: 0x0000190B
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedOrNull(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
			LordJob_PerformComedySet job = base.GetActor().GetLord().LordJob as LordJob_PerformComedySet;
			yield return new Toil
			{
				defaultCompleteMode = ToilCompleteMode.Delay,
				initAction = delegate()
				{
					this.GetActor().jobs.curDriver.ticksLeftThisToil = 1200;
				},
				tickAction = delegate()
				{
					if (job.punchline)
					{
						if (Rand.Chance(0.015f))
						{
							job.punchline = false;
						}
					}
					else
					{
						if (this.ticksLeftThisToil % 300 == 1)
						{
							MoteMaker.ThrowMetaIcon(this.GetActor().Position, this.GetActor().Map, ThingDefOf_RazzleDazzle.Mote_Comedy);
						}
						else if (Rand.Chance(0.01f))
						{
							float value = Rand.Value;
							if (value < 0.25f)
							{
								MoteMaker.ThrowMetaIcon(this.GetActor().Position, this.GetActor().Map, ThingDefOf.Mote_IncapIcon);
							}
							else if (value < 0.5f)
							{
								MoteMaker.ThrowMetaIcon(this.GetActor().Position, this.GetActor().Map, ThingDefOf.Mote_Heart);
							}
							else
							{
								MoteMaker.ThrowMetaIcon(this.GetActor().Position, this.GetActor().Map, ThingDefOf_RazzleDazzle.Mote_Tragedy);
							}
						}
						if (Rand.Chance(0.002f))
						{
							job.punchline = true;
						}
					}
					this.GetActor().skills.GetSkill(SkillDefOf.Social).Learn(1f, false);
				}
			};
			yield break;
		}
	}
}
