using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace RazzleDazzle;

public static class RazzleDazzleUtilities
{
    private static IEnumerable<Map> GetAllMapsContainingFreeSpawnedColonists => from map in Find.Maps
        where map.mapPawns.FreeColonistsSpawnedCount > 0
        select map;

    private static IEnumerable<Caravan> GetAllPlayerCaravans => from car in Find.WorldObjects.Caravans
        where car.Faction == Faction.OfPlayer
        select car;

    private static IEnumerable<Pawn> GetAllColonistsInCaravans => from car in GetAllPlayerCaravans
        from col in car.PawnsListForReading
        select new
        {
            car,
            col
        }
        into x
        where x.col.RaceProps.Humanlike && !x.col.Dead && x.col.Faction == Faction.OfPlayer
        select x.col;

    private static IEnumerable<Pawn> GetAllFreeSpawnedColonistsOnMaps =>
        from map in GetAllMapsContainingFreeSpawnedColonists
        from col in map.mapPawns.FreeColonistsSpawned
        select new
        {
            map,
            col
        }
        into x
        where x.col.RaceProps.Humanlike && !x.col.Dead && x.col.Faction == Faction.OfPlayer
        select x.col;

    public static IEnumerable<Pawn> GetAllFreeColonistsAlive =>
        GetAllFreeSpawnedColonistsOnMaps.Concat(GetAllColonistsInCaravans);

    public static IEnumerable<Pawn> GetAllColonistsLocalTo(Pawn p)
    {
        return from x in GetAllFreeColonistsAlive
            where x.RaceProps.Humanlike && x.Faction == Faction.OfPlayer && x != p &&
                  (x.Map != null && x.Map == p.Map || x.GetCaravan() != null && x.GetCaravan() == p.GetCaravan())
            select x;
    }
}