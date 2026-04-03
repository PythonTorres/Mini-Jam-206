using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string nextScene;
    public GameObject settings;
    public GameObject info;
    public GameObject mainMenu;

    void Start() {
        mainMenu.SetActive(true);
        settings.SetActive(false);
        info.SetActive(false);
    }

    public void PlayButton() {
        SceneManager.LoadScene(nextScene);
    }

    public void SettingsButton() {
        settings.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void InfoButton() {
        info.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void QuitButton() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void BackButton() {
        mainMenu.SetActive(true);
        settings.SetActive(false);
        info.SetActive(false);
    }
}
