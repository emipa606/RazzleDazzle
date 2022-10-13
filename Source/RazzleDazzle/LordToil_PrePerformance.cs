using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RazzleDazzle;

public class LordToil_PrePerformance : LordToil
{
    private readonly Thing venue;

    public LordToil_PrePerformance(Thing venue)
    {
        this.venue = venue;
    }

    public override void UpdateAllDuties()
    {
        if (lord == null)
        {
            return;
        }

        foreach (var pawn in lord.ownedPawns)
        {
            pawn.mindState.duty = new PawnDuty(DutyDefOf.Party, venue);
        }
    }

    public override ThinkTreeDutyHook VoluntaryJoinDutyHookFor(Pawn p)
    {
        return DutyDefOf.Party.hook;
    }
}