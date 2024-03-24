using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RazzleDazzle;

public class LordToil_ComedySet(Pawn performer, Thing performThing) : LordToil
{
    public readonly Pawn performer = performer;

    public readonly Thing performThing = performThing;

    public CellRect spectateRect;

    private CellRect CalculateSpectateRect()
    {
        return CellRect.CenteredOn(performThing.Position, 8);
    }

    public override void UpdateAllDuties()
    {
        foreach (var pawn in lord.ownedPawns)
        {
            if (pawn == performer)
            {
                pawn.mindState.duty.focus = performThing;
                pawn.mindState.duty = new PawnDuty(DutyDefOfRazzleDazzle.PerformComedy, performThing);
            }
            else
            {
                pawn.mindState.duty = new PawnDuty(DutyDefOfRazzleDazzle.WatchPlayQuietly, performer);
            }
        }
    }

    public override ThinkTreeDutyHook VoluntaryJoinDutyHookFor(Pawn p)
    {
        var hook = p == performer ? DutyDefOfRazzleDazzle.PerformConcert.hook : DutyDefOf.Spectate.hook;

        return hook;
    }
}