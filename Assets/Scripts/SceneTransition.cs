using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string SceneName;

    public void GoToScene()
    {
        SceneManager.LoadSceneAsync(SceneName);
    }
}
