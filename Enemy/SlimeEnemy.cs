using Godot;
using System;

public class SlimeEnemy : KinematicBody2D
{
    public int id = 1;
    Sprite sprite;
    RayCast2D bottomLeft;   //for checking collision in case of falling off left
    RayCast2D bottomRight;  //for checking collision in case of falling off right
    RayCast2D leftMiddle;   //for checking collision in case of hitting wall on left
    RayCast2D rightMiddle;  //for checking collision in case of hitting wall on right
    private Vector2 velocity;
    private int gravity = 300;
    private int speed = 30;
    private AnimationPlayer animationPlayer;
    private float health = 1;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        sprite = GetNode<Sprite>("Sprite");
        bottomLeft = GetNode<RayCast2D>("RayCastBottomLeft");
        bottomRight = GetNode<RayCast2D>("RayCastBottomRight");
        leftMiddle = GetNode<RayCast2D>("RayCastLeft");
        rightMiddle = GetNode<RayCast2D>("RayCastRight");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        velocity.x = speed;
    }

    public override void _PhysicsProcess(float delta)
    {
        velocity.y += gravity * delta;
        if(velocity.y > gravity){ //stop the character from moving too fast
            velocity.y = gravity;
        }
        if(!bottomRight.IsColliding()){
            //GD.Print("BR collide");
            velocity.x = -speed;
            sprite.FlipH = false;
        }else if(!bottomLeft.IsColliding()){
            //GD.Print("BL collide");
            velocity.x = speed;
            sprite.FlipH = true;
        }else if(rightMiddle.IsColliding()){
            //GD.Print("RM collide");
            velocity.x = -speed;
            sprite.FlipH = false;
        }else if(leftMiddle.IsColliding()){
            //GD.Print("LM collide");
            velocity.x = speed;
            sprite.FlipH = true;
        }
        if(!animationPlayer.IsPlaying()){
            animationPlayer.Play("Move");
        }
        MoveAndSlide(velocity, Vector2.Up);
    }

    //Manages damaging the player if they touch the enemy
    public void _on_Area2D_body_entered(object body){
        if(body is PlayerController){
            PlayerController player = body as PlayerController;
            player.takeDamage();
        }
    }

    //Manger to damage slime when they are attacked
    public void takeDamage(float damageAmount){
        health -= damageAmount;
        if(health <= 0){
            GameManager.QuestManager.UpdateQuest(this);
            QueueFree();
        }
    }
}
