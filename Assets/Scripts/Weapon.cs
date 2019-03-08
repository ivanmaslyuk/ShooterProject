using System.Collections;
using UnityEngine;

public enum WeaponName
{
    Riffle,
    Pistol,
    Shotgun,
    Grenade
}

public class Weapon : MonoBehaviour {
    /// <summary>
    /// Вместительность магазина.
    /// </summary>
    public int ClipCapacity;

    /// <summary>
    /// Вместительность запаса патронов.
    /// </summary>
    public int StashCapacity;

    /// <summary>
    /// Количество оставшихся патронов в магазине.
    /// </summary>
    public int AmmoLeftInClip;

    /// <summary>
    /// Количество оставшихся патронов в запасе.
    /// </summary>
    public int AmmoLeftInStash;


    public Vector3 CameraOffset;
    public Vector3 Rotation;
    public WeaponName Name;

    /// <summary>
    /// Урон от оружия
    /// </summary>
    public float damage = 1f;

    /// <summary>
    /// дальность выстрела
    /// </summary>
    public float range = 500f;	

    /// <summary>
    /// Темп стрельбы
    /// </summary>
    public float timeout = 0.2f;

    /// <summary>
    /// Время перезарядки(смены магазина)
    /// </summary>
    public float reloadTime = 1.0f;

    public bool canShoot = true; // для отключения стрельбы во время перезаряда(в дальнейшем и когда только достал оружие и идет анимация(наверно))

    public void Shoot()
    {
        AmmoLeftInClip--;

        Vector3 DirectionRay = Camera.main.transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        //Debug.DrawLine(Camera.main.transform.position, DirectionRay * range, Color.red);

        if (Physics.Raycast(Camera.main.transform.position, DirectionRay, out hit, range))
        {
            if (hit.collider.tag == "Player")
            {
                Debug.Log("Shoot in |" + hit.collider.tag + "| at a distance |" + hit.distance + "|");
                hit.transform.GetComponent<Health>().addDamage(damage);
            }
        }
        if (AmmoLeftInClip <= 0)
            StartCoroutine(CoroutineReload());
        else
        {
            StartCoroutine(CoroutineShoot());
        }
    }

    // сопрограмма стрельбы
    public IEnumerator CoroutineShoot()
    {
        // небольшая задержка
        yield return new WaitForSeconds(timeout);
        // разрешаем стрелять
        canShoot = true;
        // выходим с сопрограммы
        yield break;
        
    }

    // сопрограмма перезарядки
    public IEnumerator CoroutineReload()
    {
        yield return new WaitForSeconds(reloadTime);

        // она служит для красоты перезарядки)
        var ammo = 0;
        // если у нас были патроны в магазине то нашей временной переменной присваиваем значение оставшихся патронов
        if (AmmoLeftInClip > 0)
        {
            ammo = AmmoLeftInClip;
            AmmoLeftInClip = 0;
        }
        // (условие №2)если дополнительных патронов меньше чем максимальная емкость магазина...
        if (AmmoLeftInStash < ClipCapacity)
        {
            // (условие №3) если количество дополнительных патронов + оставшихся в магазине больше максимальной емкости магазина...
            if (AmmoLeftInStash + ammo > ClipCapacity)
            {
                
                // то кладем в магазин патроны в количестве максимального его объема
                AmmoLeftInClip = ClipCapacity;
                // а дополнительные патроны считаем по формуле: дополнительные патроны = дополнительные патроны + оставшиеся патроны - объем магазина
                AmmoLeftInStash = AmmoLeftInStash + ammo - ClipCapacity;
            }
            else
            {// если условие №3 не выполняется...
             // то кладем в магазин патроны в количетсве равное дополнительные патроны + те что остались
                AmmoLeftInClip = AmmoLeftInStash + ammo;
                // а дополнительные патроны приравниваем нулю
                AmmoLeftInStash = 0;
            }
        }
        else
        {// если условие №2 не выполняется...
         // то кладем в магазин патроны в количестве максимального его объема
            AmmoLeftInClip = ClipCapacity;
            // а дополнительные патроны считаем по формуле: дополнительные патроны = дополнительные патроны - объем магазина + оставшиеся
            AmmoLeftInStash = AmmoLeftInStash - ClipCapacity + ammo;
        }
        // включаем триггер (стрелять можно)
        canShoot = true;
    }

    //void Update() {
    //    if (Input.GetMouseButtonDown(0) & canShoot & AmmoLeftInClip > 0)
    //    {          
    //         Shoot();
    //    }
    //    if (Input.GetKeyDown(KeyCode.R) & canShoot  && AmmoLeftInClip != AmmoLeftInStash && AmmoLeftInClip != ClipCapacity )
    //    {
    //        canShoot = false;
    //        StartCoroutine(CoroutineReload());
    //    }

    //}
    

    public int RemainingAmmo
    {
        get
        {
            return AmmoLeftInClip + AmmoLeftInStash;
        }
        set
        {
            if (value > ClipCapacity)
            {
                AmmoLeftInClip = ClipCapacity;
                int left = value - ClipCapacity;
                if (left > StashCapacity)
                    AmmoLeftInStash = StashCapacity;
            }
            else
            {
                AmmoLeftInClip = value;
                AmmoLeftInStash = 0;
            }
        }
    }

    public string NameString
    {
        get
        {
            switch (Name)
            {
                case WeaponName.Riffle: return "Riffle";
                case WeaponName.Pistol: return "Pistol";
                case WeaponName.Shotgun: return "Shotgun";
                case WeaponName.Grenade: return "Grenade";
                default: return "Unknown Weapon";
            }
        }
    }   
}
