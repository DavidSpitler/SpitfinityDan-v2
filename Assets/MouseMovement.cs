using UnityEngine;
using System.Collections;

public class MouseMovement : MonoBehaviour {

	private float PLAYER_MOVEMENT_SPEED = 0.5F;
	private Vector3 dest;
	private Vector3 direction;
	private CharacterMotor motor;
	private RaycastHit hit;
	private float nextAttack = 0;
	private GarenScript player;
	private bool movingToAttack = false;
	private bool attacking = false;
	
	private bool idling;
	private float idleTime;
	
	// Use this for initialization
	void Start () {
		dest = transform.position;
		motor = GetComponent(typeof(CharacterMotor)) as CharacterMotor;
		player = gameObject.GetComponentInChildren(typeof(GarenScript)) as GarenScript;
	}
	
	// Update is called once per frame
	void Update () {
		
		if(!player.isAlive())
			return;
		
		/*
		 * Conditions that must be met to start idling:
		 * 1) Not yet idling
		 * 2) At current destination (not running)
		 * 3) Not attacking (holding down left click)
		 * 4) Not in another animation (spells, etc)
		 * */
		if(transform.position - dest == Vector3.zero && !attacking && !idling && (player.isRunAnimation() || player.noAnimation())){
			player.stopAnimation();
			idling = true;
			idleTime = Time.time + 5;
		}
		
		else if(idling && Time.time > idleTime && player.noAnimation())
			player.playIdleSequence();

		//Check for mouse click and update destination
		if(Input.GetButtonDown("Fire1")){
			idling = false;
			player.setIdling(false);
			
			movingToAttack = false;
			
			player.setRunning(true);
			
			Ray ray = Camera.allCameras[0].ScreenPointToRay(Input.mousePosition);
			
			if(Physics.Raycast(ray, out hit, 10000)){
				MinionScript enemy = hit.collider.gameObject.GetComponent(typeof(MinionScript)) as MinionScript;
				if(enemy){ //If target is an enemy
					attacking = true;
					player.setCurrentEnemy(enemy);
					if(Time.time > nextAttack && ((enemy.gameObject.transform.position - player.gameObject.transform.position).magnitude <= player.getRange())){
						nextAttack = player.autoAttack(enemy);
						//nextAttack = Time.time + player.getWeaponSpeed();
						//player.playAnimation("Attack1");
						//enemy.damage(player.getWeaponDamage()); //TODO Calculate auto attack damage
						Vector3 enemyLookAt = enemy.gameObject.transform.position;
						enemyLookAt.y = 25.20f;
						transform.LookAt(enemyLookAt);
					}
					//TODO See if we can make this all just one conditional
					//If cooldown is complete, but too far away to attack
						else if(Time.time > nextAttack){
						dest = hit.point;
						dest.y = 25.20f;
					    direction = hit.point - transform.position;
						direction.y = 22.20f;
						if(direction.magnitude > 1)
							direction = direction.normalized;
						transform.LookAt(dest);
						movingToAttack = true;
					}
					
					
				}
				else{ //If not enemy
				    dest = hit.point;
					dest.y = 25.20f;
				    direction = hit.point - transform.position;
					direction.y = 22.20f;
					if(direction.magnitude > 1)
						direction = direction.normalized;
					transform.LookAt(dest);
				}
			}
		}
		
		else if (Input.GetButtonUp("Fire1")) //Released left click
			attacking = false;
		
		
		//Moving towards target to attack, and gets within range.
		if(attacking && player.getCurrentEnemy() != null && Time.time > nextAttack && 
				(player.getCurrentEnemy().gameObject.transform.position - player.gameObject.transform.position).magnitude <= player.getRange()){
			MinionScript enemy = player.getCurrentEnemy();
			dest = transform.position; //Stop moving
			dest.y = 25.20f;
			nextAttack = player.autoAttack(enemy);
			//player.playAnimation("Attack1");
			//enemy.damage(player.getWeaponDamage()); //TODO Calculate auto attack damage
			Vector3 enemyLookAt = enemy.gameObject.transform.position;
			enemyLookAt.y = 25.20f;
			transform.LookAt(enemyLookAt);
			movingToAttack = false;
			//nextAttack = Time.time + player.getWeaponSpeed();
		}
		
		Vector3 delta = dest - transform.position;
		motor.inputMoveDirection = delta.normalized;
		transform.position = Vector3.MoveTowards(transform.position, dest, PLAYER_MOVEMENT_SPEED);
		if(delta.magnitude < .1f){ //Reach destination
			transform.position = dest;
			motor.inputMoveDirection = Vector3.zero;
			player.setRunning(false);
		
		}
		
	}
	
}
