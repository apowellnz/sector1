using UnityEngine;
using UnityEngine.Diagnostics;

public class MiniCamerFollow : MonoBehaviour
{
  public Camera Camera
  {
    get { return (Camera) FindObjectOfType(typeof(Camera)); }
  }


  // Update is called once per frame
  private void LateUpdate()
  {
    var newPosition = Camera.transform.position;
    newPosition.y = transform.position.y;
    transform.position = newPosition;
  }
}
