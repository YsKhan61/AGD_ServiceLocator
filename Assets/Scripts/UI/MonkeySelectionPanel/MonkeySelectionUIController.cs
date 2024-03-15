using ServiceLocator.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServiceLocator.UI
{
    public class MonkeySelectionUIController
    {
        private Transform cellContainer;
        private List<MonkeyCellController> monkeyCellControllers;

        public MonkeySelectionUIController(PlayerService playerService, Transform cellContainer, MonkeyCellView monkeyCellPrefab, List<MonkeyCellScriptableObject> monkeyCellScriptableObjects)
        {
            this.cellContainer = cellContainer;
            monkeyCellControllers = new List<MonkeyCellController>();

            foreach (MonkeyCellScriptableObject monkeySO in monkeyCellScriptableObjects)
            {
                MonkeyCellController monkeyCell = new MonkeyCellController(playerService, cellContainer, monkeyCellPrefab, monkeySO);
                monkeyCellControllers.Add(monkeyCell);
            }
        }

        public void SetActive(bool setActive) => cellContainer.gameObject.SetActive(setActive);

    }
}