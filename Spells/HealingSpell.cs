using Godot;
using System;

public class HealingSpell : Spell
{
    public string ResourcePath = "res://Spells/HealingSpell.tscn";
    public bool PlayerInArea;

    public HealingSpell(){
        InterfaceTexturePath = "res://Assets/spells/Heal/1.png";
        InterfaceTexture = ResourceLoader.Load(InterfaceTexturePath) as Texture;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("Idle");
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _PhysicsProcess(float delta)
        {
            if(PlayerInArea){   //only allow to heal player when they are touching the spell
                if(GameManager.Player.health < GameManager.Player.maxHealth){
                    GameManager.Player.health += delta * DamageAmount;
                } 
            }
            LifeSpan -= delta;
            if(LifeSpan < 0){
                QueueFree();
            }
        }

    public override void SetUp(bool faceDirect)
    {
        GetNode<Sprite>("Sprite").FlipH = faceDirect;
        faceDirection = faceDirect;
    }

    public override void LoadResourcePath()
    {
        throw new NotImplementedException();
    }

    public override void CastSpell()
    {
        throw new NotImplementedException();
    }

    public void _on_Area2D_body_entered(object body){
        if(body is PlayerController){
            PlayerInArea = true;
        }
    }

    public void _on_Area2D_body_exited(object body){
        if(body is PlayerController){
            PlayerInArea = false;
        }
    }
}
