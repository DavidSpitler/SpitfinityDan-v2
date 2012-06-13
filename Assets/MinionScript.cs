using UnityEngine;
using System.Collections;

public class MinionScript : MonoBehaviour {
	public GameObject p; //Health bar plane 
	private float currentHealth;
	private float maxHealth;
	private float originalXScale;
	public GarenScript player;
	
	// Use this for initialization
	void Start () {
		currentHealth = 100;
		maxHealth = 100;
		originalXScale = p.transform.localScale.x;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 currentScale = p.transform.localScale;
		currentScale.x = originalXScale * (currentHealth / maxHealth);
		p.transform.localScale = currentScale;
	}
	
	public void stun(float time){
		;//Stun duration	
	}
	
	public float getHealthPercent(){
		return currentHealth / maxHealth;	
	}
	
	public void damage(float amount){
		currentHealth -= amount;
		if(currentHealth < 0){
			currentHealth = 0;
			if(player.getCurrentEnemy() == this)
				player.setCurrentEnemy(null);
		}
	}
}
