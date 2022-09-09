using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

public class NPC : KinematicBody2D
{
    public NPCInterfaceHolder npcInterface = new NPCInterfaceHolder();
    private List<NPCDialog> npcDialog;
    [Export]
    private string npcName;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        /*DialogSelectionObject obj = new DialogSelectionObject(1, 1, "I don't care", true);
        DialogSelectionObject obj2 = new DialogSelectionObject(2, 2, "Whatever.");
        DialogSelectionObject obj3 = new DialogSelectionObject(3, -1, "Sure");
        npcInterface.InterfaceSelections.Add(obj);
        npcInterface.InterfaceSelections.Add(obj2);
        npcInterface.InterfaceSelections.Add(obj3);

        npcInterface.npcDialogs = new List<NPCDialog>{
            new NPCDialog(new List<int>(){1, 2}, "You have all the power", 0, quest: 1),
            new NPCDialog(new List<int>(){3}, "Yes you do.", 1),
            new NPCDialog(new List<int>(){3}, "Fine be that way.", 2)
        };*/
        LoadNPCDialog();
        //SaveNPCDialog();
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    //Sets up dialog when player talks to npc
    public void setNPCDialog(){
        List<Quest> activeQuest = GameManager.QuestManager.ActiveQuests;
        InterfaceManager.dialogManager.npcDialogHeader = npcName;
        foreach(var item in activeQuest){   //if player has completed an active quest for the current npc then change their dialog
            if(item.Completed){
                if(npcInterface.npcDialogs.Any(x => x.Quest == item.ID)){
                    InterfaceManager.dialogManager.npcDialog = item.FinishDialogElement;
                    GameManager.QuestManager.RemoveActiveQuest(item);
                    item.QuestElement.QueueFree();
                    return;
                }
            }
        }
        InterfaceManager.dialogManager.npcDialog = npcInterface.npcDialogs;
        InterfaceManager.dialogManager.DialogSelectionObjects = npcInterface.InterfaceSelections;
    }

    //Manager to save dialog to a json file
    public void SaveNPCDialog(){
        string json = JsonConvert.SerializeObject(npcInterface);
        var npcDialogFile = new File();
        npcDialogFile.Open($"user://NPC{npcName}Dialog.json", File.ModeFlags.Write);
        npcDialogFile.StoreLine(json);
        npcDialogFile.Close();
    }

    //Manager to load dialog from jsonfile
    public void LoadNPCDialog(){
        var npcDialogFile = new File();
        npcDialogFile.Open($"user://NPC{npcName}Dialog.json", File.ModeFlags.Read);
        string json = "";
        while(!npcDialogFile.EofReached()){
            json += npcDialogFile.GetLine();
        }
        npcInterface = JsonConvert.DeserializeObject<NPCInterfaceHolder>(json);
        npcDialogFile.Close();
    }
}
