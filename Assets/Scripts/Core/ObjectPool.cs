using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Data storage class to handle poolable
/// GameObjects.
/// </summary>
public class ObjectPool {

	List<GameObject> pool; // List of inactive GameObjects

	/// <summary>
	/// Init this instance.
	/// </summary>
	public ObjectPool () {
		pool = new List<GameObject>();
	}

	/// <summary>
	/// Adds an item to the pool, and deactivates it.
	/// </summary>
	/// <param name="item">Item to add to the pool.</param>
	public void Add (GameObject item) {
		pool.Add(item);
		item.SetActive (false);
	}

	/// <summary>
	/// Returns a reference to the object at the top
	/// of the pool.
	/// </summary>
	/// <returns>Reference to the top GameObject.</returns>
	public GameObject Peek () {
		if (Empty) return null;
		return pool[0];
	}

	/// <summary>
	/// Removes and returns an item from the pool
	/// and activates it.
	/// </summary>
	/// <returns>GameObject at the top of the pool.</returns>
	public GameObject Get () {

		// Return null if empty
		if (Empty) return null;

		GameObject result = pool[0];
		pool.RemoveAt(0);
		result.SetActive(true);
		return result;
	}

	/// <summary>
	/// Returns whether or not the pool is empty.
	/// </summary>
	public bool Empty {
		get {
			return pool.Count == 0;
		}
	}

}
