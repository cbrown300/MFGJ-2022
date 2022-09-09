using Godot;
using System;

public class GameManager : Node2D
{
    public bool GamePaused = false;

    [Export]
    public Position2D RespawnPoint;
    public static GameManager GlobalGameManager;
    public static PlayerController Player;
    public static MagicController MagicController;
    public static QuestManager QuestManager;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.RespawnPoint = this.GetNode<Position2D>("RespawnPoint");
        
        if(GlobalGameManager == null){
            GlobalGameManager = this;
        }else{
            QueueFree();
        }
        MagicController = new MagicController();
        QuestManager = new QuestManager();
    }  

    //Manages respawning the player at the respawn point
    public void RespawnPlayer(){
        PlayerController pc = GetNode<PlayerController>("Player");
        pc.GlobalPosition = RespawnPoint.GlobalPosition;
        pc.respawnPlayer();
    }

    //Function that is called on signal of player dying
    private void _on_Player_Death(){
        RespawnPlayer();
    }
}
