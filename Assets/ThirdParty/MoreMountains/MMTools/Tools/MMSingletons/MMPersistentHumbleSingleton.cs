using UnityEngine;

namespace MoreMountains.Tools
{
	/// <summary>
	///     Persistent humble singleton, basically a classic singleton but will destroy any other older components of the same
	///     type it finds on awake
	/// </summary>
	public class MMPersistentHumbleSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T _instance;

        /// the timestamp at which this singleton got initialized
        [MMReadOnly] public float InitializationTime;

        /// whether or not this singleton already has an instance
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
                        obj.hideFlags = HideFlags.HideAndDontSave;
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

            InitializationTime = Time.time;

            DontDestroyOnLoad(gameObject);
            // we check for existing objects of the same type
            var check = FindObjectsOfType<T>();
            foreach (var searched in check)
                if (searched != this)
                    // if we find another object of the same type (not this), and if it's older than our current object, we destroy it.
                    if (searched.GetComponent<MMPersistentHumbleSingleton<T>>().InitializationTime < InitializationTime)
                        Destroy(searched.gameObject);

            if (_instance == null) _instance = this as T;
        }
    }
}