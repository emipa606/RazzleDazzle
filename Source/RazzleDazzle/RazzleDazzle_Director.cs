using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace RazzleDazzle
{
	// Token: 0x0200002A RID: 42
	public class RazzleDazzle_Director
	{
		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000D6 RID: 214 RVA: 0x0000548C File Offset: 0x0000368C
		private Rot4 StageForward
		{
			get
			{
				return this.stage.Rotation;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000D7 RID: 215 RVA: 0x0000549C File Offset: 0x0000369C
		private Rot4 StageLeft
		{
			get
			{
				Rot4 result;
				if (this.stage == null)
				{
					result = Rot4.South;
				}
				else
				{
					Rot4 rotation = this.stage.Rotation;
					rotation.Rotate(RotationDirection.Counterclockwise);
					result = rotation;
				}
				return result;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x000054D0 File Offset: 0x000036D0
		private Rot4 StageRight
		{
			get
			{
				Rot4 result;
				if (this.stage == null)
				{
					result = Rot4.South;
				}
				else
				{
					Rot4 rotation = this.stage.Rotation;
					rotation.Rotate(RotationDirection.Clockwise);
					result = rotation;
				}
				return result;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000D9 RID: 217 RVA: 0x00005504 File Offset: 0x00003704
		private Rot4 StageRear
		{
			get
			{
				Rot4 result;
				if (this.stage == null)
				{
					result = Rot4.South;
				}
				else
				{
					result = this.stage.Rotation.Opposite;
				}
				return result;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000DA RID: 218 RVA: 0x00005536 File Offset: 0x00003736
		public Pawn lead
		{
			get
			{
				return this.stage.Lead;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000DB RID: 219 RVA: 0x00005543 File Offset: 0x00003743
		public Pawn support
		{
			get
			{
				return this.stage.Support;
			}
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00005550 File Offset: 0x00003750
		public Toil FaceDirection(Rot4 dir)
		{
			Toil t = new Toil();
			t.defaultCompleteMode = ToilCompleteMode.Instant;
			t.initAction = delegate()
			{
				t.GetActor().Rotation = dir;
			};
			return t;
		}

		// Token: 0x060000DD RID: 221 RVA: 0x0000559E File Offset: 0x0000379E
		public Toil FaceObject(Thing thing)
		{
			Toil toil = new Toil();
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			toil.initAction = delegate()
			{
			};
			return toil;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x000055D4 File Offset: 0x000037D4
		public void ConstructPlay()
		{
			this.lToils = new List<Toil>();
			this.sToils = new List<Toil>();
			this.lToils.Add(this.GoToStageLocation(this.stage, RazzleDazzle_Director.StageLocations.BACKSTAGE));
			this.sToils.Add(this.GoToStageLocation(this.stage, RazzleDazzle_Director.StageLocations.BACKSTAGE));
			this.lToils.Add(this.Synchronise());
			this.sToils.Add(this.Synchronise());
			for (int i = 0; i < 6; i++)
			{
				switch (Rand.RangeInclusive(0, 3))
				{
				case 0:
					this.Romance(this.lToils, this.sToils, this.stage);
					break;
				case 1:
					this.Comedy(this.lToils, this.sToils, this.stage);
					break;
				case 2:
					this.Tragedy(this.lToils, this.sToils, this.stage);
					break;
				case 3:
					this.MusicalDance(this.lToils, this.sToils, this.stage);
					break;
				}
			}
		}

		// Token: 0x060000DF RID: 223 RVA: 0x000056E4 File Offset: 0x000038E4
		public List<Toil> RequestPlayToils(Pawn pawn, Building_Performance stage)
		{
			if (this.lToils == null || this.lToils.Count == 0)
			{
				this.stage = stage;
				this.ConstructPlay();
			}
			if (((this.lead != null && !this.lead.Dead) || pawn == this.support) && (this.support == null || this.support.Dead))
			{
				Pawn lead = this.lead;
			}
			List<Toil> result;
			if (pawn == this.lead)
			{
				result = this.lToils;
			}
			else if (pawn == this.support)
			{
				result = this.sToils;
			}
			else
			{
				result = new List<Toil>();
			}
			return result;
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x0000577C File Offset: 0x0000397C
		public Toil Synchronise()
		{
			Toil t = new Toil();
			t.defaultCompleteMode = ToilCompleteMode.Delay;
			t.initAction = delegate()
			{
				t.GetActor().jobs.curDriver.ticksLeftThisToil = 10000;
				this.leadReady = (this.supportReady = false);
			};
			t.tickAction = delegate()
			{
				t.GetActor().skills.GetSkill(SkillDefOf.Social).Learn(0.5f, false);
				if (t.GetActor() == this.lead)
				{
					this.leadReady = true;
				}
				else if (t.GetActor() == this.support)
				{
					this.supportReady = true;
				}
				if (this.leadReady && this.supportReady)
				{
					this.lead.jobs.curDriver.ticksLeftThisToil = 0;
					this.support.jobs.curDriver.ticksLeftThisToil = 0;
					this.leadReady = (this.supportReady = false);
				}
			};
			return t;
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x000057E4 File Offset: 0x000039E4
		public static Toil Wait(int iTicks)
		{
			Toil toil = new Toil();
			toil.defaultCompleteMode = ToilCompleteMode.Delay;
			toil.initAction = delegate()
			{
				toil.GetActor().jobs.curDriver.ticksLeftThisToil = iTicks;
			};
			return toil;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00005834 File Offset: 0x00003A34
		public Toil ThrowDramaticMote(Pawn p1, ThingDef mote, int ticks)
		{
			Toil toil = new Toil();
			toil.defaultCompleteMode = ToilCompleteMode.Delay;
			toil.initAction = delegate()
			{
				toil.GetActor().jobs.curDriver.ticksLeftThisToil = ticks;
			};
			toil.tickAction = delegate()
			{
				this.stage.ticksIntoThisPerformance++;
				if (toil.GetActor().jobs.curDriver.ticksLeftThisToil % 100 == 0)
				{
					MoteMaker.ThrowMetaIcon(toil.GetActor().Position, toil.GetActor().Map, mote);
				}
				toil.GetActor().skills.GetSkill(SkillDefOf.Social).Learn(1.5f, false);
			};
			return toil;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x000058A8 File Offset: 0x00003AA8
		public Toil GoToStageLocation(Building_Performance stage, RazzleDazzle_Director.StageLocations loc)
		{
			Toil toil = Toils_Goto.GotoCell(RazzleDazzle_Director.GetStageCell(loc, stage), PathEndMode.OnCell);
			toil.tickAction = delegate()
			{
				stage.ticksIntoThisPerformance++;
			};
			return toil;
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x000058E8 File Offset: 0x00003AE8
		public void Romance(List<Toil> leadToils, List<Toil> supportToils, Building_Performance stage)
		{
			leadToils.Add(this.GoToStageLocation(stage, RazzleDazzle_Director.StageLocations.STAGE_L));
			leadToils.Add(this.FaceDirection(this.StageRight));
			supportToils.Add(this.GoToStageLocation(stage, RazzleDazzle_Director.StageLocations.STAGE_R));
			supportToils.Add(this.FaceDirection(this.StageLeft));
			leadToils.Add(this.Synchronise());
			supportToils.Add(this.Synchronise());
			leadToils.Add(this.ThrowDramaticMote(this.lead, ThingDefOf.Mote_Heart, 401));
			supportToils.Add(RazzleDazzle_Director.Wait(100));
			supportToils.Add(this.ThrowDramaticMote(this.support, ThingDefOf.Mote_Heart, 151));
			leadToils.Add(this.Synchronise());
			supportToils.Add(this.Synchronise());
			leadToils.Add(this.GoToStageLocation(stage, RazzleDazzle_Director.StageLocations.DOWNSTAGE_C));
			supportToils.Add(this.GoToStageLocation(stage, RazzleDazzle_Director.StageLocations.DOWNSTAGE_C));
			leadToils.Add(this.FaceDirection(this.StageRight));
			supportToils.Add(this.FaceDirection(this.StageLeft));
			leadToils.Add(this.ThrowDramaticMote(this.lead, ThingDefOf.Mote_Heart, 351));
			supportToils.Add(this.ThrowDramaticMote(this.support, ThingDefOf.Mote_Heart, 351));
			leadToils.Add(this.Synchronise());
			supportToils.Add(this.Synchronise());
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00005A3C File Offset: 0x00003C3C
		public void Tragedy(List<Toil> leadToils, List<Toil> supportToils, Building_Performance stage)
		{
			leadToils.Add(this.GoToStageLocation(stage, RazzleDazzle_Director.StageLocations.DOWNSTAGE_C));
			supportToils.Add(this.GoToStageLocation(stage, RazzleDazzle_Director.StageLocations.UPSTAGE_L));
			leadToils.Add(this.Synchronise());
			supportToils.Add(this.Synchronise());
			leadToils.Add(this.ThrowDramaticMote(this.lead, ThingDefOf_RazzleDazzle.Mote_Tragedy, 201));
			leadToils.Add(this.GoToStageLocation(stage, RazzleDazzle_Director.StageLocations.DOWNSTAGE_L));
			leadToils.Add(this.FaceDirection(this.StageForward));
			leadToils.Add(this.ThrowDramaticMote(this.lead, ThingDefOf_RazzleDazzle.Mote_Tragedy, 401));
			leadToils.Add(this.GoToStageLocation(stage, RazzleDazzle_Director.StageLocations.DOWNSTAGE_R));
			leadToils.Add(this.FaceDirection(this.StageForward));
			leadToils.Add(this.ThrowDramaticMote(this.lead, ThingDefOf_RazzleDazzle.Mote_Tragedy, 251));
			leadToils.Add(this.Synchronise());
			supportToils.Add(this.Synchronise());
			supportToils.Add(this.GoToStageLocation(stage, RazzleDazzle_Director.StageLocations.DOWNSTAGE_L));
			supportToils.Add(this.FaceDirection(this.StageRight));
			supportToils.Add(this.ThrowDramaticMote(this.support, ThingDefOf.Mote_Heart, 201));
			leadToils.Add(this.Synchronise());
			supportToils.Add(this.Synchronise());
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00005B80 File Offset: 0x00003D80
		public void Comedy(List<Toil> leadToils, List<Toil> supportToils, Building_Performance stage)
		{
			leadToils.Add(this.GoToStageLocation(stage, RazzleDazzle_Director.StageLocations.DOWNSTAGE_R));
			supportToils.Add(this.GoToStageLocation(stage, RazzleDazzle_Director.StageLocations.DOWNSTAGE_L));
			leadToils.Add(this.Synchronise());
			supportToils.Add(this.Synchronise());
			leadToils.Add(this.GoToStageLocation(stage, RazzleDazzle_Director.StageLocations.STAGE_C));
			supportToils.Add(this.GoToStageLocation(stage, RazzleDazzle_Director.StageLocations.STAGE_C));
			leadToils.Add(this.Synchronise());
			supportToils.Add(this.Synchronise());
			leadToils.Add(this.GoToStageLocation(stage, RazzleDazzle_Director.StageLocations.DOWNSTAGE_L));
			leadToils.Add(this.Synchronise());
			supportToils.Add(this.Synchronise());
			leadToils.Add(this.ThrowDramaticMote(this.lead, ThingDefOf_RazzleDazzle.Mote_Comedy, 401));
			leadToils.Add(this.ThrowDramaticMote(this.support, ThingDefOf_RazzleDazzle.Mote_Tragedy, 271));
			leadToils.Add(this.Synchronise());
			supportToils.Add(this.Synchronise());
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00005C6C File Offset: 0x00003E6C
		public void MusicalDance(List<Toil> leadToils, List<Toil> supportToils, Building_Performance stage)
		{
			leadToils.Add(this.GoToStageLocation(stage, RazzleDazzle_Director.StageLocations.DOWNSTAGE_C));
			leadToils.Add(this.FaceDirection(this.StageRight));
			supportToils.Add(this.GoToStageLocation(stage, RazzleDazzle_Director.StageLocations.DOWNSTAGE_C));
			supportToils.Add(this.FaceDirection(this.StageLeft));
			leadToils.Add(this.Synchronise());
			supportToils.Add(this.Synchronise());
			leadToils.Add(this.ThrowDramaticMote(this.lead, ThingDefOf_RazzleDazzle.Mote_Music, 401));
			supportToils.Add(this.ThrowDramaticMote(this.support, ThingDefOf_RazzleDazzle.Mote_Music, 401));
			leadToils.Add(this.Synchronise());
			supportToils.Add(this.Synchronise());
			leadToils.Add(this.GoToStageLocation(stage, RazzleDazzle_Director.StageLocations.DOWNSTAGE_L));
			supportToils.Add(this.GoToStageLocation(stage, RazzleDazzle_Director.StageLocations.DOWNSTAGE_R));
			leadToils.Add(this.Synchronise());
			supportToils.Add(this.Synchronise());
			leadToils.Add(this.ThrowDramaticMote(this.lead, ThingDefOf_RazzleDazzle.Mote_Music, 401));
			supportToils.Add(this.ThrowDramaticMote(this.support, ThingDefOf_RazzleDazzle.Mote_Music, 401));
			leadToils.Add(this.Synchronise());
			supportToils.Add(this.Synchronise());
			leadToils.Add(this.GoToStageLocation(stage, RazzleDazzle_Director.StageLocations.UPSTAGE_R));
			supportToils.Add(this.GoToStageLocation(stage, RazzleDazzle_Director.StageLocations.UPSTAGE_L));
			leadToils.Add(this.Synchronise());
			supportToils.Add(this.Synchronise());
			leadToils.Add(this.ThrowDramaticMote(this.lead, ThingDefOf_RazzleDazzle.Mote_Music, 401));
			supportToils.Add(this.ThrowDramaticMote(this.support, ThingDefOf_RazzleDazzle.Mote_Music, 401));
			leadToils.Add(this.Synchronise());
			supportToils.Add(this.Synchronise());
			leadToils.Add(this.GoToStageLocation(stage, RazzleDazzle_Director.StageLocations.DOWNSTAGE_C));
			leadToils.Add(this.FaceDirection(this.StageRight));
			supportToils.Add(this.GoToStageLocation(stage, RazzleDazzle_Director.StageLocations.DOWNSTAGE_C));
			supportToils.Add(this.FaceDirection(this.StageLeft));
			leadToils.Add(this.Synchronise());
			supportToils.Add(this.Synchronise());
			leadToils.Add(this.ThrowDramaticMote(this.lead, ThingDefOf_RazzleDazzle.Mote_Music, 401));
			supportToils.Add(this.ThrowDramaticMote(this.support, ThingDefOf_RazzleDazzle.Mote_Music, 401));
			leadToils.Add(this.Synchronise());
			supportToils.Add(this.Synchronise());
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00005ED4 File Offset: 0x000040D4
		public static IntVec3 GetStageCell(RazzleDazzle_Director.StageLocations loc, Building_Performance stage)
		{
			IntVec3 position = stage.Position;
			Rot4 rotation = stage.Rotation;
			IntVec3 result = new IntVec3(position.x, position.y, position.z);
			switch (loc)
			{
			case RazzleDazzle_Director.StageLocations.STAGE_R:
				if (rotation.IsHorizontal)
				{
					result.z -= 2;
				}
				else
				{
					result.x -= 2;
				}
				break;
			case RazzleDazzle_Director.StageLocations.STAGE_L:
				if (rotation.IsHorizontal)
				{
					result.z += 2;
				}
				else
				{
					result.x += 2;
				}
				break;
			case RazzleDazzle_Director.StageLocations.UPSTAGE_R:
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
			case RazzleDazzle_Director.StageLocations.UPSTAGE_L:
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
			case RazzleDazzle_Director.StageLocations.UPSTAGE_C:
				if (rotation.IsHorizontal)
				{
					result.x--;
				}
				else
				{
					result.z++;
				}
				break;
			case RazzleDazzle_Director.StageLocations.DOWNSTAGE_R:
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
			case RazzleDazzle_Director.StageLocations.DOWNSTAGE_L:
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
			case RazzleDazzle_Director.StageLocations.DOWNSTAGE_C:
				if (rotation.IsHorizontal)
				{
					result.x++;
				}
				else
				{
					result.z--;
				}
				break;
			case RazzleDazzle_Director.StageLocations.BACKSTAGE:
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

		// Token: 0x04000045 RID: 69
		private const int nScenes = 6;

		// Token: 0x04000046 RID: 70
		public Building_Performance stage;

		// Token: 0x04000047 RID: 71
		public bool leadReady;

		// Token: 0x04000048 RID: 72
		public bool supportReady;

		// Token: 0x04000049 RID: 73
		public List<Toil> lToils;

		// Token: 0x0400004A RID: 74
		public List<Toil> sToils;

		// Token: 0x0400004B RID: 75
		public IEnumerable<RazzleDazzle_Director.DramaticExchange> allExchanges;

		// Token: 0x02000046 RID: 70
		// (Invoke) Token: 0x0600016D RID: 365
		public delegate void DramaticExchange(Pawn p1, Pawn p2, List<Toil> leadToils, List<Toil> supportToils, Building_Stage stage);

		// Token: 0x02000047 RID: 71
		public enum StageLocations
		{
			// Token: 0x040000AD RID: 173
			STAGE_R,
			// Token: 0x040000AE RID: 174
			STAGE_L,
			// Token: 0x040000AF RID: 175
			STAGE_C,
			// Token: 0x040000B0 RID: 176
			UPSTAGE_R,
			// Token: 0x040000B1 RID: 177
			UPSTAGE_L,
			// Token: 0x040000B2 RID: 178
			UPSTAGE_C,
			// Token: 0x040000B3 RID: 179
			DOWNSTAGE_R,
			// Token: 0x040000B4 RID: 180
			DOWNSTAGE_L,
			// Token: 0x040000B5 RID: 181
			DOWNSTAGE_C,
			// Token: 0x040000B6 RID: 182
			BACKSTAGE
		}
	}
}
