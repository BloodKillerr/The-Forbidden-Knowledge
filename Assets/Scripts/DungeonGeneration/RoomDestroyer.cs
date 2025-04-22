using UnityEngine;

public class RoomDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("ClosedRoom"))
        {
            Destroy(other.gameObject);
        }
    }
}
