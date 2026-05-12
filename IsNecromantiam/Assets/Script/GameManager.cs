using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager m_Instance = null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Initialize()
    {
        GameObject obj = new GameObject("GameManager");
        m_Instance = obj.AddComponent<GameManager>();

        if (m_Instance == null)
        {
            m_Instance = new GameManager();
            DontDestroyOnLoad(m_Instance);
        }
        else
        {
            Destroy(m_Instance);
        }
    }

    public static GameManager Instance()
    {
        return m_Instance;
    }

    void Awake()
    {
        if (m_Instance == null) DontDestroyOnLoad(gameObject);
        else Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
