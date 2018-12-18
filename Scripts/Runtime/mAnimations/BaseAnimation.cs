// ReSharper disable ArrangeAccessorOwnerBody
using mFramework.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace mFramework.Animations
{
    public abstract class BaseAnimation<TSettings> : MonoBehaviour 
        where TSettings : AnimationSettings, new()
    {
        /// <summary>
        /// Normilized time
        /// </summary>
        public float Time { get; private set; }

        /// <summary>
        /// Normilized delta time
        /// </summary>
        public float DeltaTime { get; private set; }

        /// <summary>
        /// Normilized easing time
        /// </summary>
        public float EasingTime { get; private set; }

        /// <summary>
        /// Normilized delta easing time
        /// </summary>
        public float DeltaEasingTime { get; private set; }

        public ulong Repeats { get; private set; }

        [SerializeField]
        public TSettings Settings;

        [SerializeField]
        public AnimationDirection Direction;

        [SerializeField]
        public AnimationState State;

        public UnityEvent OnEnded
        {
            get { return _onEnded; }
            set { _onEnded = value; }
        }

        public UnityEvent OnRepeat
        {
            get { return _onRepeat; }
            set { _onRepeat = value; }
        }

        public UnityEvent OnRemoved
        {
            get { return _onRemoved; }
            set { _onRemoved = value; }
        }

        [SerializeField]
        [FormerlySerializedAs("AnimationEnded")]
        private UnityEvent _onEnded = new UnityEvent();

        [SerializeField]
        [FormerlySerializedAs("AnimationRemoved")]
        private UnityEvent _onRemoved = new UnityEvent();

        [SerializeField]
        [FormerlySerializedAs("AnimationRepeat")]
        private UnityEvent _onRepeat = new UnityEvent();

        protected virtual void Awake()
        {
            Reset();
        }

        public void ResetAndPlay()
        {
            Reset();
            Play();
        }

        public void Reset()
        {
            Time = 0f;
            EasingTime = 0f;
            DeltaEasingTime = 0f;
            DeltaTime = 0f;
            Repeats = 0;
            Direction = AnimationDirection.Forward;
        }

        public void Play()
        {
            State = AnimationState.Playing;
        }

        public void Stop()
        {
            State = AnimationState.Stopped;
        }

        // ReSharper disable once UnusedMember.Local
        private void Update()
        {
            if (State == AnimationState.Stopped)
                return;

            DeltaTime = (Direction == AnimationDirection.Forward ? 1f : -1f) *
                        (UnityEngine.Time.deltaTime / Settings.Duration);

            Time = Mathf.Clamp(Time + DeltaTime, 0f, 1f);

            var easingTime = Easings.Interpolate(Time, Settings.EasingType);
            DeltaEasingTime = easingTime - EasingTime;
            EasingTime = easingTime;

            Animate();
            
            if (Time >= 1f && Direction == AnimationDirection.Forward ||
                Time <= 0f && Direction == AnimationDirection.Backward)
            {
                if (Settings.PlayType == AnimationPlayType.EndReset)
                {
                    Repeats++;
                    OnRepeat.Invoke();
                    Time = 0f;
                    EasingTime = 0f;
                }
                else if (Settings.PlayType == AnimationPlayType.EndFlip)
                {
                    Repeats++;
                    OnRepeat.Invoke();
                    Direction = Direction == AnimationDirection.Forward
                        ? AnimationDirection.Backward
                        : AnimationDirection.Forward;
                }

                if (Settings.PlayType == AnimationPlayType.PlayOnce ||
                    Settings.MaxRepeats > 0 && Repeats >= Settings.MaxRepeats)
                {
                    State = AnimationState.Stopped;
                    OnEnded.Invoke();
                }
            }
        }

        private void OnDestroy()
        {
            OnRemoved.Invoke();
        }

        public abstract void Animate();
    }
}