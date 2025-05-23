using UnityEngine;

public class PlayerTracker : MonoBehaviour
{
	public Transform player;
	public float checkInterval = 0.2f;

	private Vector2Int currentRoomPos;
	private DungeonManager dungeonManager;
	private MinimapManager minimap;

	private bool inDungeon = false;

	private void Start()
	{
		dungeonManager = FindObjectOfType<DungeonManager>();
		minimap = FindObjectOfType<MinimapManager>();
		InvokeRepeating(nameof(CheckRoom), 0f, checkInterval);
	}

	public void EnterDungeon()
	{
		inDungeon = true;
	}

	void CheckRoom()
	{
		if (!inDungeon) return;

		Vector2Int playerGridPos = dungeonManager.GetRoomPositionFromWorld(player.position);

		if (playerGridPos != currentRoomPos)
		{
			currentRoomPos = playerGridPos;
			minimap.CreateRoomIcon(playerGridPos);
			minimap.HighlightPlayer(playerGridPos);
		}
	}
}
