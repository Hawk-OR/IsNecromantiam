using UnityEngine;

public class PlayScene : SceneScript
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.GetAction("Start").performed += _ => m_GameManager.ChangeSceneFade("TitleScene", 1);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
