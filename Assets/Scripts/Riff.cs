using UnityEditor;
using UnityEngine;
using System; // for enum stuff
using System.Collections;
using System.Collections.Generic;// need for using lists
using System.Linq;

public class Riff {
	public static int MAX_SUBDIVS = 2;

	//
	// VARIABLES SAVED IN PROJECT
	//

	public string name; // user-defined name of the riff
	public Instrument instrument; // instrument used for this riff
	public List<List<Note>> notes = new List<List<Note>>(); // contains notes
	public bool cutSelf = true; // if true, sounds will cut themselves off

	//
	// VARIABLES NOT SAVED IN PROJECT
	//
	public int copy = 0; // not saved

	// Default constructor makes an empty 4-beat riff (and accounts for all subdivs)
	public Riff () {
		for (int i=0; i<(int)Mathf.Pow(2f, (float)MAX_SUBDIVS+2); i++) {
			notes.Add (new List<Note> ());
		}
		copy = 0;
	}

	// Constructor to make a riff of a certain number of beats
	public Riff (int length) {
		for (int i=0; i<length*(int)Mathf.Pow(2f, (float)MAX_SUBDIVS); i++) {
			notes.Add (new List<Note>());
		}
		copy = 0;
	}

	/*// Loading from a string
	public Riff (string instrumentString) {
		currentInstrument = (Instrument)Enum.Parse (typeof(Instrument), instrumentString);
	}*/

	public Riff (string loadString) {

		/*for (int i=0; i<1*(int)Mathf.Pow(2f, (float)MAX_SUBDIVS); i++) {
			notes.Add (new List<Note>());
		}*/

		string[] vars= loadString.Split (new char[]{save_load.itemSeparator,save_load.noteSeparator});

		LoadName(vars [0]);
		//Debug.Log ("Riff name: " + name);
		instrument = (Instrument)Enum.Parse (typeof(Instrument), vars [1]);
		//Debug.Log ("Instrument: " + (string)Enum.GetName (typeof(Instrument), (int)instrument));
		cutSelf = (vars [2] == "True" ? true : false);
		//Debug.Log ("cutSelf: " + cutSelf.ToString ());
		int currentBeat = 0; // current beat being loaded in
		for (int i=3; i<vars.Length && vars[i] != ""; i++) {
			//Debug.Log ("Parsing " + vars[i]);
			int itemval;
			if (!int.TryParse(vars[i], out itemval)) {
				//Debug.Log("Note "+vars[i]+" at "+currentBeat);
				notes [currentBeat].Add (new Note (vars[i]));
				//newRiff.notes[currentBeat].Add (Note.LoadNote (item));
			} else {
				//int itemval;
				bool beat = int.TryParse (vars[i],out itemval);
				if (beat == false) {
					//Debug.Log (vars[i] + "not a number");
				} else {
					currentBeat = itemval;
					while (currentBeat >= notes.Count) {
						notes.Add(new List<Note>());
					}
					//Debug.Log ("Now on beat " + itemval);
				}
			}
		}
		//copy = MusicManager.instance.CopyNumber(name);
	}

	public void LoadName (string str) {
		Debug.Log("LoadName("+str+")");
		string[] split = str.Split(new char[]{'(',')'});
		int cpy;
		for (int j=split.Length-1; j>=0; j--) {
			if (int.TryParse(split[j], out cpy)) {
				Debug.Log("is a copy");

				name = "";
				string[] split2 = str.Split('(');
				for (int i=0; i<split2.Length-1; i++) {
					if (i!=0) name += "(";
					name += split2[i];
				}
				//copy = MusicManager.instance.CopyNumber(name);
				return;
			}
		}
		name = str;

		Debug.Log(str);
		Debug.Log(name);
		Debug.Log(copy);
	}

	public void SetInstrument (Instrument inst) {
		instrument = inst;
	}

	public int GetLength () {
		return notes.Count;
	}

	// Returns true is a note is found at a position
	public bool Lookup (Note newNote, int pos) {
		//return notes[pos].Contains(newNote);
		foreach (Note note in notes[pos]) {
			if (note.sound == newNote.sound) return true;
		}
		return false;
	}

	public void RemoveNote (Note newNote, int pos) {
		foreach (Note note in notes[pos]) {
			if (note.sound == newNote.sound) {
				notes[pos].Remove(note);
				return;
			}
		}
	}

	// Removes all notes at position
	public void Clear (int pos) {
		notes[pos].Clear();
	}

	// Adds or removes a note at pos
	public void Toggle (Note newNote, int pos) {
		// Lookup
		//foreach (Note note in notes[pos]) {
			//if (newNote.sound == note.sound) {
		if (Lookup(newNote, pos)) {
				// Note with same sound is already there
				//notes [pos].Remove (note);
			//notes[pos].Remove(newNote);
			RemoveNote (newNote, pos);
			//Debug.Log("removed note");
				return;
			}
		//}
		// Note not already there
		notes [pos].Add (newNote);
		//Debug.Log (newNote.ToString());
		//Debug.Log(this.ToString());
		MusicManager.instance.PlayOneShot(newNote.sound);
		//Debug.Log ("added note");
	}
	public void PlayRiffLoop (AudioClip clip) {
		MusicManager.instance.LoopRiff.Stop();
		MusicManager.instance.LoopRiff.clip = clip;
		MusicManager.instance.LoopRiff.loop = true;
	}

	// Plays all the notes at pos
	public void PlayRiff (int pos) { 
		//Debug.Log ("before for loop");


		foreach (Note note in notes[pos]) {
			//Debug.Log("inside for loop " + pos);
			if (cutSelf) 
				//MusicManager.instance.OneShot.Stop();
				MusicManager.instance.instrumentAudioSources[instrument].Stop();
			//if (MusicManager.instance.instrumentAudioSources[currentInstrument] == null) Debug.Log("shit");
			note.PlayNote(MusicManager.instance.instrumentAudioSources[instrument]);

		}

		
	}

	public override string ToString () {
		string result = name;
		if (copy != 0) result += " (" + copy.ToString() + ")";
		result += save_load.itemSeparator;
		result += (string)Enum.GetName (typeof(Instrument), (int)instrument) + save_load.itemSeparator;
		result += cutSelf.ToString ()+save_load.itemSeparator;
		for (int i = 0; i < notes.Count; i++) {
			result += i.ToString() + save_load.itemSeparator;
			foreach (Note note in notes[i]) {
				result += note.ToString () + save_load.noteSeparator;
			}
		}
		Debug.Log("Riff.ToString(): "+result);
		return result;
	}
		
}
