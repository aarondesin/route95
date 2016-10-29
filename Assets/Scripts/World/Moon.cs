// Moon.cs
// ©2016 Team 95

namespace Route95.World {

    /// <summary>
    /// Class to handle moon movement and lighting.
    /// </summary>
    public class Moon : GlobalLightSource<Moon> {

        #region Unity Callbacks

        new void Awake() {
            base.Awake();

			// Init vars
            _light.cullingMask =
                (1 << 0 | 1 << 1 | 1 << 2 | 1 << 4 | 1 << 5 | 1 << 8 | 1 << 9);
        }

		#endregion
    }
}
