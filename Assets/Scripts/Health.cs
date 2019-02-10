using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {
    public GameObject player;

    private float health;
    public
	// Use this for initialization
	void Start () {
        health = 100f;
	}
    private void OnCollisionEnter(Collision collision)
    {
        
    }
    // Update is called once per frame
    void Update () {
		
	}
}
