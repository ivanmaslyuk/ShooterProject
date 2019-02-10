using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpText : MonoBehaviour {
    
    public Text pickUpText;
    public Camera camera;
   
    // Use this for initialization
    void Start () {
        
    }

    // Update is called once per frame
    void FixedUpdate () {
        
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 1))
        {
            Transform objectHit = hit.transform;
            if (objectHit.tag == "Weapon")
            {
              
                pickUpText.gameObject.SetActive(true);

                print("Press E to pick up weapon");
                
            }
            else pickUpText.gameObject.SetActive(false);
        }
        else pickUpText.gameObject.SetActive(false);

    }
}
