using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace RazzleDazzle
{
	// Token: 0x0200000C RID: 12
	public class JobDriver_DoRehearsePlayBill : JobDriver
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600003A RID: 58 RVA: 0x000033F5 File Offset: 0x000015F5
		private Building_Performance Venue
		{
			get
			{
				return (Building_Performance)this.job.targetA.Thing;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600003B RID: 59 RVA: 0x0000340C File Offset: 0x0000160C
		private Thing Art
		{
			get
			{
				return this.job.targetB.Thing;
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003420 File Offset: 0x00001620
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.Venue, this.job, 1, -1, null, errorOnFailed) && this.pawn.Reserve(this.Art, this.job, 1, -1, null, errorOnFailed);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x0000347A File Offset: 0x0000167A
		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch);
			yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, false, false);
			yield return Toils_Haul.CarryHauledThingToCell(TargetIndex.A);
			yield return new Toil
			{
				initAction = delegate()
				{
					this.Venue.rehearsing = true;
					CompArt compArt = this.Art.TryGetComp<CompArt>();
					CompQuality compQuality = this.Art.TryGetComp<CompQuality>();
					if (compArt != null && compQuality != null)
					{
						this.Venue.artistName = compArt.AuthorName;
						this.Venue.artTitle = compArt.Title;
						this.Venue.artQuality = compQuality.Quality;
					}
					this.Art.Destroy(DestroyMode.Vanish);
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
			yield break;
		}
	}
}
