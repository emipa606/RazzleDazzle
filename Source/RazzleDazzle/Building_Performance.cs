using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RazzleDazzle
{
	// Token: 0x02000007 RID: 7
	[StaticConstructorOnStartup]
	public class Building_Performance : Building_WorkTable
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600001C RID: 28 RVA: 0x00002947 File Offset: 0x00000B47
		public RazzleDazzle_Director Director
		{
			get
			{
				if (this.director == null)
				{
					this.director = new RazzleDazzle_Director();
					this.director.stage = this;
				}
				return this.director;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600001D RID: 29 RVA: 0x00002970 File Offset: 0x00000B70
		// (set) Token: 0x0600001E RID: 30 RVA: 0x000029DA File Offset: 0x00000BDA
		public Pawn Lead
		{
			get
			{
				if (this.lead == null)
				{
					if (this.leadIndex != -1)
					{
						this.lead = Building_Performance.FindColonistByID(this.leadIndex);
					}
					if (this.lead == null)
					{
						IEnumerable<Pawn> source = this.ColonistsAvailableToPerform();
						if (source.Count<Pawn>() > 0)
						{
							this.lead = source.RandomElement<Pawn>();
							this.leadIndex = this.lead.thingIDNumber;
						}
					}
				}
				return this.lead;
			}
			set
			{
				this.lead = value;
				this.leadIndex = this.lead.thingIDNumber;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600001F RID: 31 RVA: 0x000029F4 File Offset: 0x00000BF4
		// (set) Token: 0x06000020 RID: 32 RVA: 0x00002AB1 File Offset: 0x00000CB1
		public Pawn Support
		{
			get
			{
				Pawn result;
				if (this.venueDef.performersNeeded < 2)
				{
					result = null;
				}
				else
				{
					if (this.support == null || this.support == this.lead)
					{
						if (this.supportIndex != -1)
						{
							this.support = Building_Performance.FindColonistByID(this.supportIndex);
						}
						if (this.support == null || this.support == this.lead)
						{
							List<Pawn> list = this.ColonistsAvailableToPerform().ToList<Pawn>();
							if (list.Contains(this.lead))
							{
								list.Remove(this.lead);
							}
							if (list.Count<Pawn>() > 0)
							{
								this.support = list.RandomElement<Pawn>();
								this.supportIndex = this.support.thingIDNumber;
							}
						}
					}
					result = this.support;
				}
				return result;
			}
			set
			{
				this.support = value;
				this.supportIndex = this.support.thingIDNumber;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00002ACB File Offset: 0x00000CCB
		public PerformanceVenueDef venueDef
		{
			get
			{
				if (this.venDef == null)
				{
					this.venDef = (PerformanceVenueDef)this.def;
				}
				return this.venDef;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000022 RID: 34 RVA: 0x00002AEC File Offset: 0x00000CEC
		protected virtual string BasicInspectString
		{
			get
			{
				return Translator.Translate("RAZ_VenueBasicInspectString");
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002AF8 File Offset: 0x00000CF8
		protected FloatMenuOption SetAsLead(Pawn pawn)
		{
			return new FloatMenuOption(Translator.Translate("RAZ_ReplaceLead"), delegate()
			{
				this.Lead = pawn;
				if (this.rehearsedFraction > 0f)
				{
					this.rehearsedFraction /= 3f;
				}
			}, MenuOptionPriority.Default, null, null, 0f, null, null);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002B40 File Offset: 0x00000D40
		protected FloatMenuOption SetAsSupport(Pawn pawn)
		{
			return new FloatMenuOption(Translator.Translate("RAZ_ReplaceSupport"), delegate()
			{
				this.Support = pawn;
				if (this.rehearsedFraction > 0f)
				{
					this.rehearsedFraction /= 2f;
				}
			}, MenuOptionPriority.Default, null, null, 0f, null, null);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002B88 File Offset: 0x00000D88
		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
		{
			IEnumerable<FloatMenuOption> result;
			if (this.rehearsedFraction >= 1f)
			{
				result = base.GetFloatMenuOptions(selPawn);
			}
			else if (this.Lead == null || selPawn == null)
			{
				result = base.GetFloatMenuOptions(selPawn);
			}
			else
			{
				bool flag = this.venueDef.performersNeeded > 1;
				if (flag && this.Support == null)
				{
					result = base.GetFloatMenuOptions(selPawn);
				}
				else
				{
					List<FloatMenuOption> list = base.GetFloatMenuOptions(selPawn).ToList<FloatMenuOption>();
					if (selPawn != this.Lead && selPawn.workSettings.WorkIsActive(WorkTypeDefOfRazzleDazzle.Performance))
					{
						list.Add(this.SetAsLead(selPawn));
					}
					if (flag && selPawn != this.Lead && selPawn != this.Support && selPawn.workSettings.WorkIsActive(WorkTypeDefOfRazzleDazzle.Performance))
					{
						list.Add(this.SetAsSupport(selPawn));
					}
					result = list;
				}
			}
			return result;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002C58 File Offset: 0x00000E58
		protected static Pawn FindColonistByID(int IDnum)
		{
			if (IDnum != -1)
			{
				foreach (Pawn pawn in RazzleDazzleUtilities.GetAllFreeColonistsAlive)
				{
					if (pawn.thingIDNumber == IDnum)
					{
						return pawn;
					}
				}
				return null;
			}
			return null;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002CBC File Offset: 0x00000EBC
		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.rehearsing)
			{
				if (this.rehearsedFraction < 1f)
				{
					stringBuilder.AppendLine(TranslatorFormattedStringExtensions.Translate("RAZ_Rehearsal_Venue", this.artTitle));
					if (this.Lead != null)
					{
						if (this.Support == null)
						{
							stringBuilder.AppendLine(TranslatorFormattedStringExtensions.Translate("RAZ_Rehearsal_Performer", this.Lead.Named("PAWN")));
						}
						else
						{
							stringBuilder.AppendLine(TranslatorFormattedStringExtensions.Translate("RAZ_Rehearsal_Performers", this.Lead.Named("PAWN1"), this.Support.Named("PAWN2")));
						}
					}
					stringBuilder.AppendLine(TranslatorFormattedStringExtensions.Translate("RAZ_Rehearsal_Progress", this.rehearsedFraction.ToStringPercent()));
				}
				else
				{
					stringBuilder.AppendLine(TranslatorFormattedStringExtensions.Translate("RAZ_PerformancePending", this.artTitle));
				}
			}
			else
			{
				stringBuilder.AppendLine(this.BasicInspectString);
			}
			return stringBuilder.ToString().TrimEndNewlines();
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002DC3 File Offset: 0x00000FC3
		public override void Draw()
		{
			base.Draw();
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002DCC File Offset: 0x00000FCC
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.ticksSinceLastPerformance, "ticksSinceLastPerformance", 0, false);
			Scribe_Values.Look<int>(ref this.ticksIntoThisPerformance, "ticksIntoThisPerformance", 0, false);
			Scribe_Values.Look<bool>(ref this.rehearsing, "rehearsing", false, false);
			Scribe_Values.Look<string>(ref this.artTitle, "artTitle", "", false);
			Scribe_Values.Look<string>(ref this.artistName, "artistName", "", false);
			Scribe_Values.Look<QualityCategory>(ref this.artQuality, "artQuality", QualityCategory.Normal, false);
			Scribe_Values.Look<float>(ref this.rehearsedFraction, "rehearsedFraction", 0f, false);
			Scribe_Values.Look<int>(ref this.leadIndex, "leadIndex", -1, false);
			Scribe_Values.Look<int>(ref this.supportIndex, "supportIndex", -1, false);
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002E90 File Offset: 0x00001090
		public Thing SelectPerformThing()
		{
			Thing result;
			if (base.CurrentlyUsableForBills())
			{
				result = this;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002EAC File Offset: 0x000010AC
		public bool IsGoodAndSafeTimeForPerformance()
		{
			return GenLocalDate.HourOfDay(base.Map) >= 13 && GenLocalDate.HourOfDay(base.Map) <= 21 && base.Map.dangerWatcher.DangerRating == StoryDanger.None && base.Map.mapPawns.FreeColonistsSpawnedCount >= 4;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002F0C File Offset: 0x0000110C
		public IEnumerable<Pawn> ColonistsAvailableToPerform()
		{
			IEnumerable<Pawn> result;
			if (base.Map == null || base.Map.mapPawns == null)
			{
				result = new List<Pawn>();
			}
			else
			{
				result = from colonist in base.Map.mapPawns.FreeColonistsSpawned
				where colonist.workSettings.WorkIsActive(WorkTypeDefOfRazzleDazzle.Performance) && !colonist.Downed && !colonist.Dead && colonist.Awake() && colonist.health.capacities.CapableOf(PawnCapacityDefOf.Moving)
				select colonist;
			}
			return result;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002F6C File Offset: 0x0000116C
		public bool IsPerformanceHappeningElsewhere()
		{
			foreach (Pawn p in base.Map.mapPawns.FreeColonistsSpawned)
			{
				if (p.GetLord() != null && (p.GetLord().LordJob is LordJob_Joinable_MarriageCeremony || p.GetLord().LordJob is LordJob_Joinable_Party || p.GetLord().LordJob is LordJob_Performance))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00003004 File Offset: 0x00001204
		public virtual bool IsForbidden()
		{
			if (this == null)
			{
				return false;
			}
			CompForbiddable comp = this.GetComp<CompForbiddable>();
			return comp != null && comp.Forbidden;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x0000302C File Offset: 0x0000122C
		public virtual bool CanDoPerformanceNow()
		{
			return !this.IsForbidden() && this.canPerformSwitch && this.rehearsing && this.ColonistsAvailableToPerform().Count<Pawn>() >= this.venueDef.performersNeeded && this.ticksSinceLastPerformance >= this.venueDef.ticksBetweenPerformances && this.IsGoodAndSafeTimeForPerformance();
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00003084 File Offset: 0x00001284
		public virtual bool TryToStartPerformance()
		{
			this.ticksSinceLastPerformance = 0;
			Thing thing = this.SelectPerformThing();
			return this.Lead != null && thing != null && (this.Support != null || this.venueDef.performersNeeded <= 1) && !this.Lead.InMentalState && (this.Support == null || !this.Support.InMentalState) && this.Lead.mindState.duty == null && (this.Support == null || this.Support.mindState.duty == null);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003113 File Offset: 0x00001313
		public void ClearPerformers()
		{
			this.lead = null;
			this.leadIndex = -1;
			this.support = null;
			this.supportIndex = -1;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003134 File Offset: 0x00001334
		public override void TickRare()
		{
			base.TickRare();
			if (this.ticksSinceLastPerformance < this.venueDef.ticksBetweenPerformances)
			{
				this.ticksSinceLastPerformance++;
				return;
			}
			if (this.CanDoPerformanceNow())
			{
				if (this.rehearsing && this.rehearsedFraction < 1f)
				{
					if (this.Lead != null && (this.Lead.Dead || this.Lead.Faction != Faction.OfPlayer))
					{
						Messages.Message(TranslatorFormattedStringExtensions.Translate("RAZ_MessageRehearsalsDelayed", this.Lead.Named("PAWN")), MessageTypeDefOf.NegativeEvent, true);
						this.lead = null;
						this.leadIndex = -1;
						this.rehearsedFraction /= 3f;
					}
					if (this.venueDef.performersNeeded > 1 && this.Support != null && (this.Support.Dead || this.Support.Faction != Faction.OfPlayer))
					{
						Messages.Message(TranslatorFormattedStringExtensions.Translate("RAZ_MessageRehearsalsDelayed", this.Support.Named("PAWN")), MessageTypeDefOf.NegativeEvent, true);
						this.support = null;
						this.supportIndex = -1;
						this.rehearsedFraction /= 3f;
						return;
					}
				}
				else if (Rand.Value <= 0.1f)
				{
					this.TryToStartPerformance();
				}
			}
		}

		// Token: 0x0400000B RID: 11
		protected PerformanceVenueDef venDef;

		// Token: 0x0400000C RID: 12
		public int ticksSinceLastPerformance = 5000;

		// Token: 0x0400000D RID: 13
		public int ticksIntoThisPerformance;

		// Token: 0x0400000E RID: 14
		public bool canPerformSwitch = true;

		// Token: 0x0400000F RID: 15
		public bool rehearsing;

		// Token: 0x04000010 RID: 16
		public float rehearsedFraction;

		// Token: 0x04000011 RID: 17
		public QualityCategory artQuality = QualityCategory.Normal;

		// Token: 0x04000012 RID: 18
		public string artistName = "";

		// Token: 0x04000013 RID: 19
		public string artTitle = "";

		// Token: 0x04000014 RID: 20
		private int leadIndex = -1;

		// Token: 0x04000015 RID: 21
		private int supportIndex = -1;

		// Token: 0x04000016 RID: 22
		private Pawn lead;

		// Token: 0x04000017 RID: 23
		private Pawn support;

		// Token: 0x04000018 RID: 24
		private RazzleDazzle_Director director;
	}
}
