using UnityEngine;

public class EquipmentDatabase : MonoBehaviour
{
    public static EquipmentDatabase Instance { get; private set; }

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

    public Equipment GetByName(string equipmentName)
    {
        if (string.IsNullOrEmpty(equipmentName)) return null;
        Equipment eq = Resources.Load<Equipment>($"Equipment/{equipmentName}");
        if (eq == null)
            Debug.LogWarning($"[EquipmentDatabase] Missing Resources/Equipment/{equipmentName}");
        return eq;
    }
}
