using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RazzleDazzle
{
    // Token: 0x02000010 RID: 16
    public class JobDriver_PerformComedy : JobDriver
    {
        // Token: 0x0600004E RID: 78 RVA: 0x000036DC File Offset: 0x000018DC
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        // Token: 0x0600004F RID: 79 RVA: 0x0000370B File Offset: 0x0000190B
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            var lordJob = GetActor().GetLord().LordJob as LordJob_PerformComedySet;
            yield return new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Delay,
                initAction = delegate { GetActor().jobs.curDriver.ticksLeftThisToil = 1200; },
                tickAction = delegate
                {
                    if (lordJob != null && lordJob.punchline)
                    {
                        if (Rand.Chance(0.015f))
                        {
                            lordJob.punchline = false;
                        }
                    }
                    else
                    {
                        if (ticksLeftThisToil % 300 == 1)
                        {
                            FleckMaker.ThrowMetaIcon(GetActor().Position, GetActor().Map,
                                ThingDefOf_RazzleDazzle.Mote_Comedy);
                        }
                        else if (Rand.Chance(0.01f))
                        {
                            var value = Rand.Value;
                            if (value < 0.25f)
                            {
                                FleckMaker.ThrowMetaIcon(GetActor().Position, GetActor().Map, FleckDefOf.IncapIcon);
                            }
                            else if (value < 0.5f)
                            {
                                FleckMaker.ThrowMetaIcon(GetActor().Position, GetActor().Map, FleckDefOf.Heart);
                            }
                            else
                            {
                                FleckMaker.ThrowMetaIcon(GetActor().Position, GetActor().Map,
                                    ThingDefOf_RazzleDazzle.Mote_Tragedy);
                            }
                        }

                        if (Rand.Chance(0.002f))
                        {
                            if (lordJob != null)
                            {
                                lordJob.punchline = true;
                            }
                        }
                    }

                    GetActor().skills.GetSkill(SkillDefOf.Social).Learn(1f);
                }
            };
        }
    }
}