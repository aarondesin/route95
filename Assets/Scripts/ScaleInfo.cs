using UnityEngine;
using System.Collections;

public class ScaleInfo {
	public int rootIndex;
	public int secondIndex;
	public int thirdIndex;
	public int fourthIndex;
	public int fifthIndex;
	public int sixthIndex;
	public int seventhIndex;

	public static ScaleInfo Minor = new ScaleInfo () {
		secondIndex = 2,
		thirdIndex = 1,
		fourthIndex = 2,
		fifthIndex = 2,
		sixthIndex = 1,
		seventhIndex = 2,
		rootIndex = 2
	};

	public static ScaleInfo Major = new ScaleInfo () {
		secondIndex = 2,
		thirdIndex = 2,
		fourthIndex = 1,
		fifthIndex = 2,
		sixthIndex = 2,
		seventhIndex = 2,
		rootIndex = 1
	};

}