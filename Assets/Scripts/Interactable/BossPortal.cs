using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossPortal : Interactable
{
    public int SceneIndex;
    public bool isFinal = false;
    private bool informationShown = false;
    [SerializeField] private TMP_Text interactMessage;

    private void Start()
    {
        if (isFinal)
        {
            interactMessage.text = "You have finished the run!";
        }
        else
        {
            interactMessage.text = "Move Forward";
        }
    }

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
                Player.Instance.GetComponent<PlayerTracker>().ExitDungeon();
                MinimapManager.Instance.ClearMinimap();
                SceneManager.LoadScene(SceneIndex);
            }
            else
            {
                interactMessage.text = "Move Forward";
                informationShown = true;
            }
        }
        else
        {
            SceneManager.LoadScene(SceneIndex);
        }
    }
}
