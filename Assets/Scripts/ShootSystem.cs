using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Inventory))]
public class ShootSystem : MonoBehaviour {

    private AudioSource m_AudioSource;
    private Inventory inventory;
    private bool switchWeapon;
    
	void Start () {
        m_AudioSource = gameObject.GetComponent<AudioSource>();
        switchWeapon = false;
        inventory = gameObject.GetComponent<Inventory>();
	}
	
	void Update () {
        switchWeapon = false;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            switchWeapon = true;
        }
	}

    void FixedUpdate()
    {
        if (switchWeapon)
        {
            inventory.SwitchWeapon();
        }
    }
   
}
