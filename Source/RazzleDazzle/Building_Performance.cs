using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RazzleDazzle;

[StaticConstructorOnStartup]
public class Building_Performance : Building_WorkTable
{
    public readonly bool canPerformSwitch = true;
    public string artistName = "";

    public QualityCategory artQuality = QualityCategory.Normal;

    public string artTitle = "";

    private RazzleDazzle_Director director;

    private Pawn lead;

    private int leadIndex = -1;

    public float rehearsedFraction;

    public bool rehearsing;

    private Pawn support;

    private int supportIndex = -1;

    public int ticksIntoThisPerformance;

    private int ticksSinceLastPerformance = 5000;

    private PerformanceVenueDef venDef;

    public RazzleDazzle_Director Director
    {
        get
        {
            director ??= new RazzleDazzle_Director
            {
                stage = this
            };

            return director;
        }
    }

    public Pawn Lead
    {
        get
        {
            if (lead != null)
            {
                return lead;
            }

            if (leadIndex != -1)
            {
                lead = FindColonistByID(leadIndex);
            }

            if (lead != null)
            {
                return lead;
            }

            var source = ColonistsAvailableToPerform();
            if (!source.Any())
            {
                return lead;
            }

            lead = source.RandomElement();
            leadIndex = lead.thingIDNumber;

            return lead;
        }
        set
        {
            lead = value;
            leadIndex = lead.thingIDNumber;
        }
    }

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
                        var list = ColonistsAvailableToPerform().ToList();
                        if (list.Contains(lead))
                        {
                            list.Remove(lead);
                        }

                        if (list.Any())
                        {
                            support = list.RandomElement();
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

    public PerformanceVenueDef VenueDef
    {
        get
        {
            venDef ??= (PerformanceVenueDef)def;

            return venDef;
        }
    }

    protected virtual string BasicInspectString => "RAZ_VenueBasicInspectString".Translate();

    private FloatMenuOption SetAsLead(Pawn pawn)
    {
        return new FloatMenuOption("RAZ_ReplaceLead".Translate(), delegate
        {
            Lead = pawn;
            if (rehearsedFraction > 0f)
            {
                rehearsedFraction /= 3f;
            }
        });
    }

    private FloatMenuOption SetAsSupport(Pawn pawn)
    {
        return new FloatMenuOption("RAZ_ReplaceSupport".Translate(), delegate
        {
            Support = pawn;
            if (rehearsedFraction > 0f)
            {
                rehearsedFraction /= 2f;
            }
        });
    }

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
            if (VenueDef.performersNeeded > 1 && Support == null)
            {
                result = base.GetFloatMenuOptions(selPawn);
            }
            else
            {
                var list = base.GetFloatMenuOptions(selPawn).ToList();
                if (selPawn != Lead && selPawn.workSettings.WorkIsActive(WorkTypeDefOfRazzleDazzle.Performance))
                {
                    list.Add(SetAsLead(selPawn));
                }

                if (VenueDef.performersNeeded > 1 && selPawn != Lead && selPawn != Support &&
                    selPawn.workSettings.WorkIsActive(WorkTypeDefOfRazzleDazzle.Performance))
                {
                    list.Add(SetAsSupport(selPawn));
                }

                result = list;
            }
        }

        return result;
    }

    private static Pawn FindColonistByID(int IDnum)
    {
        if (IDnum == -1)
        {
            return null;
        }

        foreach (var pawn in RazzleDazzleUtilities.GetAllFreeColonistsAlive)
        {
            if (pawn.thingIDNumber == IDnum)
            {
                return pawn;
            }
        }

        return null;
    }

    public override string GetInspectString()
    {
        var stringBuilder = new StringBuilder();
        if (rehearsing)
        {
            if (rehearsedFraction < 1f)
            {
                stringBuilder.AppendLine("RAZ_Rehearsal_Venue".Translate(artTitle));
                if (Lead != null)
                {
                    stringBuilder.AppendLine(Support == null
                        ? "RAZ_Rehearsal_Performer".Translate(Lead.Named("PAWN"))
                        : "RAZ_Rehearsal_Performers".Translate(Lead.Named("PAWN1"), Support.Named("PAWN2")));
                }

                stringBuilder.AppendLine("RAZ_Rehearsal_Progress".Translate(rehearsedFraction.ToStringPercent()));
            }
            else
            {
                stringBuilder.AppendLine("RAZ_PerformancePending".Translate(artTitle));
            }
        }
        else
        {
            stringBuilder.AppendLine(BasicInspectString);
        }

        return stringBuilder.ToString().TrimEndNewlines();
    }


    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref ticksSinceLastPerformance, "ticksSinceLastPerformance");
        Scribe_Values.Look(ref ticksIntoThisPerformance, "ticksIntoThisPerformance");
        Scribe_Values.Look(ref rehearsing, "rehearsing");
        Scribe_Values.Look(ref artTitle, "artTitle", "");
        Scribe_Values.Look(ref artistName, "artistName", "");
        Scribe_Values.Look(ref artQuality, "artQuality", QualityCategory.Normal);
        Scribe_Values.Look(ref rehearsedFraction, "rehearsedFraction");
        Scribe_Values.Look(ref leadIndex, "leadIndex", -1);
        Scribe_Values.Look(ref supportIndex, "supportIndex", -1);
    }

    private Thing SelectPerformThing()
    {
        Thing result = CurrentlyUsableForBills() ? this : null;

        return result;
    }

    private bool IsGoodAndSafeTimeForPerformance()
    {
        return GenLocalDate.HourOfDay(Map) >= 13 && GenLocalDate.HourOfDay(Map) <= 21 &&
               Map.dangerWatcher.DangerRating == StoryDanger.None && Map.mapPawns.FreeColonistsSpawnedCount >= 4;
    }

    private IEnumerable<Pawn> ColonistsAvailableToPerform()
    {
        IEnumerable<Pawn> result;
        if (Map?.mapPawns == null)
        {
            result = new List<Pawn>();
        }
        else
        {
            result = from colonist in Map.mapPawns.FreeColonistsSpawned
                where colonist.workSettings.WorkIsActive(WorkTypeDefOfRazzleDazzle.Performance) &&
                      !colonist.Downed && !colonist.Dead && colonist.Awake() &&
                      colonist.health.capacities.CapableOf(PawnCapacityDefOf.Moving)
                select colonist;
        }

        return result;
    }

    public bool IsPerformanceHappeningElsewhere()
    {
        foreach (var p in Map.mapPawns.FreeColonistsSpawned)
        {
            if (p.GetLord() != null && (p.GetLord().LordJob is LordJob_Joinable_MarriageCeremony ||
                                        p.GetLord().LordJob is LordJob_Joinable_Party ||
                                        p.GetLord().LordJob is LordJob_Performance))
            {
                return true;
            }
        }

        return false;
    }

    protected virtual bool IsForbidden()
    {
        var comp = GetComp<CompForbiddable>();
        return comp is { Forbidden: true };
    }

    protected virtual bool CanDoPerformanceNow()
    {
        return !IsForbidden() && canPerformSwitch && rehearsing &&
               ColonistsAvailableToPerform().Count() >= VenueDef.performersNeeded &&
               ticksSinceLastPerformance >= VenueDef.ticksBetweenPerformances && IsGoodAndSafeTimeForPerformance();
    }

    protected virtual bool TryToStartPerformance()
    {
        ticksSinceLastPerformance = 0;
        var thing = SelectPerformThing();
        return Lead != null && thing != null && (Support != null || VenueDef.performersNeeded <= 1) &&
               !Lead.InMentalState && Support is not { InMentalState: true } && Lead.mindState.duty == null &&
               Support?.mindState.duty == null;
    }

    public void ClearPerformers()
    {
        lead = null;
        leadIndex = -1;
        support = null;
        supportIndex = -1;
    }

    public override void TickRare()
    {
        base.TickRare();
        if (ticksSinceLastPerformance < VenueDef.ticksBetweenPerformances)
        {
            ticksSinceLastPerformance++;
            return;
        }

        if (!CanDoPerformanceNow())
        {
            return;
        }

        if (rehearsing && rehearsedFraction < 1f)
        {
            if (Lead != null && (Lead.Dead || Lead.Faction != Faction.OfPlayer))
            {
                Messages.Message("RAZ_MessageRehearsalsDelayed".Translate(Lead.Named("PAWN")),
                    MessageTypeDefOf.NegativeEvent);
                lead = null;
                leadIndex = -1;
                rehearsedFraction /= 3f;
            }

            if (VenueDef.performersNeeded <= 1 || Support == null ||
                !Support.Dead && Support.Faction == Faction.OfPlayer)
            {
                return;
            }

            Messages.Message("RAZ_MessageRehearsalsDelayed".Translate(Support.Named("PAWN")),
                MessageTypeDefOf.NegativeEvent);
            support = null;
            supportIndex = -1;
            rehearsedFraction /= 3f;
        }
        else if (Rand.Value <= 0.1f)
        {
            TryToStartPerformance();
        }
    }
}