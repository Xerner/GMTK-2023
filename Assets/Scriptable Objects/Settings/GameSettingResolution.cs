using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Resolution Setting", menuName = "Settings/Resolution")]
public class GameSettingResolution : GameSettingPlayerPrefArray<Resolution> {
    private Dictionary<string, Resolution> savedResolutions = new();

    public override List<InputValue<Resolution>> Choices {
        get {
            SyncChoicesToResolution();
            return choices; 
        }
    }

    protected override void OnValidate() {
        base.OnValidate();
        //SyncChoicesToResolution();
    }

    private void Awake() {
        SyncChoicesToResolution();
    }

    protected void SyncChoicesToResolution() {

        choices.Clear();
        savedResolutions.Clear();
        // Get unique resolutions without considering refresh rate
        foreach (Resolution resolution in Screen.resolutions) {
            string key = $"{resolution.width} x {resolution.height}";
            if (!savedResolutions.ContainsKey(key))
                savedResolutions.Add(key, resolution);
        }
        // Add unique resolutions to a List because its serializable and can be shown in editor
        foreach (var pair in savedResolutions)
        {
            choices.Add(new InputValue<Resolution>(pair.Key, pair.Value));
        }
    }
}