using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RazzleDazzle;

public class JobDriver_FaceObject : JobDriver
{
    private Pawn Actor => GetActor();

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return true;
    }

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

                if (lj is not LordJob_PerformComedySet { Lead: { } } lordJob_PerformComedySet)
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