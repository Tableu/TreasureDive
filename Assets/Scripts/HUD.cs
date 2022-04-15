using UnityEngine;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

public class HUD : MonoBehaviour
{
    public Slider oxygenBar;
    public Text treasureLabel;
    public RectTransform harpoonCooldown;
    [Header("Death Screen")] public Text scoreLabel;
    public GameObject deathScreen;

    private float harpoonHeight;

    private void Start()
    {
        harpoonHeight = harpoonCooldown.rect.height;
        PlayerManager.Instance.OnDeath += OnDeath;
        DungeonManager.Instance.OnRestart += OnRestart;
    }

    private void Update()
    {
        oxygenBar.value = PlayerManager.Instance.OxygenPercent;
        harpoonCooldown.sizeDelta = new Vector2(harpoonCooldown.sizeDelta.x,
            harpoonHeight * PlayerManager.Instance.WeaponCooldownPercent);
        treasureLabel.text = PlayerManager.Instance.Treasure.ToString();
    }

    private void OnDeath()
    {
        deathScreen.SetActive(true);
        scoreLabel.text = "Score: " + PlayerManager.Instance.Treasure;
    }

    private void OnRestart()
    {
        deathScreen.SetActive(false);
        oxygenBar.value = 1;
        treasureLabel.text = "0";
    }

    private void OnDestroy()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnDeath -= OnDeath;
        }

        if (DungeonManager.Instance != null)
        {
            DungeonManager.Instance.OnRestart -= OnRestart;
        }
    }
}
