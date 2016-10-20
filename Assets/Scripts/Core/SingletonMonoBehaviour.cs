// SingletonMonoBehaviour.cs
// ©2016 Aaron Desin 

using UnityEngine;
using System;
using System.Runtime.InteropServices;

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

	static SingletonMonoBehaviour<T> _Instance;

	#endregion
	#region Unity Callbacks

	public void Awake () {

		// Check if an Instance already exists
		if (_Instance != null)
			throw new MultipleInstanceException();
		else _Instance = this;
	}

	#endregion
	#region Methods

	/// <summary>
	/// Returns the Instance of this class.
	/// </summary>
	public static T Instance {
		get { return _Instance as T; }
	}

	#endregion
}
