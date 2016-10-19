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

	static SingletonMonoBehaviour<T> _instance;

	#endregion
	#region Unity Callbacks

	public void Awake () {

		// Check if an instance already exists
		if (_instance != null)
			throw new MultipleInstanceException();
		else _instance = this;
	}

	#endregion
	#region Methods

	/// <summary>
	/// Returns the instance of this class.
	/// </summary>
	public static T Instance {
		get { return _instance as T; }
	}

	#endregion
}
