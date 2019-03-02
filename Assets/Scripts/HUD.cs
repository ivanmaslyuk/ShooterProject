using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Inventory))]
public class HUD : MonoBehaviour {

    public Text pickUpText;
    public Text StashAmmoText;
    public Text ClipAmmoText;
    public new Camera camera;

    private Inventory inventory;
    private bool lookingAtWeapon;
    private Weapon weaponToPickUp;

    void Start()
    {
        inventory = gameObject.GetComponent<Inventory>();
        lookingAtWeapon = false;
    }
    
    void FixedUpdate()
    {
        CheckIfLookingAtWeapon();
        UpdateAmmoText();

        if (lookingAtWeapon && Input.GetKeyDown(KeyCode.E))
        {
            inventory.EquipWeapon(weaponToPickUp);
        }
    }

    private void CheckIfLookingAtWeapon()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 3))
        {
            Weapon w = hit.transform.gameObject.GetComponent<Weapon>();

            if (w != null && !inventory.ContainsWeapon(w.Name))
            {
                pickUpText.text = "Press E to pick up " + w.NameString;
                pickUpText.gameObject.SetActive(true);
                lookingAtWeapon = true;
                weaponToPickUp = w;
            }
            else
            {
                pickUpText.gameObject.SetActive(false);
                lookingAtWeapon = false;
                weaponToPickUp = null;
            }
        }
        else
        {
            pickUpText.gameObject.SetActive(false);
            lookingAtWeapon = false;
            weaponToPickUp = null;
        }
    }

    private void UpdateAmmoText()
    {
        if (ClipAmmoText != null)
        {
            ClipAmmoText.text = inventory.ActiveWeapon.AmmoLeftInClip.ToString();
        }

        if (StashAmmoText != null)
        {
            StashAmmoText.text = inventory.ActiveWeapon.AmmoLeftInStash.ToString();
        }
    }

}
