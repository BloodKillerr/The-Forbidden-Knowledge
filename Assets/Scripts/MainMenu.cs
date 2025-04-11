using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private TextMeshProUGUI optionsLabel;

    public void StartGame()
    {
        Debug.Log("Start game");
    }

    public void ContinueGame()
    {
        Debug.Log("Continue game");
    }

    public void Options()
    {
        optionsPanel.SetActive(true);
        menuPanel.SetActive(false);
        optionsLabel.text = "Options";
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit game");
    }

    public void BackButton()
    {
        optionsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
}
