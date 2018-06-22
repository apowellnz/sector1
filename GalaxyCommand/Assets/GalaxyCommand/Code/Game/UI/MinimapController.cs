using System;
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
    var width = GetComponent<RawImage>().rectTransform.rect.width;
    var height = GetComponent<RawImage>().rectTransform.rect.height;

    var point = new Vector2(MiniMapCamera.pixelWidth / width * eventData.position.x,
      MiniMapCamera.pixelHeight / height * eventData.position.y);
    RaycastHit portalHit;
    if (Physics.Raycast(MiniMapCamera.ScreenPointToRay(point), out portalHit))
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
          (x, y) => { x.OnPointerClick((PointerEventData) y); });
    }
  }

  // Use this for initialization
  private void Start()
  {
    if (MiniMapCamera == null) throw new NullReferenceException("MinimapCamera reference has not been sent.");
  }
}
