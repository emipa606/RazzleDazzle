using System;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RazzleDazzle
{
	// Token: 0x02000021 RID: 33
	public class LordToil_ComedySet : LordToil
	{
		// Token: 0x060000B1 RID: 177 RVA: 0x00004BAF File Offset: 0x00002DAF
		public LordToil_ComedySet(Pawn performer, Thing performThing)
		{
			this.performer = performer;
			this.performThing = performThing;
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00004BC5 File Offset: 0x00002DC5
		private CellRect CalculateSpectateRect()
		{
			return CellRect.CenteredOn(performThing.Position, 8);
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00004BD8 File Offset: 0x00002DD8
		public override void UpdateAllDuties()
		{
			for (int i = 0; i < lord.ownedPawns.Count; i++)
			{
				Pawn pawn = lord.ownedPawns[i];
				if (pawn == performer)
				{
					pawn.mindState.duty.focus = performThing;
					pawn.mindState.duty = new PawnDuty(DutyDefOfRazzleDazzle.PerformComedy, performThing, -1f);
				}
				else
				{
					pawn.mindState.duty = new PawnDuty(DutyDefOfRazzleDazzle.WatchPlayQuietly, performer, -1f);
				}
			}
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00004C88 File Offset: 0x00002E88
		public override ThinkTreeDutyHook VoluntaryJoinDutyHookFor(Pawn p)
		{
			ThinkTreeDutyHook hook;
			if (p == performer)
			{
				hook = DutyDefOfRazzleDazzle.PerformConcert.hook;
			}
			else
			{
				hook = DutyDefOf.Spectate.hook;
			}
			return hook;
		}

		// Token: 0x0400002D RID: 45
		public Pawn performer;

		// Token: 0x0400002E RID: 46
		public Thing performThing;

		// Token: 0x0400002F RID: 47
		public CellRect spectateRect;
	}
}
