using UnityEngine;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

public class HUDManager : MonoBehaviour
{
    public Slider oxygenBar;
    public Text treasureLabel;

    private void Start()
    {
        PlayerManager.Instance.OnDeath += OnDeath;
    }

    private void Update()
    {
        oxygenBar.value = PlayerManager.Instance.OxygenPercent;
        treasureLabel.text = PlayerManager.Instance.Treasure.ToString();
    }

    private void OnDeath()
    {
        //show death screen
    }
}
