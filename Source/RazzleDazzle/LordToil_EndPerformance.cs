﻿using System;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RazzleDazzle
{
	// Token: 0x02000023 RID: 35
	public class LordToil_EndPerformance : LordToil
	{
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x00003512 File Offset: 0x00001712
		public override bool ShouldFail
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00004DC3 File Offset: 0x00002FC3
		public LordToil_EndPerformance(Building_Performance venue)
		{
			this.venue = venue;
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00004DD4 File Offset: 0x00002FD4
		public void CleanUpStage()
		{
			this.venue.rehearsing = false;
			this.venue.rehearsedFraction = 0f;
			this.venue.ticksIntoThisPerformance = 0;
			this.venue.artTitle = "";
			this.venue.artistName = "";
			this.venue.artQuality = QualityCategory.Normal;
			this.venue.ClearPerformers();
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00004E40 File Offset: 0x00003040
		public override void UpdateAllDuties()
		{
			foreach (Pawn pawn in this.lord.ownedPawns)
			{
				pawn.jobs.StopAll(false);
			}
			this.CleanUpStage();
		}

		// Token: 0x04000033 RID: 51
		private Building_Performance venue;
	}
}