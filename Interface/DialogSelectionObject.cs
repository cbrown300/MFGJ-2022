using Godot;
using System;

public class DialogSelectionObject 
{
    public int ID;
    public string selectionText = "Test";
    public int selectionIndex;
    public bool AcceptQuest;

    //Constructor for player responses to NPC Dialog
    public DialogSelectionObject(int id, int index, string text, bool acceptQuest = false){
        ID = id;
        selectionText = text;
        selectionIndex = index;
        AcceptQuest = acceptQuest;
    }
}
