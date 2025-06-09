using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public Item GetByName(string itemName)
    {
        if (string.IsNullOrEmpty(itemName)) return null;
        Item item = Resources.Load<Item>($"Items/{itemName}");
        if (item == null)
            Debug.LogWarning($"[ItemDatabase] Missing Resources/Items/{itemName}");
        return item;
    }
}
