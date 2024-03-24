using System.Linq;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RazzleDazzle;

public class LordJob_Performance : LordJob_VoluntarilyJoinable
{
    protected Building_Performance venue;

    public LordJob_Performance()
    {
    }

    public LordJob_Performance(Building_Performance venue)
    {
        this.venue = venue;
    }

    public Building_Performance Venue => venue;

    public Pawn Lead => venue.Lead;

    public Pawn Support => venue.Support;

    public override void ExposeData()
    {
        Scribe_References.Look(ref venue, "venue");
        base.ExposeData();
    }

    public bool IsPerformanceValid()
    {
        return Venue != null && Lead != null && (Venue.VenueDef.performersNeeded <= 1 || Support != null) &&
               Venue.artTitle != "";
    }

    public bool HasPerformanceFinished()
    {
        return lord.ticksInToil > Venue.VenueDef.minTicksInPerformance && Rand.Chance(0.01f);
    }

    public bool TryStartPerformance()
    {
        bool result;
        if (!IsPerformanceValid())
        {
            result = false;
        }
        else
        {
            LordMaker.MakeNewLord(Faction.OfPlayer, this, Venue.Map);
            result = true;
        }

        return result;
    }

    public bool PerformersAreReady()
    {
        bool result;
        if (!IsValidLead(Lead) || !GatheringsUtility.InGatheringArea(Lead.Position, Venue.Position, Venue.Map))
        {
            result = false;
        }
        else
        {
            if (Venue.VenueDef.performersNeeded > 1 && (!IsValidSupport(Support) ||
                                                        !GatheringsUtility.InGatheringArea(Support.Position,
                                                            Venue.Position, Venue.Map)))
            {
                return false;
            }

            result = true;
        }

        return result;
    }

    private bool IsValidLead(Pawn p)
    {
        return p == Lead && !p.Dead && !p.Downed && p.health.capacities.CapableOf(PawnCapacityDefOf.Moving);
    }

    private bool IsValidSupport(Pawn p)
    {
        return p == Support && !p.Dead && !p.Downed && p.health.capacities.CapableOf(PawnCapacityDefOf.Moving);
    }

    private bool IsValidAttendee(Pawn p)
    {
        return p is { Spawned: true } && GatheringsUtility.ShouldGuestKeepAttendingGathering(p) &&
               !p.Faction.HostileTo(Faction.OfPlayer);
    }

    private bool ShouldPlayBeCalledOff()
    {
        return Lead.DestroyedOrNull() || Lead.Dead || Venue.Position.GetDangerFor(Lead, venue.Map) != Danger.None ||
               Venue.VenueDef.performersNeeded > 1 && (Support.DestroyedOrNull() || Support.Dead ||
                                                       Venue.Position.GetDangerFor(Support, venue.Map) !=
                                                       Danger.None) || Venue.IsBurning() || Venue.Destroyed ||
               !GatheringsUtility.AcceptableGameConditionsToContinueGathering(Venue.Map);
    }

    protected virtual float GetStatModifier()
    {
        return 1f;
    }

    protected virtual float GetFinalQuality()
    {
        var num = (float)GetQualityModifier(Venue.artQuality);
        var num2 = (float)GetQualityModifier(QualityUtility.GenerateQualityCreatedByPawn(Lead, SkillDefOf.Social));
        var statModifier = GetStatModifier();
        if (Venue.VenueDef.performersNeeded > 1)
        {
            num2 = (0.6f * num2) + (0.4f *
                                    GetQualityModifier(
                                        QualityUtility.GenerateQualityCreatedByPawn(Support, SkillDefOf.Social)));
        }

        num2 *= statModifier;
        return Rand.Range(num2, num2 + num);
    }

    protected int GetQualityModifier(QualityCategory qc)
    {
        var result = 0;
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

    public virtual string GetMessageForQualityLevel(float fQ)
    {
        return "";
    }

    public virtual ThoughtDef GetThoughtForQualityLevel(float fQ)
    {
        return ThoughtDefOfRazzleDazzle.AttendedConcert;
    }

    protected virtual void UpdateBroadcastTower(float qualityScore)
    {
        var list = Venue.Map.listerThings.ThingsOfDef(ThingDefOf_RazzleDazzle.BroadcastTowerDef);
        list = (from t in list
            where !t.IsForbidden(Faction.OfPlayer)
            select t).ToList();
        if (!list.NullOrEmpty())
        {
            ((Building_BroadcastTower)list.FirstOrDefault())!.qualityValue +=
                Venue.VenueDef.entertainmentWeight * (qualityScore - 5f);
        }
    }

    public virtual void GiveWatchedPlayThoughts()
    {
        var finalQuality = GetFinalQuality();
        if (finalQuality >= 14f)
        {
            if (Venue.VenueDef.performersNeeded > 1)
            {
                TaleRecorder.RecordTale(Venue.VenueDef.taleDef, Lead, Support);
            }
            else
            {
                TaleRecorder.RecordTale(Venue.VenueDef.taleDef, Lead);
            }
        }

        var messageForQualityLevel = GetMessageForQualityLevel(finalQuality);
        var thoughtForQualityLevel = GetThoughtForQualityLevel(finalQuality);
        foreach (var pawn in lord.ownedPawns)
        {
            if (pawn == Lead || pawn == Support)
            {
                if (Venue.GetRoom(RegionType.Set_Passable).GetStat(RoomStatDefOf.Impressiveness) >= 160f)
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfRazzleDazzle
                        .PerformedInExtremelyImpressiveSpace);
                }
                else if (Venue.GetRoom(RegionType.Set_Passable).GetStat(RoomStatDefOf.Impressiveness) >= 115f)
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfRazzleDazzle
                        .PerformedInVeryImpressiveSpace);
                }
                else if (Venue.GetRoom(RegionType.Set_Passable).GetStat(RoomStatDefOf.Impressiveness) >= 80f)
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfRazzleDazzle
                        .PerformedInImpressiveSpace);
                }
                else
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOfRazzleDazzle.Performed);
                }
            }
            else
            {
                pawn.needs.mood.thoughts.memories.TryGainMemory(thoughtForQualityLevel);
            }
        }

        UpdateBroadcastTower(finalQuality);
        Messages.Message(messageForQualityLevel, Venue, MessageTypeDefOf.NeutralEvent);
    }

    public override float VoluntaryJoinPriorityFor(Pawn p)
    {
        float result;
        if (IsValidLead(p))
        {
            result = 100f;
        }
        else if (IsValidSupport(p))
        {
            result = 100f;
        }
        else if (IsValidAttendee(p))
        {
            result = 20f;
        }
        else
        {
            result = 0f;
        }

        return result;
    }

    protected virtual LordToil GetPerformanceLordToil()
    {
        return new LordToil_End();
    }

    public override StateGraph CreateGraph()
    {
        var stateGraph = new StateGraph();
        LordToil lordToil = new LordToil_PrePerformance(Venue);
        LordToil lordToil2 = new LordToil_SettleForPerformance(Venue);
        var performanceLordToil = GetPerformanceLordToil();
        LordToil lordToil3 = new LordToil_EndPerformance(Venue);
        stateGraph.AddToil(lordToil);
        stateGraph.AddToil(lordToil2);
        stateGraph.AddToil(performanceLordToil);
        stateGraph.AddToil(lordToil3);
        var transition = new Transition(lordToil, lordToil2);
        var transition2 = new Transition(lordToil, lordToil3);
        var transition3 = new Transition(lordToil, lordToil3);
        transition.AddPreAction(new TransitionAction_Message("RAZ_PerformanceStarting".Translate(),
            MessageTypeDefOf.PositiveEvent));
        transition.AddPreAction(new TransitionAction_Custom(delegate() { Venue.ticksIntoThisPerformance = 0; }));
        transition.AddTrigger(new Trigger_TickCondition(() => lord.ticksInToil >= 3000 && PerformersAreReady()));
        transition2.AddTrigger(new Trigger_TickCondition(() => lord.ticksInToil >= 6000));
        transition2.AddPreAction(new TransitionAction_Message("RAZ_PerformanceCancelledLate".Translate()));
        transition3.AddTrigger(new Trigger_TickCondition(ShouldPlayBeCalledOff));
        transition3.AddPreAction(new TransitionAction_Message("RAZ_PerformanceCancelledThreat".Translate()));
        stateGraph.AddTransition(transition);
        stateGraph.AddTransition(transition2);
        stateGraph.AddTransition(transition3);
        var transition4 = new Transition(lordToil2, performanceLordToil);
        var transition5 = new Transition(lordToil2, lordToil3);
        transition4.AddTrigger(new Trigger_TickCondition(() => lord.ticksInToil > 500));
        transition5.AddTrigger(new Trigger_TickCondition(ShouldPlayBeCalledOff));
        stateGraph.AddTransition(transition4);
        stateGraph.AddTransition(transition5);
        var transition6 = new Transition(performanceLordToil, lordToil3);
        transition6.AddTrigger(
            new Trigger_TickCondition(() => HasPerformanceFinished() || lord.ticksInToil > 16000));
        transition6.AddPreAction(new TransitionAction_EndAllJobs());
        transition6.AddPreAction(new TransitionAction_Custom(GiveWatchedPlayThoughts));
        var transition7 = new Transition(performanceLordToil, lordToil3);
        transition7.AddTrigger(new Trigger_TickCondition(ShouldPlayBeCalledOff));
        transition7.AddPreAction(new TransitionAction_Message("RAZ_PerformanceCancelledThreat".Translate()));
        stateGraph.AddTransition(transition6);
        stateGraph.AddTransition(transition7);
        return stateGraph;
    }
}