using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoadScene : SceneScript
{
    [SerializeField] Image m_Sercle = null;
    [SerializeField] Image m_Bar = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var color = new Color(1f, 1f, 1f, 0f);
        m_Sercle.color = color; color.a = 1f;
        m_Sercle.DOColor(color, 1.0f).OnComplete(() => GameManager.GetInstance().ChangeNextSceneFade());
    }

    // Update is called once per frame
    void Update()
    {
        var rote = m_Sercle.rectTransform.rotation.eulerAngles;
        rote.z -= 180.0f * Time.unscaledDeltaTime;
        m_Sercle.rectTransform.rotation = Quaternion.Euler(rote);

        var load = m_GameManager.LoadingSceneProgress();
        m_Bar.fillAmount = load;
    }
}
