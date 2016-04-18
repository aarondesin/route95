using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScaleInfo {
	public string name;
	public int scaleIndex; // index in list of all scales

	public int rootIndex;
	public int secondIndex;
	public int thirdIndex;
	public int fourthIndex;
	public int fifthIndex;
	public int sixthIndex;
	public int seventhIndex;

	public static ScaleInfo Major = new ScaleInfo () {
		name = "Major",
		scaleIndex = 0,
		secondIndex = 2,
		thirdIndex = 2,
		fourthIndex = 1,
		fifthIndex = 2,
		sixthIndex = 2,
		seventhIndex = 2,
		rootIndex = 1
	};

	public static ScaleInfo Minor = new ScaleInfo () {
		name = "Minor",
		scaleIndex = 1,
		secondIndex = 2,
		thirdIndex = 1,
		fourthIndex = 2,
		fifthIndex = 2,
		sixthIndex = 1,
		seventhIndex = 2,
		rootIndex = 2
	};
			
	public static List<ScaleInfo> AllScales = new List<ScaleInfo> () {
		Major,
		Minor
	};

}