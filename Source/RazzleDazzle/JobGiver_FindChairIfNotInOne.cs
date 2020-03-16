using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace RazzleDazzle
{
	// Token: 0x02000017 RID: 23
	public class JobGiver_FindChairIfNotInOne : ThinkNode_JobGiver
	{
		// Token: 0x0600006E RID: 110 RVA: 0x00003B58 File Offset: 0x00001D58
		public bool CellHasChair(IntVec3 pos, Map m)
		{
			Building edifice = pos.GetEdifice(m);
			return edifice != null && edifice.def.category == ThingCategory.Building && edifice.def.building.isSittable;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00003B94 File Offset: 0x00001D94
		private static bool TryToFindChairNear(IntVec3 center, Pawn sitter, out Thing chair)
		{
			for (int i = 0; i < JobGiver_FindChairIfNotInOne.rpo.Count; i++)
			{
				IntVec3 c = center + JobGiver_FindChairIfNotInOne.rpo[i];
				Building edifice = c.GetEdifice(sitter.Map);
				if (edifice != null && edifice.def.category == ThingCategory.Building && edifice.def.building.isSittable && sitter.CanReserve(edifice, 1, -1, null, false) && !edifice.IsForbidden(sitter) && GenSight.LineOfSight(center, edifice.Position, sitter.Map, true, null, 0, 0) && c.GetFirstPawn(sitter.Map) == null)
				{
					chair = edifice;
					return true;
				}
			}
			chair = null;
			return false;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00003C48 File Offset: 0x00001E48
		private static bool TryToFindGroundSpotNear(IntVec3 center, Pawn sitter, out IntVec3 result)
		{
			for (int i = 0; i < 30; i++)
			{
				IntVec3 intVec = center + JobGiver_FindChairIfNotInOne.rpo[i];
				if (sitter.CanReserveAndReach(intVec, PathEndMode.OnCell, Danger.None, 1, -1, null, false) && intVec.GetEdifice(sitter.Map) == null && GenSight.LineOfSight(center, intVec, sitter.Map, true, null, 0, 0) && intVec.GetFirstPawn(sitter.Map) == null)
				{
					result = intVec;
					return true;
				}
			}
			result = IntVec3.Invalid;
			return false;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00003CCC File Offset: 0x00001ECC
		protected override Job TryGiveJob(Pawn pawn)
		{
			Job result;
			Thing t;
			IntVec3 c;
			if (this.CellHasChair(pawn.Position, pawn.Map) && pawn.CanSee(pawn.mindState.duty.focus.Thing, null))
			{
				result = null;
			}
			else if (JobGiver_FindChairIfNotInOne.TryToFindChairNear(pawn.mindState.duty.focus.Cell, pawn, out t))
			{
				result = new Job(JobDefOf.Goto, t);
			}
			else if (JobGiver_FindChairIfNotInOne.TryToFindGroundSpotNear(pawn.mindState.duty.focus.Cell, pawn, out c))
			{
				result = new Job(JobDefOf.Goto, c);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x04000029 RID: 41
		private static int numCellsInRadius = GenRadial.NumCellsInRadius(7f);

		// Token: 0x0400002A RID: 42
		private static List<IntVec3> rpo = GenRadial.RadialPattern.Take(JobGiver_FindChairIfNotInOne.numCellsInRadius).ToList<IntVec3>();
	}
}
