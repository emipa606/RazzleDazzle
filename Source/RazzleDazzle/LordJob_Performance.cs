using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RazzleDazzle
{
	// Token: 0x0200001C RID: 28
	public class LordJob_Performance : LordJob_VoluntarilyJoinable
	{
		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600007C RID: 124 RVA: 0x00003EAD File Offset: 0x000020AD
		public Building_Performance Venue
		{
			get
			{
				return this.venue;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600007D RID: 125 RVA: 0x00003EB5 File Offset: 0x000020B5
		public Pawn Lead
		{
			get
			{
				return this.venue.Lead;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600007E RID: 126 RVA: 0x00003EC2 File Offset: 0x000020C2
		public Pawn Support
		{
			get
			{
				return this.venue.Support;
			}
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00003ECF File Offset: 0x000020CF
		public LordJob_Performance()
		{
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00003ED7 File Offset: 0x000020D7
		public LordJob_Performance(Building_Performance venue)
		{
			this.venue = venue;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00003EE6 File Offset: 0x000020E6
		public override void ExposeData()
		{
			Scribe_References.Look<Building_Performance>(ref this.venue, "venue", false);
			base.ExposeData();
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00003F00 File Offset: 0x00002100
		public bool IsPerformanceValid()
		{
			return this.Venue != null && this.Lead != null && (this.Venue.venueDef.performersNeeded <= 1 || this.Support != null) && !(this.Venue.artTitle == "");
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00003F52 File Offset: 0x00002152
		public bool HasPerformanceFinished()
		{
			return this.lord.ticksInToil > this.Venue.venueDef.minTicksInPerformance && Rand.Chance(0.01f);
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00003F80 File Offset: 0x00002180
		public bool TryStartPerformance()
		{
			bool result;
			if (!this.IsPerformanceValid())
			{
				result = false;
			}
			else
			{
				LordMaker.MakeNewLord(Faction.OfPlayer, this, this.Venue.Map, null);
				result = true;
			}
			return result;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00003FB4 File Offset: 0x000021B4
		public bool PerformersAreReady()
		{
			bool result;
			if (!this.IsValidLead(this.Lead) || !GatheringsUtility.InGatheringArea(this.Lead.Position, this.Venue.Position, this.Venue.Map))
			{
				result = false;
			}
			else
			{
				if (this.Venue.venueDef.performersNeeded > 1 && (!this.IsValidSupport(this.Support) || !GatheringsUtility.InGatheringArea(this.Support.Position, this.Venue.Position, this.Venue.Map)))
				{
					return false;
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x0000404B File Offset: 0x0000224B
		private bool IsValidLead(Pawn p)
		{
			return p == this.Lead && !p.Dead && !p.Downed && p.health.capacities.CapableOf(PawnCapacityDefOf.Moving);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x0000407D File Offset: 0x0000227D
		private bool IsValidSupport(Pawn p)
		{
			return p == this.Support && !p.Dead && !p.Downed && p.health.capacities.CapableOf(PawnCapacityDefOf.Moving);
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000040AF File Offset: 0x000022AF
		private bool IsValidAttendee(Pawn p)
		{
			return p != null && p.Spawned && GatheringsUtility.ShouldGuestKeepAttendingGathering(p) && !p.Faction.HostileTo(Faction.OfPlayer);
		}

		// Token: 0x06000089 RID: 137 RVA: 0x000040E0 File Offset: 0x000022E0
		private bool ShouldPlayBeCalledOff()
		{
			return this.Lead.DestroyedOrNull() || this.Lead.Dead || this.Venue.Position.GetDangerFor(this.Lead, this.venue.Map) != Danger.None || (this.Venue.venueDef.performersNeeded > 1 && (this.Support.DestroyedOrNull() || this.Support.Dead || this.Venue.Position.GetDangerFor(this.Support, this.venue.Map) != Danger.None)) || this.Venue.IsBurning() || this.Venue.Destroyed || !GatheringsUtility.AcceptableGameConditionsToContinueGathering(this.Venue.Map);
		}

		// Token: 0x0600008A RID: 138 RVA: 0x000041B1 File Offset: 0x000023B1
		protected virtual float GetStatModifier()
		{
			return 1f;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x000041B8 File Offset: 0x000023B8
		protected virtual float GetFinalQuality()
		{
			float num = (float)this.GetQualityModifier(this.Venue.artQuality);
			float num2 = (float)this.GetQualityModifier(QualityUtility.GenerateQualityCreatedByPawn(this.Lead, SkillDefOf.Social));
			float statModifier = this.GetStatModifier();
			if (this.Venue.venueDef.performersNeeded > 1)
			{
				num2 = 0.6f * num2 + 0.4f * (float)this.GetQualityModifier(QualityUtility.GenerateQualityCreatedByPawn(this.Support, SkillDefOf.Social));
			}
			num2 *= statModifier;
			return Rand.Range(num2, num2 + num);
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00004240 File Offset: 0x00002440
		protected int GetQualityModifier(QualityCategory qc)
		{
			int result = 0;
			switch (qc)
			{
			case QualityCategory.Awful:
				result = 0;
				break;
			case QualityCategory.Poor:
				result = 3;
				break;
			case QualityCategory.Normal:
				result = 5;
				break;
			case QualityCategory.Good:
				result = 7;
				break;
			case QualityCategory.Excellent:
				result = 12;
				break;
			case QualityCategory.Masterwork:
				result = 15;
				break;
			case QualityCategory.Legendary:
				result = 20;
				break;
			}
			return result;
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00004291 File Offset: 0x00002491
		public virtual string GetMessageForQualityLevel(float fQ)
		{
			return "";
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00004298 File Offset: 0x00002498
		public virtual ThoughtDef GetThoughtForQualityLevel(float fQ)
		{
			return ThoughtDefOfRazzleDazzle.AttendedConcert;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000042A0 File Offset: 0x000024A0
		protected virtual void UpdateBroadcastTower(float qualityScore)
		{
			List<Thing> list = this.Venue.Map.listerThings.ThingsOfDef(ThingDefOf_RazzleDazzle.BroadcastTowerDef);
			list = (from t in list
			where !t.IsForbidden(Faction.OfPlayer)
			select t).ToList<Thing>();
			if (!list.NullOrEmpty<Thing>())
			{
				(list.FirstOrDefault<Thing>() as Building_BroadcastTower).qualityValue += this.Venue.venueDef.entertainmentWeight * (qualityScore - 5f);
			}
		}

		// Token: 0x06000090 RID: 144 RVA: 0x0000432C File Offset: 0x0000252C
		public virtual void GiveWatchedPlayThoughts()
		{
			float finalQuality = this.GetFinalQuality();
			if (finalQuality >= 14f)
			{
				if (this.Venue.venueDef.performersNeeded > 1)
				{
					TaleRecorder.RecordTale(this.Venue.venueDef.taleDef, new object[]
					{
						this.Lead,
						this.Support
					});
				}
				else
				{
					TaleRecorder.RecordTale(this.Venue.venueDef.taleDef, new object[]
					{
						this.Lead
					});
				}
			}
			string messageForQualityLevel = this.GetMessageForQualityLevel(finalQuality);
			ThoughtDef thoughtForQualityLevel = this.GetThoughtForQualityLevel(finalQuality);
			foreach (Pawn pawn in this.lord.ownedPawns)
			{
				if (pawn == this.Lead || pawn == this.Support)
				{
					if (this.Venue.GetRoom(RegionType.Set_Passable).GetStat(RoomStatDefOf.Impressiveness) >= 160f)
					{
						pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfRazzleDazzle.PerformedInExtremelyImpressiveSpace, null);
					}
					else if (this.Venue.GetRoom(RegionType.Set_Passable).GetStat(RoomStatDefOf.Impressiveness) >= 115f)
					{
						pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfRazzleDazzle.PerformedInVeryImpressiveSpace, null);
					}
					else if (this.Venue.GetRoom(RegionType.Set_Passable).GetStat(RoomStatDefOf.Impressiveness) >= 80f)
					{
						pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfRazzleDazzle.PerformedInImpressiveSpace, null);
					}
					else
					{
						pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfRazzleDazzle.Performed, null);
					}
				}
				else
				{
					pawn.needs.mood.thoughts.memories.TryGainMemory(thoughtForQualityLevel, null);
				}
			}
			this.UpdateBroadcastTower(finalQuality);
			Messages.Message(messageForQualityLevel, this.Venue, MessageTypeDefOf.NeutralEvent, true);
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00004554 File Offset: 0x00002754
		public override float VoluntaryJoinPriorityFor(Pawn p)
		{
			float result;
			if (this.IsValidLead(p))
			{
				result = 100f;
			}
			else if (this.IsValidSupport(p))
			{
				result = 100f;
			}
			else if (this.IsValidAttendee(p))
			{
				result = 20f;
			}
			else
			{
				result = 0f;
			}
			return result;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x0000459B File Offset: 0x0000279B
		protected virtual LordToil GetPerformanceLordToil()
		{
			return new LordToil_End();
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000045A4 File Offset: 0x000027A4
		public override StateGraph CreateGraph()
		{
			StateGraph stateGraph = new StateGraph();
			LordToil lordToil = new LordToil_PrePerformance(this.Venue);
			LordToil lordToil2 = new LordToil_SettleForPerformance(this.Venue);
			LordToil performanceLordToil = this.GetPerformanceLordToil();
			LordToil lordToil3 = new LordToil_EndPerformance(this.Venue);
			stateGraph.AddToil(lordToil);
			stateGraph.AddToil(lordToil2);
			stateGraph.AddToil(performanceLordToil);
			stateGraph.AddToil(lordToil3);
			Transition transition = new Transition(lordToil, lordToil2, false, true);
			Transition transition2 = new Transition(lordToil, lordToil3, false, true);
			Transition transition3 = new Transition(lordToil, lordToil3, false, true);
			transition.AddPreAction(new TransitionAction_Message(Translator.Translate("RAZ_PerformanceStarting"), MessageTypeDefOf.PositiveEvent, null, 1f));
			transition.AddPreAction(new TransitionAction_Custom(delegate()
			{
				this.Venue.ticksIntoThisPerformance = 0;
			}));
			transition.AddTrigger(new Trigger_TickCondition(() => this.lord.ticksInToil >= 3000 && this.PerformersAreReady(), 1));
			transition2.AddTrigger(new Trigger_TickCondition(() => this.lord.ticksInToil >= 6000, 1));
			transition2.AddPreAction(new TransitionAction_Message(Translator.Translate("RAZ_PerformanceCancelledLate"), null, 1f));
			transition3.AddTrigger(new Trigger_TickCondition(() => this.ShouldPlayBeCalledOff(), 1));
			transition3.AddPreAction(new TransitionAction_Message(Translator.Translate("RAZ_PerformanceCancelledThreat"), null, 1f));
			stateGraph.AddTransition(transition, false);
			stateGraph.AddTransition(transition2, false);
			stateGraph.AddTransition(transition3, false);
			Transition transition4 = new Transition(lordToil2, performanceLordToil, false, true);
			Transition transition5 = new Transition(lordToil2, lordToil3, false, true);
			transition4.AddTrigger(new Trigger_TickCondition(() => this.lord.ticksInToil > 500, 1));
			transition5.AddTrigger(new Trigger_TickCondition(() => this.ShouldPlayBeCalledOff(), 1));
			stateGraph.AddTransition(transition4, false);
			stateGraph.AddTransition(transition5, false);
			Transition transition6 = new Transition(performanceLordToil, lordToil3, false, true);
			transition6.AddTrigger(new Trigger_TickCondition(() => this.HasPerformanceFinished() || this.lord.ticksInToil > 16000, 1));
			transition6.AddPreAction(new TransitionAction_EndAllJobs());
			transition6.AddPreAction(new TransitionAction_Custom(delegate()
			{
				this.GiveWatchedPlayThoughts();
			}));
			Transition transition7 = new Transition(performanceLordToil, lordToil3, false, true);
			transition7.AddTrigger(new Trigger_TickCondition(() => this.ShouldPlayBeCalledOff(), 1));
			transition7.AddPreAction(new TransitionAction_Message(Translator.Translate("RAZ_PerformanceCancelledThreat"), null, 1f));
			stateGraph.AddTransition(transition6, false);
			stateGraph.AddTransition(transition7, false);
			return stateGraph;
		}

		// Token: 0x0400002B RID: 43
		protected Building_Performance venue;
	}
}
