using Unity.VisualScripting;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] SoundStorage m_Storage = null;

    [SerializeField] AudioSource m_Player = null;

    [SerializeField, Range(0.0f, 1.0f)] float m_Volume = 1.0f;

    [SerializeField, Range(0, 1)] float m_SpatialBlend = 1f;

    [SerializeField] bool m_PlayOnAwake = false;
    [SerializeField] bool m_Loop = false;

    private void Awake()
    {
        if (m_Storage == null)
        {
            enabled = false;
            Debug.LogError("SoundStorage is null", this);
            return;
        }

        if (m_Player == null)
            m_Player = this.GetOrAddComponent<AudioSource>();

        m_Player.outputAudioMixerGroup = m_Storage.GetMixerGroup();
        m_Player.volume = m_Volume;
        m_Player.spatialBlend = m_SpatialBlend;
        m_Player.playOnAwake = m_PlayOnAwake;
        m_Player.loop = m_Loop;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public SoundStorage Storage() => m_Storage;

    public AudioSource Player() => m_Player;

    public AudioClip ActiveClip() => m_Player.clip;

    public bool IsPlayClip(AudioClip clip) => m_Player.clip == clip;

    public bool IsPlay() => m_Player.isPlaying;

    public float Volume(float volume) => m_Player.volume = m_Volume = volume;

    public float Volume() => m_Volume = m_Player.volume;

    public bool Loop(bool loop) => m_Player.loop = m_Loop = loop;
    public bool Loop() => m_Loop = m_Player.loop;

    public float SpatialBlend(int blend) => m_Player.spatialBlend = m_SpatialBlend = blend;
    public float SpatialBlend() => m_SpatialBlend = m_Player.spatialBlend;

    #region Play

    /// <summary>
    /// 通常再生
    /// </summary>
    /// <param name="clip">クリップ</param>
    /// <param name="volume">ローカルボリューム</param>
    /// <param name="loop">ループ</param>
    /// <returns></returns>
    public bool Play(AudioClip clip, float volume, bool loop)
    {
        if (clip == null) return false;

        Stop();

        m_Player.clip = clip;
        m_Player.volume = Volume(volume);
        m_Player.loop = Loop(loop);
        m_Player.Play();

        return true;
    }
    /// <summary>
    /// 通常再生
    /// </summary>
    /// <param name="name">クリップ名</param>
    /// <param name="volume">ローカルボリューム</param>
    /// <param name="loop">ループ</param>
    /// <returns></returns>
    public bool Play(string name, float volume, bool loop) => Play(m_Storage[name], volume, loop);

    /// <summary>
    /// 通常再生
    /// </summary>
    /// <param name="clip">クリップ</param>
    /// <param name="volume">ローカルボリューム</param>
    /// <returns></returns>
    public bool Play(AudioClip clip, float volume) => Play(clip, volume, m_Loop);
    /// <summary>
    /// 通常再生
    /// </summary>
    /// <param name="name">クリップ名</param>
    /// <param name="volume">ローカルボリューム</param>
    /// <returns></returns>
    public bool Play(string name, float volume) => Play(name, volume, m_Loop);

    /// <summary>
    /// 通常再生
    /// </summary>
    /// <param name="clip">クリップ</param>
    /// <param name="loop">ループ</param>
    /// <returns></returns>
    public bool Play(AudioClip clip, bool loop) => Play(clip, m_Volume, loop);
    /// <summary>
    /// 通常再生
    /// </summary>
    /// <param name="name">クリップ名</param>
    /// <param name="loop">ループ</param>
    /// <returns></returns>
    public bool Play(string name, bool loop) => Play(name, m_Volume, loop);

    /// <summary>
    /// 通常再生
    /// </summary>
    /// <param name="clip">クリップ</param>
    /// <returns></returns>
    public bool Play(AudioClip clip) => Play(clip, m_Volume, m_Loop);
    /// <summary>
    /// 通常再生
    /// </summary>
    /// <param name="name">クリップ名</param>
    /// <returns></returns>
    public bool Play(string name) => Play(name, m_Volume, m_Loop);

    #endregion

    #region PlayDelay

    /// <summary>
    /// 通常再生（遅延機能持ち）
    /// </summary>
    /// <param name="name">クリップ名</param>
    /// <param name="delay">遅延時間</param>
    /// <param name="volume">ローカルボリューム</param>
    /// <param name="loop">ループ</param>
    /// <returns></returns>
    public bool PlayDelay(string name, float delay, float volume, bool loop)
    {
        var clip = m_Storage[name]; if (clip == null) return false;
        Stop();

        m_Player.clip = clip;
        m_Player.volume = Volume(volume);
        m_Player.loop = Loop(loop);
        m_Player.PlayDelayed(delay);

        return true;
    }

    /// <summary>
    /// 通常再生（遅延機能持ち）
    /// </summary>
    /// <param name="name">クリップ名</param>
    /// <param name="delay">遅延時間</param>
    /// <param name="volume">ローカルボリューム</param>
    /// <returns></returns>
    public bool PlayDelay(string name, float delay, float volume) => PlayDelay(name, delay, volume, m_Loop);
    /// <summary>
    /// 通常再生（遅延機能持ち）
    /// </summary>
    /// <param name="name">クリップ名</param>
    /// <param name="delay">遅延時間</param>
    /// <param name="loop">ループ</param>
    /// <returns></returns>
    public bool PlayDelay(string name, float delay, bool loop) => PlayDelay(name, delay, m_Volume, loop);
    /// <summary>
    /// 通常再生（遅延機能持ち）
    /// </summary>
    /// <param name="name">クリップ名</param>
    /// <param name="delay">遅延時間</param>
    /// <returns></returns>
    public bool PlayDelay(string name, float delay) => PlayDelay(name, delay, m_Volume, m_Loop);

    #endregion

    #region PlayOnShot

    /// <summary>
    /// 重複可能再生
    /// </summary>
    /// <param name="name">クリップ名</param>
    /// <param name="volume">ローカルボリューム</param>
    /// <param name="stop">再生中のものを止める</param>
    /// <returns></returns>
    public bool PlayOnShot(string name, float volume, bool stop)
    {
        var clip = m_Storage[name]; if (clip == null) return false;

        if (stop) m_Player.Stop();

        //m_Player.loop = Loop(loop);
        m_Player.PlayOneShot(clip, Volume(volume));

        return true;
    }

    /// <summary>
    /// 重複可能再生
    /// </summary>
    /// <param name="name">クリップ名</param>
    /// <param name="volume">ローカルボリューム</param>
    /// <returns></returns>
    public bool PlayOnShot(string name, float volume) => PlayOnShot(name, volume, false);
    /// <summary>
    /// 重複可能再生
    /// </summary>
    /// <param name="name">クリップ名</param>
    /// <param name="stop">再生中のものを止める</param>
    /// <returns></returns>
    public bool PlayOnShot(string name, bool stop) => PlayOnShot(name, m_Volume, stop);
    /// <summary>
    /// 重複可能再生
    /// </summary>
    /// <param name="name">クリップ名</param>
    /// <returns></returns>
    public bool PlayOnShot(string name) => PlayOnShot(name, m_Volume, false);

    #endregion

    #region PlayClipAtPoint

    /// <summary>
    /// 座標指定再生
    /// </summary>
    /// <param name="name">クリップ名</param>
    /// <param name="position">再生位置</param>
    /// <param name="volume">ローカルボリューム</param>
    /// <returns></returns>
    public bool PlayClipAtPoint(string name, Vector3 position, float volume)
    {
        var clip = m_Storage[name]; if (clip == null) return false;

        //AudioSource.PlayClipAtPoint(clip, position, volume);
        // ↓実装はほぼコピペ
        {
            GameObject gameObject = new GameObject("One shot audio");
            gameObject.transform.position = position;
            AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
            audioSource.clip = clip;
            audioSource.spatialBlend = 1f;
            audioSource.volume = volume;
            audioSource.outputAudioMixerGroup = m_Player.outputAudioMixerGroup; // ここ追加
            audioSource.Play();
            Object.Destroy(gameObject, clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
        }

        return true;
    }

    /// <summary>
    /// 座標指定再生
    /// </summary>
    /// <param name="name">クリップ名</param>
    /// <param name="position">再生位置</param>
    /// <returns></returns>
    public bool PlayClipAtPoint(string name, Vector3 position) => PlayClipAtPoint(name, position, m_Volume);
    /// <summary>
    /// 座標指定再生
    /// </summary>
    /// <param name="clip">クリップ名</param>
    /// <param name="volume">ローカルボリューム</param>
    /// <returns></returns>
    public bool PlayClipAtPoint(string clip, float volume) => PlayClipAtPoint(clip, transform.position, volume);
    /// <summary>
    /// 座標指定再生
    /// </summary>
    /// <param name="clip">クリップ名</param>
    /// <returns></returns>
    public bool PlayClipAtPoint(string clip) => PlayClipAtPoint(clip, transform.position, m_Volume);

    #endregion

    public void Stop() => m_Player.Stop();

    public void Pause() => m_Player.Pause();
}
