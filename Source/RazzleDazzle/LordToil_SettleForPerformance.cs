using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RazzleDazzle;

public class LordToil_SettleForPerformance(Building_Performance venue) : LordToil
{
    private Building_Performance Venue { get; } = venue;

    private Pawn Lead => Venue.Lead;

    private Pawn Support => Venue.Support;

    public override void UpdateAllDuties()
    {
        foreach (var pawn in lord.ownedPawns)
        {
            if (pawn == Lead || pawn == Support)
            {
                pawn.mindState.duty.focus = Venue;
                pawn.mindState.duty = new PawnDuty(DutyDefOfRazzleDazzle.GoToStageAndWait, Venue);
            }
            else
            {
                var duty = new PawnDuty(DutyDefOfRazzleDazzle.FindSeatWithStageViewAndChat, Venue);
                pawn.mindState.duty = duty;
            }
        }
    }

    public override ThinkTreeDutyHook VoluntaryJoinDutyHookFor(Pawn p)
    {
        ThinkTreeDutyHook hook;
        if (p == Lead || p == Support)
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