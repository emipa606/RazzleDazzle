using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RazzleDazzle;

public class PerformanceVenueDef : ThingDef
{
    public readonly float entertainmentWeight = 1f;

    public readonly int minTicksInPerformance = 2500;

    public readonly int numTicksToRehearse = 1;

    public readonly int performersNeeded = 1;

    public readonly int ticksBetweenPerformances = 600;
    public ThingDef artDef;

    public List<FleckDef> rehearsalMotes;

    public TaleDef taleDef;
}