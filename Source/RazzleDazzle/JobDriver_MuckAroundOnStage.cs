using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace RazzleDazzle
{
	// Token: 0x0200000E RID: 14
	public class JobDriver_MuckAroundOnStage : JobDriver
	{
		// Token: 0x06000044 RID: 68 RVA: 0x00003528 File Offset: 0x00001728
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(job.targetA, job, 3, 0, null, errorOnFailed);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00003557 File Offset: 0x00001757
		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
			yield return new Toil
			{
				defaultCompleteMode = ToilCompleteMode.Delay,
				initAction = delegate()
				{
                    GetActor().jobs.curDriver.ticksLeftThisToil = 1000;
				},
				tickAction = delegate()
				{
                    GetActor().skills.GetSkill(SkillDefOf.Social).Learn(0.05f, false);
					if (ticksLeftThisToil % 100 == 0)
					{
						var value = Rand.Value;
						ThingDef moteDef;
						if ((double)value < 0.4)
						{
							moteDef = ThingDefOf_RazzleDazzle.Mote_Comedy;
						}
						else if ((double)value < 0.8)
						{
							moteDef = ThingDefOf_RazzleDazzle.Mote_Tragedy;
						}
						else if ((double)value < 0.9)
						{
							moteDef = ThingDefOf_RazzleDazzle.Mote_Music;
						}
						else
						{
							moteDef = ThingDefOf.Mote_Heart;
						}
						MoteMaker.ThrowMetaIcon(GetActor().Position, GetActor().Map, moteDef);
					}
				}
			};
			yield break;
		}
	}
}
