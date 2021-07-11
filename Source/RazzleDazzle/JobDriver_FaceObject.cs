using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RazzleDazzle
{
    // Token: 0x0200000D RID: 13
    public class JobDriver_FaceObject : JobDriver
    {
        // Token: 0x1700000F RID: 15
        // (get) Token: 0x06000040 RID: 64 RVA: 0x0000350A File Offset: 0x0000170A
        private Pawn Actor => GetActor();

        // Token: 0x06000041 RID: 65 RVA: 0x00003512 File Offset: 0x00001712
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        // Token: 0x06000042 RID: 66 RVA: 0x00003515 File Offset: 0x00001715
        protected override IEnumerable<Toil> MakeNewToils()
        {
            var lj = Actor.GetLord().LordJob;
            yield return new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Delay,
                initAction = delegate { ticksLeftThisToil = 500; },
                tickAction = delegate
                {
                    if (lj is not LordJob_PerformComedySet)
                    {
                        return;
                    }

                    if (lj is not LordJob_PerformComedySet {Lead: { }} lordJob_PerformComedySet)
                    {
                        return;
                    }

                    var num = 1f * lordJob_PerformComedySet.Lead.skills.GetSkill(SkillDefOf.Social).Level;
                    if (lordJob_PerformComedySet.punchline && Rand.Value < num * 0.001f)
                    {
                        FleckMaker.ThrowMetaIcon(Actor.Position, Actor.Map,
                            ThingDefOf_RazzleDazzle.Mote_Comedy);
                    }
                }
            };
        }
    }
}