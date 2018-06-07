namespace Assets.GalaxyCommand.Code.AI
{
    public abstract class State<TOwner>
    {
        public abstract bool CheckState(TOwner owner);
        public abstract void EnterState(TOwner owner);

        public abstract void ExitState(TOwner owner);

        public abstract void UpdateState(TOwner owner);
    }
}