// Sun.cs
// ©2016 Team 95

using UnityEngine;

namespace Route95.World {

    public class Sun : GlobalLightSource<Sun> {

		[SerializeField]
        bool _invert = false;

        new void Start() {
			base.Start();

            GetComponent<Light>().cullingMask = (1 << 0 | 1 << 1 | 1 << 2 | 1 << 4 | 1 << 5 | 1 << 8 | 1 << 9);
        }

		new void UpdateTransform() {
            base.UpdateTransform();

            if (_invert) transform.Rotate(new Vector3(180f, 0f, 0f));
        }
    }
}
