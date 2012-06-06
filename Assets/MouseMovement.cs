using UnityEngine;
using System.Collections;

public class MouseMovement : MonoBehaviour {
	int lastTime = 0;
	int PLAYER_MOVEMENT_SPEED = 10;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
    void OnGUI() {
        Event e = Event.current;
		int currentTime = Time.time;
        if (e.isMouse){
			Vector3 pos = Input.mousePosition;
            int deltaX = pos.x - Screen.width / 2;
			int deltaY = pos.y - Screen.height / 2;
			int deltaTime = currenTime - lastTime;
			
    }
}
