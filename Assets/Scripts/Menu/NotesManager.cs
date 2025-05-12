using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class NotesManager : MonoBehaviour
{
	public static NotesManager Instance { get; private set; }

	[Header("UI Elements")]
	public GameObject noteButtonPrefab;
	public Transform notesListParent;
	public TextMeshProUGUI noteContentText;

	[Header("All Notes")]
	public List<Note> allNotes = new List<Note>();

	private HashSet<string> unlockedNoteIDs = new HashSet<string>();

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

	public void OnNotesPanelOpened()
	{
		PopulateNotesList();
		noteContentText.text = "";
	}

	public void UnlockNote(string noteID)
	{
		if (unlockedNoteIDs.Add(noteID))
		{
			Debug.Log("Note Unlocked: " + noteID);
		}
	}

	public void PopulateNotesList()
	{
		if (notesListParent == null)
		{
			return;
		}

		foreach (Transform child in notesListParent)
			Destroy(child.gameObject);

		for (int i = 0; i < allNotes.Count; i++)
		{
			Note note = allNotes[i];
			GameObject btn = Instantiate(noteButtonPrefab, notesListParent);
			TMP_Text btnText = btn.GetComponentInChildren<TMP_Text>();

			btnText.text = unlockedNoteIDs.Contains(note.noteID) ? note.title : "???";

			int index = i;
			btn.GetComponent<Button>().onClick.AddListener(() =>
			{
				ShowNote(index);
			});

			UnlockNote(note.noteID);
		}
	}

	public void ShowNote(int index)
	{
		Note note = allNotes[index];
		if (unlockedNoteIDs.Contains(note.noteID))
			noteContentText.text = String.Format("{0}\n\n{1}",note.title,note.content);
		else
			noteContentText.text = "This note is locked";
	}
}
