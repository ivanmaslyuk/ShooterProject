using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShootSystem : MonoBehaviour {

    private AudioSource m_AudioSource;
    private Inventory inventory;
    //var weapon; // = Inventory.CurrentWeapon;

	// Use this for initialization
	void Start () {
        m_AudioSource = gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPickupEnter(Pickup pickup)
    {
        // Стандартная реализация.
        Destroy(pickup.gameObject);
        m_AudioSource.clip = pickup.sound;
        m_AudioSource.Play();
    }
}
