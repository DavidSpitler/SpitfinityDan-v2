using UnityEngine;
using System.Collections;

public class GarenScript : MonoBehaviour {
	
	//Spells
	private Decisive_Strike dStrike;
	private Courage courage;
	private Judgement judgement;
	private Demacian_Justice dJustice;
	private Valor valor;
	
	//Spell textures
	public Texture2D dStrikeTexture;
	public Texture2D courageTexture;
	public Texture2D judgementTexture;
	public Texture2D dJusticeTexture;
	
	
	//Health textures
	public Texture2D enemyHealthTexture;
	public Texture2D playerHealthTexture;
	
	private bool alive;
	
	
	private float nextAttack = 0;
	private float currentHealth;
	private float maxHealth = 100;
	private bool courageActive;
	private Quaternion originalRotation;
	private bool spinning;
	private Animation a;
	private bool running;
	private float range = 30;
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
		
		GUI.Button(new Rect(0, 0, IconWidth, IconHeight), dStrikeTexture);
		GUI.Button(new Rect(IconWidth, 0, IconWidth, IconHeight), courageTexture);
		GUI.Button(new Rect(IconWidth * 2, 0, IconWidth, IconHeight), judgementTexture);
		GUI.Button(new Rect(IconWidth * 3, 0, IconWidth, IconHeight), dJusticeTexture);

		GUI.EndGroup();
		
		//Creates mask over buttons based on cooldown.
		
		GUI.BeginGroup(new Rect(Screen.width / 2 - (2 * IconWidth), Screen.height - 70, IconWidth * 4, IconHeight * 4));
		
		GUI.Button(new Rect(0, 0, IconWidth, IconHeight * dStrike.getCooldown()), "");
		GUI.Button(new Rect(IconWidth, 0, IconWidth, IconHeight * courage.getCooldown()), "");
		GUI.Button(new Rect(IconWidth * 2, 0, IconWidth, IconHeight * judgement.getCooldown()), "");
		GUI.Button(new Rect(IconWidth * 3, 0, IconWidth, IconHeight * dJustice.getCooldown()), "");
		
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
		dStrike = new Decisive_Strike();
		dStrike.setScript(this);
		courage = new Courage();
		courage.setScript(this);
		judgement = new Judgement();
		judgement.setScript(this);
		dJustice = new Demacian_Justice();
		dJustice.setScript(this);
		valor = new Valor();
		valor.setScript(this);
		
		IconWidth = judgementTexture.width;
		IconHeight = judgementTexture.height;
		
		//Initially not running
		running = false;
		
		idling = true;
		
		a = gameObject.GetComponent(typeof(Animation)) as Animation;
		
		originalRotation = transform.localRotation;
		spinning = false;
		
		alive = true;
	}
	
	
	// Update is called once per frame
	void Update () {
		if(!alive)
			return;
		
		//Check for spells being cast
		if(Input.GetKeyDown(KeyCode.Alpha1)){
			dStrike.Execute();
		}
		
		else if(Input.GetKeyDown(KeyCode.Alpha2)){
			courage.Execute();
		}
		
		else if(Input.GetKeyDown(KeyCode.Alpha3)){
			judgement.Execute();
		}
		
		else if(Input.GetKeyDown(KeyCode.Alpha4)){
			dJustice.Execute();
		}
		
		else if(Input.GetButtonDown("Fire2")){ //Right click
			valor.Execute();
		}
		
		if(running && !a.IsPlaying("Run") && !a.IsPlaying("Spell3")) {
			a.Stop();
			a.Play("Run");
			idling = false;
		}
		else if (!a.isPlaying)
			a.Play("Idle1");
		
		//Update all spells
		courage.Update();
		judgement.Update();
		
		//Pick up any items within range
		Collider[] items = Physics.OverlapSphere(gameObject.transform.position, pickUpRange);
		foreach(Collider itemCollider in items){
			PickUp item	= itemCollider.gameObject.GetComponent(typeof(HealthOrbScript)) as PickUp;
			if(item)
				item.trigger(this);
		}
	
	}
	
	public void playIdleSequence(){
		if(!a.isPlaying){
			int num = ((int) (Random.value * 4)) + 1;
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
	
	public void setSpinning(bool active){
		string animationName = active?"Spell3":"Idle2";
		spinning = active;
			a.Stop();
			a.Play(animationName);
	}
	
	public bool isSpinning(){
		return spinning;	
	}
	
	public void activateCourage(bool active){
			courageActive = active;
	}
	
	public float getRange(){
		return range;
	}
	
	public void awardHealth(float amount){
		//Calculate additional healing based on passive or skills
		currentHealth += amount;
		if(currentHealth > maxHealth)
			currentHealth = maxHealth;
	}
	
	public void damage(float amount){
		//Compute damage reductions
		if(courageActive)
			amount *= 0.7f;
		
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
			a.Play("Attack" + (((int)(Random.value * 3)) + 1));
		enemy.damage(WeaponDamage);
		return Time.time + WeaponSpeed;
	}
	
	
	class Decisive_Strike : Ability {
		
		private GarenScript player;
		private double totalDamage;
		private float nextAttack = 0;
		private float coolDown = 5;
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
			Vector3 position = player.gameObject.transform.position;
			Collider[] enemies = Physics.OverlapSphere(position, 30);
			foreach(Collider enemyCol in enemies){
				MinionScript enemy = enemyCol.gameObject.GetComponent(typeof(MinionScript)) as MinionScript;
				if(enemy){
					Vector3 direction = Vector3.Normalize(enemy.gameObject.transform.position - player.gameObject.transform.position);
					float dot = Vector3.Dot(direction, player.gameObject.transform.forward);
					if(dot > 0.707f)
						enemy.damage((float)totalDamage);
				}
			}
			
			nextAttack = Time.time + coolDown;
		}
		
		public void setScript(GarenScript script){
			player = script;
			
			totalDamage = player.getWeaponDamage() * 1.6f;
		}
		
		public float getCooldown(){
			if(Time.time > nextAttack)
				return 0;
			else
				return (float)((nextAttack - Time.time) / coolDown);
		}
		
	}
	
	class Courage : Ability {
		private GarenScript player;
		private bool active = false;
		private float cooldown = 5;
		private float nextAttack = 0;
		private string animationName = "Spell2";
	
		
		public void Update(){
			if(nextAttack < Time.time && active){
				active = false;
				player.activateCourage(false);
				Debug.Log("Courage finished!");
			}
		}
		
		public void Execute(){
			
			//Check if on cooldown
			if(Time.time < nextAttack)
				return;
			
			player.activateCourage(true);
			nextAttack = Time.time + cooldown;
			player.playAnimation(animationName);
			active = true;
			Debug.Log("Courage active!");
		}
		
		public void setScript(GarenScript script){
			player = script;
		}
		
		public float getCooldown(){
			if(Time.time > nextAttack)
				return 0;
			else
				return (float)((nextAttack - Time.time) / cooldown);
		}
	}
	
	/*
	 * To simulate damage over time, judgement uses a variable called "numTicks". While numTicks > 0, every
	 * second nearby enemies are damage for totalDamage / numTicks damage (calculated when numTicks is at its original value),
	 * and numTicks is decremented by one. This allows for even distribution of damage over time, and ensures that enemies are 
	 * able to "run away" from the damage zone.
	 * */
	class Judgement : Ability {
		private GarenScript player;
		private float nextTickTime = 0;
		private int radius = 30;
		private int numTicks = 0;
		private double totalDamage;
		private double damagePerTick = 0;
		private double nextAttack = 0;
		private double coolDown = 10;
		public void Execute(){
			
			//If this ability is on cooldown, don't do anything.
			//TODO Play "On Cooldown" sound?
			if(nextAttack > Time.time)
				return;
			
			numTicks = 3;
			damagePerTick = totalDamage / (double)numTicks; //TODO Calculate damage using equipment and skills
			nextAttack = Time.time + coolDown; //Put ability on cooldown
			
			//SPIN TO WIN!!!!! (animation)
			player.setSpinning(true);
		}
		
		public void setScript(GarenScript script){
			player = script;
			totalDamage = 1.8f * player.getWeaponDamage();
		}
		
		public void Update(){
			
			//If there are still ticks of damage to be applied, and it's time for another tick.
			if(numTicks > 0 && Time.time > nextTickTime){
				Vector3 position = player.gameObject.transform.position;
				Collider[] targets = Physics.OverlapSphere(position, radius);
				foreach(Collider t in targets){
					MinionScript enemy = t.gameObject.GetComponent(typeof(MinionScript)) as MinionScript;
					if(enemy){
						enemy.damage((float)damagePerTick);
					}
				}
				
				nextTickTime = Time.time + 1;
				numTicks--;
			}
			
			//If the player is spinning, but no damage can be applied, stop spinning.
			else if(numTicks <= 0 && player.isSpinning()){
				player.setSpinning(false);
			}
		}
		
		public float getCooldown(){
			if(Time.time > nextAttack)
				return 0;
			else
				return (float)((nextAttack - Time.time) / coolDown);
		}
		
	}
	
	class Demacian_Justice : Ability {
		private GarenScript player;
		private float cooldown = 120;
		private float nextAttack = 0;
		private int numPossibleTargets = 5;
		private int radius = 60;
		private string animationName = "Spell4";
		
		public void Execute(){
			
			//If on cooldown, return
			if(Time.time < nextAttack)
				return;
			
			player.playAnimation(animationName);
			
			nextAttack = Time.time + cooldown;
			Vector3 position = player.gameObject.transform.position;
			MinionScript[] enemies = new MinionScript[numPossibleTargets];
			int index = 0;
			Collider[] targets = Physics.OverlapSphere(position, radius);
			foreach(Collider t in targets){
				MinionScript enemy = t.gameObject.GetComponent(typeof(MinionScript)) as MinionScript;
				if(enemy){
				enemies[index++] = enemy;
				if(index > numPossibleTargets - 1) //If the maximum number of targets has been reached, stop adding new ones.
					break;
					}
				}
			
			//If there are no enemies to hit, return
			if(index == 0) 
				return;
			
			float damagePerTarget = (player.WeaponDamage * 3.0f) / index;
			foreach(MinionScript enemy in enemies){
				if(enemy){
					enemy.damage(damagePerTarget);
					enemy.stun(1.5f);
				}
			}
		}
		
		public void setScript(GarenScript script){
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
		private GarenScript player;
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
				MinionScript enemy = t.gameObject.GetComponent(typeof(MinionScript)) as MinionScript;
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
		
		public void setScript(GarenScript script){
			player = script;
		}
	}

}
