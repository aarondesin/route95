using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scale {
	public List<string> root;
	public List<string> second;
	public List<string> third;
	public List<string> fourth;
	public List<string> fifth;
	public List<string> sixth;
	public List<string> seventh;

	public List<string> allNotes;

	public Scale () {
		root = new List<string> ();
		second = new List<string> ();
		third = new List<string> ();
		fourth = new List<string> ();
		fifth = new List<string> ();
		sixth = new List<string> ();
		seventh = new List<string> ();

		allNotes = new List<string> ();
	}
}
