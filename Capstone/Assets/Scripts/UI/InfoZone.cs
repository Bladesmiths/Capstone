using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Trigger areas that display a tutorial UI panel while the player is present
public class InfoZone : MonoBehaviour
{
    public string gamepadIndex;
    public string kbmIndex;
    public int textIndex;

    public InfoPanel infoPanel;

    // Start is called before the first frame update
    void Start()
    {
        //The UIManager should really be static or something, but right now it isn't
        infoPanel = GameObject.Find("InfoPanel").GetComponent<InfoPanel>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the object collided with is the player
        if (other.transform.root.gameObject.CompareTag("Player"))
        {
            infoPanel.withinZone = true;
            infoPanel.SetInfoUI(gamepadIndex, kbmIndex, textIndex);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the object collided with is the player
        if (other.transform.root.gameObject.CompareTag("Player"))
        {
            infoPanel.withinZone = false;
            infoPanel.ClearInfoUI();
        }
    }
}