using System.Text;
using Verse;

namespace RazzleDazzle;

[StaticConstructorOnStartup]
public class Building_Microphone : Building_Performance
{
    protected override string BasicInspectString => "RAZ_MicrophoneBasicInpectString".Translate();

    public override bool TryToStartPerformance()
    {
        bool result;
        if (base.TryToStartPerformance())
        {
            new LordJob_PerformComedySet(this).TryStartPerformance();
            result = true;
        }
        else
        {
            result = false;
        }

        return result;
    }

    public override string GetInspectString()
    {
        var stringBuilder = new StringBuilder();
        if (rehearsing)
        {
            if (rehearsedFraction < 1f)
            {
                stringBuilder.AppendLine("RAZ_Rehearsal_Microphone".Translate(Lead.Named("PAWN")));
                stringBuilder.AppendLine("RAZ_Rehearsal_Progress".Translate(rehearsedFraction.ToStringPercent()));
            }
            else
            {
                stringBuilder.AppendLine("RAZ_PerformancePending_Microphone".Translate(Lead.Named("PAWN")));
            }
        }
        else
        {
            stringBuilder.AppendLine(BasicInspectString);
        }

        return stringBuilder.ToString().TrimEndNewlines();
    }
}