using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace RazzleDazzle;

public class RazzleDazzle_Director
{
    public delegate void DramaticExchange(Pawn p1, Pawn p2, List<Toil> leadToils, List<Toil> supportToils,
        Building_Stage stage);

    public enum StageLocations
    {
        STAGE_R,

        STAGE_L,

        STAGE_C,

        UPSTAGE_R,

        UPSTAGE_L,

        UPSTAGE_C,

        DOWNSTAGE_R,

        DOWNSTAGE_L,

        DOWNSTAGE_C,

        BACKSTAGE
    }

    private const int nScenes = 6;

    public IEnumerable<DramaticExchange> allExchanges;

    private bool leadReady;

    private List<Toil> lToils;

    public Building_Performance stage;

    private List<Toil> sToils;

    private bool supportReady;

    private Rot4 StageForward => stage.Rotation;

    private Rot4 StageLeft
    {
        get
        {
            Rot4 result;
            if (stage == null)
            {
                result = Rot4.South;
            }
            else
            {
                var rotation = stage.Rotation;
                rotation.Rotate(RotationDirection.Counterclockwise);
                result = rotation;
            }

            return result;
        }
    }

    private Rot4 StageRight
    {
        get
        {
            Rot4 result;
            if (stage == null)
            {
                result = Rot4.South;
            }
            else
            {
                var rotation = stage.Rotation;
                rotation.Rotate(RotationDirection.Clockwise);
                result = rotation;
            }

            return result;
        }
    }

    private Rot4 StageRear
    {
        get
        {
            var result = stage?.Rotation.Opposite ?? Rot4.South;

            return result;
        }
    }

    private Pawn Lead => stage.Lead;

    private Pawn Support => stage.Support;

    private Toil FaceDirection(Rot4 dir)
    {
        var t = new Toil
        {
            defaultCompleteMode = ToilCompleteMode.Instant
        };
        t.initAction = delegate { t.GetActor().Rotation = dir; };
        return t;
    }

    public Toil FaceObject(Thing thing)
    {
        var toil = new Toil
        {
            defaultCompleteMode = ToilCompleteMode.Instant,
            initAction = delegate { }
        };
        return toil;
    }

    private void ConstructPlay()
    {
        lToils = [];
        sToils = [];
        lToils.Add(GoToStageLocation(stage, StageLocations.BACKSTAGE));
        sToils.Add(GoToStageLocation(stage, StageLocations.BACKSTAGE));
        lToils.Add(Synchronise());
        sToils.Add(Synchronise());
        for (var i = 0; i < nScenes; i++)
        {
            switch (Rand.RangeInclusive(0, 3))
            {
                case 0:
                    Romance(lToils, sToils, stage);
                    break;
                case 1:
                    Comedy(lToils, sToils, stage);
                    break;
                case 2:
                    Tragedy(lToils, sToils, stage);
                    break;
                case 3:
                    MusicalDance(lToils, sToils, stage);
                    break;
            }
        }
    }

    public List<Toil> RequestPlayToils(Pawn pawn, Building_Performance buildingPerformance)
    {
        if (lToils == null || lToils.Count == 0)
        {
            stage = buildingPerformance;
            ConstructPlay();
        }

        if ((Lead is { Dead: false } || pawn == Support) && (Support == null || Support.Dead))
        {
            _ = Lead;
        }

        List<Toil> result;
        if (pawn == Lead)
        {
            result = lToils;
        }
        else if (pawn == Support)
        {
            result = sToils;
        }
        else
        {
            result = [];
        }

        return result;
    }

    private Toil Synchronise()
    {
        var t = new Toil
        {
            defaultCompleteMode = ToilCompleteMode.Delay
        };
        t.initAction = delegate
        {
            t.GetActor().jobs.curDriver.ticksLeftThisToil = 10000;
            leadReady = supportReady = false;
        };
        t.tickAction = delegate
        {
            t.GetActor().skills.GetSkill(SkillDefOf.Social).Learn(0.5f);
            if (t.GetActor() == Lead)
            {
                leadReady = true;
            }
            else if (t.GetActor() == Support)
            {
                supportReady = true;
            }

            if (!leadReady || !supportReady)
            {
                return;
            }

            Lead.jobs.curDriver.ticksLeftThisToil = 0;
            Support.jobs.curDriver.ticksLeftThisToil = 0;
            leadReady = supportReady = false;
        };
        return t;
    }

    private static Toil Wait(int iTicks)
    {
        var toil = new Toil
        {
            defaultCompleteMode = ToilCompleteMode.Delay
        };
        toil.initAction = delegate { toil.GetActor().jobs.curDriver.ticksLeftThisToil = iTicks; };
        return toil;
    }

    private Toil ThrowDramaticMote(FleckDef fleck, int ticks)
    {
        var toil = new Toil
        {
            defaultCompleteMode = ToilCompleteMode.Delay
        };
        toil.initAction = delegate { toil.GetActor().jobs.curDriver.ticksLeftThisToil = ticks; };
        toil.tickAction = delegate
        {
            stage.ticksIntoThisPerformance++;
            if (toil.GetActor().jobs.curDriver.ticksLeftThisToil % 100 == 0)
            {
                FleckMaker.ThrowMetaIcon(toil.GetActor().Position, toil.GetActor().Map, fleck);
            }

            toil.GetActor().skills.GetSkill(SkillDefOf.Social).Learn(1.5f);
        };
        return toil;
    }

    private Toil GoToStageLocation(Building_Performance buildingPerformance, StageLocations loc)
    {
        var toil = Toils_Goto.GotoCell(GetStageCell(loc, buildingPerformance), PathEndMode.OnCell);
        toil.tickAction = delegate { buildingPerformance.ticksIntoThisPerformance++; };
        return toil;
    }

    private void Romance(List<Toil> leadToils, List<Toil> supportToils, Building_Performance buildingPerformance)
    {
        leadToils.Add(GoToStageLocation(buildingPerformance, StageLocations.STAGE_L));
        leadToils.Add(FaceDirection(StageRight));
        supportToils.Add(GoToStageLocation(buildingPerformance, StageLocations.STAGE_R));
        supportToils.Add(FaceDirection(StageLeft));
        leadToils.Add(Synchronise());
        supportToils.Add(Synchronise());
        leadToils.Add(ThrowDramaticMote(FleckDefOf.Heart, 401));
        supportToils.Add(Wait(100));
        supportToils.Add(ThrowDramaticMote(FleckDefOf.Heart, 151));
        leadToils.Add(Synchronise());
        supportToils.Add(Synchronise());
        leadToils.Add(GoToStageLocation(buildingPerformance, StageLocations.DOWNSTAGE_C));
        supportToils.Add(GoToStageLocation(buildingPerformance, StageLocations.DOWNSTAGE_C));
        leadToils.Add(FaceDirection(StageRight));
        supportToils.Add(FaceDirection(StageLeft));
        leadToils.Add(ThrowDramaticMote(FleckDefOf.Heart, 351));
        supportToils.Add(ThrowDramaticMote(FleckDefOf.Heart, 351));
        leadToils.Add(Synchronise());
        supportToils.Add(Synchronise());
    }

    private void Tragedy(List<Toil> leadToils, List<Toil> supportToils, Building_Performance buildingPerformance)
    {
        leadToils.Add(GoToStageLocation(buildingPerformance, StageLocations.DOWNSTAGE_C));
        supportToils.Add(GoToStageLocation(buildingPerformance, StageLocations.UPSTAGE_L));
        leadToils.Add(Synchronise());
        supportToils.Add(Synchronise());
        leadToils.Add(ThrowDramaticMote(ThingDefOf_RazzleDazzle.Mote_Tragedy, 201));
        leadToils.Add(GoToStageLocation(buildingPerformance, StageLocations.DOWNSTAGE_L));
        leadToils.Add(FaceDirection(StageForward));
        leadToils.Add(ThrowDramaticMote(ThingDefOf_RazzleDazzle.Mote_Tragedy, 401));
        leadToils.Add(GoToStageLocation(buildingPerformance, StageLocations.DOWNSTAGE_R));
        leadToils.Add(FaceDirection(StageForward));
        leadToils.Add(ThrowDramaticMote(ThingDefOf_RazzleDazzle.Mote_Tragedy, 251));
        leadToils.Add(Synchronise());
        supportToils.Add(Synchronise());
        supportToils.Add(GoToStageLocation(buildingPerformance, StageLocations.DOWNSTAGE_L));
        supportToils.Add(FaceDirection(StageRight));
        supportToils.Add(ThrowDramaticMote(FleckDefOf.Heart, 201));
        leadToils.Add(Synchronise());
        supportToils.Add(Synchronise());
    }

    private void Comedy(List<Toil> leadToils, List<Toil> supportToils, Building_Performance buildingPerformance)
    {
        leadToils.Add(GoToStageLocation(buildingPerformance, StageLocations.DOWNSTAGE_R));
        supportToils.Add(GoToStageLocation(buildingPerformance, StageLocations.DOWNSTAGE_L));
        leadToils.Add(Synchronise());
        supportToils.Add(Synchronise());
        leadToils.Add(GoToStageLocation(buildingPerformance, StageLocations.STAGE_C));
        supportToils.Add(GoToStageLocation(buildingPerformance, StageLocations.STAGE_C));
        leadToils.Add(Synchronise());
        supportToils.Add(Synchronise());
        leadToils.Add(GoToStageLocation(buildingPerformance, StageLocations.DOWNSTAGE_L));
        leadToils.Add(Synchronise());
        supportToils.Add(Synchronise());
        leadToils.Add(ThrowDramaticMote(ThingDefOf_RazzleDazzle.Mote_Comedy, 401));
        leadToils.Add(ThrowDramaticMote(ThingDefOf_RazzleDazzle.Mote_Tragedy, 271));
        leadToils.Add(Synchronise());
        supportToils.Add(Synchronise());
    }

    private void MusicalDance(List<Toil> leadToils, List<Toil> supportToils, Building_Performance buildingPerformance)
    {
        leadToils.Add(GoToStageLocation(buildingPerformance, StageLocations.DOWNSTAGE_C));
        leadToils.Add(FaceDirection(StageRight));
        supportToils.Add(GoToStageLocation(buildingPerformance, StageLocations.DOWNSTAGE_C));
        supportToils.Add(FaceDirection(StageLeft));
        leadToils.Add(Synchronise());
        supportToils.Add(Synchronise());
        leadToils.Add(ThrowDramaticMote(ThingDefOf_RazzleDazzle.Mote_Music, 401));
        supportToils.Add(ThrowDramaticMote(ThingDefOf_RazzleDazzle.Mote_Music, 401));
        leadToils.Add(Synchronise());
        supportToils.Add(Synchronise());
        leadToils.Add(GoToStageLocation(buildingPerformance, StageLocations.DOWNSTAGE_L));
        supportToils.Add(GoToStageLocation(buildingPerformance, StageLocations.DOWNSTAGE_R));
        leadToils.Add(Synchronise());
        supportToils.Add(Synchronise());
        leadToils.Add(ThrowDramaticMote(ThingDefOf_RazzleDazzle.Mote_Music, 401));
        supportToils.Add(ThrowDramaticMote(ThingDefOf_RazzleDazzle.Mote_Music, 401));
        leadToils.Add(Synchronise());
        supportToils.Add(Synchronise());
        leadToils.Add(GoToStageLocation(buildingPerformance, StageLocations.UPSTAGE_R));
        supportToils.Add(GoToStageLocation(buildingPerformance, StageLocations.UPSTAGE_L));
        leadToils.Add(Synchronise());
        supportToils.Add(Synchronise());
        leadToils.Add(ThrowDramaticMote(ThingDefOf_RazzleDazzle.Mote_Music, 401));
        supportToils.Add(ThrowDramaticMote(ThingDefOf_RazzleDazzle.Mote_Music, 401));
        leadToils.Add(Synchronise());
        supportToils.Add(Synchronise());
        leadToils.Add(GoToStageLocation(buildingPerformance, StageLocations.DOWNSTAGE_C));
        leadToils.Add(FaceDirection(StageRight));
        supportToils.Add(GoToStageLocation(buildingPerformance, StageLocations.DOWNSTAGE_C));
        supportToils.Add(FaceDirection(StageLeft));
        leadToils.Add(Synchronise());
        supportToils.Add(Synchronise());
        leadToils.Add(ThrowDramaticMote(ThingDefOf_RazzleDazzle.Mote_Music, 401));
        supportToils.Add(ThrowDramaticMote(ThingDefOf_RazzleDazzle.Mote_Music, 401));
        leadToils.Add(Synchronise());
        supportToils.Add(Synchronise());
    }

    private static IntVec3 GetStageCell(StageLocations loc, Building_Performance stage)
    {
        var position = stage.Position;
        var rotation = stage.Rotation;
        var result = new IntVec3(position.x, position.y, position.z);
        switch (loc)
        {
            case StageLocations.STAGE_R:
                if (rotation.IsHorizontal)
                {
                    result.z -= 2;
                }
                else
                {
                    result.x -= 2;
                }

                break;
            case StageLocations.STAGE_L:
                if (rotation.IsHorizontal)
                {
                    result.z += 2;
                }
                else
                {
                    result.x += 2;
                }

                break;
            case StageLocations.UPSTAGE_R:
                if (rotation.IsHorizontal)
                {
                    result.z -= 2;
                    result.x--;
                }
                else
                {
                    result.x -= 2;
                    result.z++;
                }

                break;
            case StageLocations.UPSTAGE_L:
                if (rotation.IsHorizontal)
                {
                    result.z += 2;
                    result.x--;
                }
                else
                {
                    result.x += 2;
                    result.z++;
                }

                break;
            case StageLocations.UPSTAGE_C:
                if (rotation.IsHorizontal)
                {
                    result.x--;
                }
                else
                {
                    result.z++;
                }

                break;
            case StageLocations.DOWNSTAGE_R:
                if (rotation.IsHorizontal)
                {
                    result.z -= 2;
                    result.x++;
                }
                else
                {
                    result.x -= 2;
                    result.z--;
                }

                break;
            case StageLocations.DOWNSTAGE_L:
                if (rotation.IsHorizontal)
                {
                    result.z += 2;
                    result.x++;
                }
                else
                {
                    result.x += 2;
                    result.z--;
                }

                break;
            case StageLocations.DOWNSTAGE_C:
                if (rotation.IsHorizontal)
                {
                    result.x++;
                }
                else
                {
                    result.z--;
                }

                break;
            case StageLocations.BACKSTAGE:
                if (rotation.IsHorizontal)
                {
                    result.x += 3;
                }
                else
                {
                    result.z -= 3;
                }

                break;
        }

        return result;
    }
}