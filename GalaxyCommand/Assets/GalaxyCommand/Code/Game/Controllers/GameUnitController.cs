using System;
using System.Collections.Generic;
using Assets.GalaxyCommand.Code.Common;
using Assets.GalaxyCommand.Code.Game.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace Assets.GalaxyCommand.Code.Game.Controllers
{
    public abstract class GameUnitController :
        NetworkBehaviour
        , ISelectHandler
        , IPointerClickHandler
        , IDeselectHandler
    {
        private static GameUnitService _gameUnitService;

        protected static HashSet<GameObject> AllUnits
        {
            get { return _gameUnitService.GetAllUnits(); }
        }

        public bool IsSelected { get; set; }


        public virtual void OnDeselect(BaseEventData eventData)
        {
            IsSelected = false;
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            OnSelect(eventData);
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            if (!Input.GetKey(KeyCode.RightControl) && !Input.GetKey(KeyCode.LeftControl))
            {
                foreach (var unit in AllUnits)
                    unit.GetComponent<GameUnitController>().OnDeselect(eventData);
            }

            IsSelected = true;

            EventsService.TiggerUnitSelectEvent(this,this.gameObject);
        }

        public void Start()
        {
            _gameUnitService = new GameUnitService();

            tag = TagCollection.GameUnitTag;
        }
    }
}