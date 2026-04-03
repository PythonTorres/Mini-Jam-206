using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [System.Serializable]
    public class SoundEntry
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    public SoundEntry[] sounds;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Play(string soundName)
    {
        SoundEntry entry = System.Array.Find(sounds, s => s.name == soundName);
        if (entry == null)
        {
            Debug.LogWarning($"SoundManager: sound '{soundName}' not found.");
            return;
        }
        AudioSource.PlayClipAtPoint(entry.clip, Camera.main.transform.position, entry.volume * AudioListener.volume);
    }
}