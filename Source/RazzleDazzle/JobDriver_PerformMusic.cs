using System.Collections.Generic;
using RimWorld;
using Verse.AI;

namespace RazzleDazzle;

public class JobDriver_PerformMusic : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
        yield return new Toil
        {
            defaultCompleteMode = ToilCompleteMode.Delay,
            initAction = delegate { GetActor().jobs.curDriver.ticksLeftThisToil = 250; },
            tickAction = delegate
            {
                if (ticksLeftThisToil % 200 == 0)
                {
                    FleckMaker.ThrowMetaIcon(GetActor().Position, GetActor().Map,
                        ThingDefOf_RazzleDazzle.Mote_Music);
                }

                GetActor().skills.GetSkill(SkillDefOf.Social).Learn(1f);
            }
        };
    }
}