using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject optionsPanel;

    [SerializeField] private Button mainMenuSelectedButton;
    [SerializeField] private Button optionsMenuSelectedButton;

    public void StartGame()
    {
        SceneManager.LoadScene(1);
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
        UIManager.Instance.ChangeSelectedElement(optionsMenuSelectedButton.gameObject);
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
        UIManager.Instance.ChangeSelectedElement(mainMenuSelectedButton.gameObject);
    }
}
