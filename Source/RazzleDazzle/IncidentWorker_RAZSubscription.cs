using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RazzleDazzle
{
	// Token: 0x0200000A RID: 10
	public class IncidentWorker_RAZSubscription : IncidentWorker
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00003310 File Offset: 0x00001510
		public virtual float AdjustedChance
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00003318 File Offset: 0x00001518
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			ThingSetMakerParams parms2 = default;
			parms2.techLevel = new TechLevel?(parms.faction.def.techLevel);
			parms2.countRange = new IntRange?(new IntRange(1, 4));
			parms2.totalMarketValueRange = new FloatRange?(new FloatRange(parms.points, parms.points));
			parms2.podContentsType = new PodContentsType?(PodContentsType.Empty);
			List<Thing> list = ThingSetMakerDefOf.VisitorGift.root.Generate(parms2);
			Map map = parms.target as Map;
			for (int i = 0; i < list.Count; i++)
			{
				TradeUtility.SpawnDropPod(DropCellFinder.TradeDropSpot(map), map, list[i]);
			}
			Messages.Message(TranslatorFormattedStringExtensions.Translate("RAZ_MessagePaymentArrived", parms.faction.Name), MessageTypeDefOf.PositiveEvent, true);
			return true;
		}
	}
}
