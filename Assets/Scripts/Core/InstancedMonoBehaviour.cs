using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Class to enable instancing of a class.
/// </summary>
public class InstancedMonoBehaviour : MonoBehaviour {

	#region InstancedMonoBehaviour Exceptions

	public class MultipleInstanceException : ApplicationException {
		public MultipleInstanceException (string info, Exception standard) : base (info, standard) { }
		public MultipleInstanceException (string info) : base (info) { }
		public MultipleInstanceException () { }
	}

	#endregion
	#region InstancedMonoBehaviour Vars

	public static InstancedMonoBehaviour instance; // Quick reference to this instance

	#endregion
	#region Unity Callbacks

	void Awake () {
		if (instance != null)
			throw new MultipleInstanceException ("An instance of this already exists!");

		instance = this;
	}

	#endregion
}
