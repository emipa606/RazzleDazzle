using Verse;

namespace RazzleDazzle;

[StaticConstructorOnStartup]
public class Building_GrandPiano : Building_Performance
{
    protected override bool TryToStartPerformance()
    {
        bool result;
        if (base.TryToStartPerformance())
        {
            new LordJob_PerformConcert(this).TryStartPerformance();
            result = true;
        }
        else
        {
            result = false;
        }

        return result;
    }
}