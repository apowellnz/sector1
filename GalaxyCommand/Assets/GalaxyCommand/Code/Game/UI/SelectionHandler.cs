using System;
using System.Collections.Generic;
using Assets.GalaxyCommand.Code.Game.Controllers;
using Assets.GalaxyCommand.Code.Game.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.GalaxyCommand.Code.Game.UI
{
    public class SelectionHandler
        : MonoBehaviour
            , IBeginDragHandler
            , IDragHandler
            , IEndDragHandler
            , IPointerClickHandler
    {
        private Rect _selectionRect;
        public Image SelectionBoxImage;

        private Camera _camera
        {
            get { return FindObjectOfType<Camera>(); }
        }

        private Vector2 _startPosition { get; set; }


        public void OnBeginDrag(PointerEventData eventData)
        {
            if (Input.GetMouseButton(0))
            {
                GameUnitController.DeselectAll(new BaseEventData(EventSystem.current));
                SelectionBoxImage.gameObject.SetActive(true);
                _startPosition = eventData.position;
                _selectionRect = new Rect();
            }
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
                if (_selectionRect.Contains(_camera.WorldToScreenPoint(unit.transform.position)))
                    unit.GetComponent<GameUnitController>().OnSelect(eventData);
        }

        /// <summary>
        /// Will loop through all the ray cast hits and get the first one past the invisble drag image
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            float myDistance = 0;

            foreach (var result in results)
                if (result.gameObject == gameObject)
                {
                    myDistance = result.distance;
                    break;
                }

            GameObject nextObject = null;
            var maxDistance = Mathf.Infinity;
            var resultPosition = new RaycastResult();
            foreach (var result in results)
                if (result.distance > myDistance && result.distance < maxDistance)
                {
                    nextObject = result.gameObject;
                    resultPosition = result;
                    maxDistance = result.distance;
                }


            if (nextObject)
                ExecuteEvents.Execute<IPointerClickHandler>(nextObject, new PointerEventData(EventSystem.current)
                    {
                        button = eventData.button,
                        pointerCurrentRaycast = resultPosition
                    },
                    (x, y) => { x.OnPointerClick((PointerEventData) y); });
        }

        // Use this for initialization
        public void Start()
        {
            if (SelectionBoxImage == null)
                throw new NullReferenceException("SelectionBoxImage is not set in the SelectionHandler.");
            SelectionBoxImage.gameObject.SetActive(false);
        }
    }
}