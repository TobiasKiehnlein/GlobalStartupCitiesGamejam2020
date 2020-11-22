using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public void ChangeScene(int sceneId)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneId);
    }

    public void Quit()
    {
        Application.Quit();
    }
}