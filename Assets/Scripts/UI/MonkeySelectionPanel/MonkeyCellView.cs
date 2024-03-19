using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ServiceLocator.UI
{
    public class MonkeyCellView : MonoBehaviour
    {
        private MonkeyCellController controller;

        [SerializeField] private MonkeyImageHandler monkeyImageHandler;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private TextMeshProUGUI unlockText;

        public void SetController(MonkeyCellController controllerToSet) => controller = controllerToSet;

        public void ConfigureCellUI(Sprite spriteToSet, string nameToSet, int costToUnlock, int costToSet)
        {
            monkeyImageHandler.ConfigureImageHandler(spriteToSet, controller);
            nameText.SetText(nameToSet);
            unlockText.SetText("Click\nto\nunlock\n" + "<b>" + costToUnlock.ToString() + "</b>");
            costText.SetText(costToSet.ToString());
        }

        public void ShowCostToPlaceText()
        {
            costText.gameObject.SetActive(true);
        }

        public void HideCostToPlaceText()
        {
            costText.gameObject.SetActive(false);
        }

        public void ShowUnlockText()
        {
            unlockText.gameObject.SetActive(true);
        }

        public void HideUnlockText()
        {
            unlockText.gameObject.SetActive(false);
        }
    }
}