using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager m_Instance = null;

    private Image m_Image = null;

    private AsyncOperation m_AsyncOperation = null;
    private string m_NextScene = "TitleScene";

    private static PlayerInput m_InputAction = null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Initialize()
    {
        //  create GameManager
        GameObject obj = new GameObject("GameManager");
        m_Instance = obj.AddComponent<GameManager>();

        //  create input action
        var input = obj.AddComponent<PlayerInput>();
        {
            input.actions = Resources.Load<InputActionAsset>("InputActions");
            input.defaultActionMap = "Player";
            input.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;

            //  enable all actions
            foreach (var action in input.actions) action.Enable();

            m_InputAction = input;
        }

        DontDestroyOnLoad(obj);
    }

    public static GameManager GetInstance() => m_Instance;

    public static PlayerInput GetInputActions()
    {
        if (m_InputAction == null) m_InputAction = m_Instance.GetOrAddComponent<PlayerInput>();

        return m_InputAction;
    }

    public static InputAction GetAction(string name, bool throwIfNotFound = true) => GetInputActions().actions.FindAction(name, throwIfNotFound);

    void Awake()
    {
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //  create fade image
        m_Image = CreateImage();

        GetInputActions().actions["Quit"].performed += __ => EndGame();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public float LoadingSceneProgress() => m_AsyncOperation != null ? m_AsyncOperation.progress : 0.0f;

    public void ChangeScene(string sceneName, float second)
    {
        StartCoroutine(ChangeSceneWait(sceneName, second));
    }

    public void ChangeSceneFade(string sceneName, float fadeTime)
    {
        Fade(true, fadeTime);
        StartCoroutine(ChangeSceneWaitFadeOut(sceneName, fadeTime + 0.1f, fadeTime));
    }

    public void ChangeSceneLoadFade(string sceneName, float fadeTime)
    {
        Fade(true, fadeTime);
        StartCoroutine(ChangeSceneWait("LoadScene", fadeTime + 0.1f));

        m_NextScene = sceneName;
    }

    public void ChangeNextSceneFade()
    {
        StartCoroutine(ChangeSceneWaitFadeOut(m_NextScene, 1.0f, 1.0f));
    }

    private IEnumerator ChangeSceneWait(string sceneName, float second = 1.0f)
    {
        m_AsyncOperation = SceneManager.LoadSceneAsync(sceneName);
        m_AsyncOperation.allowSceneActivation = false;

        yield return new WaitForSeconds(second);
        while (m_AsyncOperation.progress < 0.90f) yield return null;

        m_AsyncOperation.allowSceneActivation = true;
        m_AsyncOperation = null;
    }

    private IEnumerator ChangeSceneWaitFadeOut(string sceneName, float waitTime, float fadeTime)
    {
        m_AsyncOperation = SceneManager.LoadSceneAsync(sceneName);
        m_AsyncOperation.allowSceneActivation = false;

        yield return new WaitForSeconds(waitTime);
        while (m_AsyncOperation.progress < 0.90f) yield return null;


        m_AsyncOperation.allowSceneActivation = true;
        m_AsyncOperation = null;

        Fade(false, fadeTime);
    }

    public void Fade(bool fade, float fadeTime = 1.0f)
    {
        Time.timeScale = 0.0f;

        var color = m_Image.color;

        var start = color; start.a = fade ? 0.0f : 1.0f;
        var end = color; end.a = fade ? 1.0f : 0.0f;

        m_Image.color = start;
        m_Image.DOColor(end, fadeTime).SetUpdate(true).OnComplete(() => Time.timeScale = 1.0f);
    }

    private IEnumerator FadeWait(bool fade, float fadeTime, float second = 1.0f)
    {
        yield return new WaitForSeconds(second);

        Fade(fade, fadeTime);
    }

    private Image CreateImage()
    {
        //  create canvas
        var obj = new GameObject("Canvas");
        {
            //  canvas in the This
            obj.transform.parent = transform;

            var canvas = obj.AddComponent<Canvas>();
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 1024;
            }
            var scaler = obj.AddComponent<CanvasScaler>();
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new(1920, 1080);
            }

            //  create image
            var imageObj = new GameObject("Image");
            {
                //  image in the Canvas
                imageObj.transform.parent = obj.transform;

                var image = imageObj.AddComponent<Image>();
                {
                    image.color = new(0.0f, 0.0f, 0.0f, 0.0f);

                    var rect = image.rectTransform;
                    {
                        rect.sizeDelta = new(1920, 1080);
                        rect.localPosition = Vector3.zero;
                    }
                }

                return image;
            }
        }
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
