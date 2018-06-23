using Assets.GalaxyCommand.Code.Enums;

namespace Assets.GalaxyCommand.Code.Game.Units
{
  public abstract class BaseAction
  {
    public ActionType ActionName;
    public abstract void ExecuteAction();
  }
}
