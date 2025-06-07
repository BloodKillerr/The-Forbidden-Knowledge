using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LevelingManager : MonoBehaviour
{
    [SerializeField] private int statPoints = 0;

    [SerializeField] private int currentXP = 0;

    [SerializeField] private int requiredXP = 400;

    public int StatPoints { get => statPoints; set => statPoints = value; }
    public int StatPoints1 { get => statPoints; set => statPoints = value; }
    public int CurrentXP { get => currentXP; set => currentXP = value; }
    public int RequiredXP { get => requiredXP; set => requiredXP = value; }

    public UnityEvent<int, int> XPChanged = new UnityEvent<int, int>();
    public UnityEvent<int> StatPointsChanged = new UnityEvent<int>();

    public static LevelingManager Instance { get; private set; }

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

        XPChanged.Invoke(currentXP, requiredXP);

        if (currentXP >= requiredXP)
        {
            statPoints++;
            StatPointsChanged.Invoke(statPoints);

            int remainder = currentXP - requiredXP;
            currentXP = remainder;
            requiredXP = (int)((requiredXP + 50) * 1.05f);

            XPChanged.Invoke(currentXP, requiredXP);
        }
    }

    public bool UseStatPoint()
    {
        if(statPoints > 0)
        {
            statPoints--;
            StatPointsChanged.Invoke(statPoints);
            return true;
        }
        return false;
    }

    public LevelingData GetLevelingData()
    {
        return new LevelingData
        {
            statPoints = this.statPoints,
            currentXP = this.currentXP,
            requiredXP = this.requiredXP
        };
    }

    public void ApplyLevelingData(LevelingData data)
    {
        if (data == null)
        {
            return;
        }

        statPoints = data.statPoints;
        currentXP = data.currentXP;
        requiredXP = data.requiredXP;

        XPChanged?.Invoke(currentXP, requiredXP);
        StatPointsChanged?.Invoke(statPoints);
    }
}
