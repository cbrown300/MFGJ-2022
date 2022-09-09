using Godot;
using System;

public abstract class Spell : Node2D
{
    public bool faceDirection;
    [Export]
    public float DamageAmount;
    [Export]
    public int Speed;
    [Export]
    public float ManaCost;
    [Export]
    public float LifeSpan;
    public PackedScene SpellScene;
    public string InterfaceTexturePath;
    public Texture InterfaceTexture; 
    public abstract void CastSpell();
    public abstract void LoadResourcePath();
    public abstract void SetUp(bool faceDirection);
}
