using System;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RazzleDazzle
{
	// Token: 0x02000027 RID: 39
	public class LordToil_SettleForPlay : LordToil
	{
		// Token: 0x060000CA RID: 202 RVA: 0x0000514C File Offset: 0x0000334C
		public LordToil_SettleForPlay(Pawn lead, Pawn support, Thing performThing)
		{
			this.lead = lead;
			this.support = support;
			this.performThing = performThing;
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00005169 File Offset: 0x00003369
		private CellRect CalculateSpectateRect()
		{
			return CellRect.CenteredOn(performThing.Position, 8);
		}

		// Token: 0x060000CC RID: 204 RVA: 0x0000517C File Offset: 0x0000337C
		public override void Init()
		{
			base.Init();
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00005184 File Offset: 0x00003384
		public override void UpdateAllDuties()
		{
			for (int i = 0; i < lord.ownedPawns.Count; i++)
			{
				Pawn pawn = lord.ownedPawns[i];
				if (pawn == lead || pawn == support)
				{
					pawn.mindState.duty.focus = performThing;
					pawn.mindState.duty = new PawnDuty(DutyDefOfRazzleDazzle.GoToStageAndWait, performThing, -1f);
				}
				else
				{
					PawnDuty duty = new PawnDuty(DutyDefOfRazzleDazzle.FindSeatWithStageViewAndChat, performThing, -1f);
					pawn.mindState.duty = duty;
				}
			}
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00005240 File Offset: 0x00003440
		public override ThinkTreeDutyHook VoluntaryJoinDutyHookFor(Pawn p)
		{
			ThinkTreeDutyHook hook;
			if (p == lead || p == support)
			{
				hook = DutyDefOfRazzleDazzle.PerformConcert.hook;
			}
			else
			{
				hook = DutyDefOf.Spectate.hook;
			}
			return hook;
		}

		// Token: 0x04000039 RID: 57
		public Pawn lead;

		// Token: 0x0400003A RID: 58
		public Pawn support;

		// Token: 0x0400003B RID: 59
		public Thing performThing;

		// Token: 0x0400003C RID: 60
		public CellRect spectateRect;
	}
}
