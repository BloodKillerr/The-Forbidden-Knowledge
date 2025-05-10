using UnityEngine;

public class PauseManager : MonoBehaviour
{
	public static PauseManager Instance { get; private set; }

	[SerializeField] private GameObject pausePanel;
	[SerializeField] private GameObject optionsPanel;
	[SerializeField] private GameObject notesPanel;

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
			NotesManager.Instance.PopulateNotesList();
		}
	}

	public void Options()
	{
		if (optionsPanel.activeInHierarchy)
		{
			optionsPanel.SetActive(false);
			pausePanel.SetActive(true);
		}
		else
		{
			optionsPanel.SetActive(true);
			pausePanel.SetActive(false);
			notesPanel.SetActive(false);
		}
	}

	public void MainMenu()
	{
		Debug.Log("Main Menu");
	}

	public void Quit()
	{
		Debug.Log("Quit");
		Application.Quit();
	}
}
