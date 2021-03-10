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
        public Building_Performance Venue { get; }

        // Token: 0x17000018 RID: 24
        // (get) Token: 0x060000C5 RID: 197 RVA: 0x0000502D File Offset: 0x0000322D
        public Pawn Lead => Venue.Lead;

        // Token: 0x17000019 RID: 25
        // (get) Token: 0x060000C6 RID: 198 RVA: 0x0000503A File Offset: 0x0000323A
        public Pawn Support => Venue.Support;

        // Token: 0x060000C7 RID: 199 RVA: 0x00005047 File Offset: 0x00003247
        public LordToil_SettleForPerformance(Building_Performance venue)
		{
			Venue = venue;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00005058 File Offset: 0x00003258
		public override void UpdateAllDuties()
		{
			for (var i = 0; i < lord.ownedPawns.Count; i++)
			{
				Pawn pawn = lord.ownedPawns[i];
				if (pawn == Lead || pawn == Support)
				{
					pawn.mindState.duty.focus = Venue;
					pawn.mindState.duty = new PawnDuty(DutyDefOfRazzleDazzle.GoToStageAndWait, Venue, -1f);
				}
				else
				{
					var duty = new PawnDuty(DutyDefOfRazzleDazzle.FindSeatWithStageViewAndChat, Venue, -1f);
					pawn.mindState.duty = duty;
				}
			}
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00005114 File Offset: 0x00003314
		public override ThinkTreeDutyHook VoluntaryJoinDutyHookFor(Pawn p)
		{
			ThinkTreeDutyHook hook;
			if (p == Lead || p == Support)
			{
				hook = DutyDefOfRazzleDazzle.PerformConcert.hook;
			}
			else
			{
				hook = DutyDefOf.Spectate.hook;
			}
			return hook;
		}
    }
}
