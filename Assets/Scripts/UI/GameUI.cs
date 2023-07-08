using Assets.Scripts.Cells;
using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Slider PlayerHealth;
    public Image DashCooldownImage;

    void OnValidate()
    {
        if (PlayerHealth == null) Debug.LogError("No Slider provided for the players health");;
        if (DashCooldownImage == null) Debug.LogError("No Image provided for the players dash cooldown");;
    }

    private void Start()
    {
        Player.Instance.OnCellSwap += Subscribe;
    }

    private void OnDestroy()
    {
        Player.Instance.OnCellSwap -= Unsubscribe;
    }

    void Subscribe(Cell cell)
    {
        cell.OnHealthChange += UpdateHealth;
        cell.OnDashCooldownChange += UpdateDashCooldown;
    }

    void Unsubscribe(Cell cell)
    {
        cell.OnHealthChange -= UpdateHealth;
        cell.OnDashCooldownChange -= UpdateDashCooldown;
    }

    [Command("update-health")]
    void UpdateHealth(float newHealth, float totalHealth)
    {
        PlayerHealth.maxValue = totalHealth;
        PlayerHealth.value = newHealth;
    }

    [Command("update-dash")]
    void UpdateDashCooldown(float cooldown, float currentCooldown)
    {
        DashCooldownImage.fillAmount = Mathf.Clamp((cooldown - currentCooldown) / cooldown, 0f, 1f);
    }
}
