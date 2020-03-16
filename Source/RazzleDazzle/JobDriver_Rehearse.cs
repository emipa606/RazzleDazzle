using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace RazzleDazzle
{
	// Token: 0x02000014 RID: 20
	public class JobDriver_Rehearse : JobDriver
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000060 RID: 96 RVA: 0x000033F5 File Offset: 0x000015F5
		private Building_Performance Venue
		{
			get
			{
				return (Building_Performance)this.job.targetA.Thing;
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00003860 File Offset: 0x00001A60
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			int stackCount = (this.Venue.venueDef.performersNeeded > 1) ? 0 : -1;
			return this.pawn.Reserve(this.Venue, this.job, this.Venue.venueDef.performersNeeded, stackCount, null, errorOnFailed);
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000038B6 File Offset: 0x00001AB6
		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return new Toil
			{
				initAction = delegate()
				{
					if (this.Venue.Director.stage == null)
					{
						this.Venue.Director.stage = this.Venue;
					}
					if ((this.Venue.Lead == null || this.Venue.Lead.Dead) && this.Venue.Support != base.GetActor())
					{
						this.Venue.Lead = base.GetActor();
						return;
					}
					if (this.Venue.venueDef.performersNeeded > 1 && (this.Venue.Support == null || this.Venue.Support.Dead) && this.Venue.Lead != base.GetActor())
					{
						this.Venue.Support = base.GetActor();
					}
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
			if (this.Venue.InteractionCell.IsValid)
			{
				yield return Toils_Goto.GotoCell(this.Venue.InteractionCell, PathEndMode.OnCell);
			}
			else
			{
				yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
			}
			yield return new Toil
			{
				defaultCompleteMode = ToilCompleteMode.Delay,
				initAction = delegate()
				{
					base.GetActor().jobs.curDriver.ticksLeftThisToil = 1001;
				},
				tickAction = delegate()
				{
					if (this.ticksLeftThisToil % 100 == 0)
					{
						ThingDef moteDef = this.Venue.venueDef.rehearsalMotes.RandomElement<ThingDef>();
						MoteMaker.ThrowMetaIcon(base.GetActor().Position, base.GetActor().Map, moteDef);
						this.Venue.rehearsedFraction += 100f / (float)this.Venue.venueDef.numTicksToRehearse;
						base.GetActor().skills.GetSkill(SkillDefOf.Social).Learn(1f, false);
					}
					if (this.Venue.rehearsedFraction >= 1f)
					{
						this.Venue.rehearsedFraction = 1.00000012f;
						this.ticksLeftThisToil = 0;
					}
				}
			};
			yield break;
		}
	}
}
