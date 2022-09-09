using Godot;
using System;
using System.Collections.Generic;

public class NPCDialog 
{
    public int Index;
    public List<int> DialogSelectionObjects;
    public List<NPCDialog> NPCDialogs;
    public string DisplayText;
    public int Quest;

    //Constructor for setting up NPC Dialog
    public NPCDialog(List<int> dialogSelectionObjects, string displayText, int index, List<NPCDialog> dialogs = null, int quest = 0){
        DialogSelectionObjects = dialogSelectionObjects;
        DisplayText = displayText;
        Index = index;
        if(dialogs != null){
            NPCDialogs = dialogs;
        }
        if(quest != 0){
            Quest = quest;
        }
    }
}
