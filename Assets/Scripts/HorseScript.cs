using UnityEngine;

public class HorseScript : MonoBehaviour
{
    public AudioSource horseSteps;
    public AudioSource horseVoice;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        horseSteps.Play();
        horseVoice.Play();
    }

}
