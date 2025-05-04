using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StatBlock : MonoBehaviour
{
    [SerializeField] private TMP_Text statText;
    [SerializeField] private Button upgradeButton;

    private string label;
    private Func<string> readout;
    public Action OnUpgrade;

    public void Initialize(string label, Func<string> readout, Action onUpgrade)
    {
        this.label = label;
        this.readout = readout;
        this.OnUpgrade = onUpgrade;

        Refresh();
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(() => onUpgrade());
    }

    public void Refresh()
    {
        statText.text = $"{label}: {readout()}";
    }
}
