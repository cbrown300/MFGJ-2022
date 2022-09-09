using Godot;
using System;
using System.Collections.Generic;

public class KillQuest : Quest 
{
    public int EnemyId;
    public int NumberToKill;
    public int NumberLeftToKill;

    public KillQuest(int id, int rewardXP, bool accepted = false, 
    bool completed = false, List<NPCDialog> finishDialog = null, 
    int enemyId = 0, int numberToKill = 0){
        ID = id;
        RewardXP = rewardXP;
        Accepted = accepted;
        Completed = completed;
        FinishDialogElement = finishDialog;
        EnemyId = enemyId;
        NumberToKill = numberToKill;
    }

    //Manages the updating of number left to kill count for quest
    public override void Update(object obj){
        if(obj is SlimeEnemy){ //currently checks if enemy is Slime enemy, should refactor later to a more universal way for all quests
            SlimeEnemy slime = obj as SlimeEnemy;
            if(slime.id == EnemyId){
                NumberLeftToKill -= 1;
                if(NumberLeftToKill <= 0){
                    Completed = true;
                    InterfaceManager.questInterfaceManager.UpdateQuestElement(QuestElement, Title, CompletedDesc);
                }
            }
        }
    }

}
