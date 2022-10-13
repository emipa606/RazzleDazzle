using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RazzleDazzle;

public class LordToil_Concert : LordToil
{
    public Pawn performer;

    public Thing performThing;

    public CellRect spectateRect;

    public LordToil_Concert(Pawn performer, Thing performThing)
    {
        this.performer = performer;
        this.performThing = performThing;
    }

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
                pawn.mindState.duty = new PawnDuty(DutyDefOfRazzleDazzle.PerformConcert, performThing);
            }
            else
            {
                var duty = new PawnDuty(DutyDefOfRazzleDazzle.WatchPlayQuietly, performer);
                pawn.mindState.duty = duty;
            }
        }
    }

    public override ThinkTreeDutyHook VoluntaryJoinDutyHookFor(Pawn p)
    {
        var hook = p == performer ? DutyDefOfRazzleDazzle.PerformConcert.hook : DutyDefOf.Spectate.hook;

        return hook;
    }
}