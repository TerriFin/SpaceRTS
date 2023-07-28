using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MineralPriceButton : MonoBehaviour {

    public bool Plus;
    public bool Minus;
    public bool AutomaticPriceToggle;

    public Image AutomaticPricingImage;
    public Sprite AutomaticPriceToggleSpriteON;
    public Sprite AutomaticPriceTooggleSpriteOFF;

    private void Start() {
        GetComponent<Button>().onClick.AddListener(HandleClick);

        if (AutomaticPriceToggle) {
            if (SelectionManager.selected[0].GetComponent<MineralPriceUpdater>().AutomaticallyUpdatingPrice) AutomaticPricingImage.sprite = AutomaticPriceTooggleSpriteOFF;
            else AutomaticPricingImage.sprite = AutomaticPriceToggleSpriteON;
        }
    }

    private void HandleClick() {
        if (Plus) {
            foreach (Selectable selected in SelectionManager.selected) {
                MineralPriceUpdater priceHandle = selected.GetComponent<MineralPriceUpdater>();
                priceHandle.IncreasePrice(5);
            }
        } else if (Minus) {
            foreach (Selectable selected in SelectionManager.selected) {
                MineralPriceUpdater priceHandle = selected.GetComponent<MineralPriceUpdater>();
                priceHandle.DecreasePrice(5);
            }
        } else if (AutomaticPriceToggle) {
            bool stateToSetTo = !SelectionManager.selected[0].GetComponent<MineralPriceUpdater>().AutomaticallyUpdatingPrice;
            foreach (Selectable selected in SelectionManager.selected) {
                MineralPriceUpdater priceHandle = selected.GetComponent<MineralPriceUpdater>();
                if (priceHandle != null) {
                    priceHandle.ToggleAutomaticPricing(stateToSetTo);
                }
            }

            if (stateToSetTo) AutomaticPricingImage.sprite = AutomaticPriceTooggleSpriteOFF;
            else AutomaticPricingImage.sprite = AutomaticPriceToggleSpriteON;
        }
    }
}
