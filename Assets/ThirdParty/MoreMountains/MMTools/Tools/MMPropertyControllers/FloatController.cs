using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MoreMountains.Tools
{
    public class MonoAttribute
    {
        public enum MemberTypes
        {
            Property,
            Field
        }

        public FieldInfo MemberFieldInfo;
        public string MemberName;
        public PropertyInfo MemberPropertyInfo;
        public MemberTypes MemberType;
        public MonoBehaviour TargetObject;

        public MonoAttribute(MonoBehaviour targetObject, MemberTypes type, PropertyInfo propertyInfo,
            FieldInfo fieldInfo, string memberName)
        {
            TargetObject = targetObject;
            MemberType = type;
            MemberPropertyInfo = propertyInfo;
            MemberFieldInfo = fieldInfo;
            MemberName = memberName;
        }

        public virtual float GetValue()
        {
            if (MemberType == MemberTypes.Property)
                return (float)MemberPropertyInfo.GetValue(TargetObject);
            if (MemberType == MemberTypes.Field) return (float)MemberFieldInfo.GetValue(TargetObject);
            return 0f;
        }

        public virtual void SetValue(float newValue)
        {
            if (MemberType == MemberTypes.Property)
                MemberPropertyInfo.SetValue(TargetObject, newValue);
            else if (MemberType == MemberTypes.Field) MemberFieldInfo.SetValue(TargetObject, newValue);
        }
    }

    /// <summary>
    ///     A class used to control a float in any other class, over time
    ///     To use it, simply drag a monobehaviour in its target field, pick a control mode (ping pong or random), and tweak
    ///     the settings
    /// </summary>
    [AddComponentMenu("More Mountains/Tools/Property Controllers/FloatController")]
    [MMRequiresConstantRepaint]
    public class FloatController : MMMonoBehaviour
    {
        public enum AudioAnalyzerModes
        {
            Beat,
            NormalizedBufferedBandLevels
        }

        /// the possible control modes
        public enum ControlModes
        {
            PingPong,
            Random,
            OneTime,
            AudioAnalyzer,
            ToDestination,
            Driven
        }

        public const string _undefinedString = "<Undefined Attribute>";

        [Header("Target")]
        /// the mono on which the float you want to control is
        public MonoBehaviour TargetObject;

        [Header("Global Settings")]
        /// the control mode (ping pong or random)
        public ControlModes ControlMode;

        /// whether or not the updated value should be added to the initial one
        public bool AddToInitialValue;

        /// whether or not to use unscaled time
        public bool UseUnscaledTime = true;

        /// whether or not you want to revert to the InitialValue after the control ends
        public bool RevertToInitialValueAfterEnd = true;

        [Header("Driven")]
        /// the value that will be applied to the controlled float in driven mode 
        public float DrivenLevel;

        [Header("Ping Pong")]
        /// the curve to apply to the tween
        public MMTweenType Curve = new(MMTween.MMTweenCurve.EaseInCubic);

        /// the minimum value for the ping pong
        public float MinValue;

        /// the maximum value for the ping pong
        public float MaxValue = 5f;

        /// the duration of one ping (or pong)
        public float Duration = 1f;

        /// the duration (in seconds) between a ping and a pong
        public float PingPongPauseDuration;

        [Header("Random")] [MMVector("Min", "Max")]
        /// the noise amplitude
        public Vector2 Amplitude = new(0f, 5f);

        [MMVector("Min", "Max")]
        /// the noise frequency
        public Vector2 Frequency = new(1f, 1f);

        [MMVector("Min", "Max")]
        /// the noise shift
        public Vector2 Shift = new(0f, 1f);

        /// if this is true, will let you remap the noise value (without amplitude) to the bounds you've specified
        public bool RemapNoiseValues;

        /// the value to which to remap the random's zero bound
        [MMCondition("RemapNoiseValues", true)]
        public float RemapNoiseZero;

        /// the value to which to remap the random's one bound
        [MMCondition("RemapNoiseValues", true)]
        public float RemapNoiseOne = 1f;

        [Header("OneTime")]
        /// the duration of the One Time shake
        public float OneTimeDuration = 1f;

        /// the amplitude of the One Time shake (this will be multiplied by the curve's height)
        public float OneTimeAmplitude = 1f;

        /// the low value to remap the normalized curve value to
        public float OneTimeRemapMin;

        /// the high value to remap the normalized curve value to
        public float OneTimeRemapMax = 1f;

        /// the curve to apply to the one time shake
        public AnimationCurve OneTimeCurve = new(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));

        /// whether or not this controller should go to sleep after a one time shake
        public bool DisableAfterOneTime;

        /// whether or not this controller should go back to sleep after a OneTime
        public bool DisableGameObjectAfterOneTime;

        [MMInspectorButton("OneTime")]
        /// a test button for the one time shake
        public bool OneTimeButton;

        [Header("ToDestination")]
        /// the duration of the tween to the destination value
        public float ToDestinationDuration = 1f;

        /// the value to tween to
        public float ToDestinationValue = 1f;

        /// the curve to use when tweening a value to destination
        public AnimationCurve ToDestinationCurve =
            new(new Keyframe(0, 0), new Keyframe(0.5f, 0.6f), new Keyframe(1f, 1f));

        /// whether or not this controller should go to sleep after a to destination shake
        public bool DisableAfterToDestination;

        [MMInspectorButton("ToDestination")]
        /// a test button for the one time shake
        public bool ToDestinationButton;

        [Header("AudioAnalyzer")]
        /// the audio analyzer to read the value on
        public MMAudioAnalyzer AudioAnalyzer;

        /// whether to look at a Beat or at the normalized buffered band levels
        public AudioAnalyzerModes AudioAnalyzerMode = AudioAnalyzerModes.Beat;

        /// the ID of the beat to listen to
        public int BeatID;

        /// when in NormalizedBufferedBandLevels
        public int NormalizedLevelID;

        /// a multiplier to apply to the output beat value
        public float AudioAnalyzerMultiplier = 1f;

        [Header("Debug")] [MMReadOnly]
        /// the initial value of the controlled float
        public float InitialValue;

        [MMReadOnly]
        /// the current value of the controlled float
        public float CurrentValue;

        [MMReadOnly]
        /// the current value of the controlled float, normalized
        public float CurrentValueNormalized;

        /// internal use only
        [HideInInspector] public float PingPong;

        /// internal use only
        [HideInInspector] public string[] AttributeNames;

        /// internal use only
        [HideInInspector] public string PropertyName;

        /// internal use only
        [HideInInspector] public int ChoiceIndex;

        protected bool _attributeFound;

        protected List<string> _attributesNamesTempList;
        protected float _elapsedTime;
        protected FieldInfo[] _fieldReferences;
        protected float _initialValue;
        protected float _lastPingPongPauseAt;

        protected float _pingPongDirection = 1f;
        protected PropertyInfo[] _propertyReferences;

        protected float _randomAmplitude;
        protected float _randomFrequency;
        protected float _randomShift;
        protected float _remappedTimeSinceStart;
        protected float _shakeStartTimestamp;

        protected bool _shaking;
        protected MonoAttribute _targetAttributeLastFrame;

        protected MonoBehaviour _targetObjectLastFrame;

        /// internal use only
        [HideInInspector] public MonoAttribute TargetAttribute;

        /// <summary>
        ///     On start we initialize our controller
        /// </summary>
        protected virtual void Awake()
        {
            Initialization();
        }

        /// <summary>
        ///     On Update, we move our value based on the defined settings
        /// </summary>
        protected virtual void Update()
        {
            _targetObjectLastFrame = TargetObject;
            _targetAttributeLastFrame = TargetAttribute;

            if (TargetObject == null || TargetAttribute == null || !_attributeFound) return;

            switch (ControlMode)
            {
                case ControlModes.PingPong:

                    if (GetTime() - _lastPingPongPauseAt < PingPongPauseDuration) return;
                    PingPong += GetDeltaTime() * _pingPongDirection;

                    if (PingPong < 0f)
                    {
                        PingPong = 0f;
                        _pingPongDirection = -_pingPongDirection;
                        _lastPingPongPauseAt = GetTime();
                    }

                    if (PingPong > Duration)
                    {
                        PingPong = Duration;
                        _pingPongDirection = -_pingPongDirection;
                        _lastPingPongPauseAt = GetTime();
                    }

                    CurrentValue = MMTween.Tween(PingPong, 0f, Duration, MinValue, MaxValue, Curve);
                    CurrentValueNormalized = MMMaths.Remap(CurrentValue, MinValue, MaxValue, 0f, 1f);
                    break;
                case ControlModes.Random:
                    _elapsedTime += GetDeltaTime();
                    CurrentValueNormalized = Mathf.PerlinNoise(_randomFrequency * _elapsedTime, _randomShift);
                    if (RemapNoiseValues)
                    {
                        CurrentValue = CurrentValueNormalized;
                        CurrentValue = MMMaths.Remap(CurrentValue, 0f, 1f, RemapNoiseZero, RemapNoiseOne);
                    }
                    else
                    {
                        CurrentValue = (CurrentValueNormalized * 2.0f - 1.0f) * _randomAmplitude;
                    }

                    break;
                case ControlModes.OneTime:
                    if (!_shaking) return;
                    _remappedTimeSinceStart =
                        MMMaths.Remap(GetTime() - _shakeStartTimestamp, 0f, OneTimeDuration, 0f, 1f);
                    CurrentValueNormalized = OneTimeCurve.Evaluate(_remappedTimeSinceStart);
                    CurrentValue = MMMaths.Remap(CurrentValueNormalized, 0f, 1f, OneTimeRemapMin, OneTimeRemapMax);
                    CurrentValue *= OneTimeAmplitude;
                    break;
                case ControlModes.AudioAnalyzer:
                    if (AudioAnalyzerMode == AudioAnalyzerModes.Beat)
                        CurrentValue = AudioAnalyzer.Beats[BeatID].CurrentValue * AudioAnalyzerMultiplier;
                    else
                        CurrentValue = AudioAnalyzer.NormalizedBufferedBandLevels[NormalizedLevelID] *
                                       AudioAnalyzerMultiplier;
                    CurrentValueNormalized = Mathf.Clamp(CurrentValue, 0f, 1f);
                    break;
                case ControlModes.Driven:
                    CurrentValue = DrivenLevel;
                    CurrentValueNormalized = Mathf.Clamp(CurrentValue, 0f, 1f);
                    break;
                case ControlModes.ToDestination:
                    if (!_shaking) return;
                    _remappedTimeSinceStart = MMMaths.Remap(GetTime() - _shakeStartTimestamp, 0f, ToDestinationDuration,
                        0f, 1f);
                    var time = ToDestinationCurve.Evaluate(_remappedTimeSinceStart);
                    CurrentValue = Mathf.LerpUnclamped(_initialValue, ToDestinationValue, time);
                    CurrentValueNormalized = MMMaths.Remap(CurrentValue, _initialValue, ToDestinationValue, 0f, 1f);
                    break;
            }


            if (AddToInitialValue) CurrentValue += InitialValue;

            if (ControlMode == ControlModes.OneTime)
                if (_shaking && GetTime() - _shakeStartTimestamp > OneTimeDuration)
                {
                    _shaking = false;
                    if (RevertToInitialValueAfterEnd)
                    {
                        CurrentValue = InitialValue;
                        TargetAttribute.SetValue(CurrentValue);
                    }
                    else
                    {
                        CurrentValue = OneTimeCurve.Evaluate(1f);
                        CurrentValue = MMMaths.Remap(CurrentValue, 0f, 1f, OneTimeRemapMin, OneTimeRemapMax);
                        CurrentValue *= OneTimeAmplitude;
                        TargetAttribute.SetValue(CurrentValue);
                    }

                    if (DisableAfterOneTime) enabled = false;
                    if (DisableGameObjectAfterOneTime) gameObject.SetActive(false);
                    return;
                }

            if (ControlMode == ControlModes.ToDestination)
                if (_shaking && GetTime() - _shakeStartTimestamp > ToDestinationDuration)
                {
                    _shaking = false;
                    if (RevertToInitialValueAfterEnd)
                        CurrentValue = InitialValue;
                    else
                        CurrentValue = ToDestinationValue;
                    TargetAttribute.SetValue(CurrentValue);

                    if (DisableAfterOneTime) enabled = false;
                    if (DisableGameObjectAfterOneTime) gameObject.SetActive(false);
                    return;
                }

            TargetAttribute.SetValue(CurrentValue);
        }

        /// <summary>
        ///     On enable, grabs the initial value
        /// </summary>
        protected virtual void OnEnable()
        {
            InitialValue = GetInitialValue();
        }

        /// <summary>
        ///     On disable we revert to the previous value if needed
        /// </summary>
        protected virtual void OnDisable()
        {
            if (RevertToInitialValueAfterEnd)
            {
                CurrentValue = InitialValue;
                TargetAttribute.SetValue(CurrentValue);
            }
        }

        /// <summary>
        ///     When the contents of the inspector change, and if the target changed, we grab all its properties and store them
        /// </summary>
        protected virtual void OnValidate()
        {
            FillDropDownList();
            if (Application.isPlaying
                && (_targetAttributeLastFrame != TargetAttribute || _targetObjectLastFrame != TargetObject))
                Initialization();
        }

        /// <summary>
        ///     Finds an attribute (property or field) on the target object
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual bool FindAttribute(string propertyName)
        {
            FieldInfo fieldInfo = null;
            PropertyInfo propInfo = null;
            TargetAttribute = null;

            propInfo = TargetObject.GetType().GetProperty(propertyName);
            if (propInfo == null) fieldInfo = TargetObject.GetType().GetField(propertyName);
            if (propInfo != null)
                TargetAttribute = new MonoAttribute(TargetObject, MonoAttribute.MemberTypes.Property, propInfo, null,
                    propertyName);
            if (fieldInfo != null)
                TargetAttribute = new MonoAttribute(TargetObject, MonoAttribute.MemberTypes.Field, null, fieldInfo,
                    propertyName);
            if (PropertyName == _undefinedString)
            {
                Debug.LogError("FloatController " + name + " : you need to pick a property from the Property list");
                return false;
            }

            if (propInfo == null && fieldInfo == null)
            {
                Debug.LogError("FloatController " + name + " couldn't find any property or field named " +
                               propertyName + " on " + TargetObject.name);
                return false;
            }

            if (TargetAttribute.MemberType == MonoAttribute.MemberTypes.Property)
                TargetAttribute.MemberPropertyInfo = TargetObject.GetType().GetProperty(TargetAttribute.MemberName);
            else if (TargetAttribute.MemberType == MonoAttribute.MemberTypes.Field)
                TargetAttribute.MemberFieldInfo = TargetObject.GetType().GetField(TargetAttribute.MemberName);

            return true;
        }

        /// <summary>
        ///     Grabs the target property and initializes stuff
        /// </summary>
        public virtual void Initialization()
        {
            _attributeFound = FindAttribute(PropertyName);
            if (!_attributeFound) return;

            if (TargetObject == null || string.IsNullOrEmpty(TargetAttribute.MemberName)) return;

            _elapsedTime = 0f;
            _randomAmplitude = Random.Range(Amplitude.x, Amplitude.y);
            _randomFrequency = Random.Range(Frequency.x, Frequency.y);
            _randomShift = Random.Range(Shift.x, Shift.y);

            InitialValue = GetInitialValue();

            _shaking = false;
        }

        /// <summary>
        ///     Grabs the initial float value
        /// </summary>
        /// <returns></returns>
        protected virtual float GetInitialValue()
        {
            if (TargetAttribute.MemberType == MonoAttribute.MemberTypes.Property)
                return (float)TargetAttribute.MemberPropertyInfo.GetValue(TargetObject);
            if (TargetAttribute.MemberType == MonoAttribute.MemberTypes.Field)
                return (float)TargetAttribute.MemberFieldInfo.GetValue(TargetObject);
            return 0f;
        }

        /// <summary>
        ///     Sets the level to the value passed in parameters
        /// </summary>
        /// <param name="level"></param>
        public virtual void SetDrivenLevelAbsolute(float level)
        {
            DrivenLevel = level;
        }

        /// <summary>
        ///     Sets the level to the remapped value passed in parameters
        /// </summary>
        /// <param name="normalizedLevel"></param>
        /// <param name="remapZero"></param>
        /// <param name="remapOne"></param>
        public virtual void SetDrivenLevelNormalized(float normalizedLevel, float remapZero, float remapOne)
        {
            DrivenLevel = MMMaths.Remap(normalizedLevel, 0f, 1f, remapZero, remapOne);
        }

        /// <summary>
        ///     Triggers a one time shake of the float controller
        /// </summary>
        public virtual void OneTime()
        {
            if (TargetObject == null || TargetAttribute == null) return;

            gameObject.SetActive(true);
            enabled = true;
            _shakeStartTimestamp = GetTime();
            _shaking = true;
        }

        /// <summary>
        ///     Triggers a one time shake of the controller to a specified destination value
        /// </summary>
        public virtual void ToDestination()
        {
            if (TargetObject == null || TargetAttribute == null) return;

            enabled = true;
            ControlMode = ControlModes.ToDestination;
            _shakeStartTimestamp = GetTime();
            _shaking = true;
            _initialValue = GetInitialValue();
        }

        /// <summary>
        ///     Returns the relevant delta time
        /// </summary>
        /// <returns></returns>
        protected float GetDeltaTime()
        {
            return UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        }

        /// <summary>
        ///     Returns the relevant time
        /// </summary>
        /// <returns></returns>
        protected float GetTime()
        {
            return UseUnscaledTime ? Time.unscaledTime : Time.time;
        }

        /// <summary>
        ///     Interrupts any tween in progress, and disables itself
        /// </summary>
        public virtual void Stop()
        {
            _shaking = false;
            enabled = false;
        }

        /// <summary>
        ///     Fills the inspector dropdown with all the possible choices
        /// </summary>
        public virtual void FillDropDownList()
        {
            AttributeNames = new string[0];

            if (TargetObject == null) return;

            _propertyReferences = TargetObject.GetType().GetProperties();
            _attributesNamesTempList = new List<string>();
            _attributesNamesTempList.Add(_undefinedString);

            foreach (var propertyInfo in _propertyReferences)
                if (propertyInfo.PropertyType.Name == "Single")
                    _attributesNamesTempList.Add(propertyInfo.Name);

            _fieldReferences = TargetObject.GetType().GetFields();
            foreach (var fieldInfo in _fieldReferences)
                if (fieldInfo.FieldType.Name == "Single")
                    _attributesNamesTempList.Add(fieldInfo.Name);

            // we fill our dropdown list of names :
            AttributeNames = _attributesNamesTempList.ToArray();
        }
    }
}