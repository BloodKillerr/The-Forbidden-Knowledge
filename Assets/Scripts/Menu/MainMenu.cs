using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

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
        EventSystem.current.SetSelectedGameObject(mainMenuSelectedButton.gameObject);
    }
}
