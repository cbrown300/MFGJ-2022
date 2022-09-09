using Godot;
using System;

public class IceKnife : Spell
{
    public string ResourcePath = "res://Spells/IceKnife.tscn";
    AnimationPlayer player;
    [Export]
    public bool canMove;    //allows the spell to move across the screen

    public IceKnife(){
        InterfaceTexturePath = "res://Assets/spells/IceKnife/I5050-7.png";
        InterfaceTexture = ResourceLoader.Load(InterfaceTexturePath) as Texture;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        player = GetNode<AnimationPlayer>("AnimationPlayer");
        player.Play("Cast");
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        if(canMove){
            if(!player.IsPlaying()){
                player.Play("Idle");
            }
            if(faceDirection){
                Position -= (Transform.x * delta * Speed);
            }else{
                Position += (Transform.x * delta * Speed);
            }
            LifeSpan -= delta;
            if(LifeSpan < 0){
                QueueFree();
            }
        }
    }

    public override void SetUp(bool faceDirect)
    {
        //throw new NotImplementedException();
        GetNode<Sprite>("Sprite").FlipH = faceDirect;
        faceDirection = faceDirect;
    }

    public override void CastSpell()
    {
        //throw new NotImplementedException();
    }

    public override void LoadResourcePath()
    {
        //throw new NotImplementedException();
    }

    public void _on_Area2D_body_entered(object body){
        player.Play("Finish");
        if(body is SlimeEnemy){
            SlimeEnemy slime = body as SlimeEnemy;
            slime.takeDamage(DamageAmount);
        }
    }
}
