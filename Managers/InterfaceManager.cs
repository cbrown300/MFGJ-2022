using Godot;
using System;

public class InterfaceManager : CanvasLayer
{
    public static DialogManager dialogManager;
    public static ProgressBar healthBar;
    public static ProgressBar manaBar;
    public static TextureRect spellTextureRect;
    public static QuestInterfaceManager questInterfaceManager;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        dialogManager = GetNode("DialogManager") as DialogManager;
        healthBar = GetNode("MainInterface/HealthBar") as ProgressBar;
        manaBar = GetNode("MainInterface/ManaBar") as ProgressBar;
        spellTextureRect = GetNode("MainInterface/MagicSpellPanel/MagicSpellTexture") as TextureRect;
        questInterfaceManager = GetNode("QuestInterfaceManager") as QuestInterfaceManager;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    //Manages the display for the health bar
    public static void updateHealth(float maxHealth, float health){
        healthBar.Value = (health / maxHealth) * healthBar.MaxValue;
    }

    //Manages the display for the mana bar
    public static void updateMana(float maxMana, float mana){
        manaBar.Value = (mana / maxMana) * manaBar.MaxValue;
    }

    //Manages the display image for the spell selection
    public static void SetSpellSprite(Texture spellImage){
        spellTextureRect.Texture = spellImage;
    }
}
