using UnityEngine;

public enum WeaponName
{
    SciFiGun,
    HandGun,
    Shotgun,
    Grenade
}

public class Weapon : MonoBehaviour {

    public Vector3 CameraOffset;
    public Vector3 Rotation;
    public WeaponName Name;

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
    /// Количество оставшихся потронов в запасе.
    /// </summary>
    public int AmmoLeftInStash;

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
                case WeaponName.SciFiGun: return "Sci-Fi Gun";
                case WeaponName.HandGun: return "Hand Gun";
                case WeaponName.Shotgun: return "Shotgun";
                case WeaponName.Grenade: return "Grenade";
                default: return "Unknown Weapon";
            }
        }
    }
}
