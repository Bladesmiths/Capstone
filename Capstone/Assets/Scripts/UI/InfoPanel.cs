using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Bladesmiths.Capstone.UI;

public class InfoPanel : SerializedMonoBehaviour
{
    public Image infoImage;
    public TextMeshProUGUI infoText;

    [TitleGroup("Tutorial Text")]
    [HorizontalGroup("Tutorial Text/FirstRow")]
    [BoxGroup("Tutorial Text/FirstRow/Tutorial Text")] [LabelWidth(70)]

    [OdinSerialize]
    Dictionary<int, string> infoTextDictionary = new Dictionary<int, string>();
    Dictionary<string, Sprite> infoImageDictionary = new Dictionary<string, Sprite>();

    [SerializeField] UIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        infoImageDictionary = uiManager.xboxInputs;

        infoImage = transform.GetChild(0).gameObject.GetComponent<Image>();
        infoText = transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Modify info panel image and text to display information
    public void SetInfoUI(int actionIndex, int textIndex)
    {
        infoImage.enabled = true;
        infoText.enabled = true;

        infoImage.sprite = infoImageDictionary["a"];
        infoText.text = " - " + infoTextDictionary[textIndex];
    }

    //Hide info panel when not in use
    public void ClearInfoUI()
    { 
        infoImage.sprite = null;
        infoText.text = "";

        infoImage.enabled = false;
        infoText.enabled = false;
    }
}
