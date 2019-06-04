using StateMachines;
using UnityEngine;

public class AttackEnemyState : State<AI>
{

    #region State Instance
    private static AttackEnemyState instance; //Static instance of the state

    private AttackEnemyState() //Constructor for the state
    {
        if (instance != null) //If we already have an instance of this state, we don't need another one
            return;
        instance = this;
    }

    public static AttackEnemyState Instance //Public acsessor of the state, which will return the instance
    {
        get
        {
            if (instance == null)
                new AttackEnemyState();  //Constructs the state if we don't yet have an instance
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

    private float AttackCooldownCurrent = 0; //The current cooldown that this agents attack is on

    public override void UpdateState(AI owner)
    {
        //In Range
        //Attack Enemy
        if (AttackCooldownCurrent > 0) //If the cooldown hasn't expired, it will decrement it
            AttackCooldownCurrent -= Time.deltaTime;

        if (owner.GetAgentSenses().GetEnemiesInView().Count > 0) //If it can still see an enemy
        {
            GameObject enemy = owner.GetAgentSenses().GetEnemiesInView()[0]; //Get a reference to the enemy to simplify it
            owner.GetAgentActions().MoveTo(enemy);
            if (owner.GetAgentSenses().IsInAttackRange(enemy))  //If it can attack the enemy
            {
                if (AttackCooldownCurrent <= 0) //If the cooldown has been met
                {
                    owner.GetAgentActions().AttackEnemy(enemy); //Attack the enemy
                    AttackCooldownCurrent = AIConstants.AttackCooldown; //Set the cooldown
                }
            }
        }
        else //If it can't see an enemy, fall back to the chasing state
            owner.StateMachine.ChangeState(ChaseEnemyState.Instance);

    }
}
