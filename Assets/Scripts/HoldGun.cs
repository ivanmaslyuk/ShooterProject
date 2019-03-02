using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class HoldGun : MonoBehaviour {

    private Inventory inventory;
    private int previouslyActiveSlot;
    private GameObject currentGun;
    [SerializeField] private new GameObject camera;

    private bool didSwitchSlot
    {
        get
        {
            return previouslyActiveSlot != inventory.ActiveSlot;
        }
    }
    
	void Start () {
        inventory = GetComponent<Inventory>();
        previouslyActiveSlot = inventory.ActiveSlot;
    }

    void Update()
    {
        if (didSwitchSlot)
        {
            UpdateWeapon();
        }
        previouslyActiveSlot = inventory.ActiveSlot;
    }

    public void InventoryReady()
    {
        UpdateWeapon();
    }
    
    public void UpdateWeapon()
    {
        // Новое оружие.
        Weapon w = inventory.ActiveWeapon;

        // Если в инвентаре по какой-либо причине не задано ни одного оружия,
        // то выходим из функции.
        if (w == null)
        {
            print("InstantiateWeapon() вызвана, когда Inventory возвращает null при запросе текущего оружия.");
            return;
        }

        // Выключаем прошлое оружие.
        Weapon inactive = inventory.InactiveWeapon;
        if (inactive != null)
        {
            inactive.gameObject.SetActive(false);
        }

        currentGun = w.gameObject;
        currentGun.SetActive(true);
        currentGun.transform.SetParent(camera.transform);
        currentGun.transform.localPosition = w.CameraOffset;
        currentGun.transform.localRotation = Quaternion.Euler(w.Rotation);
    }
    
}
