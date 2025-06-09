using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour
{
	[SerializeField] private GameObject minimapIconPrefab;
	[SerializeField] private Transform minimapParent;
	[SerializeField] private RectTransform minimapBounds;
	[SerializeField] private int iconSpacing = 25;

	private Vector2Int playerRoomPosition;

	public static MinimapManager Instance { get; private set; }

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

	private Dictionary<Vector2Int, GameObject> roomIcons = new Dictionary<Vector2Int, GameObject>();

	private bool IsInsideMinimapBounds(Vector2 anchoredPosition)
	{
		Vector2 halfSize = minimapBounds.rect.size / 2f;

		return Mathf.Abs(anchoredPosition.x) <= halfSize.x &&
			   Mathf.Abs(anchoredPosition.y) <= halfSize.y;
	}

	public void CreateRoomIcon(Vector2Int position)
	{
		if (roomIcons.ContainsKey(position)) return;

		GameObject icon = Instantiate(minimapIconPrefab, minimapParent);
		icon.GetComponent<RectTransform>().localScale = Vector3.one;
		icon.GetComponent<Image>().color = Color.white;

		roomIcons[position] = icon;

		if (playerRoomPosition != null)
		{
			Vector2Int relativePos = position - playerRoomPosition;
			icon.GetComponent<RectTransform>().anchoredPosition = relativePos * iconSpacing;
		}
	}

	public void HighlightPlayer(Vector2Int currentRoom)
	{
		playerRoomPosition = currentRoom;

		foreach (var kvp in roomIcons)
		{
			Vector2Int relativePos = kvp.Key - playerRoomPosition;
			Vector2 anchoredPos = relativePos * iconSpacing;

			bool inside = IsInsideMinimapBounds(anchoredPos);
			kvp.Value.SetActive(inside);

			if (inside)
			{
				kvp.Value.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
				kvp.Value.GetComponent<Image>().color = Color.white;
			}
		}

		if (roomIcons.TryGetValue(currentRoom, out GameObject playerIcon))
		{
			playerIcon.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			playerIcon.GetComponent<Image>().color = Color.yellow;
		}
	}


	public void ClearMinimap()
	{
		foreach (GameObject icon in roomIcons.Values)
		{
			Destroy(icon);
		}
		roomIcons.Clear();
	}

    public List<Vector2Int> GetVisitedRoomPositions()
    {
        return roomIcons.Keys.ToList();
    }

    public void SetVisitedRoomPositions(List<Vector2Int> positions)
    {
        ClearMinimap();
        foreach (Vector2Int pos in positions)
            CreateRoomIcon(pos);
    }
}
