using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private GameObject interactIcon;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (interactIcon != null)
            {
                interactIcon.SetActive(true);
            }
            Debug.Log("Player Entered Interactable");
            Player.Instance.SubscribeToInteraction(Interact);
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (interactIcon != null)
            {
                interactIcon.SetActive(false);
            }
            Debug.Log("Player Left Interactable");
            Player.Instance.UnsubscribeFromInteraction(Interact);
        }
    }

    public virtual void Interact()
    {
        Debug.Log(gameObject.name + " interacted!");
    }
}
