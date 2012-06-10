using UnityEngine;
using System.Collections;

public class GarenScript : MonoBehaviour {
	public BoxCollider coneCollider;
	private Decisive_Strike dStrike;
	private Courage courage;
	private Judgement judgement;
	private float nextAttack = 0;
	private float currentHealth;
	private bool courageActive;
	private Quaternion originalRotation;
	private bool spinning;
	
	// Use this for initialization
	void Start () {
		coneCollider.enabled = false;
		
		//Initialize Abilities - TODO Should this be done statically?
		dStrike = new Decisive_Strike();
		dStrike.setScript(this);
		courage = new Courage();
		courage.setScript(this);
		judgement = new Judgement();
		judgement.setScript(this);
		
		
		originalRotation = transform.localRotation;
		spinning = false;
	}
	
	// Update is called once per frame
	void Update () {
		//if(Input.GetButtonDown("Fire2")){
		//	dStrike.Execute();
		//}
		if(spinning){ //This needs to be fixed - GameObject needed between top level GameObject and Garen Object
			Quaternion rot = transform.localRotation;
			rot.y += 1;
			transform.localRotation = rot;
		}
		
		if(Input.GetButtonDown("Fire2")){
			courage.Execute();
		}
		
		courage.Update();
	
	}
	
	public void setSpinning(bool active){
		spinning = active;	
	}
	
	public void activateCourage(bool active){
			courageActive = active;
	}
	
	public void damage(float amount){
		//Compute damage reductions
		if(courageActive)
			amount *= 0.7f;
		
		//Apply damage
		currentHealth -= amount;
		if(currentHealth < 0)
			currentHealth = 0;
		
		//Check for death
	}
	
	public BoxCollider getConeCollider(){
		return coneCollider;
	}
	
	class Decisive_Strike : Ability {
		private GarenScript player;
		public void Execute(){
			player.getConeCollider().enabled = true;
		}
		
		public void setScript(GarenScript script){
			player = script;
		}
		
	}
	
	class Courage : Ability {
		private GarenScript player;
		private bool active = false;
		private float cooldown = 5;
		private float nextAttack = 0;
		
		public void Update(){
			if(nextAttack < Time.time && active){
				active = false;
				player.activateCourage(false);
				Debug.Log("Courage finished!");
			}
		}
		
		public void Execute(){
			player.activateCourage(true);
			nextAttack = Time.time + cooldown;
			active = true;
			Debug.Log("Courage active!");
		}
		
		public void setScript(GarenScript script){
			player = script;
		}
	}
	
	class Judgement : Ability {
		private GarenScript player;
		
		public void Execute(){
			//SPIN TO WIN!!!!!
			player.setSpinning(true);
		}
		
		public void setScript(GarenScript script){
			player = script;
		}
	}
	
	void OnTriggerEnter(Collider other) {
		coneCollider.enabled = false;
		MinionScript enemy = other.gameObject.GetComponent(typeof(MinionScript)) as MinionScript;
		//if(enemy && Time.time > nextAttack){
			//nextAttack = Time.time + 5;
			enemy.damage(50);
		//}
        Debug.Log("Hit!");
    }
}
