using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    [SerializeField] Image m_Image = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var color = m_Image.color; color.a = 1.0f;
        m_Image.DOColor(color, 1.0f).OnComplete(() => GameManager.Instance().ChangeNextSceneFade());
    }

    // Update is called once per frame
    void Update()
    {
        var rote = m_Image.rectTransform.rotation.eulerAngles;
        rote.z -= 180.0f * Time.unscaledDeltaTime;
        m_Image.rectTransform.rotation = Quaternion.Euler(rote);
    }
}
