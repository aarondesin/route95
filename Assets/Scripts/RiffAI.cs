using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RiffAI : MonoBehaviour{

	//public RiffAI instance;
	int minimumSimilarityValue = 0;

	//Compares the given riff to all of the cases in 
	//melodyCaseList and RhtyhmCaseList, to find the case that
	//is most similar. Rhythmic and melodic similarities are
	//calculated seperately and weighted evenly.
	//
	//"minimumSimilarityValue" is an option value that determines
	//how similar a riff has to be to be considered similar at all.
	//This is useful if none of the cases match very well, and you 
	//want to only highlight hints if you have a good hint.
	public Riff FindSimilarCase(Riff playerRiff){
		Dictionary<Riff, int> SimilarityDictionary = new Dictionary<Riff, int>();
		foreach(Riff caseRiff in CaseLibrary.cases) {
			SimilarityDictionary.Add(caseRiff, 0);
			//Compare riff in each case to the given riff,
			//adding points to that case's score in the dicitonary.
			for (int i = 0; i < playerRiff.beatsShown*4; i++) {
				if (playerRiff.notes[i].Count == caseRiff.notes[i].Count) {
					SimilarityDictionary [caseRiff] += playerRiff.notes[i].Count + 1;
				}
				/*for (int j=0; j <playerRiff.notes[i].Count; j++) {
					if (playerRiff.notes[i][j].Equals(caseRiff.notes[i][j])) {
						Debug.Log(playerRiff.name);
						Debug.Log(playerRiff.notes[i].Count);
						SimilarityDictionary [caseRiff] += playerRiff.notes[i].Count + 1;
					}
				}*/
			}
		}

		//Select the case that has the highest score, indicating that it is
		//overall the most similar case.
		int bestScore = -1;
		Riff bestCase = null;
		//int bestRhythmScore = -1;
		//string bestRhythmCase = null;
		//find best rhythm case
		foreach (Riff key in SimilarityDictionary.Keys) {
			if (SimilarityDictionary[key] > bestScore){
				bestScore = SimilarityDictionary[key];
				bestCase = key;
			}
		}
		//find best melody case
		//		if (!isPercussion) {
		//			foreach (Riff key in melodySimilarityDicitonary.Keys) {
		//				if (melodySimilarityDicitonary[key] > bestMelodyScore) {
		//					bestMelodyScore = melodySimilarityDicitonary [key];
		//					bestMelodyCase = key;
		//				}
		//			}
		//		}

		//if we have a good match, return it
		Debug.Log ("inside similaeity, bestscore " + bestScore);
		if (bestScore > minimumSimilarityValue){
			return bestCase;
		}
		//else, return null
		else{
			return null;
		}
	}

	//Returns the note to be highlighted.
	//If no further note should be highlighted, returns null.
	//
	//Loops backward through the player's riff, finding the furthest
	//non-empty note, since any hints will be after that note.
	//Then loops forward from that position in the closest case,
	//until it finds a non-empty note. This non-empty note is the hint
	//for the next note the player should play.
	public int FindHintXPosition(Riff closestCase, Riff playerRiff){
		int playerPosition = -1;
		Debug.Log ("inside findhintX " );
		//int i = 16;
		//foreach(Note note in playerRiff[i]){
		for (int i = playerRiff.beatsShown*4-1; i >= 0; --i){
			if (playerRiff.notes[i].Any() == true){
				playerPosition = i;
				break;
			}
		}
		return playerPosition;
	}

	public int FindHintYPosition (Riff playerRiff, Scale scaleDude) {
		//Debug.Log ("inside findhintY ");
		Riff closestCase = FindSimilarCase(playerRiff);
		int playerPosition = FindHintXPosition (closestCase, playerRiff);

		Note caseNote = new Note();
		//caseNote = closestCase.notes[playerPosition][0];
		Debug.Log ("inside findhintY, below Note");
		Debug.Log ("inside findhintY, closestCase " + (closestCase == null));

		if (closestCase == null) return 0;
		for (; playerPosition < closestCase.beatsShown*4-1; ++playerPosition){// replace 16 with a dynamic value from riff class
			Debug.Log ("inside findhintY, in for, outside if ");
			if (closestCase.notes[playerPosition].Any() == true){
				Debug.Log ("inside findhintY, in if ");
				caseNote = closestCase.notes[playerPosition][0];// return note at a specific position still broken
			}
		}
		Debug.Log ("inside findhintY scale dude " + scaleDude.allNotes);
		List<string> matchNotes = scaleDude.allNotes;
		for (int i = 0; i < matchNotes.Count; i++) {
			if (matchNotes [i] == caseNote.filename)
				return i;
		}



		return -1;
	}

}
