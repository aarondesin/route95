using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Measure {

	[SerializeField]
	public int index;

	[SerializeField]
	public List<int> riffIndices;

	public Measure () {
		riffIndices = new List<int>();
	}
}
