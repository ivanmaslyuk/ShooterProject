using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupName
{
    Gun,
    Grenade
}

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Pickup : MonoBehaviour {

    [SerializeField] int RemainingAmmo;
    [SerializeField] PickupName Name;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ShootSystem shootSystem = other.gameObject.GetComponent<ShootSystem>();
            shootSystem.OnPickupEnter(this);
        }
    }
}
