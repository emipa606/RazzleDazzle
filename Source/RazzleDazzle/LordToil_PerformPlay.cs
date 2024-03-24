using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RazzleDazzle;

public class LordToil_PerformPlay(Pawn lead, Pawn support, Thing stage) : LordToil
{
    public readonly Pawn lead = lead;

    public readonly Thing stage = stage;

    public readonly Pawn support = support;

    public bool HasPlayFinished()
    {
        return false;
    }

    public override void UpdateAllDuties()
    {
        if (lord == null)
        {
            return;
        }

        foreach (var pawn in lord.ownedPawns)
        {
            if (pawn == lead || pawn == support)
            {
                pawn.mindState.duty = new PawnDuty(DutyDefOfRazzleDazzle.PerformPlay, stage);
            }
            else
            {
                pawn.mindState.duty = new PawnDuty(DutyDefOfRazzleDazzle.WatchPlayQuietly, lead);
            }
        }
    }

    public override ThinkTreeDutyHook VoluntaryJoinDutyHookFor(Pawn p)
    {
        ThinkTreeDutyHook hook;
        if (p == lead || p == support)
        {
            hook = DutyDefOfRazzleDazzle.PerformPlay.hook;
        }
        else
        {
            hook = DutyDefOfRazzleDazzle.WatchPlayQuietly.hook;
        }

        return hook;
    }
}