using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class QuestManager
{
    public List<Quest> AvailableQuests = new List<Quest>();
    public List<Quest> ActiveQuests = new List<Quest>();

    public QuestManager(){
        /*KillQuest quest = new KillQuest(1, 1, finishDialog:
        new List<NPCDialog>{
            new NPCDialog(
                new List<int>{
                    1,
                }, "Thanks for killing the enemy!", 1
            )
        },
        enemyId: 1,
        numberToKill: 1
        );
        AvailableQuests.Add(quest);*/
        //SaveQuests();
        LoadQuests();
    }

    //Manages adding an accepted quest to the active quest list
    public void AddActiveQuest(Quest questToAdd){
        ActiveQuests.Add(questToAdd);
        InterfaceManager.questInterfaceManager.AddQuestElement(questToAdd);
    }

    //Manages removing a completed quest from the active quest list
    public void RemoveActiveQuest(Quest questToRemove){
        ActiveQuests.Remove(questToRemove);
        if(questToRemove.Completed){
            GameManager.Player.XP += questToRemove.RewardXP;
        }
    }

    //Manages updating of quest requirements
    public void UpdateQuest(object obj){
        foreach(var item in ActiveQuests){
            item.Update(obj);
        }
    }

    //Manager for saving quests
    public void SaveQuests(){
        string json = JsonConvert.SerializeObject(AvailableQuests);
        var questsFile = new File();
        questsFile.Open($"user://AvailableQuests.json", File.ModeFlags.Write);
        questsFile.StoreLine(json);
        questsFile.Close();

        json = JsonConvert.SerializeObject(ActiveQuests);
        questsFile = new File();
        questsFile.Open($"user://ActiveQuests.json", File.ModeFlags.Write);
        questsFile.StoreLine(json);
        questsFile.Close();
    }

    //Manager for loading quests
    public void LoadQuests(){
        var questsFile = new File();
        questsFile.Open($"user://AvailableQuests.json", File.ModeFlags.Read);
        string json = "";
        while(!questsFile.EofReached()){
            json += questsFile.GetLine();
        }
        AvailableQuests = JsonConvert.DeserializeObject<List<Quest>>(json);
        questsFile.Close();

        questsFile = new File();
        questsFile.Open($"user://ActiveQuests.json", File.ModeFlags.Read);
        json = "";
        while(!questsFile.EofReached()){
            json += questsFile.GetLine();
        }
        ActiveQuests = JsonConvert.DeserializeObject<List<Quest>>(json);
        questsFile.Close();
    }
}
