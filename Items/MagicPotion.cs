using Godot;
using System;

public class MagicPotion : Pickupable
{
    public float manaRegenAmount = 20f; //Mana potion regenerates for player
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("Bounce");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    private void _on_Area2D_body_entered(object body){
        if(body is PlayerController){
            GetNode<RichTextLabel>("Node2D/RichTextLabel").Show();
        }
    }

    private void _on_Area2D_body_exited(object body){
        if(body is PlayerController){
            GetNode<RichTextLabel>("Node2D/RichTextLabel").Hide();
        }
    }

    //Manager for using potion
    public void usePotion(){
        GameManager.Player.updateMana(manaRegenAmount);
        QueueFree();
    }
}
