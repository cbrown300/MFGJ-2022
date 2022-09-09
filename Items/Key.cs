using Godot;
using System;

public class Key : KinematicBody2D
{
    private Vector2 velocity = new Vector2(0,0);
    private bool followingPlayer;   //handles if key is picked up and following player

    [Export]
    public string DoorToOpen;       //Name of door to open, must match DoorKey

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    //Manages how the key moves following the player
    public override void _PhysicsProcess(float delta)
    {
        if(followingPlayer){
            if(GameManager.Player.GetNode<AnimatedSprite>("AnimatedSprite").FlipH){
                velocity = GameManager.Player.GetNode<Position2D>("KeyFollowPositionRight").GlobalPosition - GlobalPosition;
            }else{
                velocity = GameManager.Player.GetNode<Position2D>("KeyFollowPositionLeft").GlobalPosition - GlobalPosition;
            }
        }
        MoveAndSlide(velocity * 5f, Vector2.Up);
    }

    //Manager for when player picks up the key
    private void _on_Area2D_body_entered(object body){
        if(!followingPlayer && body is PlayerController){
            followingPlayer = true;
            GameManager.Player.Keys.Add(this);
        }
    }
}
