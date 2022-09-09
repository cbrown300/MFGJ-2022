using Godot;
using System;
using System.Collections.Generic;

public class MagicController : Node
{
    public Spell EquippedSpell;
    public List<Spell> AvailableSpells = new List<Spell>();
    private int currentCount;

    // Called when the node is constructed.
    public MagicController()
    {
        IceKnife iceKnife = new IceKnife();
        iceKnife.SpellScene = (ResourceLoader.Load(iceKnife.ResourcePath) as PackedScene);
        AvailableSpells.Add(iceKnife);
        HealingSpell healingSpell = new HealingSpell();
        healingSpell.SpellScene = (ResourceLoader.Load(healingSpell.ResourcePath) as PackedScene);
        AvailableSpells.Add(healingSpell);
        FireBall fireBall = new FireBall();
        fireBall.SpellScene = (ResourceLoader.Load(fireBall.ResourcePath) as PackedScene);
        AvailableSpells.Add(fireBall);
        EquippedSpell = AvailableSpells[0];
        InterfaceManager.SetSpellSprite(EquippedSpell.InterfaceTexture);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    //Manages the spawning of a spell
    public void CastSpell(bool faceDirection){
        Spell currentSpell = EquippedSpell.SpellScene.Instance() as Spell;
        currentSpell.SetUp(faceDirection);
        if(faceDirection){
            currentSpell.GlobalPosition = GameManager.Player.GetNode<Position2D>("SpellCastLeft").GlobalPosition;
        }else{
            currentSpell.GlobalPosition = GameManager.Player.GetNode<Position2D>("SpellCastRight").GlobalPosition;
        }
        GameManager.GlobalGameManager.AddChild(currentSpell);
        GameManager.Player.updateMana(-currentSpell.ManaCost);
    }

    //Manages the switching of spells
    public void SwitchSpell(){
        currentCount += 1;
        if(AvailableSpells.Count - 1 < currentCount){
            currentCount = 0;
        }
        EquippedSpell = AvailableSpells[currentCount];
        InterfaceManager.SetSpellSprite(EquippedSpell.InterfaceTexture);
    }
}
