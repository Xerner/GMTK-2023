using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(MenuHandler))]
public class GlobalVariables : MonoBehaviour
{
    [SerializeField] GameSetting<FullScreenMode> fullscreenSetting;
    [SerializeField] GameSetting<Resolution> resolutionSetting;
    [SerializeField] GameSetting<float> sfxVolumeSetting;
    [SerializeField] GameSetting<float> musicVolumeSetting;
    [SerializeField] GameSetting<Vector3> unitVectorSetting;
    [SerializeField] DialogBoxFactory dialogBoxFactory;
    [SerializeField] GameObject dialogBoxParent;

    public static GameSetting<float> SFXVolumeSetting;
    public static GameSetting<float> MusicVolumeSetting;
    public static GameSetting<Vector3> UnitVectorSetting;

    public static GlobalVariables Singleton;

    void Awake() {
        fullscreenSetting.OnValueChange.AddListener(UpdateResolution);
        resolutionSetting.OnValueChange.AddListener(UpdateResolution);
        SFXVolumeSetting = sfxVolumeSetting;
        MusicVolumeSetting = musicVolumeSetting;
        UnitVectorSetting = unitVectorSetting;

        if (dialogBoxFactory == null) Debug.LogError("No DialogBoxFactory provided", gameObject);
        else {
            DialogBoxFactory.InitializeQuickBoxes(dialogBoxFactory.DialogBoxes);
            if (dialogBoxParent == null) Debug.LogError("No DialogBox Parent GameObject provided", gameObject);
            DialogBoxFactory.DialogBoxesParent = dialogBoxParent;
        } 

        if (Singleton == null) {
            Singleton = this;
        } else if (Singleton != this) {
            Destroy(gameObject);
            return;
        }
    }
    
    public void UpdateResolution(FullScreenMode fullScreenMode) => UpdateResolution(resolutionSetting.Get(), fullScreenMode);

    public void UpdateResolution(Resolution resolution) => UpdateResolution(resolution, fullscreenSetting.Get());

    public void UpdateResolution(Resolution resolution, FullScreenMode fullScreenMode)
    {
        Screen.SetResolution(resolution.width, resolution.height, fullScreenMode, resolution.refreshRateRatio);
    }
}
