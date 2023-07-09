using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Dialog Box", menuName = "UI/Dialog Box")]
public class DialogBox : ScriptableObject {
    //public static DialogBox Singleton;
    [SerializeField] 
    new string name;
    [Description("The root GameObject of the prefab should have a DialogBoxBehaviour component")]
    public DialogBoxBehaviour Prefab;
    public Sprite Frame;
    public GameSetting<Sprite> FrameSetting;
    [Space]

    public Color TextColor;
    public Color TextShadowColor;

    public string Name { get => name; }
}