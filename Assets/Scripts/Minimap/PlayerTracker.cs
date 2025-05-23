using UnityEngine;

public class PlayerTracker : MonoBehaviour
{
	public float checkInterval = 0.2f;

	private Vector2Int currentRoomPos = Vector2Int.zero;

	private bool inDungeon = false;

    private void Start()
    {
        InvokeRepeating(nameof(CheckRoom), 0f, checkInterval);
    }

    public void EnterDungeon()
	{
		inDungeon = true;
	}

    public void ExitDungeon()
    {
        inDungeon = false;
    }

    public void CheckRoom()
	{
		if (!inDungeon) return;

		Vector2Int playerGridPos = DungeonManager.Instance.GetRoomPositionFromWorld(Player.Instance.transform.localPosition);

		if (playerGridPos != currentRoomPos)
		{
			currentRoomPos = playerGridPos;
			MinimapManager.Instance.CreateRoomIcon(playerGridPos);
            MinimapManager.Instance.HighlightPlayer(playerGridPos);
			Debug.Log("Changed Room");
		}
	}
}
