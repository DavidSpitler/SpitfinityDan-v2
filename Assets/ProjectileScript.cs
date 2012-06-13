using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {
	Vector3 destination;
	private float movespeed = 0.2f;
	private float deathTime;
	private float radius = 5;
	private float damage = 5;
	
	private GarenScript player;

	// Use this for initialization
	void Start () {
		deathTime = Time.time + 15;
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time > deathTime){
			Destroy(gameObject);
			return;
		}
		
		Vector3 playerPosition = player.transform.position;
		Vector3 misslePosition = gameObject.transform.position;
		
		misslePosition.y = playerPosition.y; //Do not worry about differences in Y
		
		if(Vector3.Distance(playerPosition, misslePosition) < radius)
		{
			player.damage(damage);
			Destroy(gameObject);
		}
		
		//TODO Implement pathfinding algorithm to avoid running through objects
			int directionX = destination.x > gameObject.transform.position.x ? 1 : -1;
			
			float deltaX = destination.x - gameObject.transform.position.x;
			if(Mathf.Abs(deltaX) > movespeed)
				deltaX = movespeed;
			
			int directionZ = destination.z > gameObject.transform.position.z ? 1 : -1;
			
			float deltaZ = destination.z - gameObject.transform.position.z;
			if(Mathf.Abs(deltaZ) > movespeed)
				deltaZ = movespeed;
			
			Vector3 newPos = new Vector3(gameObject.transform.position.x + (deltaX * directionX), gameObject.transform.position.y, gameObject.transform.position.z + (deltaZ * directionZ));
			
			gameObject.transform.position = newPos;
	}
	
	public void setDest(Vector3 dest){
		destination = dest;	
	}
	
	public void setPlayer(GarenScript player){
		this.player = player;	
	}
	
}
