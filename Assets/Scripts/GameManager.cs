using UnityEngine;

public class GameManager : MonoBehaviour
{
    public void loadScene(string scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }

    public void loadScene(int index)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(index);
    }

    public void quit()
    {
        Application.Quit();
    }
}
