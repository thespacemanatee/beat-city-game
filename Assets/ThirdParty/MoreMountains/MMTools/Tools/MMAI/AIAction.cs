using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     Actions are behaviours and describe what your character is doing. Examples include patrolling, shooting, jumping,
    ///     etc.
    /// </summary>
    public abstract class AIAction : MonoBehaviour
    {
        public string Label;
        protected AIBrain _brain;
        public bool ActionInProgress { get; set; }

        /// <summary>
        ///     On Awake we grab our AIBrain
        /// </summary>
        protected virtual void Awake()
        {
            _brain = gameObject.GetComponentInParent<AIBrain>();
        }

        public abstract void PerformAction();

        /// <summary>
        ///     Initializes the action. Meant to be overridden
        /// </summary>
        public virtual void Initialization()
        {
        }

        /// <summary>
        ///     Describes what happens when the brain enters the state this action is in. Meant to be overridden.
        /// </summary>
        public virtual void OnEnterState()
        {
            ActionInProgress = true;
        }

        /// <summary>
        ///     Describes what happens when the brain exits the state this action is in. Meant to be overridden.
        /// </summary>
        public virtual void OnExitState()
        {
            ActionInProgress = false;
        }
    }
}