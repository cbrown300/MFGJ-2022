using Godot;
using System;

public class Door : Node2D
{

    [Export]
    public string DoorKey; //Name of key that goes to this door, must match DoorToOpen

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    //Manages when the player touches the door
    public void _on_Area2D_body_entered(object body){
        if(body is PlayerController){
            if(GameManager.Player.Keys.FindAll(k => k.DoorToOpen == DoorKey).Count != 0){ //Find a list of keys matching the door
                Key k = GameManager.Player.Keys.Find(x => x.DoorToOpen.Contains(DoorKey)); // pick a key from the list
                GameManager.Player.Keys.Remove(k); //remove key from list
                k.QueueFree(); //delete key
                GD.Print("Open Door");
                GetNode<AnimationPlayer>("AnimationPlayer").Play("OpenDoor");
            }else{
                GD.Print("Need Key");
            }
        }
    }
}
