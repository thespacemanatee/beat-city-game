using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MoreMountains.Tools
{
	/// <summary>
	///     Add this component to an object and it will show a healthbar above it
	///     You can either use a prefab for it, or have the component draw one at the start
	/// </summary>
	[AddComponentMenu("More Mountains/Tools/GUI/MMHealthBar")]
    public class MMHealthBar : MonoBehaviour
    {
        /// the possible health bar types
        public enum HealthBarTypes
        {
            Prefab,
            Drawn
        }

        /// the possible timescales the bar can work on
        public enum TimeScales
        {
            UnscaledTime,
            Time
        }

        [MMInformation(
            "Add this component to an object and it'll add a healthbar next to it to reflect its health level in real time. You can decide here whether the health bar should be drawn automatically or use a prefab.",
            MMInformationAttribute.InformationType.Info, false)]
        /// whether the healthbar uses a prefab or is drawn automatically
        public HealthBarTypes HealthBarType = HealthBarTypes.Drawn;

        /// defines whether the bar will work on scaled or unscaled time (whether or not it'll keep moving if time is slowed down for example)
        public TimeScales TimeScale = TimeScales.UnscaledTime;

        [Header("Select a Prefab")]
        [MMInformation(
            "Select a prefab with a progress bar script on it. There is one example of such a prefab in Common/Prefabs/GUI.",
            MMInformationAttribute.InformationType.Info, false)]
        /// the prefab to use as the health bar
        public MMProgressBar HealthBarPrefab;

        [Header("Drawn Healthbar Settings ")]
        [MMInformation("Set the size (in world units), padding, back and front colors of the healthbar.",
            MMInformationAttribute.InformationType.Info, false)]
        /// if the healthbar is drawn, its size in world units
        public Vector2 Size = new(1f, 0.2f);

        /// if the healthbar is drawn, the padding to apply to the foreground, in world units
        public Vector2 BackgroundPadding = new(0.01f, 0.01f);

        /// the rotation to apply to the MMHealthBarContainer when drawing it
        public Vector3 InitialRotationAngles;

        /// if the healthbar is drawn, the color of its foreground
        public Gradient ForegroundColor = new()
        {
            colorKeys = new GradientColorKey[2]
            {
                new(MMColors.BestRed, 0),
                new(MMColors.BestRed, 1f)
            },
            alphaKeys = new GradientAlphaKey[2] { new(1, 0), new(1, 1) }
        };

        /// if the healthbar is drawn, the color of its delayed bar
        public Gradient DelayedColor = new()
        {
            colorKeys = new GradientColorKey[2]
            {
                new(MMColors.Orange, 0),
                new(MMColors.Orange, 1f)
            },
            alphaKeys = new GradientAlphaKey[2] { new(1, 0), new(1, 1) }
        };

        /// if the healthbar is drawn, the color of its border
        public Gradient BorderColor = new()
        {
            colorKeys = new GradientColorKey[2]
            {
                new(MMColors.AntiqueWhite, 0),
                new(MMColors.AntiqueWhite, 1f)
            },
            alphaKeys = new GradientAlphaKey[2] { new(1, 0), new(1, 1) }
        };

        /// if the healthbar is drawn, the color of its background
        public Gradient BackgroundColor = new()
        {
            colorKeys = new GradientColorKey[2]
            {
                new(MMColors.Black, 0),
                new(MMColors.Black, 1f)
            },
            alphaKeys = new GradientAlphaKey[2] { new(1, 0), new(1, 1) }
        };

        /// the name of the sorting layer to put this health bar on
        public string SortingLayerName = "UI";

        /// the delay to apply to the delayed bar if drawn
        public float Delay = 0.5f;

        /// whether or not the front bar should lerp
        public bool LerpFrontBar = true;

        /// the speed at which the front bar lerps
        public float LerpFrontBarSpeed = 15f;

        /// whether or not the delayed bar should lerp
        public bool LerpDelayedBar = true;

        /// the speed at which the delayed bar lerps
        public float LerpDelayedBarSpeed = 15f;

        /// if this is true, bumps the scale of the healthbar when its value changes
        public bool BumpScaleOnChange = true;

        /// the duration of the bump animation
        public float BumpDuration = 0.2f;

        /// the animation curve to map the bump animation on
        public AnimationCurve BumpAnimationCurve = AnimationCurve.Constant(0, 1, 1);

        /// the mode the bar should follow the target in
        public MMFollowTarget.UpdateModes FollowTargetMode = MMFollowTarget.UpdateModes.LateUpdate;

        public bool NestDrawnHealthBar;

        [Header("Death")]
        /// a gameobject (usually a particle system) to instantiate when the healthbar reaches zero
        public GameObject InstantiatedOnDeath;

        [Header("Offset")]
        [MMInformation(
            "Set the offset (in world units), relative to the object's center, to which the health bar will be displayed.",
            MMInformationAttribute.InformationType.Info, false)]
        /// the offset to apply to the healthbar compared to the object's center
        public Vector3 HealthBarOffset = new(0f, 1f, 0f);

        [Header("Display")]
        [MMInformation(
            "Here you can define whether or not the healthbar should always be visible. If not, you can set here how long after a hit it'll remain visible.",
            MMInformationAttribute.InformationType.Info, false)]
        /// whether or not the bar should be permanently displayed
        public bool AlwaysVisible = true;

        /// the duration (in seconds) during which to display the bar
        public float DisplayDurationOnHit = 1f;

        /// if this is set to true the bar will hide itself when it reaches zero
        public bool HideBarAtZero = true;

        /// the delay (in seconds) after which to hide the bar
        public float HideBarAtZeroDelay = 1f;

        protected Image _backgroundImage;
        protected Image _borderImage;
        protected Image _delayedImage;
        protected bool _finalHideStarted;
        protected MMFollowTarget _followTransform;
        protected Image _foregroundImage;
        protected float _lastShowTimestamp;

        protected MMProgressBar _progressBar;
        protected bool _showBar;

        /// <summary>
        ///     On Start, creates or sets the health bar up
        /// </summary>
        protected virtual void Awake()
        {
            Initialization();
        }

        /// <summary>
        ///     On Update, we hide or show our healthbar based on our current status
        /// </summary>
        protected virtual void Update()
        {
            if (_progressBar == null) return;

            if (_finalHideStarted) return;

            UpdateDrawnColors();

            if (AlwaysVisible) return;

            if (_showBar)
            {
                _progressBar.gameObject.SetActive(true);
                var currentTime = TimeScale == TimeScales.UnscaledTime ? Time.unscaledTime : Time.time;
                if (currentTime - _lastShowTimestamp > DisplayDurationOnHit) _showBar = false;
            }
            else
            {
                _progressBar.gameObject.SetActive(false);
            }
        }

        /// <summary>
        ///     On enable, initializes the bar again
        /// </summary>
        protected void OnEnable()
        {
            _finalHideStarted = false;

            if (!AlwaysVisible && _progressBar != null) _progressBar.gameObject.SetActive(false);
        }

        public virtual void Initialization()
        {
            _finalHideStarted = false;

            if (_progressBar != null)
            {
                _progressBar.gameObject.SetActive(AlwaysVisible);
                return;
            }

            if (HealthBarType == HealthBarTypes.Prefab)
            {
                if (HealthBarPrefab == null)
                {
                    Debug.LogWarning(name +
                                     " : the HealthBar has no prefab associated to it, nothing will be displayed.");
                    return;
                }

                _progressBar = Instantiate(HealthBarPrefab, transform.position + HealthBarOffset, transform.rotation);
                SceneManager.MoveGameObjectToScene(_progressBar.gameObject, gameObject.scene);
                _progressBar.transform.SetParent(transform);
                _progressBar.gameObject.name = "HealthBar";
            }

            if (HealthBarType == HealthBarTypes.Drawn)
            {
                DrawHealthBar();
                UpdateDrawnColors();
            }

            if (!AlwaysVisible) _progressBar.gameObject.SetActive(false);

            if (_progressBar != null) _progressBar.SetBar(100f, 0f, 100f);
        }

        /// <summary>
        ///     Draws the health bar.
        /// </summary>
        protected virtual void DrawHealthBar()
        {
            var newGameObject = new GameObject();
            SceneManager.MoveGameObjectToScene(newGameObject, gameObject.scene);
            newGameObject.name = "HealthBar|" + gameObject.name;

            if (NestDrawnHealthBar) newGameObject.transform.SetParent(transform);

            _progressBar = newGameObject.AddComponent<MMProgressBar>();

            _followTransform = newGameObject.AddComponent<MMFollowTarget>();
            _followTransform.Offset = HealthBarOffset;
            _followTransform.Target = transform;
            _followTransform.FollowRotation = false;
            _followTransform.InterpolatePosition = false;
            _followTransform.InterpolateRotation = false;
            _followTransform.UpdateMode = FollowTargetMode;

            var newCanvas = newGameObject.AddComponent<Canvas>();
            newCanvas.renderMode = RenderMode.WorldSpace;
            newCanvas.transform.localScale = Vector3.one;
            newCanvas.GetComponent<RectTransform>().sizeDelta = Size;
            if (!string.IsNullOrEmpty(SortingLayerName)) newCanvas.sortingLayerName = SortingLayerName;

            var container = new GameObject();
            container.transform.SetParent(newGameObject.transform);
            container.name = "MMProgressBarContainer";
            container.transform.localScale = Vector3.one;

            var borderImageGameObject = new GameObject();
            borderImageGameObject.transform.SetParent(container.transform);
            borderImageGameObject.name = "HealthBar Border";
            _borderImage = borderImageGameObject.AddComponent<Image>();
            _borderImage.transform.position = Vector3.zero;
            _borderImage.transform.localScale = Vector3.one;
            _borderImage.GetComponent<RectTransform>().sizeDelta = Size;
            _borderImage.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

            var bgImageGameObject = new GameObject();
            bgImageGameObject.transform.SetParent(container.transform);
            bgImageGameObject.name = "HealthBar Background";
            _backgroundImage = bgImageGameObject.AddComponent<Image>();
            _backgroundImage.transform.position = Vector3.zero;
            _backgroundImage.transform.localScale = Vector3.one;
            _backgroundImage.GetComponent<RectTransform>().sizeDelta = Size - BackgroundPadding * 2;
            _backgroundImage.GetComponent<RectTransform>().anchoredPosition =
                -_backgroundImage.GetComponent<RectTransform>().sizeDelta / 2;
            _backgroundImage.GetComponent<RectTransform>().pivot = Vector2.zero;

            var delayedImageGameObject = new GameObject();
            delayedImageGameObject.transform.SetParent(container.transform);
            delayedImageGameObject.name = "HealthBar Delayed Foreground";
            _delayedImage = delayedImageGameObject.AddComponent<Image>();
            _delayedImage.transform.position = Vector3.zero;
            _delayedImage.transform.localScale = Vector3.one;
            _delayedImage.GetComponent<RectTransform>().sizeDelta = Size - BackgroundPadding * 2;
            _delayedImage.GetComponent<RectTransform>().anchoredPosition =
                -_delayedImage.GetComponent<RectTransform>().sizeDelta / 2;
            _delayedImage.GetComponent<RectTransform>().pivot = Vector2.zero;

            var frontImageGameObject = new GameObject();
            frontImageGameObject.transform.SetParent(container.transform);
            frontImageGameObject.name = "HealthBar Foreground";
            _foregroundImage = frontImageGameObject.AddComponent<Image>();
            _foregroundImage.transform.position = Vector3.zero;
            _foregroundImage.transform.localScale = Vector3.one;
            _foregroundImage.color = ForegroundColor.Evaluate(1);
            _foregroundImage.GetComponent<RectTransform>().sizeDelta = Size - BackgroundPadding * 2;
            _foregroundImage.GetComponent<RectTransform>().anchoredPosition =
                -_foregroundImage.GetComponent<RectTransform>().sizeDelta / 2;
            _foregroundImage.GetComponent<RectTransform>().pivot = Vector2.zero;

            _progressBar.LerpDecreasingDelayedBar = LerpDelayedBar;
            _progressBar.LerpForegroundBar = LerpFrontBar;
            _progressBar.LerpDecreasingDelayedBarSpeed = LerpDelayedBarSpeed;
            _progressBar.LerpForegroundBarSpeedIncreasing = LerpFrontBarSpeed;
            _progressBar.ForegroundBar = _foregroundImage.transform;
            _progressBar.DelayedBarDecreasing = _delayedImage.transform;
            _progressBar.DecreasingDelay = Delay;
            _progressBar.BumpScaleOnChange = BumpScaleOnChange;
            _progressBar.BumpDuration = BumpDuration;
            _progressBar.BumpScaleAnimationCurve = BumpAnimationCurve;
            _progressBar.TimeScale = TimeScale == TimeScales.Time
                ? MMProgressBar.TimeScales.Time
                : MMProgressBar.TimeScales.UnscaledTime;
            container.transform.localEulerAngles = InitialRotationAngles;
            _progressBar.Initialization();
        }

        /// <summary>
        ///     Hides the bar when it reaches zero
        /// </summary>
        /// <returns>The hide bar.</returns>
        protected virtual IEnumerator FinalHideBar()
        {
            _finalHideStarted = true;
            if (InstantiatedOnDeath != null)
            {
                var instantiatedOnDeath = Instantiate(InstantiatedOnDeath, transform.position + HealthBarOffset,
                    transform.rotation);
                SceneManager.MoveGameObjectToScene(instantiatedOnDeath.gameObject, gameObject.scene);
            }

            if (HideBarAtZeroDelay == 0)
            {
                _showBar = false;
                _progressBar.gameObject.SetActive(false);
                yield return null;
            }
            else
            {
                _progressBar.HideBar(HideBarAtZeroDelay);
            }
        }

        /// <summary>
        ///     Updates the colors of the different bars
        /// </summary>
        protected virtual void UpdateDrawnColors()
        {
            if (HealthBarType != HealthBarTypes.Drawn) return;

            if (_progressBar.Bumping) return;

            if (_borderImage != null) _borderImage.color = BorderColor.Evaluate(_progressBar.BarProgress);

            if (_backgroundImage != null) _backgroundImage.color = BackgroundColor.Evaluate(_progressBar.BarProgress);

            if (_delayedImage != null) _delayedImage.color = DelayedColor.Evaluate(_progressBar.BarProgress);

            if (_foregroundImage != null) _foregroundImage.color = ForegroundColor.Evaluate(_progressBar.BarProgress);
        }

        /// <summary>
        ///     Updates the bar
        /// </summary>
        /// <param name="currentHealth">Current health.</param>
        /// <param name="minHealth">Minimum health.</param>
        /// <param name="maxHealth">Max health.</param>
        /// <param name="show">Whether or not we should show the bar.</param>
        public virtual void UpdateBar(float currentHealth, float minHealth, float maxHealth, bool show)
        {
            // if the healthbar isn't supposed to be always displayed, we turn it on for the specified duration
            if (!AlwaysVisible && show)
            {
                _showBar = true;
                _lastShowTimestamp = TimeScale == TimeScales.UnscaledTime ? Time.unscaledTime : Time.time;
            }

            if (_progressBar != null)
            {
                _progressBar.UpdateBar(currentHealth, minHealth, maxHealth);

                if (HideBarAtZero && _progressBar.BarTarget <= 0) StartCoroutine(FinalHideBar());

                if (BumpScaleOnChange) _progressBar.Bump();
            }
        }
    }
}