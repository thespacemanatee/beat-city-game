using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class BeatCityGUIManager : MultiplayerGUIManager
{
    /// the energy bars to update
    [Tooltip("the energy bars to update")] public MMProgressBar[] EnergyBars;

    /// <summary>
    /// Updates the energy bar.
    /// </summary>
    /// <param name="currentEnergy">Current energy.</param>
    /// <param name="minEnergy">Minimum energy.</param>
    /// <param name="maxEnergy">Max energy.</param>
    /// <param name="playerID">Player I.</param>
    public void UpdateEnergyBars(float currentEnergy, float minEnergy, float maxEnergy, string playerID)
    {
        if (EnergyBars == null)
        {
            return;
        }

        foreach (var energyBar in EnergyBars)
        {
            if (energyBar == null)
            {
                continue;
            }

            if (energyBar.PlayerID == playerID)
            {
                energyBar.UpdateBar(currentEnergy, minEnergy, maxEnergy);
            }
        }
    }
}