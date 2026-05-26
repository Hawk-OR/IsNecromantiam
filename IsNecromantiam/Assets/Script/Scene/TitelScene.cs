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
        GameManager.GetInstance().EndGame();
    }

    public void ChangeScene(string sceneName)
    {
        GameManager.GetInstance().ChangeSceneLoadFade(sceneName, 1.0f);
    }
}
