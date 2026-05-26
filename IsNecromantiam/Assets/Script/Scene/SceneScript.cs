using UnityEngine;

public class SceneScript : MonoBehaviour
{
    protected GameManager m_GameManager = null;

    protected virtual void Awake()
    {
        m_GameManager = GameManager.GetInstance();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
