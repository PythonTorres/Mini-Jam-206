using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public List<FadeAppearScript> tutors;
    public string nextScene;
    int i = 0;

    void Start() {
        foreach (var t in tutors) {
            t.turnOff();
        }
    }

    public void ShowNext() {
        tutors[i].turnOn();
        i++;
    }

    public void PlayButton() {
        SceneManager.LoadScene(nextScene);
    }
}
