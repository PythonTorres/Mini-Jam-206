using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Range(0f, 1f)] public float volume = 0.5f;

    public AudioSource _source;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        _source.loop = true;
        _source.playOnAwake = false;

        ApplyVolume(volume);
        _source.Play();
    }

    public void ApplyVolume(float val)
    {
        Debug.Log("volume " + val);
        AudioListener.volume = val;  // глобальная громкость — влияет на всё в игре
    }
}