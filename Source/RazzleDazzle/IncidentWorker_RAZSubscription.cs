using RimWorld;
using Verse;

namespace RazzleDazzle;

public class IncidentWorker_RAZSubscription : IncidentWorker
{
    public virtual float AdjustedChance => 0f;

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