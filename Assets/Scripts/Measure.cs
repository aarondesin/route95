using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Measure {

	[SerializeField]
	public List<Riff> riffs;

	public Measure () {
		riffs = new List<Riff>();
	}
}
