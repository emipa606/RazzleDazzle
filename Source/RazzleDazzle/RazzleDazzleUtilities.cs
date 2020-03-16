using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace RazzleDazzle
{
	// Token: 0x02000029 RID: 41
	public static class RazzleDazzleUtilities
	{
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x000052AF File Offset: 0x000034AF
		public static IEnumerable<Map> GetAllMapsContainingFreeSpawnedColonists
		{
			get
			{
				return from map in Find.Maps
				where map.mapPawns.FreeColonistsSpawnedCount > 0
				select map;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000D1 RID: 209 RVA: 0x000052DA File Offset: 0x000034DA
		public static IEnumerable<Caravan> GetAllPlayerCaravans
		{
			get
			{
				return from car in Find.WorldObjects.Caravans
				where car.Faction == Faction.OfPlayer
				select car;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000D2 RID: 210 RVA: 0x0000530C File Offset: 0x0000350C
		public static IEnumerable<Pawn> GetAllColonistsInCaravans
		{
			get
			{
				return from car in RazzleDazzleUtilities.GetAllPlayerCaravans
				from col in car.PawnsListForReading
				select new
				{
					car,
					col
				} into x
				where x.col.RaceProps.Humanlike && !x.col.Dead && x.col.Faction == Faction.OfPlayer
				select x.col;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000D3 RID: 211 RVA: 0x000053AC File Offset: 0x000035AC
		public static IEnumerable<Pawn> GetAllFreeSpawnedColonistsOnMaps
		{
			get
			{
				return from map in RazzleDazzleUtilities.GetAllMapsContainingFreeSpawnedColonists
				from col in map.mapPawns.FreeColonistsSpawned
				select new
				{
					map,
					col
				} into x
				where x.col.RaceProps.Humanlike && !x.col.Dead && x.col.Faction == Faction.OfPlayer
				select x.col;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000D4 RID: 212 RVA: 0x00005449 File Offset: 0x00003649
		public static IEnumerable<Pawn> GetAllFreeColonistsAlive
		{
			get
			{
				return RazzleDazzleUtilities.GetAllFreeSpawnedColonistsOnMaps.Concat(RazzleDazzleUtilities.GetAllColonistsInCaravans);
			}
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x0000545C File Offset: 0x0000365C
		public static IEnumerable<Pawn> GetAllColonistsLocalTo(Pawn p)
		{
			return from x in RazzleDazzleUtilities.GetAllFreeColonistsAlive
			where x.RaceProps.Humanlike && x.Faction == Faction.OfPlayer && x != p && ((x.Map != null && x.Map == p.Map) || (x.GetCaravan() != null && x.GetCaravan() == p.GetCaravan()))
			select x;
		}
	}
}
