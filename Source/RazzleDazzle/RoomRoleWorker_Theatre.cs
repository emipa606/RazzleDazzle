using Verse;

namespace RazzleDazzle;

public class RoomRoleWorker_Theatre : RoomRoleWorker
{
    public override float GetScore(Room room)
    {
        var num = 0;
        var containedAndAdjacentThings = room.ContainedAndAdjacentThings;
        foreach (var thing in containedAndAdjacentThings)
        {
            if (thing is Building_Stage)
            {
                num++;
            }
        }

        return 15f * num;
    }
}