using UnityEngine;

public class SetPlayerStartingPosition : MonoBehaviour
{
    public Vector3 StartingPosition = Vector3.zero;
    public Vector3 StartingRotation = Vector3.zero;

    private Rigidbody rb;

    private void Awake()
    {
        rb = Player.Instance.GetComponent<PlayerMovement>().Rb;

        transform.position = StartingPosition;
        transform.rotation = Quaternion.Euler(StartingRotation);

        rb.position = StartingPosition;
        rb.rotation = transform.rotation;

        RigidbodyInterpolation interpolation = rb.interpolation;
        rb.interpolation = RigidbodyInterpolation.None;

        StartCoroutine(ReenableInterpolation(interpolation));
    }

    private System.Collections.IEnumerator ReenableInterpolation(RigidbodyInterpolation prev)
    {
        yield return null;
        rb.interpolation = prev;
    }
}
