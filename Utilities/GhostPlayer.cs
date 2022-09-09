using Godot;
using System;

//Handles the process for the dash sprite
public class GhostPlayer : Node2D
{

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("FadeOut");
    }

    public void SetHValue(bool value){
        GetNode<Sprite>("Sprite").FlipH = value;
    }
    
    public void Destroy(){
        QueueFree();
    }
}
