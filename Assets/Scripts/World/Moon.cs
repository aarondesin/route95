using UnityEngine;
using System.Collections;

namespace Route95.World {

    /// <summary>
    /// Class to handle moon movement and lighting.
    /// </summary>
    public class Moon : GlobalLightSource {

        #region Moon Vars

        public static Moon Instance;  // Quick reference to this Instance
        Light _light;                 // Reference to this light component

        public Light shadowCaster;

        public float radius;
        public float scale;

        private Vector3 target; // target for the sun to point at: the car or the origin

        #endregion
        #region Unity Callbacks

        void Awake() {
            Instance = this;
            GetComponent<Light>().cullingMask =
                (1 << 0 | 1 << 1 | 1 << 2 | 1 << 4 | 1 << 5 | 1 << 8 | 1 << 9);
        }

        void Start() {
            transform.SetParent(PlayerMovement.Instance.transform);
            transform.localScale = new Vector3(scale, scale, scale);
        }

        void Update() {
            UpdateTransform();
        }

        #endregion
        #region Moon Callbacks

        private void UpdateTransform() {
            target = PlayerMovement.Instance.transform.position;

            float newX = -radius * Mathf.Cos(WorldManager.Instance.timeOfDay);
            float newY = -radius * Mathf.Sin(WorldManager.Instance.timeOfDay);
            float newZ = -radius * Mathf.Cos(WorldManager.Instance.timeOfDay + Mathf.PI / 5);
            this.transform.position = new Vector3(newX, newY, newZ);

            this.transform.LookAt(target);
        }

        #endregion

    }
}
