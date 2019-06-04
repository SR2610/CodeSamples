using StateMachines;
using UnityEngine;

public class ChaseEnemyState : State<AI>
{

    #region State Instance
    private static ChaseEnemyState instance; //Static instance of the state

    private ChaseEnemyState() //Constructor for the state
    {
        if (instance != null) //If we already have an instance of this state, we don't need another one
            return;
        instance = this;
    }

    public static ChaseEnemyState Instance //Public acsessor of the state, which will return the instance
    {
        get
        {
            if (instance == null)
                new ChaseEnemyState();  //Constructs the state if we don't yet have an instance
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
            owner.StateMachine.ChangeState(HealState.Instance);
        else
        {
            if (owner.GetAgentSenses().GetEnemiesInView().Count > 0) //If they can still seen an enemy in their view
            {
                GameObject enemy = owner.GetAgentSenses().GetEnemiesInView()[0];
                owner.GetAgentActions().MoveTo(enemy); //Move towards / pursue enemy
                if (owner.GetAgentSenses().IsInAttackRange(enemy)) //If they are in attack range, they should try to attack
                {
                    if (owner.GetAgentInventory().HasItem(Names.PowerUp)) //If they have a powerup, they should use it to ensure victory
                        owner.StateMachine.ChangeState(UsePowerupState.Instance);
                    else
                        owner.StateMachine.ChangeState(AttackEnemyState.Instance); //If they don't they should just try to attack

                }
            }
            else
                owner.StateMachine.ChangeState(GotoEnemyBaseState.Instance);

        }
    }
}