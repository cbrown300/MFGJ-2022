using Godot;
using System;

public class GruntEnemy : Node2D
{
    private PlayerController player;
    private AnimatedSprite animatedSprite;
    private bool active = false;    //Activiates enemy to attack mode when player is near
    private bool canShoot = true;   //Allows the enemy to shoot
    private float shootTimer = 1f;  //The timer between shooting
    private float shootTimerReset = 1f; //The reset timer for shooting
    public bool isShooting = false;

    [Export]
    public PackedScene Projectile;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {
    if(active){
        var angle = GlobalPosition.AngleToPoint(player.GlobalPosition);
        if(Mathf.Abs(angle) > Mathf.Pi/2){ //flip Enemy sprite based on player position
            animatedSprite.FlipH = false;
        }else{
            animatedSprite.FlipH = true;

        }
        if(canShoot){
            var spaceState = GetWorld2d().DirectSpaceState; //get access to physics system
            //check if it can see the player
            Godot.Collections.Dictionary result = spaceState.IntersectRay(this.Position, player.Position, new Godot.Collections.Array {this}); //calls a raycast from enemy to player and ignores the enemy collision
            if(result != null){
                if(result.Contains("collider")){
                    this.GetNode<Position2D>("ProjectileSpawn").LookAt(player.Position); //makes projectile look at player
                    if(result["collider"] == player){
                        animatedSprite.Play("ShootingStart");                        
                    }
                }
            }
            
            
        }
    }else{
        if(!isShooting){
            animatedSprite.Play("Idle");
        }
    }
    if(shootTimer <= 0){
        canShoot = true;
    }else{
        shootTimer -= delta;
    }
  }

    //Manages shooting at player
    private void shootAtPlayer(){
        GD.Print("Shooting");
        isShooting = true;
        GruntProjectile projectile = Projectile.Instance() as GruntProjectile;
        Owner.AddChild(projectile);
        projectile.GlobalTransform = this.GetNode<Position2D>("ProjectileSpawn").GlobalTransform;
        canShoot = false;
        shootTimer = shootTimerReset;
    }

    //Manages setting enemy to active mode
    private void _on_Detection_Radius_body_entered(object body){
        GD.Print("Body has entered " + body);
        if(body is PlayerController){
            player = body as PlayerController;
            active = true;
        }
    }

    //Manages to set enemy to non-active(calm) mode
    private void _on_Detection_Radius_body_exited(object body){
        GD.Print("Body has exited " + body);
        if(body is PlayerController){
            active = false;
        }
    }

    //Manages to play and split shooting animation 
    private void _on_AnimatedSprite_animation_finished(){
        if(animatedSprite.Animation == "ShootingStart"){
            shootAtPlayer();
            animatedSprite.Play("ShootingEnd");

		}
        if(animatedSprite.Animation == "ShootingEnd"){
            //animatedSprite.Play("Idle");
            isShooting = false;
        }
    }
}
