using UnityEngine;

public class InventoryTabButton : MonoBehaviour
{
    [SerializeField] private InventoryTab tab;
    public void SwitchTab()
    {
        UIManager.Instance.ChangeInventoryTab(tab);
    }
}
