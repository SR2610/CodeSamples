using StateMachines;
using UnityEngine;

public class GotoEnemyFlagState : State<AI>
{
    #region State Instance
    private static GotoEnemyFlagState instance; //Static instance of the state

    private GotoEnemyFlagState() //Constructor for the state
    {
        if (instance != null) //If we already have an instance of this state, we don't need another one
            return;
        instance = this;
    }

    public static GotoEnemyFlagState Instance //Public acsessor of the state, which will return the instance
    {
        get
        {
            if (instance == null)
                new GotoEnemyFlagState();  //Constructs the state if we don't yet have an instance
            return instance;
        }
    }
    #endregion

    public override void EnterState(AI owner)
    {

    }

    public override void ExitState(AI owner)
    {

    }

    public override void UpdateState(AI owner)
    {
        if (owner.GetAgentInventory().GetItem(Names.HealthKit) && owner.GetAgentData().CurrentHitPoints / owner.GetAgentData().MaxHitPoints * 100 < AIConstants.HealThreshold) //If their health is low, they should try to save themselves
        {
            owner.StateMachine.ChangeState(HealState.Instance); //Try to heal
        }

        else if (!owner.GetAgentData().EnemyBase.GetComponent<SetScore>().IsFriendlyFlagInBase()) //If the enemy flag is not in the enemy base anymore
        {
            owner.StateMachine.ChangeState(GoHomeState.Instance); //Go home, as that is where the flag will be going, so get there ready to defend it
        }
        else
        {

            GameObject flag = owner.GetAgentSenses().GetObjectInViewByName(owner.GetAgentData().EnemyFlagName); //Check if the enemy flag is in view
            if (flag != null) //If they can see the flag
            {
                owner.GetAgentActions().MoveTo(flag); //Moves the agent towards the enemy flag
                owner.GetAgentActions().CollectItem(flag); //Attempts to collect the flag if it is in range
                if (owner.GetAgentInventory().HasItem(owner.GetAgentData().EnemyFlagName)) //If we have the flag, take it back to base
                    owner.StateMachine.ChangeState(GoHomeState.Instance);

            }
            else //If they cant see the flag, AND it is still in the base, they should look for it in the base
                owner.StateMachine.ChangeState(GotoEnemyBaseState.Instance);


        }
    }
}