﻿using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace RazzleDazzle
{
    // Token: 0x02000030 RID: 48
    public class WorkGiver_PreparePerformance : WorkGiver_Scanner
    {
        // Token: 0x04000067 RID: 103
        private static readonly List<Thing> correctArt = new List<Thing>();

        // Token: 0x04000068 RID: 104
        private static readonly List<Thing> newCorrectArt = new List<Thing>();

        // Token: 0x060000EE RID: 238 RVA: 0x000061A4 File Offset: 0x000043A4
        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            bool result;
            if (!(t is Building_Performance))
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

        // Token: 0x060000EF RID: 239 RVA: 0x0000622C File Offset: 0x0000442C
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

        // Token: 0x060000F0 RID: 240 RVA: 0x0000627E File Offset: 0x0000447E
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return from t in pawn.Map.listerThings.AllThings
                where t is Building_Performance
                select t;
        }

        // Token: 0x060000F1 RID: 241 RVA: 0x000062B4 File Offset: 0x000044B4
        private Job AssignRehearsalDirectJob(Pawn pawn, Building_Performance giver)
        {
            return new Job(JobDefOfRazzleDazzle.JobDef_DoStartRehearsalDirectly, giver);
        }

        // Token: 0x060000F2 RID: 242 RVA: 0x000062C8 File Offset: 0x000044C8
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

        // Token: 0x060000F3 RID: 243 RVA: 0x00006320 File Offset: 0x00004520
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
                        var num = (float) (t1.Position - pawn.Position).LengthHorizontalSquared;
                        var value = (float) (t2.Position - pawn.Position).LengthHorizontalSquared;
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
}