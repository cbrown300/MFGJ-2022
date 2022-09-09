using Godot;
using System;

public class DialogSelection : Control
{
    public bool Selected = false;
    public DialogSelectionObject dialogSelectionObject;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.GetNode<Label>("Label").Text = dialogSelectionObject.selectionText;
    }

    public void setSelected(bool selected){
        Selected = selected;
        if(selected){
            GetNode<TextureRect>("TextureRect").Visible = true;
        }else{
            GetNode<TextureRect>("TextureRect").Visible = false;

        }
    }
}
