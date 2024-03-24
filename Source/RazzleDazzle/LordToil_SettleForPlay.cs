using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RazzleDazzle;

public class LordToil_SettleForPlay(Pawn lead, Pawn support, Thing performThing) : LordToil
{
    public readonly Pawn lead = lead;

    public readonly Thing performThing = performThing;

    public readonly Pawn support = support;

    public CellRect spectateRect;

    private CellRect CalculateSpectateRect()
    {
        return CellRect.CenteredOn(performThing.Position, 8);
    }


    public override void UpdateAllDuties()
    {
        foreach (var pawn in lord.ownedPawns)
        {
            if (pawn == lead || pawn == support)
            {
                pawn.mindState.duty.focus = performThing;
                pawn.mindState.duty = new PawnDuty(DutyDefOfRazzleDazzle.GoToStageAndWait, performThing);
            }
            else
            {
                var duty = new PawnDuty(DutyDefOfRazzleDazzle.FindSeatWithStageViewAndChat, performThing);
                pawn.mindState.duty = duty;
            }
        }
    }

    public override ThinkTreeDutyHook VoluntaryJoinDutyHookFor(Pawn p)
    {
        ThinkTreeDutyHook hook;
        if (p == lead || p == support)
        {
            hook = DutyDefOfRazzleDazzle.PerformConcert.hook;
        }
        else
        {
            hook = DutyDefOf.Spectate.hook;
        }

        return hook;
    }
}