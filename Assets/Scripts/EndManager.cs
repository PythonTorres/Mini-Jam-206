using UnityEngine;
using UnityEngine.SceneManagement;

public class EndManager : MonoBehaviour
{
    public void goBackMain() {
        SceneManager.LoadScene(0);
    }
}
