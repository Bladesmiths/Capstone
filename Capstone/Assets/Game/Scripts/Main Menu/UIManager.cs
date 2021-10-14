using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bladesmiths.Capstone.UI
{
    public class UIManager : MonoBehaviour
    {
        
        [TitleGroup("HUD")]
        [HorizontalGroup("HUD/Split")]
        [VerticalGroup("HUD/Split/Left")] [BoxGroup("HUD/Split/Left/Health Bar")]
        [LabelWidth(85)]
        [SerializeField] private Image healthBarFill;
        
        [VerticalGroup("HUD/Split/Right")] [BoxGroup("HUD/Split/Right/Points UI")]
        [LabelWidth(70)]
        [SerializeField] private TextMeshProUGUI pointsText;

        [TitleGroup("Menus")] 
        [BoxGroup("Menus/Pause Menu")]
        [SerializeField] private GameObject pauseMenu;
    
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            float fillPercentage = Mathf.Clamp(currentHealth / maxHealth, 0, 1);
            healthBarFill.fillAmount = fillPercentage;
        }

        public void UpdateScore(int currentScore)
        {
            pointsText.text = currentScore.ToString().Trim();
        }

        public void Pause()
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }

        public void Unpause()
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }
}

