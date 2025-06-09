using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistBetweenScenes : MonoBehaviour
{
    public int[] allowedScenes = { 1, 2, 3, 4 };

    public static PersistBetweenScenes Instance { get; private set; }

    private void Awake()
    {
        int idx = SceneManager.GetActiveScene().buildIndex;

        if (!allowedScenes.Contains(idx))
        {
            Destroy(gameObject);
            return;
        }

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!allowedScenes.Contains(scene.buildIndex) && Instance == this)
        {
            Destroy(gameObject);
            Instance = null;
        }

        if ((scene.buildIndex == allowedScenes[0] || scene.buildIndex == allowedScenes[1])
        && !SaveManager.IsLoadingSave)
        {
            GameManager.Instance.ResetPlayer();
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
