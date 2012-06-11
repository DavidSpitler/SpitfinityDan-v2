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
	private Animation a;
	private bool running;
	
	/*
	 * CONE
	 * CENTER: 12.7 101.9 91.5
	 * SIZE: 225.3 179.23 154.9
	 * 
	 * AOE:
	 * CENTER 12.7 104.9 52.8
	 * SIZE: 316.1 179.23 328.3
	 * */
	
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
		
		//Initially not running
		running = false;
		
		a = gameObject.GetComponent(typeof(Animation)) as Animation;
		
		originalRotation = transform.localRotation;
		spinning = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1)){
			judgement.Execute();
			a.Play("Spell3");
		}
		/*if(spinning){ //This needs to be fixed - GameObject needed between top level GameObject and Garen Object
			Quaternion rot = transform.localRotation;
			rot.y += 1;
			transform.localRotation = rot;
		}*/
		
		if(Input.GetButtonDown("Fire2")){
			dStrike.Execute();
			a.Stop();
				a.Play("Spell1");
		}
		
		if(running && a.IsPlaying("Idle2")) {
			a.Stop();
			a.Play("Run");
		}
		else if (!a.isPlaying)
			a.Play("Idle2");
		
		courage.Update();
		judgement.Update();
	
	}
	
	public void setRunning(bool active){
		running = active;
		/*if(!active){ //If stopped running
			a.Stop();
			a.Play("Idle2");
		}*/
	}
	
	public void setSpinning(bool active){
		spinning = active;
		if(!active){
			a.Stop();
			a.Play("Idle2");
		}
	}
	
	public bool isSpinning(){
		return spinning;	
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
			BoxCollider collider = player.getConeCollider();
			collider.enabled = true;
			collider.center = new Vector3(12.7f, 101.9f, 91.5f);
			collider.size = new Vector3(225.3f, 179.23f, 154.9f); 
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
		private float timeTillFinished = 0;
		public void Execute(){
			
			//If this ability is on cooldown, don't do anything.
			//TODO Play "On Cooldown" sound?
			if(timeTillFinished > Time.time)
				return;
			
			
			//Update Collider
			BoxCollider collider = player.getConeCollider();
			collider.enabled = true;
			collider.center = new Vector3(12.7f, 104.9f, 52.8f);
			collider.size = new Vector3(316.1f, 179.23f, 328.3f); 
			
			
			//SPIN TO WIN!!!!!
			player.setSpinning(true);
			timeTillFinished = Time.time + 3.0f;
		}
		
		public void setScript(GarenScript script){
			player = script;
		}
		
		public void Update(){
			if(timeTillFinished < Time.time && player.isSpinning()){
				player.setSpinning(false);
			}
		}
	}
	
	void OnTriggerEnter(Collider other) {
		//TODO This trigger is used for all non-single target abilities
		//We need to make a state system that will determine which ability
		//was used last, and apply damage / debuffs based on that.
		
		coneCollider.enabled = false;
		MinionScript enemy = other.gameObject.GetComponent(typeof(MinionScript)) as MinionScript;
		//if(enemy && Time.time > nextAttack){
			//nextAttack = Time.time + 5;
			enemy.damage(50);
		//}
        Debug.Log("Hit!");
    }
}
