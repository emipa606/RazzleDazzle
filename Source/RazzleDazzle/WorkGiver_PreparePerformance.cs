using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace RazzleDazzle
{
	// Token: 0x02000030 RID: 48
	public class WorkGiver_PreparePerformance : WorkGiver_Scanner
	{
		// Token: 0x060000EE RID: 238 RVA: 0x000061A4 File Offset: 0x000043A4
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			bool result;
			if (!(t is Building_Performance))
			{
				result = false;
			}
			else
			{
				Building_Performance building_Performance = t as Building_Performance;
				result = (building_Performance != null && building_Performance.CurrentlyUsableForBills() && pawn.CanReserve(t, 1, 0, null, false) && !building_Performance.IsBurning() && !building_Performance.IsForbidden(pawn) && building_Performance.canPerformSwitch && !building_Performance.rehearsing && pawn.CanReach(t.InteractionCell, PathEndMode.OnCell, Danger.Some, false, TraverseMode.ByPawn) && this.JobOnThing(pawn, t, false) != null);
			}
			return result;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x0000622C File Offset: 0x0000442C
		public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
		{
			Building_Performance building_Performance = thing as Building_Performance;
			Job result;
			if (building_Performance.venueDef.artDef == null)
			{
				if (!pawn.workSettings.WorkIsActive(this.def.workType))
				{
					result = null;
				}
				else
				{
					result = this.AssignRehearsalDirectJob(pawn, building_Performance);
				}
			}
			else
			{
				result = this.ChooseArtAndCarryItJob(pawn, building_Performance);
			}
			return result;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x0000627E File Offset: 0x0000447E
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			return from t in pawn.Map.listerThings.AllThings
			where t is Building_Performance
			select t;
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x000062B4 File Offset: 0x000044B4
		private Job AssignRehearsalDirectJob(Pawn pawn, Building_Performance giver)
		{
			return new Job(JobDefOfRazzleDazzle.JobDef_DoStartRehearsalDirectly, giver);
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x000062C8 File Offset: 0x000044C8
		private Job ChooseArtAndCarryItJob(Pawn pawn, Building_Performance giver)
		{
			WorkGiver_PreparePerformance.correctArt.Clear();
			Thing t;
			Job result;
			if (!WorkGiver_PreparePerformance.TryFindBestArt(giver, pawn, out t))
			{
				result = null;
			}
			else
			{
				result = new Job(JobDefOfRazzleDazzle.JobDef_DoRehearsePlayBill, giver, t)
				{
					targetB = t,
					count = 1,
					haulMode = HaulMode.ToCellNonStorage
				};
			}
			return result;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00006320 File Offset: 0x00004520
		private static bool TryFindBestArt(Building_Performance venue, Pawn pawn, out Thing art)
		{
			art = null;
			IntVec3 position = venue.Position;
			Region validRegionAt = venue.Map.regionGrid.GetValidRegionAt(position);
			bool result;
			if (validRegionAt == null)
			{
				result = false;
			}
			else
			{
				bool found = false;
				Predicate<Thing> validArt = (Thing t) => t.Spawned && !t.IsForbidden(pawn) && (t.Position - pawn.Position).LengthHorizontalSquared < 999 && t.def == venue.venueDef.artDef && pawn.CanReserve(t, 1, -1, null, false);
				WorkGiver_PreparePerformance.newCorrectArt.Clear();
				RegionProcessor regionProcessor = delegate(Region r)
				{
					List<Thing> list = r.ListerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.HaulableEver));
					for (int i = 0; i < list.Count; i++)
					{
						Thing thing = list[i];
						if (validArt(thing) && !thing.def.IsMedicine)
						{
							WorkGiver_PreparePerformance.newCorrectArt.Add(thing);
						}
					}
					if (WorkGiver_PreparePerformance.newCorrectArt.Count > 0)
					{
						Comparison<Thing> comparison = delegate(Thing t1, Thing t2)
						{
							float num = (float)(t1.Position - pawn.Position).LengthHorizontalSquared;
							float value = (float)(t2.Position - pawn.Position).LengthHorizontalSquared;
							return num.CompareTo(value);
						};
						WorkGiver_PreparePerformance.newCorrectArt.Sort(comparison);
						WorkGiver_PreparePerformance.correctArt.AddRange(WorkGiver_PreparePerformance.newCorrectArt);
						WorkGiver_PreparePerformance.newCorrectArt.Clear();
						if (WorkGiver_PreparePerformance.correctArt.Count > 0)
						{
							found = true;
							return true;
						}
					}
					return false;
				};
				TraverseParms traverseParams = TraverseParms.For(pawn, Danger.Some, TraverseMode.ByPawn, false);
				RegionEntryPredicate entryCondition = (Region from, Region r) => r.Allows(traverseParams, false);
				RegionTraverser.BreadthFirstTraverse(validRegionAt, entryCondition, regionProcessor, 999, RegionType.Set_Passable);
				if (found)
				{
					art = WorkGiver_PreparePerformance.correctArt[0];
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x04000067 RID: 103
		private static List<Thing> correctArt = new List<Thing>();

		// Token: 0x04000068 RID: 104
		private static List<Thing> newCorrectArt = new List<Thing>();
	}
}
