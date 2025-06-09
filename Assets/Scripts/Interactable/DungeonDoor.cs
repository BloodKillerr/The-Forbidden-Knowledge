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
        if (Player.Instance.GetComponent<PlayerMovement>().IsDodging || Player.Instance.GetComponent<PlayerAttack>().IsAttacking)
        {
            return;
        }
        base.Interact();
        roomController.MovePlayerThroughDoor(direction);

        if(GetComponentInParent<RoomController>().IsFinished)
        {
            SoundManager.PlaySound(SoundType.DOOR, GetComponent<AudioSource>(), 1);
        }
    }
}
