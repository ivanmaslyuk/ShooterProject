using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootSystem : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPickupEnter(Pickup pickup)
    {
        // Стандартная реализация.
        Destroy(pickup.gameObject);
    }
}
