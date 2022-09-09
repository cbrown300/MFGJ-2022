using Godot;
using System;
using System.Collections.Generic;

public class PlayerController : KinematicBody2D
{
	public enum PlayerState{ //names of states player can be in
		Idle,
		Running,
		Attacking,
		Climbing,
		Dashing,
		TakingDamage,
		Dead,
		WallJump
	}
	public PlayerState CurrentState = PlayerState.Idle;
	private Vector2 velocity = new Vector2();	//the players movement
	private Vector2 facingDirection = new Vector2(0,0);	//the players faciong direction
	private int speed = 200;	//the speed the player moves at
	private int gravity = 300;	//the gravity that affects the player
	private int jumpHeight = 200;	//the players jump height
	private float friction = .1f; //how fast the player slows down after letting go of direction
	private float acceleration = .25f; //how fast the player speeds up when pressing a direction
	private int dashSpeed = 500;	//the speed the player dashes at
	private float dashTimer = .2f;	//how long the dash lasts
	private float dashTimerReset = .2f;	//reset for dash
	private bool isDashAvailable = true;	
	private bool isWallJumping = false;
	private float wallJumpTimer = .4f;	//how long the player jumps from the wall
	private float wallJumpTimerReset = .4f;	//reset for wall jump
	private int climbSpeed = 100;	//speed player moves up.down when climbing
	private float climbTimer = 5f;	//how long the player can climb
	private float climbTimerReset = 5f;	//reset for climb
	public float health = 3;	//the players health
	public float maxHealth = 3;	//the players health reset/max amount it can go up to
	private float mana = 100f;	//the players mana
	private float maxMana = 100f;	//the players mana reset/max amount it can go up to
	private float manaRegen = 2;	//how fast the players mana regens
	private float manaTimer = 2f;	//how long before mana can start regenerating after an action
	private float manaTimerReset = 2f;	//mana timer delay
	private float dashCost = 10f;	//mana cast for a dash
	public List<Key> Keys = new List<Key>();	//list of keys the player is currently holding
	public bool pauseInput;		//stops the player from being able to move
	private bool isJumping = false;
	private float damageTimer = .3f;	//timer for invicibility frames/damage animation
	private float damageTimerReset = .3f;	//reset timer for damage
	public float XP = 0;	//the players current experience
	
	[Export]
	public PackedScene GhostPlayerInstance;	//the players dash animation
	private AnimatedSprite animatedSprite;
	private RayCast2D rayCastLeft;	//raycast to check for left collision
	private RayCast2D rayCastRight;	//raycast to check for right collision

	[Signal]
	public delegate void Death();	//signal for respawn point when player dies


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		GameManager.Player = this;
		InterfaceManager.updateHealth(maxHealth, health);
		rayCastLeft = GetNode<RayCast2D>("RayCastLeft");
		rayCastRight = GetNode<RayCast2D>("RayCastRight");
	}

    // Called every frame the player moves. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(float delta)
	{
		InterfaceManager.updateMana(maxMana, mana);
		InterfaceManager.updateHealth(maxHealth, health);
		if(!pauseInput){
			inputManager(delta);
			processGravity(delta);
		}
		switch(CurrentState){
			case PlayerState.Idle:
				idle(delta);
				break;
			case PlayerState.Running:
				processMovement(delta);
				break;
			case PlayerState.Attacking:
				attack();
				break;
			case PlayerState.Climbing:
				processClimb(delta);
				break;
			case PlayerState.Dashing:
				processDash(delta);
				break;
			case PlayerState.TakingDamage:
				break;
			case PlayerState.Dead:
				break;
			case PlayerState.WallJump:
				processWallJump(delta);
				break;
		}
		processTimers(delta);
		MoveAndSlide(velocity, Vector2.Up);
	}

	//Manages the inputs for the player if an action is pressed
	private void inputManager(float delta){
		facingDirection = new Vector2(0,0);
		if(Input.IsActionJustPressed("attack")){
			attack();
		}
		if(Input.IsActionJustPressed("switch_spell")){
			GameManager.MagicController.SwitchSpell();
		}
		if(Input.IsActionJustPressed("ui_accept")){
			processInteraction();
		}
		if(Input.IsActionPressed("ui_left") && CurrentState != PlayerState.Climbing){
			facingDirection.x -= 1;
			animatedSprite.FlipH = true;
			CurrentState = PlayerState.Running;
		}
		if(Input.IsActionPressed("ui_right") && CurrentState != PlayerState.Climbing){
			facingDirection.x += 1;
			animatedSprite.FlipH = false;
			CurrentState = PlayerState.Running;
		}
		if(Input.IsActionPressed("ui_up")){
			facingDirection.y = -1;
		}
		if(Input.IsActionPressed("ui_down")){
			facingDirection.y = 1;
		}
		if(Input.IsActionPressed("ui_down") && Input.IsActionJustPressed("jump")){
			Position = new Vector2(Position.x, Position.y + 2); //allow to fall through 1 way platforms
		}else
		if(Input.IsActionJustPressed("jump")){
			if(IsOnFloor()){
				isJumping = true;
			}
			Node obj = rayCastLeft.IsColliding() 
			? (Node)rayCastLeft.GetCollider() : rayCastRight.IsColliding() 
			? (Node)rayCastRight.GetCollider() : null; //if raycast is colliding get object, else null
			if(obj == null){
				return; //nothing is colliding with left or right
			}else if(obj.IsInGroup("WallJumpable")){
				CurrentState = PlayerState.WallJump;
			}
		}else if(Input.IsActionJustPressed("dash")){
			CurrentState = PlayerState.Dashing;
		}

		if(Input.IsActionJustPressed("climb") && climbTimer > 0 && (rayCastLeft.IsColliding() || rayCastRight.IsColliding())){
			CurrentState = PlayerState.Climbing;
		}
		if(Input.IsActionJustReleased("climb")){
			CurrentState = PlayerState.Idle;
		}
	}

	//handles all timer related tasks all on the physics process
	private void processTimers(float delta){
		switch(CurrentState){
			case PlayerState.Dashing:
				if(!isDashAvailable){
					dashTimer -= delta;
					GhostPlayer ghost = GhostPlayerInstance.Instance() as GhostPlayer; //create ghost character
					Owner.AddChild(ghost); //connect ghost outside of player
					ghost.GlobalPosition = this.GlobalPosition; //move ghost to player location
					ghost.SetHValue(animatedSprite.FlipH);
					if(dashTimer <=0){
						velocity = new Vector2(0,0);
						CurrentState = PlayerState.Idle;
						pauseInput = false;
						isDashAvailable = true;
					}
				}
				break;
			case PlayerState.Climbing:
				climbTimer -= delta;
				if(climbTimer <= 0){
					CurrentState = PlayerState.Idle;
				}
				break;
			case PlayerState.WallJump:
				if(isWallJumping){
					wallJumpTimer -= delta;
					if(wallJumpTimer <= 0){
						isWallJumping = false;
						wallJumpTimer = wallJumpTimerReset;
						CurrentState = PlayerState.Idle;
						pauseInput = false;
					}
					
				}
				break;
			case PlayerState.TakingDamage:
				damageTimer -= delta;
				if(damageTimer <= 0){
					pauseInput = false;
					damageTimer = damageTimerReset;
					CurrentState = PlayerState.Idle;
				}
				break;

		}

		if(mana < maxMana && manaTimer <= 0){
			updateMana(delta * manaRegen);
			//GD.Print("ManaRegen: " + mana);
		}else if(mana != 100){
			manaTimer -= delta * 1;
		}
	}

	//Handles the speed for how fast the player falls
	private void processGravity(float delta){
		velocity.y += gravity * delta; //falling at gravity * time
		if(velocity.y > gravity){
			velocity.y = gravity;
		}
	}

	//Manages the process for the player being idle
	private void idle(float delta){
		processJump(delta); //check if jumping
		if(Mathf.Abs(velocity.x) < 5){
			if(IsOnFloor()){
				climbTimer = climbTimerReset;
				animatedSprite.Play("Idle");
				velocity.x = 0;
			}else{
				animatedSprite.Play("Jump");
			}
		}else{
			velocity.x = Mathf.Lerp(velocity.x , 0, friction); //slow player down slightly if they let go of direction
		}
		
	}

	//Manages the process for the player dash
	private void processDash(float delta){
		if(isDashAvailable && mana >= dashCost){
				dashTimer = dashTimerReset;
				isDashAvailable = false;
				updateMana(-dashCost);
				GD.Print("mana: " + mana);
				manaTimer = manaTimerReset;
				velocity = new Vector2(dashSpeed * facingDirection.x, dashSpeed * facingDirection.y);
				pauseInput = true;
		}
	}

	//Manages the process when the player interacts with an npc or pickupable item
	private void processInteraction(){
		Node obj = rayCastLeft.IsColliding() 
			? (Node)rayCastLeft.GetCollider() : rayCastRight.IsColliding() 
			? (Node)rayCastRight.GetCollider() : null;
		if(obj is null){
			return;
		}
		if(obj.Owner is Pickupable){
			if(obj.Owner is MagicPotion){
				MagicPotion potion = obj.Owner as MagicPotion;
				potion.usePotion();

			}
		}else if(obj is NPC){
			NPC npc = obj as NPC;
			npc.setNPCDialog();
			InterfaceManager.dialogManager.showDialogElement();
		}
	}

	//Manages the process for running
	private void processMovement(float delta){
		processJump(delta);
		if(!IsOnFloor()){
			animatedSprite.Play("Jump");
		}else{
			climbTimer = climbTimerReset; //reset climb Timer in case they go straight to running on the floor after climbing
			animatedSprite.Play("Run");
		}
		velocity.x = Mathf.Lerp(velocity.x,facingDirection.x * speed, acceleration);	//accelerates the player to max speed
		if(Mathf.Abs(velocity.x) > speed){
			velocity.x = speed * facingDirection.x;
		}
		CurrentState = PlayerState.Idle;
	}

	//Manages the process for jumping
	private void processJump(float delta){
		if(isJumping && IsOnFloor()){
			isJumping = false;
			velocity.y = -jumpHeight;
			animatedSprite.Play("Jump");
		}
	}

	//Manages the process for wall jumping
	private void processWallJump(float delta){
		if(!isWallJumping){
			if(animatedSprite.FlipH){
				velocity.y = -jumpHeight;
				velocity.x = jumpHeight;
				isWallJumping = true;
				animatedSprite.FlipH = false;
			}else{
				velocity.y = -jumpHeight;
				velocity.x = -jumpHeight;
				isWallJumping = true;
				animatedSprite.FlipH = true;
			}
			pauseInput =  true;
		}
		animatedSprite.Play("Jump");
	}

	//Manages the process for climbing
	private void processClimb(float delta){
		if(rayCastLeft.IsColliding() || rayCastRight.IsColliding()){	
			velocity.y = climbSpeed * facingDirection.y;
		}
	}

	//Manages the process for when the player takes damage
	public void takeDamage(){
		GD.Print("Player has taken damage");
		if(health > 0){
			health -= 1;
			GD.Print("Player Health: " + health);
			InterfaceManager.updateHealth(maxHealth, health);
			velocity = new Vector2(100f * (animatedSprite.FlipH ? 1: -1), -100); //knockback player
			animatedSprite.Play("Take Damage");
			pauseInput = true;
			CurrentState = PlayerState.TakingDamage;
			if(health <= 0){
				health = 0;
				GD.Print("Player has died!");
				animatedSprite.Play("Death");
				CurrentState = PlayerState.Dead;
			}
		}
	}

	//process to signal to stop playing death animation
	private void _on_AnimatedSprite_animation_finished(){
		if(animatedSprite.Animation == "Death"){
			GD.Print("Death Animation Finished");
			animatedSprite.Stop();
			Hide();
			EmitSignal(nameof(Death));
			GameManager.GlobalGameManager.RespawnPlayer();
		}
	}

	//Manages the process to respawn player 
	public void respawnPlayer(){
		Show();
		//reset everything for the player
		health = maxHealth;
		mana = maxMana;
		velocity = new Vector2(0,0);
		InterfaceManager.updateHealth(maxHealth, health);
		InterfaceManager.updateMana(maxMana, mana);
		CurrentState = PlayerState.Idle;
		pauseInput = false;
	}

	//Manages the process for increasing and decreasing the players mana
	public void updateMana(float manaAmount){
		mana += manaAmount;
		if(mana >= maxMana){
			mana = maxMana;
		}else if (mana <= 0){
			mana = 0;
		}
	}

	//Manages the process for the players attacks
	private void attack(){
		Spell currentSpell = GameManager.MagicController.EquippedSpell.SpellScene.Instance() as Spell;
		if(mana >= currentSpell.ManaCost){ //stop player from casting if they dont have the mana
			GD.Print("mana: " + mana);
			GD.Print("SpellMana: " + currentSpell.ManaCost);
			GameManager.MagicController.CastSpell(GameManager.Player.GetNode<AnimatedSprite>("AnimatedSprite").FlipH);
		}
	}

}
