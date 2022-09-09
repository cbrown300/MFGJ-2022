using Godot;
using JsonKnownTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[JsonConverter(typeof(JsonKnownTypesConverter<Quest>))]
public abstract class Quest 
{
    public int ID;
    public int RewardXP;
    public string Title;
    public string Description;
    public bool Accepted;
    public bool Completed;
    public string CompletedDesc;
    public QuestElement QuestElement;
    public List<NPCDialog> FinishDialogElement;

    public abstract void Update(object obj);
}
