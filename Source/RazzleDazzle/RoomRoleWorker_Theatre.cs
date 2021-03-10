using System;
using System.Collections.Generic;
using Verse;

namespace RazzleDazzle
{
	// Token: 0x0200002D RID: 45
	public class RoomRoleWorker_Theatre : RoomRoleWorker
	{
		// Token: 0x060000EC RID: 236 RVA: 0x00006160 File Offset: 0x00004360
		public override float GetScore(Room room)
		{
			var num = 0;
			List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
			for (var i = 0; i < containedAndAdjacentThings.Count; i++)
			{
				if (containedAndAdjacentThings[i] is Building_Stage)
				{
					num++;
				}
			}
			return 15f * (float)num;
		}
	}
}
