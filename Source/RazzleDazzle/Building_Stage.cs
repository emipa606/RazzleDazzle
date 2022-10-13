using Verse;

namespace RazzleDazzle;

[StaticConstructorOnStartup]
public class Building_Stage : Building_Performance
{
    protected override string BasicInspectString => "RAZ_StageBasicInspectString".Translate();

    public override bool TryToStartPerformance()
    {
        bool result;
        if (base.TryToStartPerformance())
        {
            new LordJob_PerformPlay(this).TryStartPerformance();
            result = true;
        }
        else
        {
            result = false;
        }

        return result;
    }
}