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
				if (director == null)
				{
                    director = new RazzleDazzle_Director
                    {
                        stage = this
                    };
                }
				return director;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600001D RID: 29 RVA: 0x00002970 File Offset: 0x00000B70
		// (set) Token: 0x0600001E RID: 30 RVA: 0x000029DA File Offset: 0x00000BDA
		public Pawn Lead
		{
			get
			{
				if (lead == null)
				{
					if (leadIndex != -1)
					{
						lead = FindColonistByID(leadIndex);
					}
					if (lead == null)
					{
						IEnumerable<Pawn> source = ColonistsAvailableToPerform();
						if (source.Count<Pawn>() > 0)
						{
							lead = source.RandomElement<Pawn>();
							leadIndex = lead.thingIDNumber;
						}
					}
				}
				return lead;
			}
			set
			{
				lead = value;
				leadIndex = lead.thingIDNumber;
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
				if (VenueDef.performersNeeded < 2)
				{
					result = null;
				}
				else
				{
					if (support == null || support == lead)
					{
						if (supportIndex != -1)
						{
							support = FindColonistByID(supportIndex);
						}
						if (support == null || support == lead)
						{
							var list = ColonistsAvailableToPerform().ToList<Pawn>();
							if (list.Contains(lead))
							{
								list.Remove(lead);
							}
							if (list.Count<Pawn>() > 0)
							{
								support = list.RandomElement<Pawn>();
								supportIndex = support.thingIDNumber;
							}
						}
					}
					result = support;
				}
				return result;
			}
			set
			{
				support = value;
				supportIndex = support.thingIDNumber;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00002ACB File Offset: 0x00000CCB
		public PerformanceVenueDef VenueDef
		{
			get
			{
				if (venDef == null)
				{
					venDef = (PerformanceVenueDef)def;
				}
				return venDef;
			}
		}

        // Token: 0x1700000A RID: 10
        // (get) Token: 0x06000022 RID: 34 RVA: 0x00002AEC File Offset: 0x00000CEC
        protected virtual string BasicInspectString => Translator.Translate("RAZ_VenueBasicInspectString");

        // Token: 0x06000023 RID: 35 RVA: 0x00002AF8 File Offset: 0x00000CF8
        protected FloatMenuOption SetAsLead(Pawn pawn)
		{
			return new FloatMenuOption(Translator.Translate("RAZ_ReplaceLead"), delegate()
			{
				Lead = pawn;
				if (rehearsedFraction > 0f)
				{
					rehearsedFraction /= 3f;
				}
			}, MenuOptionPriority.Default, null, null, 0f, null, null);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002B40 File Offset: 0x00000D40
		protected FloatMenuOption SetAsSupport(Pawn pawn)
		{
			return new FloatMenuOption(Translator.Translate("RAZ_ReplaceSupport"), delegate()
			{
				Support = pawn;
				if (rehearsedFraction > 0f)
				{
					rehearsedFraction /= 2f;
				}
			}, MenuOptionPriority.Default, null, null, 0f, null, null);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002B88 File Offset: 0x00000D88
		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
		{
			IEnumerable<FloatMenuOption> result;
			if (rehearsedFraction >= 1f)
			{
				result = base.GetFloatMenuOptions(selPawn);
			}
			else if (Lead == null || selPawn == null)
			{
				result = base.GetFloatMenuOptions(selPawn);
			}
			else
			{
				var flag = VenueDef.performersNeeded > 1;
				if (flag && Support == null)
				{
					result = base.GetFloatMenuOptions(selPawn);
				}
				else
				{
					var list = base.GetFloatMenuOptions(selPawn).ToList<FloatMenuOption>();
					if (selPawn != Lead && selPawn.workSettings.WorkIsActive(WorkTypeDefOfRazzleDazzle.Performance))
					{
						list.Add(SetAsLead(selPawn));
					}
					if (flag && selPawn != Lead && selPawn != Support && selPawn.workSettings.WorkIsActive(WorkTypeDefOfRazzleDazzle.Performance))
					{
						list.Add(SetAsSupport(selPawn));
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
			var stringBuilder = new StringBuilder();
			if (rehearsing)
			{
				if (rehearsedFraction < 1f)
				{
					stringBuilder.AppendLine(TranslatorFormattedStringExtensions.Translate("RAZ_Rehearsal_Venue", artTitle));
					if (Lead != null)
					{
						if (Support == null)
						{
							stringBuilder.AppendLine(TranslatorFormattedStringExtensions.Translate("RAZ_Rehearsal_Performer", Lead.Named("PAWN")));
						}
						else
						{
							stringBuilder.AppendLine(TranslatorFormattedStringExtensions.Translate("RAZ_Rehearsal_Performers", Lead.Named("PAWN1"), Support.Named("PAWN2")));
						}
					}
					stringBuilder.AppendLine(TranslatorFormattedStringExtensions.Translate("RAZ_Rehearsal_Progress", rehearsedFraction.ToStringPercent()));
				}
				else
				{
					stringBuilder.AppendLine(TranslatorFormattedStringExtensions.Translate("RAZ_PerformancePending", artTitle));
				}
			}
			else
			{
				stringBuilder.AppendLine(BasicInspectString);
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
			Scribe_Values.Look<int>(ref ticksSinceLastPerformance, "ticksSinceLastPerformance", 0, false);
			Scribe_Values.Look<int>(ref ticksIntoThisPerformance, "ticksIntoThisPerformance", 0, false);
			Scribe_Values.Look<bool>(ref rehearsing, "rehearsing", false, false);
			Scribe_Values.Look<string>(ref artTitle, "artTitle", "", false);
			Scribe_Values.Look<string>(ref artistName, "artistName", "", false);
			Scribe_Values.Look<QualityCategory>(ref artQuality, "artQuality", QualityCategory.Normal, false);
			Scribe_Values.Look<float>(ref rehearsedFraction, "rehearsedFraction", 0f, false);
			Scribe_Values.Look<int>(ref leadIndex, "leadIndex", -1, false);
			Scribe_Values.Look<int>(ref supportIndex, "supportIndex", -1, false);
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002E90 File Offset: 0x00001090
		public Thing SelectPerformThing()
		{
			Thing result;
			if (CurrentlyUsableForBills())
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
			return GenLocalDate.HourOfDay(Map) >= 13 && GenLocalDate.HourOfDay(Map) <= 21 && Map.dangerWatcher.DangerRating == StoryDanger.None && Map.mapPawns.FreeColonistsSpawnedCount >= 4;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002F0C File Offset: 0x0000110C
		public IEnumerable<Pawn> ColonistsAvailableToPerform()
		{
			IEnumerable<Pawn> result;
			if (Map == null || Map.mapPawns == null)
			{
				result = new List<Pawn>();
			}
			else
			{
				result = from colonist in Map.mapPawns.FreeColonistsSpawned
				where colonist.workSettings.WorkIsActive(WorkTypeDefOfRazzleDazzle.Performance) && !colonist.Downed && !colonist.Dead && colonist.Awake() && colonist.health.capacities.CapableOf(PawnCapacityDefOf.Moving)
				select colonist;
			}
			return result;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002F6C File Offset: 0x0000116C
		public bool IsPerformanceHappeningElsewhere()
		{
			foreach (Pawn p in Map.mapPawns.FreeColonistsSpawned)
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
			CompForbiddable comp = GetComp<CompForbiddable>();
			return comp != null && comp.Forbidden;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x0000302C File Offset: 0x0000122C
		public virtual bool CanDoPerformanceNow()
		{
			return !IsForbidden() && canPerformSwitch && rehearsing && ColonistsAvailableToPerform().Count<Pawn>() >= VenueDef.performersNeeded && ticksSinceLastPerformance >= VenueDef.ticksBetweenPerformances && IsGoodAndSafeTimeForPerformance();
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00003084 File Offset: 0x00001284
		public virtual bool TryToStartPerformance()
		{
			ticksSinceLastPerformance = 0;
			Thing thing = SelectPerformThing();
			return Lead != null && thing != null && (Support != null || VenueDef.performersNeeded <= 1) && !Lead.InMentalState && (Support == null || !Support.InMentalState) && Lead.mindState.duty == null && (Support == null || Support.mindState.duty == null);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003113 File Offset: 0x00001313
		public void ClearPerformers()
		{
			lead = null;
			leadIndex = -1;
			support = null;
			supportIndex = -1;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003134 File Offset: 0x00001334
		public override void TickRare()
		{
			base.TickRare();
			if (ticksSinceLastPerformance < VenueDef.ticksBetweenPerformances)
			{
				ticksSinceLastPerformance++;
				return;
			}
			if (CanDoPerformanceNow())
			{
				if (rehearsing && rehearsedFraction < 1f)
				{
					if (Lead != null && (Lead.Dead || Lead.Faction != Faction.OfPlayer))
					{
						Messages.Message(TranslatorFormattedStringExtensions.Translate("RAZ_MessageRehearsalsDelayed", Lead.Named("PAWN")), MessageTypeDefOf.NegativeEvent, true);
						lead = null;
						leadIndex = -1;
						rehearsedFraction /= 3f;
					}
					if (VenueDef.performersNeeded > 1 && Support != null && (Support.Dead || Support.Faction != Faction.OfPlayer))
					{
						Messages.Message(TranslatorFormattedStringExtensions.Translate("RAZ_MessageRehearsalsDelayed", Support.Named("PAWN")), MessageTypeDefOf.NegativeEvent, true);
						support = null;
						supportIndex = -1;
						rehearsedFraction /= 3f;
						return;
					}
				}
				else if (Rand.Value <= 0.1f)
				{
					TryToStartPerformance();
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
