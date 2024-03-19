using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ServiceLocator.Wave;
using ServiceLocator.Player;
using ServiceLocator.Events;

namespace ServiceLocator.UI
{
    public class UIService : MonoBehaviour
    {
        // Dependencies:
        private WaveService waveService;
        private EventService eventService;

        [Header("Gameplay Panel")]
        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private TextMeshProUGUI waveProgressText;
        [SerializeField] private TextMeshProUGUI currentMapText;
        [SerializeField] private Button nextWaveButton;

        [Header("Level Selection Panel")]
        [SerializeField] private GameObject levelSelectionPanel;
        [SerializeField] private List<MapButton> mapButtons;

        [Header("Monkey Selection UI")]
        private MonkeySelectionUIController monkeySelectionController;
        [SerializeField] private GameObject MonkeySelectionPanel;
        [SerializeField] private Transform cellContainer;
        [SerializeField] private MonkeyCellView monkeyCellPrefab;
        [SerializeField] private List<MonkeyCellScriptableObject> monkeyCellScriptableObjects;

        [Header("Game End Panel")]
        [SerializeField] private GameObject gameEndPanel;
        [SerializeField] private TextMeshProUGUI gameEndText;
        [SerializeField] private Button playAgainButton;
        [SerializeField] private Button quitButton;

        /// <summary>
        /// Index of the map button whose map was won last time.
        /// This will remain unchanged on scene reload, only it will reset when entire game restarts.
        /// </summary>
        private static int s_LastWonMapButtonIndex = 0;

        private void Start()
        {
            gameplayPanel.SetActive(false);
            gameEndPanel.SetActive(false);

            nextWaveButton.onClick.AddListener(OnNextWaveButton);
            quitButton.onClick.AddListener(OnQuitButtonClicked);
            playAgainButton.onClick.AddListener(OnPlayAgainButtonClicked);
        }

        public void Init(WaveService waveService, PlayerService playerService, EventService eventService)
        {
            this.waveService = waveService;
            this.eventService = eventService;

            InitializeMapSelectionUI(eventService);
            InitializeMonkeySelectionUI(playerService);
            SubscribeToEvents();
        }

        private void InitializeMapSelectionUI(EventService eventService)
        {
            levelSelectionPanel.SetActive(true);
            for (int i = 0, count = mapButtons.Count; i < count; i++)
            {
                mapButtons[i].Init(eventService);
                
                if (i <= s_LastWonMapButtonIndex)
                    mapButtons[i].SetInteractable(true);
                else
                    mapButtons[i].SetInteractable(false);
            }
        }

        private void InitializeMonkeySelectionUI(PlayerService playerService)
        {
            monkeySelectionController = new MonkeySelectionUIController(playerService, cellContainer, monkeyCellPrefab, monkeyCellScriptableObjects);
            MonkeySelectionPanel.SetActive(false);
            monkeySelectionController.SetActive(false);
        }

        private void SubscribeToEvents() => eventService.OnMapSelected.AddListener(OnMapSelected);

        public void OnMapSelected(int mapID)
        {
            levelSelectionPanel.SetActive(false);
            gameplayPanel.SetActive(true);
            MonkeySelectionPanel.SetActive(true);
            monkeySelectionController.SetActive(true);
            currentMapText.SetText("Map: " + mapID);
        }

        private void OnNextWaveButton()
        {
            waveService.StarNextWave();
            SetNextWaveButton(false);
        }

        private void OnQuitButtonClicked() => Application.Quit();

        private void OnPlayAgainButtonClicked() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        public void SetNextWaveButton(bool setInteractable) => nextWaveButton.interactable = setInteractable;

        public void UpdateHealthUI(int healthToDisplay) => healthText.SetText(healthToDisplay.ToString());

        public void UpdateMoneyUI(int moneyToDisplay) => moneyText.SetText(moneyToDisplay.ToString());

        public void UpdateWaveProgressUI(int waveCompleted, int totalWaves) => waveProgressText.SetText(waveCompleted.ToString() + "/" + totalWaves.ToString());

        public void UpdateGameEndUI(bool hasWon)
        {
            gameplayPanel.SetActive(false);
            levelSelectionPanel.SetActive(false);
            gameEndPanel.SetActive(true);

            if (hasWon)
            {
                gameEndText.SetText("You Won");
                s_LastWonMapButtonIndex++;
            }
            else
                gameEndText.SetText("Game Over");
        }
    }
}