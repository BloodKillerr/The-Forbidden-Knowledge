using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject additionalPanel;
    [SerializeField] private GameObject newGamePanel;

    [SerializeField] private Button mainMenuSelectedButton;
    [SerializeField] private Button optionsMenuSelectedButton;
    [SerializeField] private Button creditsMenuSelectedButton;
    [SerializeField] private Button newGameMenuSelectedButton;
    [SerializeField] private Button continueButton;

    [SerializeField] private int mainHubSceneIndex = 1;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        MasterSaveData data = SaveManager.LoadGame();

        if (data == null)
        {
            continueButton.interactable = false;
        }
        else
        {
            continueButton.interactable = true;
        }
    }

    public void StartGame()
    {
        SaveManager.DeleteSave();
        SceneManager.LoadScene(mainHubSceneIndex);
    }

    public void ContinueGame()
    {
        MasterSaveData data = SaveManager.LoadGame();

        if (data == null)
        {
            Debug.Log("[MainMenu] No valid save found. Starting a New Game instead.");
            return;
        }
        SceneManager.LoadScene(data.LastSceneBuildIndex);
    }

    public void Options()
    {
        optionsPanel.SetActive(true);
        menuPanel.SetActive(false);
        additionalPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(optionsMenuSelectedButton.gameObject);
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
        additionalPanel.SetActive(true);
        newGamePanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(mainMenuSelectedButton.gameObject);
    }

    public void DonateButton()
    {
        Application.OpenURL("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
        //Application.OpenURL("https://paypal.me/BloodKillerr");
    }

    public void ShowCredits()
    {
        creditsPanel.SetActive(true);
        menuPanel.SetActive(false);
        additionalPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(creditsMenuSelectedButton.gameObject);
    }

    public void HideCredits()
    {
        creditsPanel.SetActive(false);
        menuPanel.SetActive(true);
        additionalPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(mainMenuSelectedButton.gameObject);
    }

    public void ShowNewGamePanel()
    {
        MasterSaveData data = SaveManager.LoadGame();

        if(data == null)
        {
            StartGame();
            return;
        }

        newGamePanel.SetActive(true);
        menuPanel.SetActive(false);
        additionalPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(newGameMenuSelectedButton.gameObject);
    }
}
