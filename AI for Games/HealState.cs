using StateMachines;
using UnityEngine;

public class HealState : State<AI> //State for using the health kit if they have it on low health, or running away to avoid more damage
{

    #region State Instance
    private static HealState instance; //Static instance of the state

    private HealState() //Constructor for the state
    {
        if (instance != null) //If we already have an instance of this state, we don't need another one
            return;
        instance = this;
    }

    public static HealState Instance //Public acsessor of the state, which will return the instance
    {
        get
        {
            if (instance == null)
                new HealState();  //Constructs the state if we don't yet have an instance
            return instance;
        }
    }
    #endregion


    public override void EnterState(AI owner)
    {
        if (owner.GetAgentInventory().GetItem(Names.HealthKit)) //If they have a health kit in their inventory
        {
            owner.GetAgentActions().UseItem(owner.GetAgentInventory().GetItem(Names.HealthKit)); //Use the health kit
            owner.StateMachine.ChangeState(GotoEnemyBaseState.Instance); //Go back to attacking or chasing the enemy
        }
    }

    public override void ExitState(AI owner)
    {
    }

    public override void UpdateState(AI owner)
    {
     
            if (owner.GetAgentSenses().GetEnemiesInView().Count > 0) //If they can currently seen an enemy
            {
                GameObject enemy = owner.GetAgentSenses().GetEnemiesInView()[0]; //Makes a reference to the enemy that was seen

                if (Random.value < AIConstants.FleeChance) //Do a random chance on fleeing from the enemy
                    owner.StateMachine.ChangeState(GoHomeState.Instance);  //Try to flee the enemy
                else
                    owner.StateMachine.ChangeState(GotoEnemyBaseState.Instance); //If they can't go back to normal logic
            }
            else
            {
                owner.StateMachine.ChangeState(GotoEnemyBaseState.Instance); //If they can't go back to normal logic
            }
        
    }
}