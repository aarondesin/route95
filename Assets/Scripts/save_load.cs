using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class save_load : MonoBehaviour {

	public void Saveriff(){
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Open(Application.persistentDataPath + "/riffInfo.dat", FileMode.Open);

		riffdata data = new riffdata ();
		data.riffs = InstrumentSetup.currentRiff;
		bf.Serialize (file, data);
		file.Close ();

	}

	
	public void Loadriff(){
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Open(Application.persistentDataPath + "/riffInfo.dat", FileMode.Open);
		
		riffdata data = (riffdata)bf.Deserialize (file);
		data.riffs = InstrumentSetup.currentRiff;
		bf.Serialize (file, data);
		file.Close ();
		
	}
	
}

 [Serializable]
class riffdata{
	
	public List<Riff> riffs = new List<Riff> ();
	private int beat;


}
