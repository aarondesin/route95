// Fadeable.cs
// ©2016 Team 95

using System.Collections;

using UnityEngine;

namespace Route95.UI {

    /// <summary>
    /// Class to fade all maskable graphics on and in a GameObject
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class Fadeable : MonoBehaviour {

        #region Fadeable Vars

        /// <summary>
        /// Does the graphic start faded?
        /// </summary>
        [Tooltip("Does the graphic start faded?")]
        [SerializeField]
        bool _startFaded = true;

        /// <summary>
        /// Rate of fade/unfade in percent per cycle.
        /// </summary>
        [Tooltip("Rate of fade/unfade in percent per cycle.")]
        [SerializeField]
        float _fadeSpeed = 0.05f;

        /// <summary>
        /// Block raycasts while faded?
        /// </summary>
        [Tooltip("Block raycasts while faded?")]
        [SerializeField]
        bool _blockRaycastsWhileFaded = false;

        /// <summary>
        /// If true, busy fading.
        /// </summary>
        bool _busy = false;

        /// <summary>
        /// CanvasGroup to control.
        /// </summary>
        CanvasGroup _group;

        #endregion
        #region Unity Callbacks

        public void Awake() {
            // Init vars
            _group = GetComponent<CanvasGroup>();

            // Initially fade if necessary
            if (_startFaded) {
                _group.alpha = 0f;
                _group.blocksRaycasts = _blockRaycastsWhileFaded;
            }
        }

        #endregion
        #region Properties

        /// <summary>
        /// Returns true if this fader is busy (read-only).
        /// </summary>
        public bool Busy { get { return _busy; } }

        /// <summary>
        /// Gets/sets whether this object starts faded.
        /// </summary>
        public bool StartFaded {
            get { return _startFaded; }
            set { _startFaded = value; }
        }

        /// <summary>
        /// Returns true if the object is done fading.
        /// </summary>
        public bool DoneFading { get {
            return _group.alpha == 0f && !_busy;
        } }

        /// <summary>
        /// Returns true if the object is done unfading.
        /// </summary>
        public bool DoneUnfading { get {
            return _group.alpha == 1f && !_busy;
        } }

        /// <summary>
        /// Returns true if this object is not busy (read-only).
        /// </summary>
        public bool NotFading {
            get {
                return DoneFading || DoneUnfading;
            }
        }

        /// <summary>
        /// Starts fading the object.
        /// </summary>
        public void Fade() {
            if (gameObject.activeSelf) StopCoroutine("DoUnFade");
            _group.blocksRaycasts = _blockRaycastsWhileFaded;
            _busy = true;
            StartCoroutine("DoFade");
        }

        /// <summary>
        /// Starts unfading the object.
        /// </summary>
        public void UnFade() {
            if (gameObject.activeSelf) StopCoroutine("DoFade");
			_group.blocksRaycasts = true;
            _busy = true;
            StartCoroutine("DoUnFade");
        }

        /// <summary>
        /// Fades an object.
        /// </summary>
        IEnumerator DoFade() {
            while (true) {
                if (_group.alpha >= _fadeSpeed) {

                    _group.alpha -= _fadeSpeed;

                    yield return null;
                }
                else {
                    _busy = false;
                    //if (disableAfterFading) gameObject.SetActive(false);
                    if (_blockRaycastsWhileFaded) _group.blocksRaycasts = true;
                    _group.alpha = 0f;
                    yield break;
                }
            }
        }

        /// <summary>
        /// Unfades an object.
        /// </summary>
        IEnumerator DoUnFade() {
            while (_group.alpha <= 1f - _fadeSpeed) {

                _group.alpha += _fadeSpeed;

                yield return null;
            }
            _busy = false;
            _group.alpha = 1f;
            yield break;
        }

        #endregion
    }
}
