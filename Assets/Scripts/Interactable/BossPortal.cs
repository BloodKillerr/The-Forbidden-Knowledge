using UnityEngine;
using UnityEngine.SceneManagement;

public class BossPortal : Interactable
{
    public int SceneIndex;
    public bool isFinal = false;
    private bool informationShown = false;

    public override void Interact()
    {
        if (Player.Instance.GetComponent<PlayerMovement>().IsDodging || Player.Instance.GetComponent<PlayerAttack>().IsAttacking)
        {
            return;
        }
        base.Interact();

        if (isFinal)
        {
            if(informationShown)
            {
                SceneManager.LoadScene(SceneIndex);
            }
            else
            {
                //Run information to be added
                informationShown = true;
            }
        }
        else
        {
            SceneManager.LoadScene(SceneIndex);
        }
    }
}
