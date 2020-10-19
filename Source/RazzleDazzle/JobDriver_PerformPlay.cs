using System;
using System.Collections.Generic;
using RimWorld;
using Verse.AI;

namespace RazzleDazzle
{
	// Token: 0x02000012 RID: 18
	public class JobDriver_PerformPlay : JobDriver
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000057 RID: 87 RVA: 0x000033F5 File Offset: 0x000015F5
		private Building_Performance Stage
		{
			get
			{
				return (Building_Performance)job.targetA.Thing;
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000037D4 File Offset: 0x000019D4
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Stage, job, 2, 0, null, errorOnFailed);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003803 File Offset: 0x00001A03
		protected override IEnumerable<Toil> MakeNewToils()
		{
			List<Toil> list = Stage.Director.RequestPlayToils(GetActor(), Stage);
			foreach (Toil toil in list)
			{
				toil.AddFailCondition(() => !GatheringsUtility.AcceptableGameConditionsToContinueGathering(GetActor().Map));
				yield return toil;
			}
			List<Toil>.Enumerator enumerator = default;
			yield break;
			yield break;
		}
	}
}
