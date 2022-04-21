using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomMouseEnter : MonoBehaviour, IPointerEnterHandler
{
    public AudioManager audioManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
        audioManager.PlaySound("HoverButton");
    }

    // Start is called before the first frame update
    void Start()
    {
        audioManager.PlaySound("");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
