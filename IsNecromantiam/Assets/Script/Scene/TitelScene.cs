using UnityEngine;

public class TitleScene : MonoBehaviour
{
    public void Awake()
    {
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
        GameManager.Instance().EndGame();
    }

    public void ChangeScene(string sceneName)
    {
        GameManager.Instance().ChangeSceneLoadFade(sceneName, 1.0f);
    }
}
