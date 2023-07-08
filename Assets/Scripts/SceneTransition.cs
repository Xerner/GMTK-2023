using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string SceneName;

    public void GoToScene()
    {
        Debug.Log($"Transitioning to {SceneName}");
        SceneManager.LoadSceneAsync(SceneName);
    }
}
