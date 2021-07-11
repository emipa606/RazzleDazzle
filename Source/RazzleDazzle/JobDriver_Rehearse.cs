using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace RazzleDazzle
{
    // Token: 0x02000014 RID: 20
    public class JobDriver_Rehearse : JobDriver
    {
        // Token: 0x17000011 RID: 17
        // (get) Token: 0x06000060 RID: 96 RVA: 0x000033F5 File Offset: 0x000015F5
        private Building_Performance Venue => (Building_Performance) job.targetA.Thing;

        // Token: 0x06000061 RID: 97 RVA: 0x00003860 File Offset: 0x00001A60
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            var stackCount = Venue.VenueDef.performersNeeded > 1 ? 0 : -1;
            return pawn.Reserve(Venue, job, Venue.VenueDef.performersNeeded, stackCount, null, errorOnFailed);
        }

        // Token: 0x06000062 RID: 98 RVA: 0x000038B6 File Offset: 0x00001AB6
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return new Toil
            {
                initAction = delegate
                {
                    if (Venue.Director.stage == null)
                    {
                        Venue.Director.stage = Venue;
                    }

                    if ((Venue.Lead == null || Venue.Lead.Dead) && Venue.Support != GetActor())
                    {
                        Venue.Lead = GetActor();
                        return;
                    }

                    if (Venue.VenueDef.performersNeeded > 1 && (Venue.Support == null || Venue.Support.Dead) &&
                        Venue.Lead != GetActor())
                    {
                        Venue.Support = GetActor();
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            if (Venue.InteractionCell.IsValid)
            {
                yield return Toils_Goto.GotoCell(Venue.InteractionCell, PathEndMode.OnCell);
            }
            else
            {
                yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
            }

            yield return new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Delay,
                initAction = delegate { GetActor().jobs.curDriver.ticksLeftThisToil = 1001; },
                tickAction = delegate
                {
                    if (ticksLeftThisToil % 100 == 0)
                    {
                        var moteDef = Venue.VenueDef.rehearsalMotes.RandomElement();
                        FleckMaker.ThrowMetaIcon(GetActor().Position, GetActor().Map, moteDef);
                        Venue.rehearsedFraction += 100f / Venue.VenueDef.numTicksToRehearse;
                        GetActor().skills.GetSkill(SkillDefOf.Social).Learn(1f);
                    }

                    if (!(Venue.rehearsedFraction >= 1f))
                    {
                        return;
                    }

                    Venue.rehearsedFraction = 1.00000012f;
                    ticksLeftThisToil = 0;
                }
            };
        }
    }
}