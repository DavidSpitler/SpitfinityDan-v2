using UnityEngine;
using System.Collections;

public class MouseMovement : MonoBehaviour {

	private float PLAYER_MOVEMENT_SPEED = 0.5F;
	private Vector3 dest;
	private Vector3 direction;
	private CharacterMotor motor;
	private RaycastHit hit;
	private float nextAttack = 0;
	
	// Use this for initialization
	void Start () {
		dest = transform.position;
		motor = GetComponent(typeof(CharacterMotor)) as CharacterMotor;
	}
	
	// Update is called once per frame
	void Update () {

		//Check for mouse click and update destination
		if(Input.GetButtonDown("Fire1")){
			
			Ray ray = Camera.allCameras[0].ScreenPointToRay(Input.mousePosition);
			
			if(Physics.Raycast(ray, out hit, 10000)){
				MinionScript enemy = hit.collider.gameObject.GetComponent(typeof(MinionScript)) as MinionScript;
				
				if(enemy && Time.time > nextAttack){
					nextAttack = Time.time + 5;
					enemy.damage(20);
				}
				else{
			    dest = hit.point;
				dest.y = 22.20f;
			    direction = hit.point - transform.position;
				direction.y = 22.20f;
				if(direction.magnitude > 1)
					direction = direction.normalized;
				}
				transform.LookAt(dest);
			}
		}
		
		Vector3 delta = dest - transform.position;
		motor.inputMoveDirection = delta.normalized;
		transform.position = Vector3.MoveTowards(transform.position, dest, PLAYER_MOVEMENT_SPEED);
		if(delta.magnitude < .1f){
			transform.position = dest;
			motor.inputMoveDirection = Vector3.zero;
		}
		
		/*
		//Check if the player currently needs to move towards the destination spot
		if(characterTransform.position.x != dest.x || characterTransform.position.z != dest.z){
			Debug.Log("You are watching me move!");
			float finalX = characterTransform.position.x;
			float finalY = characterTransform.position.y;
			float finalZ = characterTransform.position.z;
			
			
			float direction = characterTransform.position.x > dest.x ? -1.0f : 1.0f;
			float deltaX = Mathf.Abs(characterTransform.position.x - dest.x);
			if(deltaX < PLAYER_MOVEMENT_SPEED)
				finalX = dest.x;
			else
				finalX += (PLAYER_MOVEMENT_SPEED * direction);
		
			direction = characterTransform.position.z > dest.z ? -1.0f : 1.0f;
			float deltaZ = Mathf.Abs(characterTransform.position.z - dest.z);
			if(deltaZ < PLAYER_MOVEMENT_SPEED)
				finalZ = dest.z;
			else
				finalZ += (PLAYER_MOVEMENT_SPEED * direction);
			
			characterTransform.position = new Vector3(finalX, finalY, finalZ);
			
			//characterTransform.position.Set(finalX, finalY, finalZ);
		}
		*/
		
	
	}
	
}
