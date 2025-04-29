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

    [SerializeField] private int mainHubSceneIndex = 2;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(mainHubSceneIndex);
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
