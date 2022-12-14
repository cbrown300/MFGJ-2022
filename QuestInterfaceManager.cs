using Godot;
using System;

public class QuestInterfaceManager : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export]
    public PackedScene QuestInterfaceElement;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    public void AddQuestElement(Quest quest){
        QuestElement element = QuestInterfaceElement.Instance() as QuestElement;
        var questList = GetNode("QuestList") as VBoxContainer;
        GD.Print("QuestTitle" + quest.Title);
        GD.Print("QuestDesc" + quest.Description);
        questList.AddChild(element);
        element.SetPosition(new Vector2(0,0));
        UpdateQuestElement(element, quest.Title, quest.Description);
        quest.QuestElement = element;
    }

    public void UpdateQuestElement(QuestElement element, string title, string desc) => element.updateTitleDescription(title, desc);
}
