using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RazzleDazzle;

public class PerformanceVenueDef : ThingDef
{
    public ThingDef artDef;

    public float entertainmentWeight = 1f;

    public int minTicksInPerformance = 2500;

    public int numTicksToRehearse = 1;

    public int performersNeeded = 1;

    public List<FleckDef> rehearsalMotes;

    public TaleDef taleDef;

    public int ticksBetweenPerformances = 600;
}