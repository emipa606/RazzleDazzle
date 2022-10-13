using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace RazzleDazzle;

public class WorkGiver_PreparePerformance : WorkGiver_Scanner
{
    private static readonly List<Thing> correctArt = new List<Thing>();

    private static readonly List<Thing> newCorrectArt = new List<Thing>();

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        bool result;
        if (t is not Building_Performance)
        {
            result = false;
        }
        else
        {
            result = t is Building_Performance building_Performance &&
                     building_Performance.CurrentlyUsableForBills() && pawn.CanReserve(t, 1, 0) &&
                     !building_Performance.IsBurning() && !building_Performance.IsForbidden(pawn) &&
                     building_Performance.canPerformSwitch && !building_Performance.rehearsing &&
                     pawn.CanReach(t.InteractionCell, PathEndMode.OnCell, Danger.Some) &&
                     JobOnThing(pawn, t) != null;
        }

        return result;
    }

    public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
    {
        var building_Performance = thing as Building_Performance;
        Job result;
        if (building_Performance?.VenueDef.artDef == null)
        {
            result = !pawn.workSettings.WorkIsActive(def.workType)
                ? null
                : AssignRehearsalDirectJob(pawn, building_Performance);
        }
        else
        {
            result = ChooseArtAndCarryItJob(pawn, building_Performance);
        }

        return result;
    }

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        return from t in pawn.Map.listerThings.AllThings
            where t is Building_Performance
            select t;
    }

    private Job AssignRehearsalDirectJob(Pawn pawn, Building_Performance giver)
    {
        return new Job(JobDefOfRazzleDazzle.JobDef_DoStartRehearsalDirectly, giver);
    }

    private Job ChooseArtAndCarryItJob(Pawn pawn, Building_Performance giver)
    {
        correctArt.Clear();
        Job result;
        if (!TryFindBestArt(giver, pawn, out var t))
        {
            result = null;
        }
        else
        {
            result = new Job(JobDefOfRazzleDazzle.JobDef_DoRehearsePlayBill, giver, t)
            {
                targetB = t,
                count = 1,
                haulMode = HaulMode.ToCellNonStorage
            };
        }

        return result;
    }

    private static bool TryFindBestArt(Building_Performance venue, Pawn pawn, out Thing art)
    {
        art = null;
        var position = venue.Position;
        var validRegionAt = venue.Map.regionGrid.GetValidRegionAt(position);
        bool result;
        if (validRegionAt == null)
        {
            result = false;
        }
        else
        {
            var found = false;

            bool validArt(Thing t)
            {
                return t.Spawned && !t.IsForbidden(pawn) &&
                       (t.Position - pawn.Position).LengthHorizontalSquared < 999 &&
                       t.def == venue.VenueDef.artDef && pawn.CanReserve(t);
            }

            newCorrectArt.Clear();

            bool regionProcessor(Region r)
            {
                var list = r.ListerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.HaulableEver));
                foreach (var thing in list)
                {
                    if (validArt(thing) && !thing.def.IsMedicine)
                    {
                        newCorrectArt.Add(thing);
                    }
                }

                if (newCorrectArt.Count <= 0)
                {
                    return false;
                }

                int comparison(Thing t1, Thing t2)
                {
                    var num = (float)(t1.Position - pawn.Position).LengthHorizontalSquared;
                    var value = (float)(t2.Position - pawn.Position).LengthHorizontalSquared;
                    return num.CompareTo(value);
                }

                newCorrectArt.Sort(comparison);
                correctArt.AddRange(newCorrectArt);
                newCorrectArt.Clear();
                if (correctArt.Count <= 0)
                {
                    return false;
                }

                found = true;
                return true;
            }

            var traverseParams = TraverseParms.For(pawn, Danger.Some);

            bool entryCondition(Region from, Region r)
            {
                return r.Allows(traverseParams, false);
            }

            RegionTraverser.BreadthFirstTraverse(validRegionAt, entryCondition, regionProcessor, 999);
            if (found)
            {
                art = correctArt[0];
                result = true;
            }
            else
            {
                result = false;
            }
        }

        return result;
    }
}