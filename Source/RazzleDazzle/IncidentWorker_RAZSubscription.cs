using RimWorld;
using Verse;

namespace RazzleDazzle
{
    // Token: 0x0200000A RID: 10
    public class IncidentWorker_RAZSubscription : IncidentWorker
    {
        // Token: 0x1700000C RID: 12
        // (get) Token: 0x06000037 RID: 55 RVA: 0x00003310 File Offset: 0x00001510
        public virtual float AdjustedChance => 0f;

        // Token: 0x06000038 RID: 56 RVA: 0x00003318 File Offset: 0x00001518
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            ThingSetMakerParams parms2 = default;
            parms2.techLevel = parms.faction.def.techLevel;
            parms2.countRange = new IntRange(1, 4);
            parms2.totalMarketValueRange = new FloatRange(parms.points, parms.points);
            parms2.podContentsType = PodContentsType.Empty;
            var list = ThingSetMakerDefOf.VisitorGift.root.Generate(parms2);
            var map = parms.target as Map;
            foreach (var thing in list)
            {
                TradeUtility.SpawnDropPod(DropCellFinder.TradeDropSpot(map), map, thing);
            }

            Messages.Message("RAZ_MessagePaymentArrived".Translate(parms.faction.Name), MessageTypeDefOf.PositiveEvent);
            return true;
        }
    }
}