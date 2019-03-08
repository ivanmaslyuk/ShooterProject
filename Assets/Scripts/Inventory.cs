using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Inventory : MonoBehaviour {
    
    public Weapon Slot1;
    public Weapon Slot2;
    public Weapon GrenadeSlot;

    public Weapon ActiveWeapon
    {
        get
        {
            if (activeSlot == 1) return Slot1;
            else return Slot2;
        }
    }

    public Weapon InactiveWeapon
    {
        get
        {
            if (activeSlot == 1) return Slot2;
            else return Slot1;
        }
    }

    public int ActiveSlot
    {
        get { return activeSlot; }
    }

    private int grenadeCount = 0;
    private AudioSource audioSource;
    private int activeSlot = 1;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();

        if (Slot1 == null)
        {
            print("Inventory был запущен с пустым Slot1. Это может вызвать непредсказуемое поведение.");
        }

        if (Slot1 != null)
        {
            // Чтобы не получалось так, что мы редактируем переменные префаба, который мы перетащили в Slot1 в редакторе,
            // вместо префаба используем его копию.
            Slot1 = Instantiate(Slot1);
            // Выключаем у оружия по уиолчанию Pickup.
            Slot1.gameObject.GetComponent<Pickup>().SetEnabled(false);
        }

        if (Slot2 != null)
        {
            Slot2 = Instantiate(Slot2);
            // Выключаем у оружия по уиолчанию Pickup.
            Slot2.gameObject.GetComponent<Pickup>().SetEnabled(false);
        }

        // Отправляем скрипту HoldGun сообщение, что инвентарь был успешно инициализирован.
        HoldGun hg = GetComponent<HoldGun>();
        if (hg != null)
        {
            hg.InventoryReady();
        }
    }

    public void ThrowOutActiveWeapon()
    {
        // unparent
        ActiveWeapon.transform.parent = null;
        ActiveWeapon.gameObject.SetActive(true);
        // change position
        ActiveWeapon.transform.position = transform.position + transform.forward;
        // turn on gravity
        ActiveWeapon.GetComponent<Pickup>().SetEnabled(true);
        // apply force
        ActiveWeapon.GetComponent<Rigidbody>().AddForce(transform.forward, ForceMode.Impulse);
        // remove from slot
        if (activeSlot == 1)
            Slot1 = null;
        else
            Slot2 = null;
    }

    /// <summary>
    /// Изменяет текущее оружие, переключаясь между первым и вторым слотами.
    /// </summary>
    public void SwitchWeapon()
    {
        if (activeSlot == 1)
        {
            // Если второй слот пуст, не производим смену оружия.
            if (Slot2 == null) return;
            else activeSlot = 2;
        }
        else
        {
            activeSlot = 1;
        }
    }

    /// <summary>
    /// Добавляет оружие в инвентарь, или заменяет текущее оружие.
    /// </summary>
    /// <param name="weapon">Новое оружие.</param>
    public void EquipWeapon(Weapon weapon)
    {
        weapon.gameObject.GetComponent<Pickup>().SetEnabled(false);

        // Если второй слот пустой (а первый активен), то вместо того, чтобы менять оружие
        // в первом слоте, мы ставим его во второй слот и переключаемся на него.
        if (Slot2 == null)
        {
            Slot2 = weapon;
            activeSlot = 2;
            return;
        }

        // Меняем оружие в текущем слоте на новое.
        ThrowOutActiveWeapon();
        if (activeSlot == 1) Slot1 = weapon;
        else Slot2 = weapon;
        HoldGun hg = gameObject.GetComponent<HoldGun>();
        if (hg != null)
            hg.UpdateWeapon();
    }

    public void OnPickupEnter(Pickup pickup)
    {
        bool destroyPickup = false;
        bool playSound = true;

        // Если наступили на пушку.
        if (pickup.Type == PickupType.Gun)
        {
            Weapon w = null;
            Weapon pickupAsWeapon = pickup.gameObject.GetComponent<Weapon>();

            // Пыьаемся найти такое же оружие, уже присутствующее в инвентаре.
            if (Slot1 != null)
                if (Slot1.Name == pickupAsWeapon.Name)
                    w = Slot1;
            if (Slot2 != null)
                if (Slot2.Name == pickupAsWeapon.Name)
                    w = Slot2;

            // Если в инвентаре нашлось такое же оружие, забираем из пикапа патроны
            // и если в пикапе после этого не остаются еще патроны, уничтожаем пикап.
            if (w != null)
            {
                // Не играть звук если мы будем игнорировать пикап.
                if (w.AmmoLeftInStash == w.StashCapacity)
                    playSound = false;

                //BUG IS HERE 
                int bulletCount = w.AmmoLeftInStash;
                bulletCount += pickupAsWeapon.RemainingAmmo;
                if (bulletCount > w.StashCapacity)
                {
                    pickupAsWeapon.RemainingAmmo = bulletCount - w.StashCapacity;
                    bulletCount = w.StashCapacity;
                    w.AmmoLeftInStash = bulletCount;
                }
                else
                {
                    w.AmmoLeftInStash = bulletCount;
                    destroyPickup = true;
                }
            }
            else playSound = false;
        }
        // Если наступили на гранату.
        else
        {
            if (grenadeCount < 2)
            {
                grenadeCount++;
                destroyPickup = true;
            }
            else playSound = false;
        }

        // Проигрываем звук пикапа.
        if (playSound)
        {
            audioSource.clip = pickup.Sound;
            audioSource.Play();
        }

        // Уничтожаем объект пикапа если нужно.
        if (destroyPickup)
            Destroy(pickup.gameObject);
    }

    public bool ContainsWeapon(WeaponName n)
    {
        if (Slot1 != null)
            if (Slot1.Name == n)
                return true;
        if (Slot2 != null)
            if (Slot2.Name == n)
                return true;
        return false;
    }
    
}