using System.Collections.Generic;

namespace Assets.GalaxyCommand.Code.AI
{
    public class StateMachine<TOwner> : HashSet<State<TOwner>>
    {
        public TOwner Owner;
        public StateMachine(TOwner owner)
        {
            Owner = owner;
        }

        public bool AllowChangeState { get; set; }
        public State<TOwner> CurrentState { get; private set; }

        /// <summary>
        /// Will change state to the passed in one.
        /// </summary>
        /// <param name="newState">New State to change to.</param>
        public void ChangeState(State<TOwner> newState)
        {
            if (CurrentState != null)
                CurrentState.ExitState(Owner);
            CurrentState = newState;
            CurrentState.EnterState(Owner);
        }

        public void Update()
        {
            // Update the current state
            if (CurrentState != null)
                CurrentState.UpdateState(Owner);


            if (AllowChangeState)
            {
                //Check other states to see if we should change based off their own check
                foreach (var state in this)
                    if (state.CheckState(Owner))
                        ChangeState(state);
            }
            
        }
    }
}