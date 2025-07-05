using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RazzleDazzle;

public class JobDriver_PerformComedy : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
    }

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
                if (lordJob is { punchline: true })
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
                        switch (value)
                        {
                            case < 0.25f:
                                FleckMaker.ThrowMetaIcon(GetActor().Position, GetActor().Map, FleckDefOf.IncapIcon);
                                break;
                            case < 0.5f:
                                FleckMaker.ThrowMetaIcon(GetActor().Position, GetActor().Map, FleckDefOf.Heart);
                                break;
                            default:
                                FleckMaker.ThrowMetaIcon(GetActor().Position, GetActor().Map,
                                    ThingDefOf_RazzleDazzle.Mote_Tragedy);
                                break;
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