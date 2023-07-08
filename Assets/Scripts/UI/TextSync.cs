using EasyButtons;
using MarkupAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSync : MonoBehaviour
{
    public ESource sourceType;
    private bool UsingTextMeshPro => sourceType == ESource.TextMeshPro;
    private bool UsingSlider => sourceType == ESource.Slider;

    private MonoBehaviour source;
    
    public List<TMPro.TextMeshProUGUI> textsToSync;


    [Header("TextMeshPro")]
    public TMPro.TextMeshProUGUI textSource;
    public bool syncText = true;
    public bool syncFontSize = true;
    [Description(@"Syncs the text values between two Text Mesh Pro components
    
If empty, defaults to the first childs Text Mesh Pro component (if it has one)")]
    public bool syncMargin = true;
    [EndGroup]

    [Header("Slider")]
    public Slider sliderSource;

    void OnValidate()
    {
        switch (sourceType)
        {
            case ESource.Slider:
                sliderSource = GetComponent<Slider>();
                source = sliderSource;
                break;
            default:
                textSource = GetComponent<TMPro.TextMeshProUGUI>();
                source = textSource;
                break;
        }
        if (source == null) Debug.LogError("No valid text source found. Please provide one");
        SyncAll();
    }

    void Update()
    {
        SyncAll();
    }

    [Button]
    public void SyncAll()
    {
        if (source == null) return;
        foreach (TMPro.TextMeshProUGUI textToSync in textsToSync)
        {
            Sync(textToSync);
        }
    }

    public void Sync(TMPro.TextMeshProUGUI textToSync)
    {
        if (source == null) return;
        if (textToSync == null) return;
        switch (sourceType)
        {
            case ESource.Slider:
                Slider slider = (Slider)source;
                textToSync.text = slider.value.ToString();
                break;
            default:
                // TextMeshProUGUI source
                TMPro.TextMeshProUGUI text = (TMPro.TextMeshProUGUI)source;
                if (syncText) textToSync.text = text.text;
                if (syncFontSize) textToSync.fontSize = text.fontSize;
                if (syncMargin) textToSync.margin = text.margin;
                break;
        }
    }

    public enum ESource
    {
        TextMeshPro,
        Slider,
    }
}
