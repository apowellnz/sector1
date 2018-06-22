using System;
using RTS_Cam;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MinimapController
  : MonoBehaviour,
    IPointerClickHandler
  
{
  public Camera MiniMapCamera;

  public void OnPointerClick(PointerEventData eventData)
  {
  
    var rect = GetComponent<RawImage>().rectTransform.rect;
    var point = new Vector2(MiniMapCamera.pixelWidth / rect.width * eventData.position.x,
      MiniMapCamera.pixelHeight / rect.height * eventData.position.y);
    RaycastHit portalHit;
    if (Physics.Raycast(MiniMapCamera.ScreenPointToRay(point), out portalHit))
    {

      if (eventData.clickCount == 2)
      {
        var cameraControl = GameObject.FindObjectOfType<RtsCamera>();
        cameraControl.SetPosition(portalHit.point);
      }
      else
      {
        var ph = portalHit.collider.gameObject.GetComponent<IPointerClickHandler>();
        if (ph != null)
          ExecuteEvents.Execute<IPointerClickHandler>(portalHit.collider.gameObject,
            new PointerEventData(EventSystem.current)
            {
              button = eventData.button,
              pointerCurrentRaycast = new RaycastResult
              {
                worldPosition = portalHit.point
              }
            },
            (x, y) => { x.OnPointerClick((PointerEventData)y); });
      }
     

    }
  }

  // Use this for initialization
  private void Start()
  {
    if (MiniMapCamera == null) throw new NullReferenceException("MinimapCamera reference has not been sent.");
  }
}
