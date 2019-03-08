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
        if (Input.GetMouseButtonDown(0) & inventory.ActiveWeapon.canShoot & inventory.ActiveWeapon.AmmoLeftInClip > 0)
        {
            inventory.ActiveWeapon.Shoot();
        }
        if (Input.GetKeyDown(KeyCode.R) & inventory.ActiveWeapon.canShoot && inventory.ActiveWeapon.AmmoLeftInClip != inventory.ActiveWeapon.AmmoLeftInStash && inventory.ActiveWeapon.AmmoLeftInClip != inventory.ActiveWeapon.ClipCapacity)
        {
            inventory.ActiveWeapon.canShoot = false;
            StartCoroutine(inventory.ActiveWeapon.CoroutineReload());
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
