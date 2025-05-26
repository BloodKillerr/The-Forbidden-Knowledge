using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
	public static PauseManager Instance { get; private set; }

	[SerializeField] private GameObject pausePanel;
	[SerializeField] private GameObject optionsPanel;
	[SerializeField] private GameObject notesPanel;

	public GameObject ResumeButton;
	public GameObject OptionsButton;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
		{
			Destroy(gameObject);
			return;
		}
	}

	public void Resume()
	{
		Debug.Log("Resume");
        UIManager.Instance.ToogleMenu(MenuType.PAUSE);
    }

	public void Save()
	{
		Debug.Log("Save");
	}

	public void Notes()
	{
		if (notesPanel.activeInHierarchy)
		{
			notesPanel.SetActive(false);
		}
		else
		{
			notesPanel.SetActive(true);
			NotesManager.Instance.OnNotesPanelOpened();
		}
	}

	public void Options()
	{
		if (optionsPanel.activeInHierarchy)
		{
			optionsPanel.SetActive(false);
			pausePanel.SetActive(true);
            UIManager.Instance.ChangeSelectedElement(ResumeButton);
        }
		else
		{
			optionsPanel.SetActive(true);
			pausePanel.SetActive(false);
			notesPanel.SetActive(false);
            UIManager.Instance.ChangeSelectedElement(OptionsButton);
        }
	}

	public void Hide()
	{
        pausePanel.SetActive(true);
        optionsPanel.SetActive(false);
        notesPanel.SetActive(false);
    }

	public void MainMenu()
	{
		Debug.Log("Main Menu");
        SceneManager.LoadScene(0);
        GameManager.Instance.ResumeGameState();
    }

	public void Quit()
	{
		Debug.Log("Quit");
		Application.Quit();
	}
}
