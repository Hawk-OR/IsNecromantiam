using UnityEngine;

public class TitleScene : MonoBehaviour
{
    public void Awake()
    {
        Debug.Log(GameManager.Instance());
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
}
