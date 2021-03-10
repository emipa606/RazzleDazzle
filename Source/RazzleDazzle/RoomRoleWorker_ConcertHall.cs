using System;
using System.Collections.Generic;
using Verse;

namespace RazzleDazzle
{
	// Token: 0x0200002C RID: 44
	public class RoomRoleWorker_ConcertHall : RoomRoleWorker
	{
		// Token: 0x060000EA RID: 234 RVA: 0x00006114 File Offset: 0x00004314
		public override float GetScore(Room room)
		{
			var num = 0;
			List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
			for (var i = 0; i < containedAndAdjacentThings.Count; i++)
			{
				if (containedAndAdjacentThings[i] is Building_GrandPiano)
				{
					num++;
				}
			}
			return 15f * (float)num;
		}
	}
}
