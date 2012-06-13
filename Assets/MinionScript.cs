using UnityEngine;
using System.Collections;

public class MinionScript : MonoBehaviour {
	public Texture2D healthTexture;
	protected float currentHealth;
	protected float maxHealth;
	protected float originalXScale;
	protected float range = 10;
	protected float weaponDamage = 10;
	protected float weaponSpeed = 1.5f;
	protected float nextAttack = 0;
	protected float movespeed = 0.1f;
	
	protected Vector3 dest;
	
	protected bool fleeing;
	protected float fleeTime;
	
	public GameObject healthOrb;
	
	
	protected bool alive;
	protected Animation animator;
	public GarenScript player;
	
	// Use this for initialization
	void Start () {
		currentHealth = 100;
		maxHealth = 100;
		animator = gameObject.GetComponent(typeof(Animation)) as Animation;
		alive = true;
	}
	
	void OnGUI(){
		if(getHealthPercent() > 0){ //If not dead
			Vector3 healthBarPosition = Camera.main.WorldToScreenPoint(gameObject.transform.position);
			GUI.Box(new Rect(healthBarPosition.x - 23, Screen.height - healthBarPosition.y - 70, 50 * getHealthPercent(), 10), healthTexture);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(!alive)
			return;
		
		if(!player.isAlive()){
			return; //TODO make this more interesting after the player dies
		}
		
		if(Time.time > fleeTime)
			fleeing = false;
		
		if(getHealthPercent() < 0.20f && !fleeing){
			//80% chance to flee at low health
			Random.seed = (int)Time.time;
			if(Random.value <= 0.8f){
				fleeing = true;
				fleeTime = Time.time + 2;
				dest = new Vector3(100.0f * Random.value, gameObject.transform.position.y, 100.0f * Random.value);
			}
		}
		
		float distance = (player.transform.position - gameObject.transform.position).magnitude;
		
		//If in attack range
		if(distance <= range && !fleeing){
			if(animator.IsPlaying("Run"))
				animator.Stop();
			
			if(!animator.isPlaying)
				animator.Play("Idle");
			
			if(Time.time > nextAttack){
				player.damage(weaponDamage);
				nextAttack = Time.time + weaponSpeed;
				animator.Stop();
				animator.Play("Attack1");
			}
		}
		
		//If not in attack range
		else{
			if(!animator.IsPlaying("Run")){
				animator.Stop();
				animator.Play("Run");
			}
			if(!fleeing)
				dest = player.transform.position;
			
			//TODO Implement pathfinding algorithm to avoid running through objects
			int directionX = dest.x > gameObject.transform.position.x ? 1 : -1;
			
			float deltaX = dest.x - gameObject.transform.position.x;
			if(Mathf.Abs(deltaX) > movespeed)
				deltaX = movespeed;
			
			int directionZ = dest.z > gameObject.transform.position.z ? 1 : -1;
			
			float deltaZ = dest.z - gameObject.transform.position.z;
			if(Mathf.Abs(deltaZ) > movespeed)
				deltaZ = movespeed;
			
			Vector3 newPos = new Vector3(gameObject.transform.position.x + (deltaX * directionX), gameObject.transform.position.y, gameObject.transform.position.z + (deltaZ * directionZ));
			
			gameObject.transform.position = newPos;
		}
		
		
		gameObject.transform.LookAt(dest);
		
	}
	
	public void stun(float time){
		;//Stun duration	
	}
	
	public float getHealthPercent(){
		return currentHealth / maxHealth;	
	}
	
	public void damage(float amount){
		if(!alive)
			return;
		currentHealth -= amount;
		if(currentHealth < 0){
			animator.Stop();
			animator.Play("Death");
			currentHealth = 0;
			if(player.getCurrentEnemy() == this)
				player.setCurrentEnemy(null);
			alive = false;
			DropLoot();
			
			(gameObject.GetComponent(typeof(SphereCollider)) as SphereCollider).enabled = false;
		}
	}
	
	private void DropLoot(){
		if(Random.value >= 0.5f)
		{
			Vector3 itemPosition = transform.position;
			
			float deltaX = Random.value * 4;
			if(Random.value > .5)
				deltaX *= -1;
			float deltaZ = Random.value * 4;
			if(Random.value > .5)
				deltaZ *= -1;
			
			itemPosition.x += deltaX;
			itemPosition.z += deltaZ;
			itemPosition.y = 10;
			
			Instantiate(healthOrb, itemPosition, Quaternion.identity);
		}
	}
}
