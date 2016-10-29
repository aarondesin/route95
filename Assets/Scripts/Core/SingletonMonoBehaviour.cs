// SingletonMonoBehaviour.cs
// ©2016 Team 95 

using System;
using System.Runtime.InteropServices;

using UnityEngine;

/// <summary>
/// Class to ensure singleton system in a MonoBehaviour
/// </summary>
[ComVisible(true)]
public class SingletonMonoBehaviour<T> : MonoBehaviour 
	where T : MonoBehaviour {

	#region Exceptions

	/// <summary>
	/// Exception thrown when if more than one Singleton exists.
	/// </summary>
	class MultipleInstanceException : ApplicationException {
		public MultipleInstanceException (string info) : base (info) { }
		public MultipleInstanceException () { }
	}

	#endregion
	#region Static Vars

    /// <summary>
    /// Instance of this class.
    /// </summary>
	static SingletonMonoBehaviour<T> _Instance;

	#endregion
	#region Unity Callbacks

	protected void Awake () {

		// Check if an Instance already exists
		if (_Instance != null)
			throw new MultipleInstanceException();
		else _Instance = this;
	}

	#endregion
	#region Properties

	/// <summary>
	/// Returns the Instance of this class.
	/// </summary>
	public static T Instance {
		get { return _Instance as T; }
	}

	#endregion
}
