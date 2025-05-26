using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool isGameStatePaused = false;

    public bool IsGameStatePaused { get => isGameStatePaused; set => isGameStatePaused = value; }

    public static GameManager Instance { get; private set; }

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
        ResumeGameState();
    }

    public void PauseGameState()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isGameStatePaused = true;
    }

    public void ResumeGameState()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isGameStatePaused = false;
    }

    public void ResetPlayer()
    {
        Player player = Player.Instance;
        PlayerStats stats = player.GetComponent<PlayerStats>();

        int maxHp = stats.MaxHealth.GetValue();
        stats.CurrentHealth = maxHp;
        stats.HealthChanged?.Invoke(stats.CurrentHealth, maxHp);

        int maxDodges = stats.MaxDodgeCharges.GetValue();
        stats.CurrentDodgeCharges = maxDodges;
        stats.DodgeChargesChanged?.Invoke(maxDodges, maxDodges);

        ConsumableManager consumableManager = ConsumableManager.Instance;
        SpellManager spellManager = SpellManager.Instance;

        consumableManager.ResetCooldowns();
        spellManager.ResetCooldowns();

        UIManager.Instance.RefreshQuickbarUI();

        UIManager.Instance.ClearMessages();

        PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();
        playerAttack.ResetAttackState();

        player.IsDead = false;

        PlayerAnimatorHandler animHandler = player.GetComponentInChildren<PlayerAnimatorHandler>();
        if (animHandler != null && animHandler.Animator != null)
        {
            animHandler.Animator.SetFloat("Vertical", 0f);
            animHandler.Animator.SetFloat("Horizontal", 0f);
            animHandler.CanRotate = true;
        }

        Player.Instance.GetComponent<PlayerTracker>().ExitDungeon();
        MinimapManager.Instance.ClearMinimap();
    }
}
