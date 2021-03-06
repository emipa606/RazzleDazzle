﻿using System.Collections.Generic;
using RimWorld;
using Verse.AI;

namespace RazzleDazzle
{
    // Token: 0x0200000F RID: 15
    public class JobDriver_NoodleOnPiano : JobDriver
    {
        // Token: 0x06000049 RID: 73 RVA: 0x00003624 File Offset: 0x00001824
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        // Token: 0x0600004A RID: 74 RVA: 0x00003653 File Offset: 0x00001853
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            yield return new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Delay,
                initAction = delegate { GetActor().jobs.curDriver.ticksLeftThisToil = 500; },
                tickAction = delegate
                {
                    GetActor().skills.GetSkill(SkillDefOf.Social).Learn(0.05f);
                    if (ticksLeftThisToil % 124 == 0)
                    {
                        FleckMaker.ThrowMetaIcon(GetActor().Position, GetActor().Map,
                            ThingDefOf_RazzleDazzle.Mote_Music);
                    }
                }
            };
        }
    }
}