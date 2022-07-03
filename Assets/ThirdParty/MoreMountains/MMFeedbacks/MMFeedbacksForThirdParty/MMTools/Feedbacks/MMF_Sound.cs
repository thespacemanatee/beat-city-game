using System.Threading.Tasks;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace MoreMountains.Feedbacks
{
    [ExecuteAlways]
    [AddComponentMenu("")]
    [FeedbackPath("Audio/Sound")]
    [FeedbackHelp(
        "This feedback lets you play the specified AudioClip, either via event (you'll need something in your scene to catch a MMSfxEvent, for example a MMSoundManager), or cached (AudioSource gets created on init, and is then ready to be played), or on demand (instantiated on Play). For all these methods you can define a random volume between min/max boundaries (just set the same value in both fields if you don't want randomness), random pitch, and an optional AudioMixerGroup.")]
    public class MMF_Sound : MMF_Feedback
    {
        /// <summary>
        ///     The possible methods to play the sound with.
        ///     Event : sends a MMSfxEvent, you'll need a class to catch this event and play the sound
        ///     Cached : creates and stores an audiosource to play the sound with, parented to the owner
        ///     OnDemand : creates an audiosource and destroys it everytime you want to play the sound
        /// </summary>
        public enum PlayMethods
        {
            Event,
            Cached,
            OnDemand,
            Pool
        }

        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;

        protected AudioSource _cachedAudioSource;
        protected float _duration;
        protected AudioSource _editorAudioSource;
        protected AudioSource[] _pool;

        protected AudioClip _randomClip;
        protected AudioSource _tempAudioSource;

        /// the maximum pitch to play the sound at
        [Tooltip("the maximum pitch to play the sound at")]
        public float MaxPitch = 1f;

        /// the maximum volume to play the sound at
        [Tooltip("the maximum volume to play the sound at")]
        public float MaxVolume = 1f;

        [Header("Pitch")]
        /// the minimum pitch to play the sound at
        [Tooltip("the minimum pitch to play the sound at")]
        public float MinPitch = 1f;

        [MMFInspectorGroup("Sound Properties", true, 28)]
        [Header("Volume")]
        /// the minimum volume to play the sound at
        [Tooltip("the minimum volume to play the sound at")]
        public float MinVolume = 1f;

        [MMFInspectorGroup("Play Method", true, 27)]
        /// the play method to use when playing the sound (event, cached or on demand)
        [Tooltip("the play method to use when playing the sound (event, cached or on demand)")]
        public PlayMethods PlayMethod = PlayMethods.Event;

        /// the size of the pool when in Pool mode
        [Tooltip("the size of the pool when in Pool mode")] [MMFEnumCondition("PlayMethod", (int)PlayMethods.Pool)]
        public int PoolSize = 10;

        /// an array to pick a random sfx from
        [Tooltip("an array to pick a random sfx from")]
        public AudioClip[] RandomSfx;

        [MMFInspectorGroup("Sound", true, 14, true)]
        /// the sound clip to play
        [Tooltip("the sound clip to play")]
        public AudioClip Sfx;

        [Header("Mixer")]
        /// the audiomixer to play the sound with (optional)
        [Tooltip("the audiomixer to play the sound with (optional)")]
        public AudioMixerGroup SfxAudioMixerGroup;

        /// a test button used to play the sound in inspector
        public MMF_Button TestPlayButton;

        /// a test button used to stop the sound in inspector
        public MMF_Button TestStopButton;

        /// the duration of this feedback is the duration of the clip being played
        public override float FeedbackDuration => GetDuration();

        public override void InitializeCustomAttributes()
        {
            TestPlayButton = new MMF_Button("Debug Play Sound", TestPlaySound);
            TestStopButton = new MMF_Button("Debug Stop Sound", TestStopSound);
        }

        /// <summary>
        ///     Custom init to cache the audiosource if required
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(MMF_Player owner)
        {
            base.CustomInitialization(owner);
            if (PlayMethod == PlayMethods.Cached)
                _cachedAudioSource = CreateAudioSource(owner.gameObject, "CachedFeedbackAudioSource");
            if (PlayMethod == PlayMethods.Pool)
            {
                // create a pool
                _pool = new AudioSource[PoolSize];
                for (var i = 0; i < PoolSize; i++)
                    _pool[i] = CreateAudioSource(owner.gameObject, "PooledAudioSource" + i);
            }
        }

        protected virtual AudioSource CreateAudioSource(GameObject owner, string audioSourceName)
        {
            // we create a temporary game object to host our audio source
            var temporaryAudioHost = new GameObject(audioSourceName);
            SceneManager.MoveGameObjectToScene(temporaryAudioHost.gameObject, Owner.gameObject.scene);
            // we set the temp audio's position
            temporaryAudioHost.transform.position = owner.transform.position;
            temporaryAudioHost.transform.SetParent(owner.transform);
            // we add an audio source to that host
            _tempAudioSource = temporaryAudioHost.AddComponent<AudioSource>();
            _tempAudioSource.playOnAwake = false;
            return _tempAudioSource;
        }

        /// <summary>
        ///     Plays either a random sound or the specified sfx
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized) return;

            var intensityMultiplier = Timing.ConstantIntensity ? 1f : feedbacksIntensity;

            if (Sfx != null)
            {
                _duration = Sfx.length;
                PlaySound(Sfx, position, intensityMultiplier);
                return;
            }

            if (RandomSfx.Length > 0)
            {
                _randomClip = RandomSfx[Random.Range(0, RandomSfx.Length)];

                if (_randomClip != null)
                {
                    _duration = _randomClip.length;
                    PlaySound(_randomClip, position, intensityMultiplier);
                }
            }
        }

        protected virtual float GetDuration()
        {
            if (Sfx != null) return Sfx.length;

            var longest = 0f;
            if (RandomSfx != null && RandomSfx.Length > 0)
            {
                foreach (var clip in RandomSfx)
                    if (clip != null && clip.length > longest)
                        longest = clip.length;

                return longest;
            }

            return 0f;
        }

        /// <summary>
        ///     Plays a sound differently based on the selected play method
        /// </summary>
        /// <param name="sfx"></param>
        /// <param name="position"></param>
        protected virtual void PlaySound(AudioClip sfx, Vector3 position, float intensity)
        {
            var volume = Random.Range(MinVolume, MaxVolume);

            if (!Timing.ConstantIntensity) volume = volume * intensity;

            var pitch = Random.Range(MinPitch, MaxPitch);

            var timeSamples = NormalPlayDirection ? 0 : sfx.samples - 1;

            if (!NormalPlayDirection) pitch = -pitch;

            if (PlayMethod == PlayMethods.Event)
            {
                MMSfxEvent.Trigger(sfx, SfxAudioMixerGroup, volume, pitch);
                return;
            }

            if (PlayMethod == PlayMethods.OnDemand)
            {
                // we create a temporary game object to host our audio source
                var temporaryAudioHost = new GameObject("TempAudio");
                SceneManager.MoveGameObjectToScene(temporaryAudioHost.gameObject, Owner.gameObject.scene);
                // we set the temp audio's position
                temporaryAudioHost.transform.position = position;
                // we add an audio source to that host
                var audioSource = temporaryAudioHost.AddComponent<AudioSource>();
                PlayAudioSource(audioSource, sfx, volume, pitch, timeSamples, SfxAudioMixerGroup);
                // we destroy the host after the clip has played
                Owner.ProxyDestroy(temporaryAudioHost, sfx.length);
            }

            if (PlayMethod == PlayMethods.Cached)
                // we set that audio source clip to the one in paramaters
                PlayAudioSource(_cachedAudioSource, sfx, volume, pitch, timeSamples, SfxAudioMixerGroup);

            if (PlayMethod == PlayMethods.Pool)
            {
                _tempAudioSource = GetAudioSourceFromPool();
                if (_tempAudioSource != null)
                    PlayAudioSource(_tempAudioSource, sfx, volume, pitch, timeSamples, SfxAudioMixerGroup);
            }
        }

        /// <summary>
        ///     Plays the audio source with the specified volume and pitch
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="sfx"></param>
        /// <param name="volume"></param>
        /// <param name="pitch"></param>
        protected virtual void PlayAudioSource(AudioSource audioSource, AudioClip sfx, float volume, float pitch,
            int timeSamples, AudioMixerGroup audioMixerGroup = null)
        {
            // we set that audio source clip to the one in paramaters
            audioSource.clip = sfx;
            audioSource.timeSamples = timeSamples;
            // we set the audio source volume to the one in parameters
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            // we set our loop setting
            audioSource.loop = false;
            if (audioMixerGroup != null) audioSource.outputAudioMixerGroup = audioMixerGroup;
            // we start playing the sound
            audioSource.Play();
        }

        /// <summary>
        ///     Gets an audio source from the pool if possible
        /// </summary>
        /// <returns></returns>
        protected virtual AudioSource GetAudioSourceFromPool()
        {
            for (var i = 0; i < PoolSize; i++)
                if (!_pool[i].isPlaying)
                    return _pool[i];
            return null;
        }

        /// <summary>
        ///     A test method that creates an audiosource, plays it, and destroys itself after play
        /// </summary>
        protected virtual async void TestPlaySound()
        {
            AudioClip tmpAudioClip = null;

            if (Sfx != null) tmpAudioClip = Sfx;

            if (RandomSfx.Length > 0) tmpAudioClip = RandomSfx[Random.Range(0, RandomSfx.Length)];

            if (tmpAudioClip == null)
            {
                Debug.LogError(Label + " on " + Owner.gameObject.name +
                               " can't play in editor mode, you haven't set its Sfx.");
                return;
            }

            var volume = Random.Range(MinVolume, MaxVolume);
            var pitch = Random.Range(MinPitch, MaxPitch);
            var temporaryAudioHost = new GameObject("EditorTestAS_WillAutoDestroy");
            SceneManager.MoveGameObjectToScene(temporaryAudioHost.gameObject, Owner.gameObject.scene);
            temporaryAudioHost.transform.position = Owner.transform.position;
            _editorAudioSource = temporaryAudioHost.AddComponent<AudioSource>();
            PlayAudioSource(_editorAudioSource, tmpAudioClip, volume, pitch, 0);
            var length = 1000 * tmpAudioClip.length;
            await Task.Delay((int)length);
            Owner.ProxyDestroyImmediate(temporaryAudioHost);
        }

        /// <summary>
        ///     A test method that stops the test sound
        /// </summary>
        protected virtual void TestStopSound()
        {
            if (_editorAudioSource != null) _editorAudioSource.Stop();
        }

        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor
        {
            get { return MMFeedbacksInspectorColors.SoundsColor; }
        }

        public override bool HasCustomInspectors => true;

        public override bool EvaluateRequiresSetup()
        {
            var requiresSetup = false;
            if (Sfx == null) requiresSetup = true;
            if (RandomSfx != null && RandomSfx.Length > 0)
            {
                requiresSetup = false;
                foreach (var clip in RandomSfx)
                    if (clip == null)
                        requiresSetup = true;
            }

            return requiresSetup;
        }

        public override string RequiredTargetText => Sfx != null ? Sfx.name : "";

        public override string RequiresSetupText =>
            "This feedback requires that you set an Audio clip in its Sfx slot below, or one or more clips in the Random Sfx array.";
#endif
    }
}