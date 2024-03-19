using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServiceLocator.Player;
using ServiceLocator.Main;

namespace ServiceLocator.UI
{
    public class MonkeyCellController
    {
        private PlayerService playerService;
        private MonkeyCellView monkeyCellView;
        private MonkeyCellScriptableObject monkeyCellSO;

        public bool IsLocked { get; set; }

        public MonkeyCellController(PlayerService playerService, Transform cellContainer, MonkeyCellView monkeyCellPrefab, MonkeyCellScriptableObject monkeyCellScriptableObject)
        {
            this.playerService = playerService;
            this.monkeyCellSO = monkeyCellScriptableObject;
            monkeyCellView = Object.Instantiate(monkeyCellPrefab, cellContainer);
            monkeyCellView.SetController(this);
            monkeyCellView.ConfigureCellUI(monkeyCellSO.Sprite, monkeyCellSO.Name, monkeyCellSO.CostToUnlock, monkeyCellSO.CostToPlace);

            IsLocked = true;
            monkeyCellView.HideCostToPlaceText();
            monkeyCellView.ShowUnlockText();
        }

        public void MonkeyDraggedAt(Vector3 dragPosition)
        {
            playerService.ValidateSpawnPosition(monkeyCellSO.CostToPlace, dragPosition);
        }

        public void MonkeyDroppedAt(Vector3 dropPosition)
        {
            playerService.TrySpawningMonkey(monkeyCellSO.Type, monkeyCellSO.CostToPlace, dropPosition);
        }

        public void TryUnlock()
        {
            if (playerService.Money >= monkeyCellSO.CostToUnlock)
            {
                playerService.DeductMoney(monkeyCellSO.CostToUnlock);
                IsLocked = false;
                monkeyCellView.HideUnlockText();
                monkeyCellView.ShowCostToPlaceText();
            }
        }
    }
}