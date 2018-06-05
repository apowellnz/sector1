using System;
using System.Collections.Generic;
using System.Linq;
using Assets.GalaxyCommand.Code.Game.Controllers;
using Assets.GalaxyCommand.Code.Game.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragController
    : MonoBehaviour
        , IBeginDragHandler
        , IDragHandler
        , IEndDragHandler
        ,IPointerClickHandler
{
    private Camera _camera
    {
        get
        {
            return FindObjectOfType<Camera>();
        }
    }

    private Rect _selectionRect;
    public Image SelectionBoxImage;
    private Vector2 _startPosition { get; set; }


    public void OnBeginDrag(PointerEventData eventData)
    {
        GameUnitController.DeselectAll(new BaseEventData(EventSystem.current));
        SelectionBoxImage.gameObject.SetActive(true);
        _startPosition = eventData.position;
        _selectionRect = new Rect();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.position.x < _startPosition.x)
        {
            _selectionRect.xMin = eventData.position.x;
            _selectionRect.xMax = _startPosition.x;
        }
        else
        {
            _selectionRect.xMin = _startPosition.x;
            _selectionRect.xMax = eventData.position.x;
        }

        if (eventData.position.y < _startPosition.y)
        {
            _selectionRect.yMin = eventData.position.y;
            _selectionRect.yMax = _startPosition.y;
        }
        else
        {
            _selectionRect.yMin = _startPosition.y;
            _selectionRect.yMax = eventData.position.y;
        }

        SelectionBoxImage.rectTransform.offsetMin = _selectionRect.min;
        SelectionBoxImage.rectTransform.offsetMax = _selectionRect.max;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SelectionBoxImage.gameObject.SetActive(false);
        foreach (var unit in GameUnitService.GetAllUnits())
        {
            if (_selectionRect.Contains(_camera.WorldToScreenPoint(unit.transform.position)))
            {
                unit.GetComponent<GameUnitController>().OnSelect(eventData);
            }
        }
            
    }

    // Use this for initialization
    public void Start()
    {
        if (SelectionBoxImage == null)
            throw new NullReferenceException("SelectionBoxImage is not set in the DragController.");
        SelectionBoxImage.gameObject.SetActive(false);

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        float myDistance = 0;

        foreach (RaycastResult result in results)
        {
            if (result.gameObject == gameObject)
            {
                myDistance = result.distance;
                break;
            }
        }

        GameObject nextObject = null;
        float maxDistance = Mathf.Infinity;

        foreach (RaycastResult result in results)
        {
            if (result.distance > myDistance && result.distance < maxDistance)
            {
                nextObject = result.gameObject;
                maxDistance = result.distance;
            }
        }

        if (nextObject)
        {
            ExecuteEvents.Execute<IPointerClickHandler>(nextObject, eventData, (x, y) => { x.OnPointerClick((PointerEventData)y); });
        }
    }
}