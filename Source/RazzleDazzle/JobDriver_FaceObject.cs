using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RazzleDazzle
{
	// Token: 0x0200000D RID: 13
	public class JobDriver_FaceObject : JobDriver
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000040 RID: 64 RVA: 0x0000350A File Offset: 0x0000170A
		private Pawn Actor
		{
			get
			{
				return base.GetActor();
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00003512 File Offset: 0x00001712
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00003515 File Offset: 0x00001715
		protected override IEnumerable<Toil> MakeNewToils()
		{
			LordJob lj = this.Actor.GetLord().LordJob;
			yield return new Toil
			{
				defaultCompleteMode = ToilCompleteMode.Delay,
				initAction = delegate()
				{
					this.ticksLeftThisToil = 500;
				},
				tickAction = delegate()
				{
					if (lj is LordJob_PerformComedySet)
					{
						LordJob_PerformComedySet lordJob_PerformComedySet = lj as LordJob_PerformComedySet;
						if (lordJob_PerformComedySet != null && lordJob_PerformComedySet.Lead != null)
						{
							float num = 1f * (float)lordJob_PerformComedySet.Lead.skills.GetSkill(SkillDefOf.Social).Level;
							if (lordJob_PerformComedySet.punchline && Rand.Value < num * 0.001f)
							{
								MoteMaker.ThrowMetaIcon(this.Actor.Position, this.Actor.Map, ThingDefOf_RazzleDazzle.Mote_Comedy);
							}
						}
					}
				}
			};
			yield break;
		}
	}
}
