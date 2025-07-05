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
        if (t is not Building_Performance buildingPerformance)
        {
            return false;
        }

        if (pawn.Dead || pawn.Downed || pawn.IsBurning())
        {
            return false;
        }

        if (!buildingPerformance.rehearsing)
        {
            return false;
        }

        if (!buildingPerformance.canPerformSwitch || buildingPerformance.IsForbidden(pawn))
        {
            return false;
        }

        if (buildingPerformance.rehearsedFraction > 1f)
        {
            return false;
        }

        if (buildingPerformance.Lead == null || buildingPerformance.Lead == pawn)
        {
            return pawn.CanReserveAndReach(buildingPerformance, PathEndMode, Danger.Some,
                buildingPerformance.VenueDef.performersNeeded);
        }

        if (buildingPerformance.VenueDef.performersNeeded <= 1)
        {
            return false;
        }

        if (buildingPerformance.Support != null && buildingPerformance.Support != pawn)
        {
            return false;
        }

        return pawn.CanReserveAndReach(buildingPerformance, PathEndMode, Danger.Some,
            buildingPerformance.VenueDef.performersNeeded);
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        return new Job(JobDefOfRazzleDazzle.JobDef_Rehearse, t);
    }
}