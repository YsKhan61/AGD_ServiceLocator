using UnityEngine;
using UnityEngine.UI;
using ServiceLocator.Events;

namespace ServiceLocator.UI
{
    public class MapButton : MonoBehaviour
    {
        Button button;

        [SerializeField] private int MapId;
        private EventService eventService;

        public void Init(EventService eventService)
        {
            this.eventService = eventService;
            button = GetComponent<Button>();
            button.onClick.AddListener(OnMapButtonClicked);
        }

        public void SetInteractable(bool isInteractable)
        {
            button.interactable = isInteractable;
        }

        // To Learn more about Events and Observer Pattern, check out the course list here: https://outscal.com/courses
        private void OnMapButtonClicked() =>  eventService.OnMapSelected.InvokeEvent(MapId);
    }
}