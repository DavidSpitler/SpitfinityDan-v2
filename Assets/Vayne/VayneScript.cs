using UnityEngine;
using System.Collections;

public class VayneScript : MonoBehaviour {
	
	//Spells
	private Tumble tumble;
	private Silver_Bolts silverBolts;
	private Condemn condemn;
	private Final_Hour finalHour;
//	private Valor valor;
	
	//Spell textures
	public Texture2D tumbleTexture;
	public Texture2D sBoltsTexture;
	public Texture2D condemnTexture;
	public Texture2D fHourTexture;
	
	
	//Health textures
	public Texture2D enemyHealthTexture;
	public Texture2D playerHealthTexture;
	
	private bool alive;
	public Texture2D healthTexture;
	
	
	private float nextAttack = 0;
	private float currentHealth;
	private float maxHealth = 100;
	
	private bool finalHourActive;
	private Quaternion originalRotation;
	private bool tumbled;
	private Animation a;
	private bool running;

	private float range = 80;
	private bool idling;
	private float pickUpRange = 10;
	
	
	private float WeaponDamage = 20;
	private float WeaponSpeed = 1.5f;
	
	private int IconWidth;
	private int IconHeight;
	
	private MinionScript currentEnemy;
	
	/*
	 * CONE
	 * CENTER: 12.7 101.9 91.5
	 * SIZE: 225.3 179.23 154.9
	 * 
	 * AOE:
	 * CENTER 12.7 104.9 52.8
	 * SIZE: 316.1 179.23 328.3
	 * */
	
	void OnGUI() {
		GUI.BeginGroup(new Rect(Screen.width / 2 - (2 * IconWidth), Screen.height - 70, IconWidth * 4, IconHeight * 4));
		
		GUI.Button(new Rect(0, 0, IconWidth, IconHeight), tumbleTexture);
		GUI.Button(new Rect(IconWidth, 0, IconWidth, IconHeight), sBoltsTexture);
		GUI.Button(new Rect(IconWidth * 2, 0, IconWidth, IconHeight), condemnTexture);
		GUI.Button(new Rect(IconWidth * 3, 0, IconWidth, IconHeight), fHourTexture);

		GUI.EndGroup();
		
		//Creates mask over buttons based on cooldown.
		
		GUI.BeginGroup(new Rect(Screen.width / 2 - (2 * IconWidth), Screen.height - 70, IconWidth * 4, IconHeight * 4));
		
		GUI.Button(new Rect(0, 0, IconWidth, IconHeight * tumble.getCooldown()), "");
		GUI.Button(new Rect(IconWidth, 0, IconWidth, IconHeight * silverBolts.getCooldown()), "");
		GUI.Button(new Rect(IconWidth * 2, 0, IconWidth, IconHeight * condemn.getCooldown()), "");
		GUI.Button(new Rect(IconWidth * 3, 0, IconWidth, IconHeight * finalHour.getCooldown()), "");
		
		GUI.EndGroup();
		
		//Current Enemy Healthbar
		int buttonLength = 300;
		if(currentEnemy){
		
			GUI.Box(new Rect((Screen.width / 2) - (buttonLength / 2), 40, buttonLength, 30), "");
			
			GUI.BeginGroup(new Rect((Screen.width / 2) - (buttonLength / 2), 40, buttonLength * currentEnemy.getHealthPercent(), 30));
			GUI.Box(new Rect(0, 0, buttonLength,30) , enemyHealthTexture);
			GUI.EndGroup();
		}
		
		//Player Healthbar
			GUI.Box(new Rect((Screen.width / 2) - (buttonLength / 2), Screen.height - 120, buttonLength, 30), "");
			
			GUI.BeginGroup(new Rect((Screen.width / 2) - (buttonLength / 2), Screen.height - 120, buttonLength * getHealthPercent(), 30));
			GUI.Box(new Rect(0, 0, buttonLength,30) , playerHealthTexture);
			GUI.EndGroup();

	}
	
	// Use this for initialization
	void Start () {
		
		currentHealth = maxHealth;
		
		//Initialize Abilities - TODO Should this be done statically?
		tumble = new Tumble();
		tumble.setScript(this);
		silverBolts = new Silver_Bolts();
		silverBolts.setScript(this);
		condemn = new Condemn();
		condemn.setScript(this);
		finalHour = new Final_Hour();
		finalHour.setScript(this);
	//	valor = new Valor();
	//	valor.setScript(this);
		
		IconWidth = tumbleTexture.width;
		IconHeight = tumbleTexture.height;
		
		//Initially not running
		running = false;
		
		idling = true;
		
		a = gameObject.GetComponent(typeof(Animation)) as Animation;
		
		originalRotation = transform.localRotation;
		//spinning = false;
		
		alive = true;
	}
	
	
	
	
	// Update is called once per frame
	void Update () {
		
		if(!alive)
			return;
		
		//Check for spells being cast
		if(Input.GetKeyDown(KeyCode.Alpha1)){
			tumble.Execute();
		}
		
		else if(Input.GetKeyDown(KeyCode.Alpha2)){
			silverBolts.Execute();
		}
		
		else if(Input.GetKeyDown(KeyCode.Alpha3)){
			condemn.Execute();
		}
		
		else if(Input.GetKeyDown(KeyCode.Alpha4)){
			finalHour.Execute();
		}
		
	//	else if(Input.GetButtonDown("Fire2")){ //Right click
	//		valor.Execute();
	//	}
		
		if(running && !a.IsPlaying("Run")) {
			a.Stop();
			a.Play("Run");
			idling = false;
		}
		
		//TODO Add modifications to allow for tumble, ult, and tumbleult idles
		else if (!a.isPlaying)
			a.Play("Idle1");
		
		//Update all spells
		//courage.Update();
		//judgement.Update();
		
		//Pick up any items within range
		
		/* Comment this out until I figure how to change to Vayne
		Collider[] items = Physics.OverlapSphere(gameObject.transform.position, pickUpRange);
		foreach(Collider itemCollider in items){
			PickUp item	= itemCollider.gameObject.GetComponent(typeof(HealthOrbScript)) as PickUp;
			if(item)
				item.trigger(this);
		}
		*/
	
	}
	
	public void playIdleSequence(){
		if(!a.isPlaying){
			int num = ((int) (Random.value * 3)) + 1;
			a.Play("Idle"+num);
		}
		
		idling = true;
	}
	
	public bool isRunAnimation(){
		return a.IsPlaying("Run");	
	}
	
	public bool noAnimation(){
		return !a.isPlaying;	
	}
	
	public bool isAlive(){
		return alive;
	}
	
	public void stopAnimation(){
		a.Stop();	
	}
	
	public float getHealthPercent(){
		return currentHealth / maxHealth;
	}
	
	public void setCurrentEnemy(MinionScript enemy){
		this.currentEnemy = enemy;	
	}
	
	public void setIdling(bool idle){
		idling = idle;
	}

	public MinionScript getCurrentEnemy(){
		return currentEnemy;	
	}
	
	public float getWeaponDamage(){
		return WeaponDamage;	
	}
	public void setWeaponDamage(float dmg){
		WeaponDamage = dmg;	
	}	

	public float getWeaponSpeed(){
		return WeaponSpeed;
	}
	

	public void playAnimation(string animationName){
		a.Stop();
		a.Play(animationName);
	}
	
	public void setRunning(bool active){
		running = active;
	}
	
	public void setTumbled(bool active){
		tumbled = active;
	}
	
	public float getRange(){
		return range;
	}
	
	public void activateFinalHour(bool active){
			finalHourActive = active;
	}
	public void awardHealth(float amount){
		//Calculate additional healing based on passive or skills
		currentHealth += amount;
		if(currentHealth > maxHealth)
			currentHealth = maxHealth;
	}

	public void damage(float amount){
		//Compute damage reductions
	//	if(courageActive)
	//		amount *= 0.7f;
		
		//Apply damage
		currentHealth -= amount;
		if(currentHealth < 0){
			currentHealth = 0;
			alive = false;
			a.Stop();
			a.Play("Death");
		}
		
		//Check for death
	}
	
	/*
	 * Returns the time of the next auto-attack
	 * */
	public float autoAttack(MinionScript enemy){
		float damageAmount = getWeaponDamage();
		if(finalHourActive){
			damageAmount *= 1.4f;
			awardHealth (damageAmount * .5f);
		}
		
			a.Play("Attack" + (((int)(Random.value * 2)) + 1));
		enemy.damage(damageAmount);
		return Time.time + WeaponSpeed;
	}
	
	
	class Tumble : Ability {
		
		private VayneScript player;
		//private double totalDamage = 1.30d;
		private float nextAttack = 0;
		private float coolDown = 6;
		private string animationName = "Spell1";
		
		
		public void Execute(){
			
			//Check if the ability is on cooldown
			if(nextAttack > Time.time)
				return;
			
			player.playAnimation(animationName);
			/*
			 * To find enemies in a cone, we first use Physics.OverlapSphere to find all enemies close to our position.
			 * We then iterate through each enemy, and use the dot product of the direction an forward vectors to determine
			 * if the enemy is in the cone.
			 * */
			/*Vector3 position = player.gameObject.transform.position;
			Collider[] enemies = Physics.OverlapSphere(position, 30);
			foreach(Collider enemyCol in enemies){
				MinionScript enemy = enemyCol.gameObject.GetComponent(typeof(MinionScript)) as MinionScript;
				if(enemy){
					Vector3 direction = Vector3.Normalize(enemy.gameObject.transform.position - player.gameObject.transform.position);
					float dot = Vector3.Dot(direction, player.gameObject.transform.forward);
					if(dot > 0.707f)
						enemy.damage((float)totalDamage);
				}
			}*/
			
			player.setTumbled(true);
			
			nextAttack = Time.time + coolDown;
		}
		
		public void setScript(VayneScript script){
			player = script;		
			//totalDamage = player.getWeaponDamage() * 1.3f;
		}
		
		public float getCooldown(){
			if(Time.time > nextAttack)
				return 0;
			else
				return (float)((nextAttack - Time.time) / coolDown);
		}
		
	}
	
	class Silver_Bolts : Ability {
		private VayneScript player;
		private bool active = false;
		private float cooldown = 10;
		private float nextAttack = 0;
		private string animationName = "Crit";
		private int shotsFired = 0;
	
		
		public void Update(){
			if(nextAttack < Time.time && active && shotsFired == 3){
				active = false;
				//player.activateCourage(false);
				shotsFired = 0;
				Debug.Log ("SilverBolts finished!");
			}
			Execute();
			shotsFired++;
		}
		
		public void Execute(){
			
			//Check if on cooldown
			if(Time.time < nextAttack && shotsFired == 0)
				return;
			
			nextAttack = Time.time + cooldown;
			//player.playAnimation(animationName);
			active = true;
			Debug.Log("SilverBolts active!");
			SilverAttack(active, shotsFired);
			
		}
		
		public void setScript(VayneScript script){
			player = script;
		}
		
		public float getCooldown(){
			if(Time.time > nextAttack)
				return 0;
			else
				return (float)((nextAttack - Time.time) / cooldown);
		}
		
		//TODO Get "run until in range aspect added"
		public void SilverAttack(bool active, int shotsFired){
			//Check for mouse click and update destination
			if(active){
				//idling = false;
				//player.setIdling(false);				
				//movingToAttack = false;				
				//player.setRunning(true);
				
				
				if(Time.time > nextAttack && ((player.getCurrentEnemy().gameObject.transform.position - player.gameObject.transform.position).magnitude <= player.getRange())){
					if(shotsFired == 3){
						float temp = player.getWeaponDamage();
						if(player.finalHourActive){
							temp *= 1.4f;
							player.awardHealth(temp * .5f * 1.3f);
						}								
						player.setWeaponDamage((float)(temp * 1.3f));
						nextAttack = player.autoAttack(player.getCurrentEnemy());
						player.setWeaponDamage(temp);
					}
					else{
						if(player.finalHourActive){
							float temp = player.getWeaponDamage();
							player.awardHealth (temp * 1.4f * .5f * 1.3f);		
							player.setWeaponDamage(temp * 1.4f);
							nextAttack = player.autoAttack(player.getCurrentEnemy());
							player.setWeaponDamage(temp);
						}
						else
							nextAttack = player.autoAttack(player.getCurrentEnemy());
						nextAttack = Time.time + 0.2f; //Override autoattack timing
					}
					
					
					Vector3 enemyLookAt = player.getCurrentEnemy().gameObject.transform.position;
					enemyLookAt.y = 25.20f;
					transform.LookAt(enemyLookAt);
				}

			}
		}
	}
	/*
	 * To simulate damage over time, judgement uses a variable called "numTicks". While numTicks > 0, every
	 * second nearby enemies are damage for totalDamage / numTicks damage (calculated when numTicks is at its original value),
	 * and numTicks is decremented by one. This allows for even distribution of damage over time, and ensures that enemies are 
	 * able to "run away" from the damage zone.
	 * */
	class Condemn : Ability {
		private VayneScript player;
		//private float nextTickTime = 0;
		private int distance = 30;
		//private int numTicks = 0;
		private double totalDamage;
		//private double damagePerTick = 0;
		private double nextAttack = 0;
		private double coolDown = 20;
		private string animationName = "Spell3";
		
		public void Execute(){
			
			//If this ability is on cooldown, don't do anything.
			//TODO Play "On Cooldown" sound?
			if(nextAttack > Time.time)
				return;
			
			player.playAnimation(animationName);
			nextAttack = player.autoAttack(player.getCurrentEnemy());			
			nextAttack = Time.time + coolDown; //Put ability on cooldown
		
		}
		
		public void setScript(VayneScript script){
			player = script;
			totalDamage = 1.0f * player.getWeaponDamage();
		}
		
		public void Update(){
			MinionScript enemy = player.getCurrentEnemy();
			
			if(enemy != null){
				enemy.damage((float)totalDamage);
				int directionX = enemy.getDest().x > gameObject.transform.position.x ? 1 : -1;
				int directionZ = enemy.getDest().z > gameObject.transform.position.z ? 1 : -1;
				//Re-work this to be directed based on Vayne's vector to enemy instead of enemy vector of pursuit of Vayne
				Vector3 newPos = new Vector3(enemy.transform.position.x + (-10 * directionX), enemy.transform.position.y, enemy.transform.position.z + (-10 * directionZ));
				enemy.transform.position = newPos;
				
				//Get collision with environment here//
				enemy.stun (2);
			}
		}
		
		public float getCooldown(){
			if(Time.time > nextAttack)
				return 0;
			else
				return (float)((nextAttack - Time.time) / coolDown);
		}
	}
	
	class Final_Hour : Ability {
		private VayneScript player;
		private float cooldown = 120;
		private float nextAttack = 0;
		private int duration = 8;
		private float bonusDamage = 1.4f;
		private float lifeSteal = 1.5f;
		private bool isUlted = false;
		
		
		private bool active = false;
		
	
		
		public void Update(){
			if(nextAttack < Time.time && active){
				active = false;
				player.activateFinalHour(false);
				Debug.Log("Final Hour finished!");
			}
		}
		
		public void Execute(){
			
			//Check if on cooldown
			if(Time.time < nextAttack)
				return;
			
			player.activateFinalHour(true);
			nextAttack = Time.time + cooldown;
			//player.playAnimation("Ult" + animationName);
			active = true;
			Debug.Log("Final Hour active!");
		}
		
		public void setScript(VayneScript script){
			player = script;
		}
		
		public float getCooldown(){
			if(Time.time > nextAttack)
				return 0;
			else
				return (float)((nextAttack - Time.time) / cooldown);
		}
		
		
	}
	
	class Valor : Ability {
		private VayneScript player;
		private float nextAttack = 0;
		private int numPossibleTargets = 3;
		private int radius = 30;
		private string animationName = "Attack2";
		
		public void Execute(){
			
			//If on cooldown, return
			if(Time.time < nextAttack)
				return;
			
			player.playAnimation(animationName);
			
			nextAttack = Time.time + player.WeaponSpeed;
			Vector3 position = player.gameObject.transform.position;
			MinionScript[] enemies = new MinionScript[numPossibleTargets];
			int index = 0;
			Collider[] targets = Physics.OverlapSphere(position, radius);
			foreach(Collider t in targets){
				MinionScript enemy = player.getCurrentEnemy();
				if(enemy){
					Vector3 direction = Vector3.Normalize(enemy.gameObject.transform.position - player.gameObject.transform.position);
					float dot = Vector3.Dot(direction, player.gameObject.transform.forward);
					if(dot > 0.707f){
						enemies[index++] = enemy;
						if(index > numPossibleTargets - 1) //If the maximum number of targets has been reached, stop adding new ones.
						break;
					}
				}
			}
			
			//If there are no enemies to hit, return
			if(index == 0) 
				return;
			
			float damagePerTarget = player.WeaponDamage * 0.5f;
			foreach(MinionScript enemy in enemies){
				if(enemy)
					enemy.damage(damagePerTarget);
			}
		}
		
		public void setScript(VayneScript script){
			player = script;
		}
	}
}
