using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class MenuBehaviour : MonoBehaviour
{
    [SerializeField] string menuName = "A menu";
    [Description("Menu's that are not first will be deactivated")]
    public bool isFirstMenu = false;
    Canvas canvas;

    public string MenuName { get => menuName; }

    void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    protected void Start() {
        MenuHandlerSingleton.Singleton.Menus.Add(this);
        if (isFirstMenu) 
            MenuHandlerSingleton.Singleton.ChangeMenuInstantly(menuName);
        else 
            gameObject.SetActive(false);
    }

    protected void OnDestroy() {
        MenuHandlerSingleton.Singleton.Menus.Remove(this);
    }

    public virtual Selectable GetFirstSelectable() {
        return canvas.transform.FindFirst<Selectable>();
    }

    public void SelectFirstSelectable() {
        Selectable selectable = GetFirstSelectable();
        if (selectable == null) {
            Debug.LogError("Could not find a selectable in the Canvas " + canvas.gameObject.name);
            return;
        }
        AudioSourcesToggleMute(selectable.gameObject, true);
        selectable.Select(); // selecting is just navigating
        AudioSourcesToggleMute(selectable.gameObject, false);
    }

    void AudioSourcesToggleMute(GameObject gameObject, bool? value = null) {
        AudioSource[] sources = gameObject.GetComponents<AudioSource>();
        foreach (var source in sources) {
            if (value.HasValue)
                source.mute = value.Value;
            else
                source.mute = !source.mute;
        }
    }

    public void ChangeMenu(string name) => MenuHandlerSingleton.Singleton.ChangeMenu(name);

    public void CloseMenu() => MenuHandlerSingleton.Singleton.CloseMenu(this);
    public void OpenMenu() => MenuHandlerSingleton.Singleton.ChangeMenu(menuName);
}
