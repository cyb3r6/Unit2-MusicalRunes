using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MusicalRunes
{
    public class PowerupUpgradePopup : MonoBehaviour, ILocalizable
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private Image coinIconImage;
        [SerializeField] private Button purchaseButton;
        [SerializeField] private Image purchaseButtonImage;

        [SerializeField] private Color purchaseAvailableTextColor = new Color(80, 220, 65);
        [SerializeField] private Color purchaseDisabledTextColor = new Color(230, 75, 90);
        [SerializeField] private Color purchaseDisabledButtonColor = new Color(170, 170, 170);

        private PowerupConfig config;
        private int currentLevel;

        public void Setup(PowerupConfig powerupConfig)
        {
            config = powerupConfig;
            currentLevel = GameManager.Instance.GetPowerupLevel(config.powerupType);

            nameText.text = config.PowerupName;
            levelText.text = currentLevel.ToString();
            descriptionText.text = config.Description;
            priceText.text = config.GetUpgradePrice(currentLevel).ToString();

            var hasEnoughCoins = GameManager.Instance.CoinsAmount >= config.GetUpgradePrice(currentLevel);
            priceText.color = hasEnoughCoins ? purchaseAvailableTextColor : purchaseDisabledTextColor;
            purchaseButton.interactable = hasEnoughCoins;

            var tintColor = hasEnoughCoins ? Color.white : purchaseDisabledButtonColor;
            purchaseButtonImage.color = tintColor;
            coinIconImage.color = tintColor;

            purchaseButton.gameObject.SetActive(config.MaxLevel != currentLevel);
            gameObject.SetActive(true);

            GameManager.Instance.isRuneChoosingTime = false;
        }

        public void ClosePopup()
        {
            GameManager.Instance.isRuneChoosingTime = true;
            gameObject.SetActive(false);
        }

        private void OnClick()
        {
            GameManager.Instance.UpgradePowerup(config.powerupType, config.GetUpgradePrice(currentLevel));
            ClosePopup();
        }

        private void Awake()
        {
            purchaseButton.onClick.AddListener(OnClick);
            gameObject.SetActive(false);
            Localization.RegisterWatcher(this);
        }

        public void LocaleChanged()
        {
            if (config == null) return;

            nameText.text = config.PowerupName;
            descriptionText.text = config.Description;
        }

        private void OnDestroy()
        {
            Localization.DeregisterWatcher(this);
        }
    }
}