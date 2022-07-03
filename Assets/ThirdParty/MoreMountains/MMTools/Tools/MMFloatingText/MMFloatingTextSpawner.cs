using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.Tools
{
    #region Events

    /// <summary>
    ///     An event used (usually by feedbacks) to trigger the spawn of a new floating text
    /// </summary>
    public struct MMFloatingTextSpawnEvent
    {
        public delegate void Delegate(int channel, Vector3 spawnPosition, string value, Vector3 direction,
            float intensity,
            bool forceLifetime = false, float lifetime = 1f, bool forceColor = false,
            Gradient animateColorGradient = null, bool useUnscaledTime = false);

        private static event Delegate OnEvent;

        public static void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        public static void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        public static void Trigger(int channel, Vector3 spawnPosition, string value, Vector3 direction, float intensity,
            bool forceLifetime = false, float lifetime = 1f, bool forceColor = false,
            Gradient animateColorGradient = null, bool useUnscaledTime = false)
        {
            OnEvent?.Invoke(channel, spawnPosition, value, direction, intensity, forceLifetime, lifetime, forceColor,
                animateColorGradient, useUnscaledTime);
        }
    }

    #endregion

    /// <summary>
    ///     This class will let you pool, recycle and spawn floating texts, usually to show damage info.
    ///     It requires as input a MMFloatingText object.
    /// </summary>
    public class MMFloatingTextSpawner : MMMonoBehaviour
    {
        /// whether the spawned text should have a fixed alignment, orient to match the initial spawn direction, or its movement curve
        public enum AlignmentModes
        {
            Fixed,
            MatchInitialDirection,
            MatchMovementDirection
        }

        /// whether to spawn a single prefab or one at random
        public enum PoolerModes
        {
            Simple,
            Multiple
        }

        [MMInspectorGroup("General Settings", true, 10)]
        /// the channel to listen for events on. this will have to be matched in the feedbacks trying to command this spawner
        [Tooltip(
            "the channel to listen for events on. this will have to be matched in the feedbacks trying to command this spawner")]
        public int Channel;

        /// whether or not this spawner can spawn at this time
        [Tooltip("whether or not this spawner can spawn at this time")]
        public bool CanSpawn = true;

        /// whether or not this spawner should spawn objects on unscaled time
        [Tooltip("whether or not this spawner should spawn objects on unscaled time")]
        public bool UseUnscaledTime;

        [MMInspectorGroup("Pooler", true)]
        /// the selected pooler mode (single prefab or multiple ones)
        [Tooltip("the selected pooler mode (single prefab or multiple ones)")]
        public PoolerModes PoolerMode = PoolerModes.Simple;

        /// the prefab to spawn (ignored if in multiple mode)
        [Tooltip("the prefab to spawn (ignored if in multiple mode)")]
        public MMFloatingText PooledSimpleMMFloatingText;

        /// the prefabs to spawn (ignored if in simple mode)
        [Tooltip("the prefabs to spawn (ignored if in simple mode)")]
        public List<MMFloatingText> PooledMultipleMMFloatingText;

        /// the amount of objects to pool to avoid having to instantiate them at runtime. Should be bigger than the max amount of texts you plan on having on screen at any given moment
        [Tooltip(
            "the amount of objects to pool to avoid having to instantiate them at runtime. Should be bigger than the max amount of texts you plan on having on screen at any given moment")]
        public int PoolSize = 20;

        /// whether or not to nest the waiting pools
        [Tooltip("whether or not to nest the waiting pools")]
        public bool NestWaitingPool = true;

        /// whether or not to mutualize the waiting pools
        [Tooltip("whether or not to mutualize the waiting pools")]
        public bool MutualizeWaitingPools = true;

        /// whether or not the text pool can expand if the pool is empty
        [Tooltip("whether or not the text pool can expand if the pool is empty")]
        public bool PoolCanExpand = true;

        [MMInspectorGroup("Spawn Settings", true, 14)]
        /// the random min and max lifetime duration for the spawned texts (in seconds)
        [Tooltip("the random min and max lifetime duration for the spawned texts (in seconds)")]
        [MMVector("Min", "Max")]
        public Vector2 Lifetime = Vector2.one;

        [Header("Spawn Position Offset")]
        /// the random min position at which to spawn the text, relative to its intended spawn position
        [Tooltip("the random min position at which to spawn the text, relative to its intended spawn position")]
        public Vector3 SpawnOffsetMin = Vector3.zero;

        /// the random max position at which to spawn the text, relative to its intended spawn position
        [Tooltip("the random max position at which to spawn the text, relative to its intended spawn position")]
        public Vector3 SpawnOffsetMax = Vector3.zero;

        [MMInspectorGroup("Animate Position", true, 15)]
        [Header("Movement")]
        /// whether or not to animate the movement of spawned texts
        [Tooltip("whether or not to animate the movement of spawned texts")]
        public bool AnimateMovement = true;

        /// whether or not to animate the X movement of spawned texts
        [Tooltip("whether or not to animate the X movement of spawned texts")]
        public bool AnimateX;

        /// the value to which the x movement curve's zero should be remapped to
        [Tooltip("the value to which the x movement curve's zero should be remapped to")]
        [MMCondition("AnimateX", true)]
        public Vector2 RemapXZero = Vector2.zero;

        /// the value to which the x movement curve's one should be remapped to
        [Tooltip("the value to which the x movement curve's one should be remapped to")] [MMCondition("AnimateX", true)]
        public Vector2 RemapXOne = Vector2.one;

        /// the curve on which to animate the x movement
        [Tooltip("the curve on which to animate the x movement")] [MMCondition("AnimateX", true)]
        public AnimationCurve AnimateXCurve = new(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        /// whether or not to animate the Y movement of spawned texts
        [Tooltip("whether or not to animate the Y movement of spawned texts")]
        public bool AnimateY = true;

        /// the value to which the y movement curve's zero should be remapped to
        [Tooltip("the value to which the y movement curve's zero should be remapped to")]
        [MMCondition("AnimateY", true)]
        public Vector2 RemapYZero = Vector2.zero;

        /// the value to which the y movement curve's one should be remapped to
        [Tooltip("the value to which the y movement curve's one should be remapped to")] [MMCondition("AnimateY", true)]
        public Vector2 RemapYOne = new(5f, 5f);

        /// the curve on which to animate the y movement
        [Tooltip("the curve on which to animate the y movement")] [MMCondition("AnimateY", true)]
        public AnimationCurve AnimateYCurve = new(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        /// whether or not to animate the Z movement of spawned texts
        [Tooltip("whether or not to animate the Z movement of spawned texts")]
        public bool AnimateZ;

        /// the value to which the z movement curve's zero should be remapped to
        [Tooltip("the value to which the z movement curve's zero should be remapped to")]
        [MMCondition("AnimateZ", true)]
        public Vector2 RemapZZero = Vector2.zero;

        /// the value to which the z movement curve's one should be remapped to
        [Tooltip("the value to which the z movement curve's one should be remapped to")] [MMCondition("AnimateZ", true)]
        public Vector2 RemapZOne = Vector2.one;

        /// the curve on which to animate the z movement
        [Tooltip("the curve on which to animate the z movement")] [MMCondition("AnimateZ", true)]
        public AnimationCurve AnimateZCurve = new(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        [MMInspectorGroup("Facing Directions", true, 16)]
        [Header("Alignment")]
        /// the selected alignment mode (whether the spawned text should have a fixed alignment, orient to match the initial spawn direction, or its movement curve)
        [Tooltip(
            "the selected alignment mode (whether the spawned text should have a fixed alignment, orient to match the initial spawn direction, or its movement curve)")]
        public AlignmentModes AlignmentMode = AlignmentModes.Fixed;

        /// when in fixed mode, the direction in which to keep the spawned texts
        [Tooltip("when in fixed mode, the direction in which to keep the spawned texts")]
        [MMEnumCondition("AlignmentMode", (int)AlignmentModes.Fixed)]
        public Vector3 FixedAlignment = Vector3.up;

        [Header("Billboard")]
        /// whether or not spawned texts should always face the camera
        [Tooltip("whether or not spawned texts should always face the camera")]
        public bool AlwaysFaceCamera;

        /// whether or not this spawner should automatically grab the main camera on start
        [Tooltip("whether or not this spawner should automatically grab the main camera on start")]
        [MMCondition("AlwaysFaceCamera", true)]
        public bool AutoGrabMainCameraOnStart = true;

        /// if not in auto grab mode, the camera to use for billboards
        [Tooltip("if not in auto grab mode, the camera to use for billboards")] [MMCondition("AlwaysFaceCamera", true)]
        public Camera TargetCamera;

        [MMInspectorGroup("Animate Scale", true, 46)]
        /// whether or not to animate the scale of spawned texts
        [Tooltip("whether or not to animate the scale of spawned texts")]
        public bool AnimateScale = true;

        /// the value to which the scale curve's zero should be remapped to
        [Tooltip("the value to which the scale curve's zero should be remapped to")] [MMCondition("AnimateScale", true)]
        public Vector2 RemapScaleZero = Vector2.zero;

        /// the value to which the scale curve's one should be remapped to
        [Tooltip("the value to which the scale curve's one should be remapped to")] [MMCondition("AnimateScale", true)]
        public Vector2 RemapScaleOne = Vector2.one;

        /// the curve on which to animate the scale
        [Tooltip("the curve on which to animate the scale")] [MMCondition("AnimateScale", true)]
        public AnimationCurve AnimateScaleCurve = new(new Keyframe(0f, 0f), new Keyframe(0.15f, 1f),
            new Keyframe(0.85f, 1f), new Keyframe(1f, 0f));

        [MMInspectorGroup("Animate Color", true, 55)]
        /// whether or not to animate the spawned text's color over time
        [Tooltip("whether or not to animate the spawned text's color over time")]
        public bool AnimateColor;

        /// the gradient over which to animate the spawned text's color over time
        [Tooltip("the gradient over which to animate the spawned text's color over time")] [GradientUsage(true)]
        public Gradient AnimateColorGradient = new();

        [MMInspectorGroup("Animate Opacity", true, 45)]
        /// whether or not to animate the opacity of the spawned texts
        [Tooltip("whether or not to animate the opacity of the spawned texts")]
        public bool AnimateOpacity = true;

        /// the value to which the opacity curve's zero should be remapped to
        [Tooltip("the value to which the opacity curve's zero should be remapped to")]
        [MMCondition("AnimateOpacity", true)]
        public Vector2 RemapOpacityZero = Vector2.zero;

        /// the value to which the opacity curve's one should be remapped to
        [Tooltip("the value to which the opacity curve's one should be remapped to")]
        [MMCondition("AnimateOpacity", true)]
        public Vector2 RemapOpacityOne = Vector2.one;

        /// the curve on which to animate the opacity
        [Tooltip("the curve on which to animate the opacity")] [MMCondition("AnimateOpacity", true)]
        public AnimationCurve AnimateOpacityCurve = new(new Keyframe(0f, 0f), new Keyframe(0.2f, 1f),
            new Keyframe(0.8f, 1f), new Keyframe(1f, 0f));

        [MMInspectorGroup("Intensity Multipliers", true, 45)]
        /// whether or not the intensity multiplier should impact lifetime
        [Tooltip("whether or not the intensity multiplier should impact lifetime")]
        public bool IntensityImpactsLifetime;

        /// when getting an intensity multiplier, the value by which to multiply the lifetime
        [Tooltip("when getting an intensity multiplier, the value by which to multiply the lifetime")]
        [MMCondition("IntensityImpactsLifetime", true)]
        public float IntensityLifetimeMultiplier = 1f;

        /// whether or not the intensity multiplier should impact movement
        [Tooltip("whether or not the intensity multiplier should impact movement")]
        public bool IntensityImpactsMovement;

        /// when getting an intensity multiplier, the value by which to multiply the movement values
        [Tooltip("when getting an intensity multiplier, the value by which to multiply the movement values")]
        [MMCondition("IntensityImpactsMovement", true)]
        public float IntensityMovementMultiplier = 1f;

        /// whether or not the intensity multiplier should impact scale
        [Tooltip("whether or not the intensity multiplier should impact scale")]
        public bool IntensityImpactsScale;

        /// when getting an intensity multiplier, the value by which to multiply the scale values
        [Tooltip("when getting an intensity multiplier, the value by which to multiply the scale values")]
        [MMCondition("IntensityImpactsScale", true)]
        public float IntensityScaleMultiplier = 1f;

        [MMInspectorGroup("Debug", true, 12)]
        /// a random value to display when pressing the TestSpawnOne button
        [Tooltip("a random value to display when pressing the TestSpawnOne button")]
        public Vector2Int DebugRandomValue = new(100, 500);

        /// the min and max bounds within which to pick a value to output when pressing the TestSpawnMany button
        [Tooltip(
            "the min and max bounds within which to pick a value to output when pressing the TestSpawnMany button")]
        [MMVector("Min", "Max")]
        public Vector2 DebugInterval = new(0.3f, 0.5f);

        /// a button used to test the spawn of one text
        [Tooltip("a button used to test the spawn of one text")] [MMInspectorButton("TestSpawnOne")]
        public bool TestSpawnOneBtn;

        /// a button used to start/stop the spawn of texts at regular intervals
        [Tooltip("a button used to start/stop the spawn of texts at regular intervals")]
        [MMInspectorButton("TestSpawnMany")]
        public bool TestSpawnManyBtn;

        protected bool _animateColor;
        protected Gradient _colorGradient;
        protected Vector3 _direction;
        protected MMFloatingText _floatingText;

        protected float _lifetime;

        protected MMObjectPooler _pooler;
        protected Vector3 _spawnOffset;
        protected float _speed;
        protected Coroutine _testSpawnCoroutine;

        /// <summary>
        ///     On enable we start listening for floating text events
        /// </summary>
        protected virtual void OnEnable()
        {
            MMFloatingTextSpawnEvent.Register(OnMMFloatingTextSpawnEvent);
        }

        /// <summary>
        ///     On disable we stop listening for floating text events
        /// </summary>
        protected virtual void OnDisable()
        {
            MMFloatingTextSpawnEvent.Unregister(OnMMFloatingTextSpawnEvent);
        }

        /// <summary>
        ///     Spawns a new floating text
        /// </summary>
        /// <param name="value"></param>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="intensity"></param>
        /// <param name="forceLifetime"></param>
        /// <param name="lifetime"></param>
        /// <param name="forceColor"></param>
        /// <param name="animateColorGradient"></param>
        protected virtual void Spawn(string value, Vector3 position, Vector3 direction, float intensity = 1f,
            bool forceLifetime = false, float lifetime = 1f, bool forceColor = false,
            Gradient animateColorGradient = null)
        {
            if (!CanSpawn) return;

            _direction = direction != Vector3.zero ? direction + transform.up : transform.up;

            transform.position = position;

            var nextGameObject = _pooler.GetPooledGameObject();

            var lifetimeMultiplier = IntensityImpactsLifetime ? intensity * IntensityLifetimeMultiplier : 1f;
            var movementMultiplier = IntensityImpactsMovement ? intensity * IntensityMovementMultiplier : 1f;
            var scaleMultiplier = IntensityImpactsScale ? intensity * IntensityScaleMultiplier : 1f;

            _lifetime = Random.Range(Lifetime.x, Lifetime.y) * lifetimeMultiplier;
            _spawnOffset = MMMaths.RandomVector3(SpawnOffsetMin, SpawnOffsetMax);
            _animateColor = AnimateColor;
            _colorGradient = AnimateColorGradient;

            var remapXZero = Random.Range(RemapXZero.x, RemapXZero.y);
            var remapXOne = Random.Range(RemapXOne.x, RemapXOne.y) * movementMultiplier;
            var remapYZero = Random.Range(RemapYZero.x, RemapYZero.y);
            var remapYOne = Random.Range(RemapYOne.x, RemapYOne.y) * movementMultiplier;
            var remapZZero = Random.Range(RemapZZero.x, RemapZZero.y);
            var remapZOne = Random.Range(RemapZOne.x, RemapZOne.y) * movementMultiplier;
            var remapOpacityZero = Random.Range(RemapOpacityZero.x, RemapOpacityZero.y);
            var remapOpacityOne = Random.Range(RemapOpacityOne.x, RemapOpacityOne.y);
            var remapScaleZero = Random.Range(RemapScaleZero.x, RemapOpacityZero.y);
            var remapScaleOne = Random.Range(RemapScaleOne.x, RemapScaleOne.y) * scaleMultiplier;

            if (forceLifetime) _lifetime = lifetime;

            if (forceColor)
            {
                _animateColor = true;
                _colorGradient = animateColorGradient;
            }

            // mandatory checks
            if (nextGameObject == null) return;

            // we activate the object
            nextGameObject.gameObject.SetActive(true);
            nextGameObject.gameObject.MMGetComponentNoAlloc<MMPoolableObject>().TriggerOnSpawnComplete();

            // we position the object
            nextGameObject.transform.position = transform.position + _spawnOffset;

            _floatingText = nextGameObject.MMGetComponentNoAlloc<MMFloatingText>();
            _floatingText.SetUseUnscaledTime(UseUnscaledTime, true);
            _floatingText.ResetPosition();
            _floatingText.SetProperties(value, _lifetime, _direction, AnimateMovement,
                AlignmentMode, FixedAlignment, AlwaysFaceCamera, TargetCamera,
                AnimateX, AnimateXCurve, remapXZero, remapXOne,
                AnimateY, AnimateYCurve, remapYZero, remapYOne,
                AnimateZ, AnimateZCurve, remapZZero, remapZOne,
                AnimateOpacity, AnimateOpacityCurve, remapOpacityZero, remapOpacityOne,
                AnimateScale, AnimateScaleCurve, remapScaleZero, remapScaleOne,
                _animateColor, _colorGradient);
        }

        /// <summary>
        ///     When we get a floating text event on this spawner's Channel, we spawn a new floating text
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="spawnPosition"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <param name="intensity"></param>
        /// <param name="forceLifetime"></param>
        /// <param name="lifetime"></param>
        /// <param name="forceColor"></param>
        /// <param name="animateColorGradient"></param>
        public virtual void OnMMFloatingTextSpawnEvent(int channel, Vector3 spawnPosition, string value,
            Vector3 direction, float intensity,
            bool forceLifetime = false, float lifetime = 1f, bool forceColor = false,
            Gradient animateColorGradient = null, bool useUnscaledTime = false)
        {
            if (channel != Channel) return;

            UseUnscaledTime = useUnscaledTime;
            Spawn(value, spawnPosition, direction, intensity, forceLifetime, lifetime, forceColor,
                animateColorGradient);
        }

        #region Initialization

        /// <summary>
        ///     On awake we initialize our spawner
        /// </summary>
        protected virtual void Start()
        {
            Initialization();
        }

        /// <summary>
        ///     On init, we instantiate our object pool and grab the main camera
        /// </summary>
        protected virtual void Initialization()
        {
            InstantiateObjectPool();
            GrabMainCamera();
        }

        /// <summary>
        ///     Instantiates the specified type of object pool
        /// </summary>
        protected virtual void InstantiateObjectPool()
        {
            if (_pooler == null)
            {
                if (PoolerMode == PoolerModes.Simple)
                    InstantiateSimplePool();
                else
                    InstantiateMultiplePool();
            }
        }

        /// <summary>
        ///     Instantiates a simple object pooler and sets it up
        /// </summary>
        protected virtual void InstantiateSimplePool()
        {
            if (PooledSimpleMMFloatingText == null)
            {
                Debug.LogError(name + " : no PooledSimpleMMFloatingText prefab has been set.");
                return;
            }

            var newPooler = new GameObject();
            SceneManager.MoveGameObjectToScene(newPooler, gameObject.scene);
            newPooler.name = PooledSimpleMMFloatingText.name + "_Pooler";
            newPooler.transform.SetParent(transform);
            var simplePooler = newPooler.AddComponent<MMSimpleObjectPooler>();
            simplePooler.PoolSize = PoolSize;
            simplePooler.GameObjectToPool = PooledSimpleMMFloatingText.gameObject;
            simplePooler.NestWaitingPool = NestWaitingPool;
            simplePooler.MutualizeWaitingPools = MutualizeWaitingPools;
            simplePooler.PoolCanExpand = PoolCanExpand;
            simplePooler.FillObjectPool();
            _pooler = simplePooler;
        }

        /// <summary>
        ///     Instantiates a multiple object pooler and sets it up
        /// </summary>
        protected virtual void InstantiateMultiplePool()
        {
            var newPooler = new GameObject();
            SceneManager.MoveGameObjectToScene(newPooler, gameObject.scene);
            newPooler.name = name + "_Pooler";
            newPooler.transform.SetParent(transform);
            var multiplePooler = newPooler.AddComponent<MMMultipleObjectPooler>();
            multiplePooler.Pool = new List<MMMultipleObjectPoolerObject>();
            foreach (var obj in PooledMultipleMMFloatingText)
            {
                var item = new MMMultipleObjectPoolerObject();
                item.GameObjectToPool = obj.gameObject;
                item.PoolCanExpand = PoolCanExpand;
                item.PoolSize = PoolSize;
                item.Enabled = true;
                multiplePooler.Pool.Add(item);
            }

            multiplePooler.NestWaitingPool = NestWaitingPool;
            multiplePooler.MutualizeWaitingPools = MutualizeWaitingPools;
            multiplePooler.FillObjectPool();
            _pooler = multiplePooler;
        }

        /// <summary>
        ///     Grabs the main camera if needed
        /// </summary>
        protected virtual void GrabMainCamera()
        {
            if (AutoGrabMainCameraOnStart) TargetCamera = Camera.main;
        }

        #endregion

        // Test methods ----------------------------------------------------------------------------------------

        #region TestMethods

        /// <summary>
        ///     A test method that spawns one floating text
        /// </summary>
        protected virtual void TestSpawnOne()
        {
            var test = Random.Range(DebugRandomValue.x, DebugRandomValue.y).ToString();
            Spawn(test, transform.position, Vector3.zero);
        }

        /// <summary>
        ///     A method used to start/stop the regular spawning of debug floating texts
        /// </summary>
        protected virtual void TestSpawnMany()
        {
            if (_testSpawnCoroutine == null)
            {
                _testSpawnCoroutine = StartCoroutine(TestSpawnManyCo());
            }
            else
            {
                StopCoroutine(_testSpawnCoroutine);
                _testSpawnCoroutine = null;
            }
        }

        /// <summary>
        ///     A coroutine used to spawn debug floating texts until stopped
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator TestSpawnManyCo()
        {
            var lastSpawnAt = Time.time;
            var interval = Random.Range(DebugInterval.x, DebugInterval.y);
            while (true)
            {
                if (Time.time - lastSpawnAt > interval)
                {
                    TestSpawnOne();
                    lastSpawnAt = Time.time;
                    interval = Random.Range(DebugInterval.x, DebugInterval.y);
                }

                yield return null;
            }
        }

        #endregion
    }
}