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
    public Image DashChargedImage;
    public Image AssimilateCooldownImage;
    public Image AssimilateChargedImage;

    void OnValidate()
    {
        if (PlayerHealth == null) Debug.LogError("No Slider provided for the players health");;
        if (DashCooldownImage == null) Debug.LogError("No Image provided for the players dash cooldown");;
        if (DashChargedImage == null) Debug.LogError("No Image provided for the players dash charged image"); ;
        if (AssimilateCooldownImage == null) Debug.LogError("No Image provided for the players assimialte cooldown image"); ;
        if (AssimilateChargedImage == null) Debug.LogError("No Image provided for the players assimilate charged image"); ;
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
        if (cell.IsPlayer)
            cell.OnAssimilateCooldownChange += UpdateAssimilateCooldown;
    }

    void Unsubscribe(Cell cell)
    {
        cell.OnHealthChange -= UpdateHealth;
        cell.OnDashCooldownChange -= UpdateDashCooldown;
        if (cell.IsPlayer)
            cell.OnAssimilateCooldownChange -= UpdateAssimilateCooldown;
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
        if (currentCooldown <= 0 && DashChargedImage.color.a == 0f)
        {
            Color color = DashChargedImage.color;
            color.a = 1f;
            DashChargedImage.color = color;
        }
        else if (currentCooldown > 0 && DashChargedImage.color.a != 0f)
        {
            Color color = DashChargedImage.color;
            color.a = 0f;
            DashChargedImage.color = color;
        }
        
        DashCooldownImage.fillAmount = Mathf.Clamp((cooldown - currentCooldown) / cooldown, 0f, 1f);
    }

    public void UpdateAssimilateCooldown(float cooldown, float currentCooldown)
    {
        if (currentCooldown <= 0 && AssimilateChargedImage.color.a == 0f)
        {
            Color color = AssimilateChargedImage.color;
            color.a = 1f;
            AssimilateChargedImage.color = color;
        }
        else if (currentCooldown > 0 && AssimilateChargedImage.color.a != 0f)
        {
            Color color = AssimilateChargedImage.color;
            color.a = 0f;
            AssimilateChargedImage.color = color;
        }

        AssimilateCooldownImage.fillAmount = Mathf.Clamp((cooldown - currentCooldown) / cooldown, 0f, 1f);
    }
}
