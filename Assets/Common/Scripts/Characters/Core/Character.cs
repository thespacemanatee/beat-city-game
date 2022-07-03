using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	///     This class will pilot the TopDownController component of your character.
	///     This is where you'll implement all of your character's game rules, like jump, dash, shoot, stuff like that.
	///     Animator parameters : Grounded (bool), xSpeed (float), ySpeed (float),
	///     CollidingLeft (bool), CollidingRight (bool), CollidingBelow (bool), CollidingAbove (bool), Idle (bool)
	///     Random : a random float between 0 and 1, updated every frame, useful to add variance to your state entry
	///     transitions for example
	///     RandomConstant : a random int (between 0 and 1000), generated at Start and that'll remain constant for the entire
	///     lifetime of this animator, useful to have different characters of the same type
	/// </summary>
	[SelectionBase]
    [AddComponentMenu("TopDown Engine/Character/Core/Character")]
    public class Character : MonoBehaviour
    {
        public enum CharacterDimensions
        {
            Type2D,
            Type3D
        }

        /// the possible character types : player controller or AI (controlled by the computer)
        public enum CharacterTypes
        {
            Player,
            AI
        }

        /// the possible initial facing direction for your character
        public enum FacingDirections
        {
            West,
            East,
            North,
            South
        }

        protected const string _groundedAnimationParameterName = "Grounded";
        protected const string _aliveAnimationParameterName = "Alive";
        protected const string _currentSpeedAnimationParameterName = "CurrentSpeed";
        protected const string _xSpeedAnimationParameterName = "xSpeed";
        protected const string _ySpeedAnimationParameterName = "ySpeed";
        protected const string _zSpeedAnimationParameterName = "zSpeed";
        protected const string _xVelocityAnimationParameterName = "xVelocity";
        protected const string _yVelocityAnimationParameterName = "yVelocity";
        protected const string _zVelocityAnimationParameterName = "zVelocity";
        protected const string _idleAnimationParameterName = "Idle";
        protected const string _randomAnimationParameterName = "Random";
        protected const string _randomConstantAnimationParameterName = "RandomConstant";

        [MMReadOnly] public CharacterDimensions CharacterDimension;

        [MMInformation(
            "The Character script is the mandatory basis for all Character abilities. Your character can either be a Non Player Character, controlled by an AI, or a Player character, controlled by the player. In this case, you'll need to specify a PlayerID, which must match the one specified in your InputManager. Usually 'Player1', 'Player2', etc.",
            MMInformationAttribute.InformationType.Info, false)]
        /// Is the character player-controlled or controlled by an AI ?
        [Tooltip("Is the character player-controlled or controlled by an AI ?")]
        public CharacterTypes CharacterType = CharacterTypes.AI;

        /// Only used if the character is player-controlled. The PlayerID must match an input manager's PlayerID. It's also used to match Unity's input settings. So you'll be safe if you keep to Player1, Player2, Player3 or Player4
        [Tooltip(
            "Only used if the character is player-controlled. The PlayerID must match an input manager's PlayerID. It's also used to match Unity's input settings. So you'll be safe if you keep to Player1, Player2, Player3 or Player4")]
        public string PlayerID = "";

        [Header("Animator")]
        [MMInformation(
            "The engine will try and find an animator for this character. If it's on the same gameobject it should have found it. If it's nested somewhere, you'll need to bind it below. You can also decide to get rid of it altogether, in that case, just uncheck 'use mecanim'.",
            MMInformationAttribute.InformationType.Info, false)]
        /// the character animator
        [Tooltip("the character animator, that this class and all abilities should update parameters on")]
        public Animator CharacterAnimator;

        /// Set this to false if you want to implement your own animation system
        [Tooltip("Set this to false if you want to implement your own animation system")]
        public bool UseDefaultMecanim = true;

        /// If this is true, sanity checks will be performed to make sure animator parameters exist before updating them. Turning this to false will increase performance but will throw errors if you're trying to update non existing parameters. Make sure your animator has the required parameters.
        [Tooltip(
            "If this is true, sanity checks will be performed to make sure animator parameters exist before updating them. Turning this to false will increase performance but will throw errors if you're trying to update non existing parameters. Make sure your animator has the required parameters.")]
        public bool RunAnimatorSanityChecks;

        /// if this is true, animator logs for the associated animator will be turned off to avoid potential spam
        [Tooltip(
            "if this is true, animator logs for the associated animator will be turned off to avoid potential spam")]
        public bool DisableAnimatorLogs = true;

        [Header("Bindings")]
        [MMInformation(
            "Leave this unbound if this is a regular, sprite-based character, and if the SpriteRenderer and the Character are on the same GameObject. If not, you'll want to parent the actual model to the Character object, and bind it below. See the 3D demo characters for an example of that. The idea behind that is that the model may move, flip, but the collider will remain unchanged.",
            MMInformationAttribute.InformationType.Info, false)]
        /// the 'model' (can be any gameobject) used to manipulate the character. Ideally it's separated (and nested) from the collider/TopDown controller/abilities, to avoid messing with collisions.
        [Tooltip(
            "the 'model' (can be any gameobject) used to manipulate the character. Ideally it's separated (and nested) from the collider/TopDown controller/abilities, to avoid messing with collisions.")]
        public GameObject CharacterModel;

        /// the Health script associated to this Character, will be grabbed automatically if left empty
        [Tooltip("the Health script associated to this Character, will be grabbed automatically if left empty")]
        public Health CharacterHealth;

        [Header("Events")]
        [MMInformation(
            "Here you can define whether or not you want to have that character trigger events when changing state. See the MMTools' State Machine doc for more info.",
            MMInformationAttribute.InformationType.Info, false)]
        /// If this is true, the Character's state machine will emit events when entering/exiting a state
        [Tooltip("If this is true, the Character's state machine will emit events when entering/exiting a state")]
        public bool SendStateChangeEvents = true;

        [Header("Abilities")]
        /// A list of gameobjects (usually nested under the Character) under which to search for additional abilities
        [Tooltip(
            "A list of gameobjects (usually nested under the Character) under which to search for additional abilities")]
        public List<GameObject> AdditionalAbilityNodes;

        [Header("AI")]
        /// The brain currently associated with this character, if it's an Advanced AI. By default the engine will pick the one on this object, but you can attach another one if you'd like
        [Tooltip(
            "The brain currently associated with this character, if it's an Advanced AI. By default the engine will pick the one on this object, but you can attach another one if you'd like")]
        public AIBrain CharacterBrain;

        /// Whether to optimize this character for mobile. Will disable its cone of vision on mobile
        [Tooltip("Whether to optimize this character for mobile. Will disable its cone of vision on mobile")]
        public bool OptimizeForMobile = true;

        protected bool _abilitiesCachedOnce;
        protected int _aliveAnimationParameter;
        protected bool _animatorInitialized;
        protected float _animatorRandomNumber;

        protected CharacterAbility[] _characterAbilities;
        protected CharacterPersistence _characterPersistence;
        protected TopDownController _controller;
        protected int _currentSpeedAnimationParameter;
        protected int _groundedAnimationParameter;
        protected int _idleAnimationParameter;
        protected bool _onReviveRegistered;
        protected int _randomAnimationParameter;
        protected int _randomConstantAnimationParameter;
        protected bool _spawnDirectionForced;
        protected int _xSpeedAnimationParameter;
        protected int _xVelocityAnimationParameter;
        protected int _ySpeedAnimationParameter;
        protected int _yVelocityAnimationParameter;
        protected int _zSpeedAnimationParameter;
        protected int _zVelocityAnimationParameter;
        public MMStateMachine<CharacterStates.CharacterConditions> ConditionState;

        /// State Machines
        public MMStateMachine<CharacterStates.MovementStates> MovementState;

        /// the various states of the character
        public CharacterStates CharacterState { get; protected set; }

        /// associated camera and input manager
        public InputManager LinkedInputManager { get; protected set; }

        /// the animator associated to this character
        public Animator _animator { get; protected set; }

        /// a list of animator parameters
        public HashSet<int> _animatorParameters { get; set; }

        /// this character's orientation 2D ability
        public CharacterOrientation2D Orientation2D { get; protected set; }

        /// this character's orientation 3D ability
        public CharacterOrientation3D Orientation3D { get; protected set; }

        /// an object to use as the camera's point of focus and follow target
        public GameObject CameraTarget { get; set; }

        /// the direction of the camera associated to this character
        public Vector3 CameraDirection { get; protected set; }


        /// <summary>
        ///     Initializes this instance of the character
        /// </summary>
        protected virtual void Awake()
        {
            Initialization();
        }

        /// <summary>
        ///     Called when the Character dies.
        ///     Calls every abilities' Reset() method, so you can restore settings to their original value if needed
        /// </summary>
        public virtual void Reset()
        {
            _spawnDirectionForced = false;
            if (_characterAbilities == null) return;
            if (_characterAbilities.Length == 0) return;
            foreach (var ability in _characterAbilities)
                if (ability.enabled)
                    ability.ResetAbility();
        }

        /// <summary>
        ///     This is called every frame.
        /// </summary>
        protected virtual void Update()
        {
            EveryFrame();
        }

        /// <summary>
        ///     OnEnable, we register our OnRevive event
        /// </summary>
        protected virtual void OnEnable()
        {
            if (CharacterHealth != null)
            {
                if (!_onReviveRegistered)
                {
                    CharacterHealth.OnRevive += OnRevive;
                    _onReviveRegistered = true;
                }

                CharacterHealth.OnDeath += OnDeath;
                CharacterHealth.OnHit += OnHit;
            }
        }

        /// <summary>
        ///     OnDisable, we unregister our OnRevive event
        /// </summary>
        protected virtual void OnDisable()
        {
            if (CharacterHealth != null)
            {
                CharacterHealth.OnDeath -= OnDeath;
                CharacterHealth.OnHit -= OnHit;
            }
        }

        /// <summary>
        ///     Gets and stores input manager, camera and components
        /// </summary>
        protected virtual void Initialization()
        {
            if (gameObject.MMGetComponentNoAlloc<TopDownController2D>() != null)
                CharacterDimension = CharacterDimensions.Type2D;
            if (gameObject.MMGetComponentNoAlloc<TopDownController3D>() != null)
                CharacterDimension = CharacterDimensions.Type3D;

            // we initialize our state machines
            MovementState = new MMStateMachine<CharacterStates.MovementStates>(gameObject, SendStateChangeEvents);
            ConditionState = new MMStateMachine<CharacterStates.CharacterConditions>(gameObject, SendStateChangeEvents);

            // we get the current input manager
            SetInputManager();
            // we store our components for further use 
            CharacterState = new CharacterStates();
            _controller = gameObject.GetComponent<TopDownController>();
            if (CharacterHealth == null) CharacterHealth = gameObject.GetComponent<Health>();

            CacheAbilitiesAtInit();
            if (CharacterBrain == null) CharacterBrain = gameObject.GetComponent<AIBrain>();

            if (CharacterBrain != null) CharacterBrain.Owner = gameObject;

            Orientation2D = FindAbility<CharacterOrientation2D>();
            Orientation3D = FindAbility<CharacterOrientation3D>();
            _characterPersistence = FindAbility<CharacterPersistence>();

            AssignAnimator();

            // instantiate camera target
            if (CameraTarget == null) CameraTarget = new GameObject();
            CameraTarget.transform.SetParent(transform);
            CameraTarget.transform.localPosition = Vector3.zero;
            CameraTarget.name = "CameraTarget";

            if (LinkedInputManager != null)
                if (OptimizeForMobile && LinkedInputManager.IsMobile)
                    if (gameObject.MMGetComponentNoAlloc<MMConeOfVision2D>() != null)
                        gameObject.MMGetComponentNoAlloc<MMConeOfVision2D>().enabled = false;
        }

        protected virtual void CacheAbilitiesAtInit()
        {
            if (_abilitiesCachedOnce) return;
            CacheAbilities();
        }

        /// <summary>
        ///     Grabs abilities and caches them for further use
        ///     Make sure you call this if you add abilities at runtime
        ///     Ideally you'll want to avoid adding components at runtime, it's costly,
        ///     and it's best to activate/disable components instead.
        ///     But if you need to, call this method.
        /// </summary>
        public virtual void CacheAbilities()
        {
            // we grab all abilities at our level
            _characterAbilities = gameObject.GetComponents<CharacterAbility>();

            // if the user has specified more nodes
            if (AdditionalAbilityNodes != null && AdditionalAbilityNodes.Count > 0)
            {
                // we create a temp list
                var tempAbilityList = new List<CharacterAbility>();

                // we put all the abilities we've already found on the list
                for (var i = 0; i < _characterAbilities.Length; i++) tempAbilityList.Add(_characterAbilities[i]);

                // we add the ones from the nodes
                for (var j = 0; j < AdditionalAbilityNodes.Count; j++)
                {
                    var tempArray = AdditionalAbilityNodes[j].GetComponentsInChildren<CharacterAbility>();
                    foreach (var ability in tempArray) tempAbilityList.Add(ability);
                }

                _characterAbilities = tempAbilityList.ToArray();
            }

            _abilitiesCachedOnce = true;
        }

        /// <summary>
        ///     A method to check whether a Character has a certain ability or not
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T FindAbility<T>() where T : CharacterAbility
        {
            CacheAbilitiesAtInit();

            var searchedAbilityType = typeof(T);

            foreach (var ability in _characterAbilities)
                if (ability is T characterAbility)
                    return characterAbility;

            return null;
        }

        /// <summary>
        ///     Binds an animator to this character
        /// </summary>
        public virtual void AssignAnimator(bool forceAssignation = false)
        {
            if (_animatorInitialized && !forceAssignation) return;

            _animatorParameters = new HashSet<int>();

            if (CharacterAnimator != null)
                _animator = CharacterAnimator;
            else
                _animator = gameObject.GetComponent<Animator>();

            if (_animator != null)
            {
                if (DisableAnimatorLogs) _animator.logWarnings = false;
                InitializeAnimatorParameters();
            }

            _animatorInitialized = true;
        }

        /// <summary>
        ///     Initializes the animator parameters.
        /// </summary>
        protected virtual void InitializeAnimatorParameters()
        {
            if (_animator == null) return;
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _groundedAnimationParameterName,
                out _groundedAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _currentSpeedAnimationParameterName,
                out _currentSpeedAnimationParameter, AnimatorControllerParameterType.Float, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _xSpeedAnimationParameterName,
                out _xSpeedAnimationParameter, AnimatorControllerParameterType.Float, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _ySpeedAnimationParameterName,
                out _ySpeedAnimationParameter, AnimatorControllerParameterType.Float, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _zSpeedAnimationParameterName,
                out _zSpeedAnimationParameter, AnimatorControllerParameterType.Float, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _idleAnimationParameterName,
                out _idleAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _aliveAnimationParameterName,
                out _aliveAnimationParameter, AnimatorControllerParameterType.Bool, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _randomAnimationParameterName,
                out _randomAnimationParameter, AnimatorControllerParameterType.Float, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _randomConstantAnimationParameterName,
                out _randomConstantAnimationParameter, AnimatorControllerParameterType.Float, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _xVelocityAnimationParameterName,
                out _xVelocityAnimationParameter, AnimatorControllerParameterType.Float, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _yVelocityAnimationParameterName,
                out _yVelocityAnimationParameter, AnimatorControllerParameterType.Float, _animatorParameters);
            MMAnimatorExtensions.AddAnimatorParameterIfExists(_animator, _zVelocityAnimationParameterName,
                out _zVelocityAnimationParameter, AnimatorControllerParameterType.Float, _animatorParameters);

            // we update our constant float animation parameter
            var randomConstant = Random.Range(0, 1000);
            MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _randomConstantAnimationParameter, randomConstant,
                _animatorParameters, RunAnimatorSanityChecks);
        }

        /// <summary>
        ///     Gets (if it exists) the InputManager matching the Character's Player ID
        /// </summary>
        public virtual void SetInputManager()
        {
            if (CharacterType == CharacterTypes.AI)
            {
                LinkedInputManager = null;
                UpdateInputManagersInAbilities();
                return;
            }

            // we get the corresponding input manager
            if (!string.IsNullOrEmpty(PlayerID))
            {
                LinkedInputManager = null;
                var foundInputManagers = FindObjectsOfType(typeof(InputManager)) as InputManager[];
                foreach (var foundInputManager in foundInputManagers)
                    if (foundInputManager.PlayerID == PlayerID)
                        LinkedInputManager = foundInputManager;
            }

            UpdateInputManagersInAbilities();
        }

        /// <summary>
        ///     Sets a new input manager for this Character and all its abilities
        /// </summary>
        /// <param name="inputManager"></param>
        public virtual void SetInputManager(InputManager inputManager)
        {
            LinkedInputManager = inputManager;
            UpdateInputManagersInAbilities();
        }

        /// <summary>
        ///     Updates the linked input manager for all abilities
        /// </summary>
        protected virtual void UpdateInputManagersInAbilities()
        {
            if (_characterAbilities == null) return;
            for (var i = 0; i < _characterAbilities.Length; i++)
                _characterAbilities[i].SetInputManager(LinkedInputManager);
        }

        /// <summary>
        ///     Resets the input for all abilities
        /// </summary>
        public virtual void ResetInput()
        {
            if (_characterAbilities == null) return;
            foreach (var ability in _characterAbilities) ability.ResetInput();
        }

        /// <summary>
        ///     Sets the player ID
        /// </summary>
        /// <param name="newPlayerID">New player ID.</param>
        public virtual void SetPlayerID(string newPlayerID)
        {
            PlayerID = newPlayerID;
            SetInputManager();
        }

        /// <summary>
        ///     We do this every frame. This is separate from Update for more flexibility.
        /// </summary>
        protected virtual void EveryFrame()
        {
            // we process our abilities
            EarlyProcessAbilities();
            ProcessAbilities();
            LateProcessAbilities();

            // we send our various states to the animator.		 
            UpdateAnimators();
        }

        /// <summary>
        ///     Calls all registered abilities' Early Process methods
        /// </summary>
        protected virtual void EarlyProcessAbilities()
        {
            foreach (var ability in _characterAbilities)
                if (ability.enabled && ability.AbilityInitialized)
                    ability.EarlyProcessAbility();
        }

        /// <summary>
        ///     Calls all registered abilities' Process methods
        /// </summary>
        protected virtual void ProcessAbilities()
        {
            foreach (var ability in _characterAbilities)
                if (ability.enabled && ability.AbilityInitialized)
                    ability.ProcessAbility();
        }

        /// <summary>
        ///     Calls all registered abilities' Late Process methods
        /// </summary>
        protected virtual void LateProcessAbilities()
        {
            foreach (var ability in _characterAbilities)
                if (ability.enabled && ability.AbilityInitialized)
                    ability.LateProcessAbility();
        }


        /// <summary>
        ///     This is called at Update() and sets each of the animators parameters to their corresponding State values
        /// </summary>
        protected virtual void UpdateAnimators()
        {
            UpdateAnimationRandomNumber();

            if (UseDefaultMecanim && _animator != null)
            {
                MMAnimatorExtensions.UpdateAnimatorBool(_animator, _groundedAnimationParameter, _controller.Grounded,
                    _animatorParameters, RunAnimatorSanityChecks);
                MMAnimatorExtensions.UpdateAnimatorBool(_animator, _aliveAnimationParameter,
                    ConditionState.CurrentState != CharacterStates.CharacterConditions.Dead, _animatorParameters,
                    RunAnimatorSanityChecks);
                MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _currentSpeedAnimationParameter,
                    _controller.CurrentMovement.magnitude, _animatorParameters, RunAnimatorSanityChecks);
                MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _xSpeedAnimationParameter,
                    _controller.CurrentMovement.x, _animatorParameters, RunAnimatorSanityChecks);
                MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _ySpeedAnimationParameter,
                    _controller.CurrentMovement.y, _animatorParameters, RunAnimatorSanityChecks);
                MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _zSpeedAnimationParameter,
                    _controller.CurrentMovement.z, _animatorParameters, RunAnimatorSanityChecks);
                MMAnimatorExtensions.UpdateAnimatorBool(_animator, _idleAnimationParameter,
                    MovementState.CurrentState == CharacterStates.MovementStates.Idle, _animatorParameters,
                    RunAnimatorSanityChecks);
                MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _randomAnimationParameter, _animatorRandomNumber,
                    _animatorParameters, RunAnimatorSanityChecks);
                MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _xVelocityAnimationParameter,
                    _controller.Velocity.x, _animatorParameters, RunAnimatorSanityChecks);
                MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _yVelocityAnimationParameter,
                    _controller.Velocity.y, _animatorParameters, RunAnimatorSanityChecks);
                MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _zVelocityAnimationParameter,
                    _controller.Velocity.z, _animatorParameters, RunAnimatorSanityChecks);


                foreach (var ability in _characterAbilities)
                    if (ability.enabled && ability.AbilityInitialized)
                        ability.UpdateAnimator();
            }
        }

        /// <summary>
        ///     Makes the player respawn at the location passed in parameters
        /// </summary>
        /// <param name="spawnPoint">The location of the respawn.</param>
        public virtual void RespawnAt(Transform spawnPoint, FacingDirections facingDirection)
        {
            transform.position = spawnPoint.position;

            if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
            //Debug.LogError("Spawn : your Character's gameobject is inactive");
            // we raise it from the dead (if it was dead)
            ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
            // we re-enable its 2D collider
            if (gameObject.MMGetComponentNoAlloc<Collider2D>() != null)
                gameObject.MMGetComponentNoAlloc<Collider2D>().enabled = true;
            // we re-enable its 2D collider
            if (gameObject.MMGetComponentNoAlloc<Collider>() != null)
                gameObject.MMGetComponentNoAlloc<Collider>().enabled = true;

            // we make it handle collisions again
            _controller.enabled = true;
            _controller.CollisionsOn();
            _controller.Reset();

            // we kill all potential velocity
            if (gameObject.MMGetComponentNoAlloc<Rigidbody>() != null)
                gameObject.MMGetComponentNoAlloc<Rigidbody>().velocity = Vector3.zero;
            if (gameObject.MMGetComponentNoAlloc<Rigidbody2D>() != null)
                gameObject.MMGetComponentNoAlloc<Rigidbody2D>().velocity = Vector3.zero;

            Reset();
            UnFreeze();

            if (CharacterHealth != null)
            {
                if (_characterPersistence != null)
                    if (_characterPersistence.Initialized)
                    {
                        if (CharacterHealth != null) CharacterHealth.UpdateHealthBar(false);
                        return;
                    }

                CharacterHealth.ResetHealthToMaxHealth();
                CharacterHealth.Revive();
            }

            if (CharacterBrain != null) CharacterBrain.enabled = true;

            // facing direction
            if (FindAbility<CharacterOrientation2D>() != null)
                FindAbility<CharacterOrientation2D>().Face(facingDirection);
            // facing direction
            if (FindAbility<CharacterOrientation3D>() != null)
                FindAbility<CharacterOrientation3D>().Face(facingDirection);
        }

        /// <summary>
        ///     Calls flip on all abilities
        /// </summary>
        public virtual void FlipAllAbilities()
        {
            foreach (var ability in _characterAbilities)
                if (ability.enabled)
                    ability.Flip();
        }

        /// <summary>
        ///     Generates a random number to send to the animator
        /// </summary>
        protected virtual void UpdateAnimationRandomNumber()
        {
            _animatorRandomNumber = Random.Range(0f, 1f);
        }

        /// <summary>
        ///     Stores the associated camera direction
        /// </summary>
        public virtual void SetCameraDirection(Vector3 direction)
        {
            CameraDirection = direction;
        }

        /// <summary>
        ///     Freezes this character.
        /// </summary>
        public virtual void Freeze()
        {
            _controller.SetGravityActive(false);
            _controller.SetMovement(Vector2.zero);
            ConditionState.ChangeState(CharacterStates.CharacterConditions.Frozen);
        }

        /// <summary>
        ///     Unfreezes this character
        /// </summary>
        public virtual void UnFreeze()
        {
            _controller.SetGravityActive(true);
            ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
        }

        /// <summary>
        ///     Called to disable the player (at the end of a level for example.
        ///     It won't move and respond to input after this.
        /// </summary>
        public virtual void Disable()
        {
            enabled = false;
            _controller.enabled = false;
        }

        /// <summary>
        ///     On revive, we force the spawn direction
        /// </summary>
        protected virtual void OnRevive()
        {
            if (CharacterBrain != null)
            {
                CharacterBrain.enabled = true;
                CharacterBrain.ResetBrain();
            }
        }

        protected virtual void OnDeath()
        {
            if (CharacterBrain != null)
            {
                CharacterBrain.TransitionToState("");
                CharacterBrain.enabled = false;
            }

            if (MovementState.CurrentState != CharacterStates.MovementStates.FallingDownHole)
                MovementState.ChangeState(CharacterStates.MovementStates.Idle);
        }

        protected virtual void OnHit()
        {
        }
    }
}