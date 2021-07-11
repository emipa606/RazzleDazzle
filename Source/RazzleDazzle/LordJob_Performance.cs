using System.Linq;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RazzleDazzle
{
    // Token: 0x0200001C RID: 28
    public class LordJob_Performance : LordJob_VoluntarilyJoinable
    {
        // Token: 0x0400002B RID: 43
        protected Building_Performance venue;

        // Token: 0x0600007F RID: 127 RVA: 0x00003ECF File Offset: 0x000020CF
        public LordJob_Performance()
        {
        }

        // Token: 0x06000080 RID: 128 RVA: 0x00003ED7 File Offset: 0x000020D7
        public LordJob_Performance(Building_Performance venue)
        {
            this.venue = venue;
        }

        // Token: 0x17000013 RID: 19
        // (get) Token: 0x0600007C RID: 124 RVA: 0x00003EAD File Offset: 0x000020AD
        public Building_Performance Venue => venue;

        // Token: 0x17000014 RID: 20
        // (get) Token: 0x0600007D RID: 125 RVA: 0x00003EB5 File Offset: 0x000020B5
        public Pawn Lead => venue.Lead;

        // Token: 0x17000015 RID: 21
        // (get) Token: 0x0600007E RID: 126 RVA: 0x00003EC2 File Offset: 0x000020C2
        public Pawn Support => venue.Support;

        // Token: 0x06000081 RID: 129 RVA: 0x00003EE6 File Offset: 0x000020E6
        public override void ExposeData()
        {
            Scribe_References.Look(ref venue, "venue");
            base.ExposeData();
        }

        // Token: 0x06000082 RID: 130 RVA: 0x00003F00 File Offset: 0x00002100
        public bool IsPerformanceValid()
        {
            return Venue != null && Lead != null && (Venue.VenueDef.performersNeeded <= 1 || Support != null) &&
                   Venue.artTitle != "";
        }

        // Token: 0x06000083 RID: 131 RVA: 0x00003F52 File Offset: 0x00002152
        public bool HasPerformanceFinished()
        {
            return lord.ticksInToil > Venue.VenueDef.minTicksInPerformance && Rand.Chance(0.01f);
        }

        // Token: 0x06000084 RID: 132 RVA: 0x00003F80 File Offset: 0x00002180
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

        // Token: 0x06000085 RID: 133 RVA: 0x00003FB4 File Offset: 0x000021B4
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

        // Token: 0x06000086 RID: 134 RVA: 0x0000404B File Offset: 0x0000224B
        private bool IsValidLead(Pawn p)
        {
            return p == Lead && !p.Dead && !p.Downed && p.health.capacities.CapableOf(PawnCapacityDefOf.Moving);
        }

        // Token: 0x06000087 RID: 135 RVA: 0x0000407D File Offset: 0x0000227D
        private bool IsValidSupport(Pawn p)
        {
            return p == Support && !p.Dead && !p.Downed && p.health.capacities.CapableOf(PawnCapacityDefOf.Moving);
        }

        // Token: 0x06000088 RID: 136 RVA: 0x000040AF File Offset: 0x000022AF
        private bool IsValidAttendee(Pawn p)
        {
            return p != null && p.Spawned && GatheringsUtility.ShouldGuestKeepAttendingGathering(p) &&
                   !p.Faction.HostileTo(Faction.OfPlayer);
        }

        // Token: 0x06000089 RID: 137 RVA: 0x000040E0 File Offset: 0x000022E0
        private bool ShouldPlayBeCalledOff()
        {
            return Lead.DestroyedOrNull() || Lead.Dead || Venue.Position.GetDangerFor(Lead, venue.Map) != Danger.None ||
                   Venue.VenueDef.performersNeeded > 1 && (Support.DestroyedOrNull() || Support.Dead ||
                                                           Venue.Position.GetDangerFor(Support, venue.Map) !=
                                                           Danger.None) || Venue.IsBurning() || Venue.Destroyed ||
                   !GatheringsUtility.AcceptableGameConditionsToContinueGathering(Venue.Map);
        }

        // Token: 0x0600008A RID: 138 RVA: 0x000041B1 File Offset: 0x000023B1
        protected virtual float GetStatModifier()
        {
            return 1f;
        }

        // Token: 0x0600008B RID: 139 RVA: 0x000041B8 File Offset: 0x000023B8
        protected virtual float GetFinalQuality()
        {
            var num = (float) GetQualityModifier(Venue.artQuality);
            var num2 = (float) GetQualityModifier(QualityUtility.GenerateQualityCreatedByPawn(Lead, SkillDefOf.Social));
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

        // Token: 0x0600008C RID: 140 RVA: 0x00004240 File Offset: 0x00002440
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
            var list = Venue.Map.listerThings.ThingsOfDef(ThingDefOf_RazzleDazzle.BroadcastTowerDef);
            list = (from t in list
                where !t.IsForbidden(Faction.OfPlayer)
                select t).ToList();
            if (!list.NullOrEmpty())
            {
                (list.FirstOrDefault() as Building_BroadcastTower).qualityValue +=
                    Venue.VenueDef.entertainmentWeight * (qualityScore - 5f);
            }
        }

        // Token: 0x06000090 RID: 144 RVA: 0x0000432C File Offset: 0x0000252C
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

        // Token: 0x06000091 RID: 145 RVA: 0x00004554 File Offset: 0x00002754
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

        // Token: 0x06000092 RID: 146 RVA: 0x0000459B File Offset: 0x0000279B
        protected virtual LordToil GetPerformanceLordToil()
        {
            return new LordToil_End();
        }

        // Token: 0x06000093 RID: 147 RVA: 0x000045A4 File Offset: 0x000027A4
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
}