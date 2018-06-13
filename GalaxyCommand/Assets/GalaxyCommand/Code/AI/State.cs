namespace Assets.GalaxyCommand.Code.AI
{
    public abstract class State<TOwner>
    {
      public State(int priority)
      {
        Priority = priority;
      }
        public int Priority { get; set; }
        public bool HasFinished { get; set; }

      public abstract bool DecideThisState(TOwner owner);
        public abstract void EnterState(TOwner owner);

        public abstract void ExitState(TOwner owner);

        public abstract void UpdateState(TOwner owner);
    }
}
