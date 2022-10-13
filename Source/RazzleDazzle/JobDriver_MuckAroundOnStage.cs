using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace RazzleDazzle;

public class JobDriver_MuckAroundOnStage : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job, 3, 0, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
        yield return new Toil
        {
            defaultCompleteMode = ToilCompleteMode.Delay,
            initAction = delegate { GetActor().jobs.curDriver.ticksLeftThisToil = 1000; },
            tickAction = delegate
            {
                GetActor().skills.GetSkill(SkillDefOf.Social).Learn(0.05f);
                if (ticksLeftThisToil % 100 != 0)
                {
                    return;
                }

                var value = Rand.Value;
                FleckDef fleckDef;
                if (value < 0.4)
                {
                    fleckDef = ThingDefOf_RazzleDazzle.Mote_Comedy;
                }
                else if (value < 0.8)
                {
                    fleckDef = ThingDefOf_RazzleDazzle.Mote_Tragedy;
                }
                else if (value < 0.9)
                {
                    fleckDef = ThingDefOf_RazzleDazzle.Mote_Music;
                }
                else
                {
                    fleckDef = FleckDefOf.Heart;
                }

                FleckMaker.ThrowMetaIcon(GetActor().Position, GetActor().Map, fleckDef);
            }
        };
    }
}