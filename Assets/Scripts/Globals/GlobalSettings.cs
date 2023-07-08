using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(MenuHandlerSingleton))]
// Just adding other components that need to exist globally as required components
[RequireComponent(typeof(DialogBoxFactory))]
public class GlobalSettings : MonoBehaviour
{
    [SerializeField] GameSetting<FullScreenMode> fullscreenSetting;
    [SerializeField] GameSetting<Resolution> resolutionSetting;
    [SerializeField] GameSetting<float> sfxVolumeSetting;
    [SerializeField] GameSetting<float> musicVolumeSetting;

    public static GameSetting<float> SFXVolumeSetting;
    public static GameSetting<float> MusicVolumeSetting;

    public static GlobalSettings Singleton;

    void Awake() {
        fullscreenSetting.OnValueChange.AddListener(UpdateResolution);
        resolutionSetting.OnValueChange.AddListener(UpdateResolution);
        SFXVolumeSetting = sfxVolumeSetting;
        MusicVolumeSetting = musicVolumeSetting;

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
        Application.targetFrameRate = 60;
        Screen.SetResolution(resolution.width, resolution.height, fullScreenMode);
    }
}
