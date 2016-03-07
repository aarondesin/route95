using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RiffAI{

	int minimumSimilarityValue = 4;

	//Compares the given riff to all of the cases in 
	//melodyCaseList and RhtyhmCaseList, to find the case that
	//is most similar. Rhythmic and melodic similarities are
	//calculated seperately and weighted evenly.
	//
	//"minimumSimilarityValue" is an option value that determines
	//how similar a riff has to be to be considered similar at all.
	//This is useful if none of the cases match very well, and you 
	//want to only highlight hints if you have a good hint.
	public Riff FindClosestCase(Riff playerRiff, bool isPercussion){
		Dictionary<Riff, int> SimilarityDicitonary = new Dictionary<Riff, int>();
		foreach(Riff rhythmCase in CaseLibrary.rhythmcases) {
			//Compare rhythm in each case to the rhythm of the given riff,
			//adding points to that case's score in the dicitonary.
		}
		if (!isPercussion){
			foreach(Riff melodyCase in CaseLibrary.melodycases) {
			//Compare melody in each case to the meldoy of the given riff,
			//adding points to that case's score in the dicitonary.
			}		
		}
		//Select the case that has the highest score, indicating that it is
		//overall the most similar case.
		int bestScore = -1;
		Riff bestCase = null;
		foreach (Riff key in SimilarityDicitonary.Keys) {
			if (SimilarityDicitonary[key] > bestScore){
				bestScore = SimilarityDicitonary[key];
				bestCase = key;
			}
		}
		//if we have a good match, return it
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
	public Note FindHintPosition(Riff closestCase, Riff playerRiff){
		int playerPosition = -1;
		//int i = 16;
		//foreach(Note note in playerRiff[i]){
		for (int i = 16; i > 0; i--){
			if (playerRiff.notes[i].Any() == true){
				playerPosition = i;
				break;
			}
		}
		for (; playerPosition < 16; ++playerPosition){// replace 16 with a dynamic value from riff class
			if (closestCase.notes[playerPosition].Any() == true){
				return closestCase.notes[playerPosition][0];// return note at a specific position still broken
			}
		}
		return null;
	}

}
