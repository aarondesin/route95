// Sun.cs
// ©2016 Team 95

using System;
using UnityEngine;

namespace Route95.World {

    public class Sun : GlobalLightSource<Sun> {

		float _offset = Mathf.PI / 2f;

        new void Start() {
			base.Start();

            GetComponent<Light>().cullingMask = (1 << 0 | 1 << 1 | 1 << 2 | 1 << 4 | 1 << 5 | 1 << 8 | 1 << 9);
        }

		public override float Offset { get { return _offset; } }
	}
}
