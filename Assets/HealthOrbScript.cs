using UnityEngine;
using System.Collections;

public class HealthOrbScript : PickUp {
	private float healAmount;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void trigger(GarenScript player){
		player.awardHealth(healAmount);
		Destroy(gameObject);
	}
}
