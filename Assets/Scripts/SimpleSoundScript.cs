using UnityEngine;

public class SimpleSoundScript : MonoBehaviour, IExecutable
{
    AudioSource a;
    bool playing = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        a = GetComponent<AudioSource>();
    }

    public void Exec(float val) {
        if (playing) {
            a.Stop();
            playing = false;
        } else {
            a.volume = val;
            a.Play();
            playing = true;
        }
    }
}
