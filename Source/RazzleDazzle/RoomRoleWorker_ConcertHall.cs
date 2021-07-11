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
            var containedAndAdjacentThings = room.ContainedAndAdjacentThings;
            foreach (var thing in containedAndAdjacentThings)
            {
                if (thing is Building_GrandPiano)
                {
                    num++;
                }
            }

            return 15f * num;
        }
    }
}