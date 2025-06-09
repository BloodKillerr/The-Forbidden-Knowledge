using UnityEngine;

public class SpellDatabase : MonoBehaviour
{
    public static SpellDatabase Instance { get; private set; }

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

    public Spell GetByName(string spellName)
    {
        if (string.IsNullOrEmpty(spellName)) return null;
        Spell s = Resources.Load<Spell>($"Spells/{spellName}");
        if (s == null)
            Debug.LogWarning($"[SpellDatabase] Missing Resources/Spells/{spellName}");
        return s;
    }
}
