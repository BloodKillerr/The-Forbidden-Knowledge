using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    private RoomData data;

    public void Init(RoomData data)
    {
        this.data = data;
    }
    public void MovePlayerThroughDoor(Direction direction)
    {
        Vector2Int targetPos = data.Position + DirectionExtensions.ToVector2Int(direction);
        RoomData targetData = DungeonManager.Instance.GetRoomData(targetPos);
        if (targetData == null)
        {
            return;
        }

        GameObject targetRoom = DungeonManager.Instance.PlacedRooms[targetPos];
        Vector3 entrySpot = targetRoom.GetComponent<RoomController>().GetDoorEntryPosition(DirectionExtensions.Opposite(direction));

        Player.Instance.GetComponent<PlayerMovement>().Rb.MovePosition(entrySpot);
        targetRoom.GetComponent<RoomController>().TriggerOnEntryToRoom();
    }

    public Vector3 GetDoorEntryPosition(Direction incoming)
    {
        string name = $"Door_{DirectionExtensions.ToShortString(incoming)}_Entry";
        Transform t = transform
        .GetComponentsInChildren<Transform>()
        .FirstOrDefault(x => x.name == name);
        return t != null ? t.position : transform.position;
    }

    private void TriggerOnEntryToRoom()
    {
        if(!data.HasBeenEntered)
        {
            //Spawn Objects and Enemies
            data.HasBeenEntered = true;
        }
    }
}
