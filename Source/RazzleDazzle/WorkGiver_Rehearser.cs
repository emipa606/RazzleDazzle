using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace RazzleDazzle
{
    // Token: 0x02000031 RID: 49
    public class WorkGiver_Rehearser : WorkGiver_Scanner
    {
        // Token: 0x17000025 RID: 37
        // (get) Token: 0x060000F6 RID: 246 RVA: 0x00006423 File Offset: 0x00004623
        public override PathEndMode PathEndMode => PathEndMode.ClosestTouch;

        // Token: 0x060000F7 RID: 247 RVA: 0x00006426 File Offset: 0x00004626
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return from t in pawn.Map.listerThings.AllThings
                where t is Building_Performance
                select t;
        }

        // Token: 0x060000F8 RID: 248 RVA: 0x0000645C File Offset: 0x0000465C
        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            bool result;
            if (!(t is Building_Performance))
            {
                result = false;
            }
            else
            {
                var building_Performance = (Building_Performance) t;
                if (pawn.Dead || pawn.Downed || pawn.IsBurning())
                {
                    result = false;
                }
                else if (!building_Performance.rehearsing)
                {
                    result = false;
                }
                else if (!building_Performance.canPerformSwitch || t.IsForbidden(pawn))
                {
                    result = false;
                }
                else if (building_Performance.rehearsedFraction > 1f)
                {
                    result = false;
                }
                else
                {
                    if (building_Performance.Lead != null && building_Performance.Lead != pawn)
                    {
                        if (building_Performance.VenueDef.performersNeeded <= 1)
                        {
                            return false;
                        }

                        if (building_Performance.Support != null && building_Performance.Support != pawn)
                        {
                            return false;
                        }
                    }

                    result = pawn.CanReserveAndReach(building_Performance, PathEndMode, Danger.Some,
                        building_Performance.VenueDef.performersNeeded);
                }
            }

            return result;
        }

        // Token: 0x060000F9 RID: 249 RVA: 0x0000652E File Offset: 0x0000472E
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return new Job(JobDefOfRazzleDazzle.JobDef_Rehearse, t);
        }
    }
}