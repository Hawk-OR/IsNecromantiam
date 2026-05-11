using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerController
{
    private AudioMixer m_Mixer = null;

    public AudioMixerController(AudioMixer mixer)
    {
        m_Mixer = mixer;
    }

    public void SetAudioMixer(AudioMixer mixer)
    {
        m_Mixer = mixer;
    }

    /// <summary>
    /// ミキサーに値を入れる
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value">0.0 ～ 1.0</param>
    public void SetFloat(string name, float value)
    {
        if (m_Mixer != null)
            m_Mixer.SetFloat(name, ToMixer(value));
        else
            Debug.LogError("Not Found Mixer");
    }

    /// <summary>
    /// ミキサーの値をとる
    /// </summary>
    /// <param name="name"></param>
    /// <returns>0.0 ～ 1.0 </returns>
    public float GetFloat(string name)
    {
        m_Mixer.GetFloat(name, out float volume);
        return ToVolume(volume);
    }

    /// <summary>
    /// ラウドネス(人間の耳を基準にした音量指標)を考慮してミキサー値に変換する
    /// </summary>
    /// <param name="volume">0.0 ～ 1.0</param>
    /// <returns>-80.0 ～ 0.0</returns>
    private float ToMixer(float volume)
    {
        if (volume <= 0.0f)
            return -80.0f; // ゼロ除算防止のため事前に分岐
        else if (volume >= 1.0f)
            return 0.0f;
        else
            return 10.0f * Mathf.Log(volume, 2.0f);
        // オプション画面のボリューム設定に関するプラクティス _ manicreator.com
        // https://manicreator.com/articles/volume-options/
        // https://github.com/Manicreator/VolumeOptions/blob/master/Assets/Scripts/SoundUtil.cs
    }

    /// <summary>
    /// ラウドネスを考慮してボリューム値に変換する
    /// </summary>
    /// <param name="mixer">-80.0 ～ 0.0</param>
    /// <returns>0.0 ～ 1.0</returns>
    private float ToVolume(float mixer)
    {
        if (mixer <= -80.0f)
            return 0.0f; // 最小値を特別扱い
        else if (mixer >= 0.0f)
            return 1.0f;
        else
            return Mathf.Pow(2.0f, mixer / 10.0f);
    }
}
