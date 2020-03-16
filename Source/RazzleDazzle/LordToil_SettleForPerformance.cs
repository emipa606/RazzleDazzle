using System;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RazzleDazzle
{
	// Token: 0x02000026 RID: 38
	public class LordToil_SettleForPerformance : LordToil
	{
		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x00005025 File Offset: 0x00003225
		public Building_Performance Venue
		{
			get
			{
				return this.venue;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x0000502D File Offset: 0x0000322D
		public Pawn Lead
		{
			get
			{
				return this.Venue.Lead;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000C6 RID: 198 RVA: 0x0000503A File Offset: 0x0000323A
		public Pawn Support
		{
			get
			{
				return this.Venue.Support;
			}
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00005047 File Offset: 0x00003247
		public LordToil_SettleForPerformance(Building_Performance venue)
		{
			this.venue = venue;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00005058 File Offset: 0x00003258
		public override void UpdateAllDuties()
		{
			for (int i = 0; i < this.lord.ownedPawns.Count; i++)
			{
				Pawn pawn = this.lord.ownedPawns[i];
				if (pawn == this.Lead || pawn == this.Support)
				{
					pawn.mindState.duty.focus = this.venue;
					pawn.mindState.duty = new PawnDuty(DutyDefOfRazzleDazzle.GoToStageAndWait, this.Venue, -1f);
				}
				else
				{
					PawnDuty duty = new PawnDuty(DutyDefOfRazzleDazzle.FindSeatWithStageViewAndChat, this.Venue, -1f);
					pawn.mindState.duty = duty;
				}
			}
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00005114 File Offset: 0x00003314
		public override ThinkTreeDutyHook VoluntaryJoinDutyHookFor(Pawn p)
		{
			ThinkTreeDutyHook hook;
			if (p == this.Lead || p == this.Support)
			{
				hook = DutyDefOfRazzleDazzle.PerformConcert.hook;
			}
			else
			{
				hook = DutyDefOf.Spectate.hook;
			}
			return hook;
		}

		// Token: 0x04000038 RID: 56
		private Building_Performance venue;
	}
}
