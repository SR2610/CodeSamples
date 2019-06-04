namespace StateMachines
{
    public class StateMachineController<AI>
    {
        public State<AI> currentState; //The state the AI is in
        public AI stateOwner; //The AI that is executing the states
        public State<AI> lastState; //The last state that the machine was in

        public StateMachineController(AI owner) //Constructor to initilise the state, setting up the owner and ensuring it isn't already in a state
        {
            stateOwner = owner; //Sets owner of the state machine
            currentState = null; //Sets the state to null at the start, so that it won't have any issues
        }

        public void ChangeState(State<AI> nextState) //Exits the current state and enters the new one
        {
            if (currentState != null) //Check that we are already in a state before attempting to exit it
                currentState.ExitState(stateOwner); //Exits the current state
            lastState = currentState; //Sets the last state
            currentState = nextState; //Sets the current state to the new state
            currentState.EnterState(stateOwner); //Enters the new state
        }

        public void UpdateState() //Update ticks the current state
        {
            if (currentState != null) //If the state is not null, we can update it
                currentState.UpdateState(stateOwner); //Update the state with a reference to the AI agent that is currently in that state

        }


    }


    public abstract class State<AI> //The abstract of a state that all states need to be based off of
    {

        public abstract void EnterState(AI owner); //Called upon the state being entered, useful for setting up variables that will be 
        public abstract void ExitState(AI owner); //Called upon a new state being entered, used for any actions that need to be called once when the state ends
        public abstract void UpdateState(AI owner); //Called on every update call, used to handle the main logic of the state

    }
}
