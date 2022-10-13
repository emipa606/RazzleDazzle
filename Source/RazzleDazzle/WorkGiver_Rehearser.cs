using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace RazzleDazzle;

public class WorkGiver_Rehearser : WorkGiver_Scanner
{
    public override PathEndMode PathEndMode => PathEndMode.ClosestTouch;

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        return from t in pawn.Map.listerThings.AllThings
            where t is Building_Performance
            select t;
    }

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        bool result;
        if (t is not Building_Performance buildingPerformance)
        {
            result = false;
        }
        else
        {
            if (pawn.Dead || pawn.Downed || pawn.IsBurning())
            {
                result = false;
            }
            else if (!buildingPerformance.rehearsing)
            {
                result = false;
            }
            else if (!buildingPerformance.canPerformSwitch || buildingPerformance.IsForbidden(pawn))
            {
                result = false;
            }
            else if (buildingPerformance.rehearsedFraction > 1f)
            {
                result = false;
            }
            else
            {
                if (buildingPerformance.Lead != null && buildingPerformance.Lead != pawn)
                {
                    if (buildingPerformance.VenueDef.performersNeeded <= 1)
                    {
                        return false;
                    }

                    if (buildingPerformance.Support != null && buildingPerformance.Support != pawn)
                    {
                        return false;
                    }
                }

                result = pawn.CanReserveAndReach(buildingPerformance, PathEndMode, Danger.Some,
                    buildingPerformance.VenueDef.performersNeeded);
            }
        }

        return result;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        return new Job(JobDefOfRazzleDazzle.JobDef_Rehearse, t);
    }
}