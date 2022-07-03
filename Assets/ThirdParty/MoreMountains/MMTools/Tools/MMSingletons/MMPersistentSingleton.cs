using UnityEngine;

namespace MoreMountains.Tools
{
	/// <summary>
	///     Persistent singleton.
	/// </summary>
	public class MMPersistentSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T _instance;

        [Header("Persistent Singleton")]
        /// if this is true, this singleton will auto detach if it finds itself parented on awake
        [Tooltip("if this is true, this singleton will auto detach if it finds itself parented on awake")]
        public bool AutomaticallyUnparentOnAwake = true;

        protected bool _enabled;

        public static bool HasInstance => _instance != null;
        public static T Current => _instance;

        /// <summary>
        ///     Singleton design pattern
        /// </summary>
        /// <value>The instance.</value>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        var obj = new GameObject();
                        obj.name = typeof(T).Name + "_AutoCreated";
                        _instance = obj.AddComponent<T>();
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        ///     On awake, we check if there's already a copy of the object in the scene. If there's one, we destroy it.
        /// </summary>
        protected virtual void Awake()
        {
            if (!Application.isPlaying) return;

            if (AutomaticallyUnparentOnAwake) transform.SetParent(null);

            if (_instance == null)
            {
                //If I am the first instance, make me the Singleton
                _instance = this as T;
                DontDestroyOnLoad(transform.gameObject);
                _enabled = true;
            }
            else
            {
                //If a Singleton already exists and you find
                //another reference in scene, destroy it!
                if (this != _instance) Destroy(gameObject);
            }
        }
    }
}