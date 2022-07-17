using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.UI;

public class BeatCityHUD : MonoBehaviour, MMEventListener<TopDownEngineEvent>
{
    /// The playerID associated to this HUD
    [Tooltip("The playerID associated to this HUD")]
    public string PlayerID = "Player1";

    /// the progress bar to use to show the healthbar
    [Tooltip("the progress bar to use to show the healthbar")]
    public MMProgressBar HealthBar;

    /// the progress bar to use to show the energybar
    [Tooltip("the progress bar to use to show the energybar")]
    public MMProgressBar EnergyBar;

    /// the Text comp to use to display the player name
    [Tooltip("the Text comp to use to display the player name")]
    public Text PlayerName;

    /// the radial progress bar to put around the avatar
    [Tooltip("the radial progress bar to put around the avatar")]
    public MMRadialProgressBar AvatarBar;

    /// the mask to use when the target player dies
    [Tooltip("the mask to use when the target player dies")]
    public CanvasGroup DeadMask;

    /// the screen to display if the target player wins
    [Tooltip("the screen to display if the target player wins")]
    public CanvasGroup WinnerScreen;

    protected virtual void Start()
    {
        DeadMask.gameObject.SetActive(false);
        WinnerScreen.gameObject.SetActive(false);
    }

    /// <summary>
    ///     OnDisable, we start listening to events.
    /// </summary>
    protected virtual void OnEnable()
    {
        this.MMEventStartListening();
    }

    /// <summary>
    ///     OnDisable, we stop listening to events.
    /// </summary>
    protected virtual void OnDisable()
    {
        this.MMEventStopListening();
    }

    public virtual void OnMMEvent(TopDownEngineEvent tdEvent)
    {
        switch (tdEvent.EventType)
        {
            case TopDownEngineEventTypes.PlayerDeath:
                if (tdEvent.OriginCharacter.PlayerID == PlayerID)
                {
                    DeadMask.gameObject.SetActive(true);
                    DeadMask.alpha = 0f;
                    StartCoroutine(MMFade.FadeCanvasGroup(DeadMask, 0.5f, 0.8f));
                }

                break;
            case TopDownEngineEventTypes.GameOver:
                if (PlayerID == (LevelManager.Instance as GrasslandsMultiplayerLevelManager).WinnerID)
                {
                    WinnerScreen.gameObject.SetActive(true);
                    WinnerScreen.alpha = 0f;
                    StartCoroutine(MMFade.FadeCanvasGroup(WinnerScreen, 0.5f, 0.8f));
                }

                break;
        }
    }
}