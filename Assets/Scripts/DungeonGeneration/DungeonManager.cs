using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [SerializeField] private GameObject[] topRooms;
    [SerializeField] private GameObject[] bottomRooms;
    [SerializeField] private GameObject[] leftRooms;
    [SerializeField] private GameObject[] rightRooms;

    [SerializeField] private GameObject closedRoom;

    public static DungeonManager Instance { get; private set; }
    public GameObject[] TopRooms { get => topRooms; set => topRooms = value; }
    public GameObject[] BottomRooms { get => bottomRooms; set => bottomRooms = value; }
    public GameObject[] LeftRooms { get => leftRooms; set => leftRooms = value; }
    public GameObject[] RightRooms { get => rightRooms; set => rightRooms = value; }
    public GameObject ClosedRoom { get => closedRoom; set => closedRoom = value; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}
