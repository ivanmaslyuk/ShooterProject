using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {
    public int maxAmmoInClip; //емкость магазина
    public int leftAmmoInClip; //сколько осталось в магазине
    public int ammoInStock; //патронов в запасе
    public int leftAmmoInStock; //патронов осталось в запасе

    private float reloadTime;   
    public bool canShoot;
    private float damage;
    public bool auto;
    //private float recoilForce; // сила отдачи(хз надо ли это)
    //private float accuracy; // точность(хз на что это влияет и надо ли нам)
    //private float scatterAngle; // угол разброса(возможно лучше хранить это радиус или что-то еще)

    //public AudioClip fireSound; // звук выстрела 
    //public AnimationClip idle;  // анимация бездействия 
    //public AnimationClip fire;  // анимация выстрела 
    //public AnimationClip reload;   // анимация перезарядки 



    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
