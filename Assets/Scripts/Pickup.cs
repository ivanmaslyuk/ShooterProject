using UnityEngine;

public enum PickupType
{
    Gun,
    Grenade
}

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Pickup : MonoBehaviour {
    
    public PickupType Type;
    public AudioClip Sound;
    /*[HideInInspector] */public bool isEnabled = true;
    
    public void HitByCharacterController(GameObject cc)
    {
        if (!isEnabled)
        {
            print(string.Format("Пикап {0} был тронут, но выключен.", name));
            return;
        }

        if (cc.tag == "Player")
        {
            Inventory inventory = cc.GetComponent<Inventory>();
            inventory.OnPickupEnter(this);
        }
    }

    public void SetEnabled(bool value)
    {
        isEnabled = value;

        Collider col = gameObject.GetComponent<Collider>();
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (value)
        {
            rb.useGravity = true;
            col.isTrigger = false;
        }
        else
        {
            rb.useGravity = false;
            col.isTrigger = true;
        }
    }
}