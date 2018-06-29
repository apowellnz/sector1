using UnityEngine;

namespace Assets.GalaxyCommand.Code.Game.Services
{
    public class InputService
    {
        public static bool IsPressingCtrl()
        {
            return Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.A);
        }

      public static bool IsPressingAlt()
      {
        return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
      }

      public static bool IsPressingShift()
      {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
      }
    }
}
