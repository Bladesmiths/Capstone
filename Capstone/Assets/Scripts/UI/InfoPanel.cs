using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine.InputSystem;
using Bladesmiths.Capstone.UI;
using Bladesmiths.Capstone;
using System;

public class InfoPanel : SerializedMonoBehaviour
{
    public Image infoImage;
    public Image infoImageLL;
    public Image infoImageL;
    public Image infoImageR;
    public Image infoImageU;

    public TextMeshProUGUI infoText;
    public TextMeshProUGUI infoTextPlusL;
    public TextMeshProUGUI infoTextPlusR;

    [SerializeField] BreakableBox firstCluster;
    [SerializeField] LastBreakableBox finalCluster;

    [TitleGroup("Tutorial Text")]
    [HorizontalGroup("Tutorial Text/FirstRow")]
    [BoxGroup("Tutorial Text/FirstRow/Tutorial Text")] [LabelWidth(70)]

    [OdinSerialize]
    Dictionary<int, string> infoTextDictionary = new Dictionary<int, string>();

    [SerializeField] UIManager uiManager;
    [SerializeField] PlayerInput playerInputs;

    private Dictionary<string, Sprite> xboxInputs;
    private Dictionary<string, Sprite> kbmInputs;

    private int currentTextIndex;
    private string currentInputKBM;
    private string currentInputGamepad;

    // Start is called before the first frame update
    void Start()
    {
        firstCluster = GameObject.Find("Breakable Gem Cluster").GetComponent<BreakableBox>();
        finalCluster = GameObject.Find("Breakable Gem Cluster (7)").GetComponent<LastBreakableBox>();

        playerInputs = uiManager.Inputs;
        xboxInputs = uiManager.xboxInputs;
        kbmInputs = uiManager.kbmInputs;
        ChooseControlSchemeIcons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //Updates displayed info icons if control device changes
    //Called via OnControlsChanged in playerInputsScript
    public void ChooseControlSchemeIcons()
    {
        if (currentInputGamepad != null && currentInputKBM != null)
        {
            SetInfoUI(currentInputGamepad, currentInputKBM, currentTextIndex);
        }
    }

    //Modify info panel image and text to display information
    public void SetInfoUI(string gamepadIconIndex, string kbmIconIndex, int textIndex)
    {
        infoImage.enabled = true;
        infoText.enabled = true;

        infoTextPlusL.enabled = false;
        infoTextPlusR.enabled = false;

        infoImageLL.enabled = false;
        infoImageL.enabled = false;
        infoImageR.enabled = false;
        infoImageU.enabled = false;

        //Set image based on current input device
        switch (playerInputs.currentControlScheme)
        {
            case "KeyboardMouse":
                infoImage.sprite = kbmInputs[kbmIconIndex];

                //Display WASD
                if (kbmIconIndex == "s" || kbmIconIndex == "space")
                {
                    infoImageL.enabled = true;
                    infoImageL.sprite = kbmInputs["a"];

                    infoImageR.enabled = true;
                    infoImageR.sprite = kbmInputs["d"];

                    infoImageU.enabled = true;
                    infoImageU.sprite = kbmInputs["w"];
                }
                
                //Display key + WASD
                if (kbmIconIndex == "space")
                {
                    infoImageLL.enabled = true;
                    infoImageLL.sprite = kbmInputs["space"];

                    infoTextPlusL.enabled = true;

                    infoImage.sprite = kbmInputs["s"];
                }

                //Display key + move mouse
                else if (kbmIconIndex == "shift")
                {
                    infoTextPlusR.enabled = true;

                    infoImageR.enabled = true;
                    infoImageR.sprite = kbmInputs["moveMouse"];
                }
                break;

            case "Gamepad":
                infoImage.sprite = xboxInputs[gamepadIconIndex];

                //Display button + left stick
                if (gamepadIconIndex == "b")
                {
                    infoImageLL.enabled = true;
                    infoImageLL.sprite = xboxInputs["b"];

                    infoTextPlusL.enabled = true;

                    infoImage.sprite = xboxInputs["leftStick"];
                }

                //Display button + right stick
                else if (gamepadIconIndex == "rightTrigger")
                {
                    infoTextPlusR.enabled = true;

                    infoImageR.enabled = true;
                    infoImageR.sprite = xboxInputs["rightStick"];
                }
                break;

            default:
                infoImage.sprite = xboxInputs[gamepadIconIndex];
                break;
        }

        //Set text
        //Determined entirely by an index value attached to the zone the player has entered
        infoText.text = " - " + infoTextDictionary[textIndex];

        //Specific case for final cluster message
        //Hide text when enemies are defeated / cluster is active
        if(textIndex == 8 && finalCluster.boxActive)
        {
            infoText.enabled = false;
        }
        //Display new message after breaking first cluster
        else if (textIndex == 2 && firstCluster.isBroken)
        {
            infoText.text = " - " + infoTextDictionary[7];
            infoImage.enabled = false;
        }

        currentInputGamepad = gamepadIconIndex;
        currentInputKBM = kbmIconIndex;
        currentTextIndex = textIndex;
    }

    //Hide info panel when not in use
    public void ClearInfoUI()
    { 
        infoImage.sprite = null;
        infoText.text = "";

        infoImage.enabled = false;
        infoText.enabled = false;

        infoTextPlusL.enabled = false;
        infoTextPlusR.enabled = false;

        infoImageLL.enabled = false;
        infoImageL.enabled = false;
        infoImageR.enabled = false;
        infoImageU.enabled = false;
    }
}
