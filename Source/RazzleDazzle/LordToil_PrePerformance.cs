using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RazzleDazzle;

public class LordToil_PrePerformance(Thing venue) : LordToil
{
    private readonly DutyDef partyDef = DefDatabase<DutyDef>.GetNamedSilentFail("Party");

    public override void UpdateAllDuties()
    {
        if (lord == null)
        {
            return;
        }

        foreach (var pawn in lord.ownedPawns)
        {
            pawn.mindState.duty = new PawnDuty(partyDef, venue);
        }
    }

    public override ThinkTreeDutyHook VoluntaryJoinDutyHookFor(Pawn p)
    {
        return partyDef.hook;
    }
}