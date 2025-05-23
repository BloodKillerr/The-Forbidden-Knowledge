using UnityEngine;
using UnityEngine.EventSystems;

public class LevelingManager : MonoBehaviour
{
    [SerializeField] private int statPoints = 0;

    [SerializeField] private int currentXP = 0;

    [SerializeField] private int requiredXP = 100;

    public static LevelingManager Instance { get; private set; }
    public int StatPoints { get => statPoints; set => statPoints = value; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void GainXP(int xp)
    {
        currentXP += xp;

        if(currentXP >= requiredXP)
        {
            statPoints++;

            int remainder = currentXP - requiredXP;
            currentXP = remainder;
            requiredXP = (int)((requiredXP + 59) * 1.05f);
            UIManager.Instance.UpdateStatPointsText();
        }
    }

    public bool UseStatPoint()
    {
        if(statPoints > 0)
        {
            statPoints--;
            UIManager.Instance.UpdateStatPointsText();
            return true;
        }
        return false;
    }
}
