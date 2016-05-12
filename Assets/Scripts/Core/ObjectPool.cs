using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool {

	List<GameObject> pool;

	/// <summary>
	/// Init this instance.
	/// </summary>
	public ObjectPool () {
		pool = new List<GameObject>();
	}

	public void Add (GameObject item) {
		pool.Add(item);
		item.SetActive (false);
	}

	public GameObject Peek () {
		if (Empty) return null;
		return pool[0];
	}

	public GameObject Get () {
		if (Empty) return null;

		GameObject result = pool[0];
		pool.RemoveAt(0);
		result.SetActive(true);
		return result;
	}

	public bool Empty {
		get {
			return pool.Count == 0;
		}
	}

}
