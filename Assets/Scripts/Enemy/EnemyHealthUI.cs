using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text nameText;

    private EnemyStats stats;
    private Camera mainCam;

    public void Initialize(EnemyStats stats)
    {
        this.stats = stats;
        nameText.text = stats.CharacterName;
        healthSlider.maxValue = stats.MaxHealth.GetValue();
        healthSlider.value = stats.CurrentHealth;

        stats.HealthChanged.AddListener(OnHealthChanged);

        mainCam = Camera.main;
    }

    private void OnDestroy()
    {
        if (stats != null)
        {
            stats.HealthChanged.RemoveListener(OnHealthChanged);
        }
    }

    private void OnHealthChanged(int current, int max)
    {
        healthSlider.maxValue = max;
        healthSlider.value = current;
    }

    private void LateUpdate()
    {
        if (stats != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
        }
    }
}
