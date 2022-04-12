using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using UnityEngine.InputSystem;

namespace Bladesmiths.Capstone.UI
{
    public class PauseMenu : SerializedMonoBehaviour
    {
        [OdinSerialize]
        private Dictionary<string, List<GameObject>> controlImages = new Dictionary<string, List<GameObject>>();

        [SerializeField]
        private GameObject dashText;
        [SerializeField]
        private GameObject dodgeText;

        [OdinSerialize]
        private Dictionary<SwordType, GameObject> formInfos = new Dictionary<SwordType, GameObject>();

        private string currentControlScheme;
        private SwordType activeFormInfo = SwordType.Topaz;

        [SerializeField]
        private GameObject controlsObject;
        [SerializeField]
        private GameObject formInfoObject;

        // Start is called before the first frame update
        void Start()
        {
            activeFormInfo = SwordType.Topaz;
            formInfos[activeFormInfo].SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SwapFormInfo(SwordType currentSwordType)
        {
            if (currentSwordType != activeFormInfo)
            {
                formInfos[activeFormInfo].SetActive(false);

                activeFormInfo = currentSwordType;

                formInfos[activeFormInfo].SetActive(true);
            }
        }

        public void SwapControls(string newControlScheme)
        {
            if (newControlScheme == currentControlScheme)
            {
                return;
            }

            if (currentControlScheme != null)
            {
                foreach (GameObject controlImage in controlImages[currentControlScheme])
                {
                    controlImage.SetActive(false);
                }
            }

            currentControlScheme = newControlScheme;

            foreach (GameObject controlImage in controlImages[currentControlScheme])
            {
                controlImage.SetActive(true);
            }

            if (Player.instance.currentSword.SwordType == SwordType.Sapphire)
            {
                dashText.SetActive(true);
                dodgeText.SetActive(false);
            }
            else
            {
                dashText.SetActive(false);
                dodgeText.SetActive(true);
            }

        }

        public void TogglePauseInfoDisplays()
        {
            controlsObject.SetActive(!controlsObject.activeInHierarchy);
            formInfoObject.SetActive(!formInfoObject.activeInHierarchy);
        }
    }
}