using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string SceneName;
    public bool AutoSubscribe = true;

    private void Awake()
    {
        if (AutoSubscribe)
        {
            //transform.find
        }
    }

    public void GoToScene()
    {
        Debug.Log($"Transitioning to {SceneName}");
        SceneManager.LoadSceneAsync(SceneName);
    }
}
