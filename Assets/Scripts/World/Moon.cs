// Moon.cs
// ©2016 Team 95

using System;
using UnityEngine;

namespace Route95.World {

    /// <summary>
    /// Class to handle moon movement and lighting.
    /// </summary>
    public class Moon : GlobalLightSource<Moon> {

		#region Vars

		/// <summary>
		/// Reference to this SpriteRenderer.
		/// </summary>
		SpriteRenderer _renderer;

		float _offset = -Mathf.PI / 2f;

		#endregion
		#region Unity Callbacks

		new protected void Awake() {
            base.Awake();

			// Init vars
			_renderer = GetComponent<SpriteRenderer>();
            _light.cullingMask =
                (1 << 0 | 1 << 1 | 1 << 2 | 1 << 4 | 1 << 5 | 1 << 8 | 1 << 9);
        }

		#endregion
		#region Properties

		public SpriteRenderer Renderer { get { return _renderer; } }

		public override float Offset { get { return _offset; } }

		#endregion
	}
}
