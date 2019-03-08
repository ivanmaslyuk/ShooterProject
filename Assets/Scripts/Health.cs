using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    public float HP = 100;

    public void addDamage(float damage)
    {
        HP -= damage;
        print(HP);
        if (HP <=0) // Также проиграть анимацию смерти, а затем дестрой, можно еще и задержку поставить, чтоб труп повалялся чуток
        {
           
        }
    }
}
