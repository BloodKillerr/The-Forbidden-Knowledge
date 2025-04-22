using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private Direction openingDirection;

    private int rand;

    private bool spawned = false;

    private void Update()
    {
        Invoke("Spawn", 0.1f);
    }

    private void Spawn()
    {
        if(!spawned)
        {
            switch (openingDirection)
            {
                case Direction.TOP:
                    rand = Random.Range(0, DungeonManager.Instance.BottomRooms.Length);
                    Instantiate(DungeonManager.Instance.BottomRooms[rand], transform.position, DungeonManager.Instance.BottomRooms[rand].transform.rotation);
                    break;
                case Direction.BOTTON:
                    rand = Random.Range(0, DungeonManager.Instance.TopRooms.Length);
                    Instantiate(DungeonManager.Instance.TopRooms[rand], transform.position, DungeonManager.Instance.TopRooms[rand].transform.rotation);
                    break;
                case Direction.LEFT:
                    rand = Random.Range(0, DungeonManager.Instance.RightRooms.Length);
                    Instantiate(DungeonManager.Instance.RightRooms[rand], transform.position, DungeonManager.Instance.RightRooms[rand].transform.rotation);
                    break;
                case Direction.RIGHT:
                    rand = Random.Range(0, DungeonManager.Instance.LeftRooms.Length);
                    Instantiate(DungeonManager.Instance.LeftRooms[rand], transform.position, DungeonManager.Instance.LeftRooms[rand].transform.rotation);
                    break;
            }
            spawned = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        try
        {
            if (other.GetComponent<SpawnPoint>().spawned == false && spawned == false)
            {
                Instantiate(DungeonManager.Instance.ClosedRoom, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
        catch
        {
            Destroy(gameObject);
        }
    }
}

public enum Direction
{
    TOP,
    BOTTON,
    LEFT,
    RIGHT
}
