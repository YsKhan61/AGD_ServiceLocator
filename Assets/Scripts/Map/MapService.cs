using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using ServiceLocator.Main;
using ServiceLocator.Player;
using ServiceLocator.Events;
using ServiceLocator.Wave.Bloon;

namespace ServiceLocator.Map
{
    public class MapService
    {
        // Dependencies:
        private EventService m_EventService;
        private MapScriptableObject m_MapScriptableObject;

        private Grid m_CurrentGrid;
        private Tilemap m_CurrentTileMap;
        private MapData m_CurrentMapData;
        private SpriteRenderer m_TileOverlay;

        public MapService(MapScriptableObject mapScriptableObject)
        {
            this.m_MapScriptableObject = mapScriptableObject;
            m_TileOverlay = Object.Instantiate(mapScriptableObject.TileOverlay).GetComponent<SpriteRenderer>();
            ResetTileOverlay();
        }

        public void Init(EventService eventService)
        {
            this.m_EventService = eventService;
            SubscribeToEvents();
        }

        private void SubscribeToEvents() => m_EventService.OnMapSelected.AddListener(LoadMap);

        private void LoadMap(int mapId)
        {
            m_CurrentMapData = m_MapScriptableObject.MapDatas.Find(mapData => mapData.MapID == mapId);
            m_CurrentGrid = Object.Instantiate(m_CurrentMapData.MapPrefab);
            m_CurrentTileMap = m_CurrentGrid.GetComponentInChildren<Tilemap>();
        }

        public List<Vector3> GetWayPointsForCurrentMap() => m_CurrentMapData.WayPoints;

        public Vector3 GetBloonSpawnPositionForCurrentMap() => m_CurrentMapData.SpawningPoint;

        private void ResetTileOverlay() => SetTileOverlayColor(TileOverlayColor.TRANSPARENT);

        private void SetTileOverlayColor(TileOverlayColor colorToSet)
        {
            switch(colorToSet)
            {
                case TileOverlayColor.TRANSPARENT:
                    m_TileOverlay.color = m_MapScriptableObject.DefaultTileColor;
                    break;
                case TileOverlayColor.SPAWNABLE:
                    m_TileOverlay.color = m_MapScriptableObject.SpawnableTileColor;
                    break;
                case TileOverlayColor.NON_SPAWNABLE:
                    m_TileOverlay.color = m_MapScriptableObject.NonSpawnableTileColor;
                    break;
            }
        }

        public void ValidateSpawnPosition(Vector3 cursorPosition)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(cursorPosition);
            Vector3Int cellPosition = GetCellPosition(mousePosition);
            Vector3 cellCenter = GetCenterOfCell(cellPosition);

            if(CanSpawnOnPosition(cellCenter, cellPosition))
            {
                m_TileOverlay.transform.position = cellCenter;
                SetTileOverlayColor(TileOverlayColor.SPAWNABLE);
            }
            else
            {
                m_TileOverlay.transform.position = cellCenter;
                SetTileOverlayColor(TileOverlayColor.NON_SPAWNABLE);
            }
        }

        public bool TryGetMonkeySpawnPosition(Vector3 cursorPosition, out Vector3 spawnPosition)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(cursorPosition);
            Vector3Int cellPosition = GetCellPosition(mousePosition);
            Vector3 cellCenter = GetCenterOfCell(cellPosition);

            ResetTileOverlay();

            if (CanSpawnOnPosition(cellCenter, cellPosition))
            {
                spawnPosition = cellCenter;
                return true;
            }
            else
            {
                spawnPosition = Vector3.zero;
                return false;
            }
        }

        private Vector3Int GetCellPosition(Vector3 mousePosition) => m_CurrentGrid.WorldToCell(new Vector3(mousePosition.x, mousePosition.y, 0));

        private Vector3 GetCenterOfCell(Vector3Int cellPosition) => m_CurrentGrid.GetCellCenterWorld(cellPosition);

        private bool CanSpawnOnPosition(Vector3 centerCell, Vector3Int cellPosition)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(centerCell, 0.1f);
            return InisdeTilemapBounds(cellPosition) && !HasClickedOnObstacle(colliders) && !IsOverLappingMonkey(colliders);
        }

        private bool InisdeTilemapBounds(Vector3Int mouseToCell)
        {
            BoundsInt tilemapBounds = m_CurrentTileMap.cellBounds;
            return tilemapBounds.Contains(mouseToCell);
        }

        private bool HasClickedOnObstacle(Collider2D[] colliders)
        {
            foreach (Collider2D collider in colliders)
            {
                if (collider.GetComponent<TilemapCollider2D>() != null)
                    return true;
            }
            return false;
        }

        private bool IsOverLappingMonkey(Collider2D[] colliders)
        {
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.GetComponent<MonkeyView>() != null && !collider.isTrigger)
                    return true;
            }
            return false;
        }

        private enum TileOverlayColor
        {
            TRANSPARENT,
            SPAWNABLE,
            NON_SPAWNABLE
        }
    }
}