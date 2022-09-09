using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class DialogManager : Control
{
    public List<NPCDialog> npcDialog;
    public List<DialogSelection> Selections = new List<DialogSelection>();
    public List<DialogSelectionObject> DialogSelectionObjects;
    private bool isDialogUp = false;
    private int currentSelectionIndex = 0;
    public string npcDialogHeader;
    private NPCDialog currentDialogOpen;

    [Export]
    public PackedScene DialogSelectableObject;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //showDialogElement();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public async override void _PhysicsProcess(float delta)
    {
        await ToSignal(GetTree(), "physics_frame");

        if(GameManager.GlobalGameManager.GamePaused && isDialogUp){
            if(Input.IsActionJustPressed("ui_left")){
                foreach(var item in Selections){
                    item.setSelected(false); //make sure nothing is selected at start
                }
                currentSelectionIndex -= 1; //move cursor left
                if(currentSelectionIndex < 0){
                    currentSelectionIndex = 0;
                }
                Selections[currentSelectionIndex].setSelected(true); //make selection
            }else if(Input.IsActionJustPressed("ui_right")){
                foreach(var item in Selections){
                    item.setSelected(false); //make sure nothing is selected at start
                }
                currentSelectionIndex += 1; //move cursor right
                if(currentSelectionIndex > Selections.Count - 1){
                    currentSelectionIndex = Selections.Count - 1;
                }
                Selections[currentSelectionIndex].setSelected(true); //make selection
            }else if(Input.IsActionJustPressed("ui_accept")){
                DialogSelectionObject selectionObject = Selections[currentSelectionIndex].dialogSelectionObject;
                if(selectionObject.AcceptQuest){    //if you select an option to accept a quest then add it to active quests
                    GameManager.QuestManager.AddActiveQuest(GameManager.QuestManager.AvailableQuests.Where(x => x.ID == currentDialogOpen.Quest).FirstOrDefault());
                }
                displayNextDiologElement(selectionObject.selectionIndex);
            }
        }
    }

    //Manages displaying the dialog box
    public async void showDialogElement(){
        GetNode<Popup>("Popup").Popup_();
        GetNode<Label>("Popup/NPCName").Text = npcDialogHeader;
        writeDialog(npcDialog[0]);
        GameManager.Player.pauseInput = true;
        await ToSignal(GetTree(), "physics_frame");
    }

    //Adds dialog and responses to dialog box
    public void writeDialog(NPCDialog dialog){
        currentDialogOpen = dialog;
        foreach(Node item in GetNode<Node>("Popup/TextOptions").GetChildren()){ //clear old text
            item.QueueFree();
        }
        Selections = new List<DialogSelection>();
        GetNode<RichTextLabel>("Popup/DialogText").Text = dialog.DisplayText;
        foreach(var item in dialog.DialogSelectionObjects){
            DialogSelection dialogSelection = DialogSelectableObject.Instance() as DialogSelection;
            dialogSelection.dialogSelectionObject = DialogSelectionObjects.Where(x => x.ID == item).FirstOrDefault();
            GetNode<HBoxContainer>("Popup/TextOptions").AddChild(dialogSelection);
            Selections.Add(dialogSelection);
            dialogSelection.setSelected(false);
        }
        Selections[0].setSelected(true);
        currentSelectionIndex = 0;
        GameManager.GlobalGameManager.GamePaused = true;
        isDialogUp = true;
    }

    //Manages closing the dialog
    private async void endDialog(){
        GetNode<Popup>("Popup").Hide();
        GameManager.GlobalGameManager.GamePaused = false;
        isDialogUp = false;
        await ToSignal(GetTree(), "physics_frame");
        GameManager.Player.pauseInput = false;
    }

    //Manages displaying the nex dialog element
    private void displayNextDiologElement(int index){
        if(npcDialog.ElementAtOrDefault(index) == null || index == -1){ //checks if there is a dialog element or defaults to null if not found, if -1 then we are outside of the index
            endDialog();
        }else{
            writeDialog(npcDialog[index]);
        }
    }
}
