using UnityEngine;

[CreateAssetMenu(fileName = "NewNote", menuName = "Notes/Note")]
public class Note : ScriptableObject
{
	public string noteID;
	public string title;
	[TextArea]
	public string content;
}
