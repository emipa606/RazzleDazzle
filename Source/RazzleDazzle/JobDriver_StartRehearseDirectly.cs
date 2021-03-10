using System;
using System.Collections.Generic;
using RimWorld;
using Verse.AI;

namespace RazzleDazzle
{
	// Token: 0x02000015 RID: 21
	public class JobDriver_StartRehearseDirectly : JobDriver
	{
        // Token: 0x17000012 RID: 18
        // (get) Token: 0x06000067 RID: 103 RVA: 0x000033F5 File Offset: 0x000015F5
        private Building_Performance Venue => (Building_Performance)job.targetA.Thing;

        // Token: 0x06000068 RID: 104 RVA: 0x00003A74 File Offset: 0x00001C74
        public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Venue, job, 1, -1, null, errorOnFailed);
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00003AA3 File Offset: 0x00001CA3
		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Goto.GotoCell(Venue.InteractionCell, PathEndMode.OnCell);
			yield return new Toil
			{
				initAction = delegate()
				{
					Venue.rehearsing = true;
					Venue.rehearsedFraction = 0f;
					Venue.artistName = GetActor().Name.ToStringShort;
					Venue.artTitle = "Latest Set";
					Venue.artQuality = QualityCategory.Normal;
					Venue.Lead = GetActor();
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
			yield break;
		}
	}
}
