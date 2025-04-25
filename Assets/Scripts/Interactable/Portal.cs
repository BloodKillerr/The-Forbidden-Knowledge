using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : Interactable
{
    [SerializeField] private int sceneIndex;
    public override void Interact()
    {
        base.Interact();
        SceneManager.LoadScene(sceneIndex);
    }
}
