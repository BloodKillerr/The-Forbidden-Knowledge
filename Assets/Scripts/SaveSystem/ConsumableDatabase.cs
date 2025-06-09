using UnityEngine;

public class ConsumableDatabase : MonoBehaviour
{
    public static ConsumableDatabase Instance { get; private set; }

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

    public Consumable GetByName(string consumableName)
    {
        if (string.IsNullOrEmpty(consumableName)) return null;
        Consumable c = Resources.Load<Consumable>($"Consumables/{consumableName}");
        if (c == null)
            Debug.LogWarning($"[ConsumableDatabase] Missing Resources/Consumables/{consumableName}");
        return c;
    }
}
