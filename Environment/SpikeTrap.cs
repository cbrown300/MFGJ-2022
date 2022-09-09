using Godot;
using System;

public class SpikeTrap : Node2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    //Manager for damagind the player when they touch the spikes
    private void _on_Area2D_body_entered(object body){
        GD.Print("Body: " + body + " has entered");
        if(body is KinematicBody2D){
            if(body is PlayerController){
                PlayerController pc = body as PlayerController;
                pc.takeDamage();
            }
        }
    }
}
