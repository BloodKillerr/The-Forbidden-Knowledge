using UnityEngine;

public class DungeonDoor : Interactable
{
    [SerializeField] private Direction direction;
    private RoomController roomController;
    private void Start()
    {
        roomController = GetComponentInParent<RoomController>();
    }

    public override void Interact()
    {
        base.Interact();
        roomController.MovePlayerThroughDoor(direction);
    }
}
