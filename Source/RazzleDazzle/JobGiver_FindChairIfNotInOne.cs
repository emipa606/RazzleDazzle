using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace RazzleDazzle;

public class JobGiver_FindChairIfNotInOne : ThinkNode_JobGiver
{
    private static readonly int numCellsInRadius = GenRadial.NumCellsInRadius(7f);

    private static readonly List<IntVec3> rpo = GenRadial.RadialPattern.Take(numCellsInRadius).ToList();

    public bool CellHasChair(IntVec3 pos, Map m)
    {
        var edifice = pos.GetEdifice(m);
        return edifice != null && edifice.def.category == ThingCategory.Building && edifice.def.building.isSittable;
    }

    private static bool TryToFindChairNear(IntVec3 center, Pawn sitter, out Thing chair)
    {
        foreach (var intVec3 in rpo)
        {
            var c = center + intVec3;
            var edifice = c.GetEdifice(sitter.Map);
            if (edifice == null || edifice.def.category != ThingCategory.Building ||
                !edifice.def.building.isSittable || !sitter.CanReserve(edifice) || edifice.IsForbidden(sitter) ||
                !GenSight.LineOfSight(center, edifice.Position, sitter.Map, true) ||
                c.GetFirstPawn(sitter.Map) != null)
            {
                continue;
            }

            chair = edifice;
            return true;
        }

        chair = null;
        return false;
    }

    private static bool TryToFindGroundSpotNear(IntVec3 center, Pawn sitter, out IntVec3 result)
    {
        for (var i = 0; i < 30; i++)
        {
            var intVec = center + rpo[i];
            if (!sitter.CanReserveAndReach(intVec, PathEndMode.OnCell, Danger.None) ||
                intVec.GetEdifice(sitter.Map) != null || !GenSight.LineOfSight(center, intVec, sitter.Map, true) ||
                intVec.GetFirstPawn(sitter.Map) != null)
            {
                continue;
            }

            result = intVec;
            return true;
        }

        result = IntVec3.Invalid;
        return false;
    }

    protected override Job TryGiveJob(Pawn pawn)
    {
        Job result;
        if (CellHasChair(pawn.Position, pawn.Map) && pawn.CanSee(pawn.mindState.duty.focus.Thing))
        {
            result = null;
        }
        else if (TryToFindChairNear(pawn.mindState.duty.focus.Cell, pawn, out var t))
        {
            result = new Job(JobDefOf.Goto, t);
        }
        else if (TryToFindGroundSpotNear(pawn.mindState.duty.focus.Cell, pawn, out var c))
        {
            result = new Job(JobDefOf.Goto, c);
        }
        else
        {
            result = null;
        }

        return result;
    }
}