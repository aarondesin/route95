using System;
using System.Collections.Generic;

/// <summary>
/// Class to add extension functions to lists.
/// </summary>
public static class ListExtension {

	/// <summary>
	/// Returns the first element of a list.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <returns></returns>
	public static T Head<T> (this List<T> list) {
		return list[0];
	}

	/// <summary>
	/// Returns the final element of a list.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <returns></returns>
	public static T Tail<T> (this List<T> list) {
		return list[list.Count-1];
	}
	
}

