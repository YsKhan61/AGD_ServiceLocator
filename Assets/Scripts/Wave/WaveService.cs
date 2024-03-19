using System.Collections.Generic;
using UnityEngine;
using ServiceLocator.Wave.Bloon;
using System.Threading.Tasks;
using ServiceLocator.UI;
using ServiceLocator.Map;
using ServiceLocator.Sound;
using ServiceLocator.Player;
using ServiceLocator.Events;

namespace ServiceLocator.Wave
{
    public class WaveService
    {
        // Dependencies:
        private UIService m_UIService;
        private MapService m_MapService;
        private PlayerService m_PlayerService;
        private SoundService m_SoundService;
        private EventService m_EventService;

        private WaveScriptableObject m_WaveScriptableObject;
        private BloonPool m_BloonPool;

        private int currentWaveId;
        private List<WaveData> waveDatas;
        private List<BloonController> activeBloons;

        public WaveService(WaveScriptableObject waveScriptableObject) => this.m_WaveScriptableObject = waveScriptableObject;

        public void Init(UIService uiService, MapService mapService, PlayerService playerService, SoundService soundService, EventService eventService)
        {
            this.m_UIService = uiService;
            this.m_MapService = mapService;
            this.m_PlayerService = playerService;
            this.m_SoundService = soundService;
            this.m_EventService = eventService;
            InitializeBloons();
            SubscribeToEvents();
        }

        private void InitializeBloons()
        {
            m_BloonPool = new BloonPool(this, m_PlayerService, m_SoundService, m_WaveScriptableObject);
            activeBloons = new List<BloonController>();
        }

        private void SubscribeToEvents() => m_EventService.OnMapSelected.AddListener(LoadWaveDataForMap);

        private void LoadWaveDataForMap(int mapId)
        {
            currentWaveId = 0;
            waveDatas = m_WaveScriptableObject.WaveConfigurations.Find(config => config.MapID == mapId).WaveDatas;
            m_UIService.UpdateWaveProgressUI(currentWaveId, waveDatas.Count);
        }

        public void StarNextWave()
        {
            currentWaveId++;
            var bloonsToSpawn = GetBloonsForCurrentWave();
            var spawnPosition = m_MapService.GetBloonSpawnPositionForCurrentMap();
            SpawnBloons(bloonsToSpawn, spawnPosition, 0, m_WaveScriptableObject.SpawnRate);
        }

        public async void SpawnBloons(List<BloonType> bloonsToSpawn, Vector3 spawnPosition, int startingWaypointIndex, float spawnRate)
        {
            foreach(BloonType bloonType in bloonsToSpawn)
            {
                BloonController bloon = m_BloonPool.GetBloon(bloonType);
                bloon.SetPosition(spawnPosition);
                bloon.SetWayPoints(m_MapService.GetWayPointsForCurrentMap(), startingWaypointIndex);

                AddBloon(bloon);
                await Task.Delay(Mathf.RoundToInt(spawnRate * 1000));
            }
        }

        private void AddBloon(BloonController bloonToAdd)
        {
            activeBloons.Add(bloonToAdd);
            bloonToAdd.SetOrderInLayer(-activeBloons.Count);
        }

        public void RemoveBloon(BloonController bloon)
        {
            m_BloonPool.ReturnItem(bloon);
            activeBloons.Remove(bloon);
            if (HasCurrentWaveEnded())
            {
                m_SoundService.PlaySoundEffects(Sound.SoundType.WaveComplete);
                m_UIService.UpdateWaveProgressUI(currentWaveId, waveDatas.Count);

                if(IsLevelWon())
                    m_UIService.UpdateGameEndUI(true);
                else
                    m_UIService.SetNextWaveButton(true);
            }
        }

        private List<BloonType> GetBloonsForCurrentWave() => waveDatas.Find(waveData => waveData.WaveID == currentWaveId).ListOfBloons;

        private bool HasCurrentWaveEnded() => activeBloons.Count == 0;

        private bool IsLevelWon() => currentWaveId >= waveDatas.Count;
    }
}