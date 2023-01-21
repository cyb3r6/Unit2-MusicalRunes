using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MusicalRunes
{
    public class PowerupUpgradePopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private Image coinIconImage;
        [SerializeField] private Button purchaseButton;
        [SerializeField] private Image purchaseButtonImage;

        public Color purchaseAvailableTextColor = new Color(80, 220, 65);
        public Color purchaseDisabledTextColor = new Color(230, 75, 90);
        public Color purchaseDisabledButtonColor = new Color(170, 170, 170);

        private PowerupConfig config;
        private int currentLevel;

        public void Setup(PowerupConfig powerupConfig)
        {
            config = powerupConfig;
            currentLevel = GameManager.Instance.GetPowerupLevel(config.powerupType);

            nameText.text = config.powerupName;
            levelText.text = currentLevel.ToString();
            descriptionText.text = config.description;
            priceText.text = config.GetUpgradePrice(currentLevel).ToString();

            var hasEnoughCoins = GameManager.Instance.CoinsAmount >= config.GetUpgradePrice(currentLevel);
            priceText.color = hasEnoughCoins ? purchaseAvailableTextColor : purchaseDisabledTextColor;
            purchaseButton.interactable = hasEnoughCoins;

            var tintColor = hasEnoughCoins ? Color.white : purchaseDisabledButtonColor;
            purchaseButtonImage.color = tintColor;
            coinIconImage.color = tintColor;

            purchaseButton.gameObject.SetActive(config.MaxLevel != currentLevel);
            gameObject.SetActive(true);
        }

        private void OnClick()
        {
            GameManager.Instance.UpgradePowerup(config.powerupType, config.GetUpgradePrice(currentLevel));
            gameObject.SetActive(false);
        }

        private void Awake()
        {
            purchaseButton.onClick.AddListener(OnClick);
            gameObject.SetActive(false);
        }
    }
}