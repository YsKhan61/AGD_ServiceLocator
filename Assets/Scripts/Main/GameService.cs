using UnityEngine;
using ServiceLocator.Utilities;
using ServiceLocator.Events;
using ServiceLocator.Map;
using ServiceLocator.Wave;
using ServiceLocator.Sound;
using ServiceLocator.Player;
using ServiceLocator.UI;

namespace ServiceLocator.Main
{
    public class GameService : MonoBehaviour
    {
        // Services:
        public EventService EventService { get; private set; }
        public MapService MapService { get; private set; }
        public WaveService WaveService { get; private set; }
        public SoundService SoundService { get; private set; }
        public PlayerService PlayerService { get; private set; }

        [SerializeField] private UIService uiService;
        [SerializeField] private MapButton mapButton;

        public UIService UIService => uiService;


        // Scriptable Objects:
        [SerializeField] private MapScriptableObject mapScriptableObject;
        [SerializeField] private WaveScriptableObject waveScriptableObject;
        [SerializeField] private SoundScriptableObject soundScriptableObject;
        [SerializeField] private PlayerScriptableObject playerScriptableObject;

        // Scene Referneces:
        [SerializeField] private AudioSource SFXSource;
        [SerializeField] private AudioSource BGSource;

        private void Awake()
        {
            CreateServices();
            InjectDependencies();

            UIService.SubscribeToEvents();
        }

        private void Update()
        {
            PlayerService.Update();
        }

        private void CreateServices()
        {
            PlayerService = new PlayerService(playerScriptableObject);
            EventService = new EventService();
            MapService = new MapService(mapScriptableObject, EventService);
            WaveService = new WaveService(waveScriptableObject);
            SoundService = new SoundService(soundScriptableObject, SFXSource, BGSource);
        }

        private void InjectDependencies()
        {
            PlayerService.Init(UIService, MapService, SoundService);
            WaveService.Init(PlayerService, EventService, UIService, MapService, SoundService);
            UIService.Init(PlayerService, EventService, WaveService);
            mapButton.Init(EventService);
        }
    }
}