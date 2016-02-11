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
		data.riff = InstrumentSetup.currentRiff;
		bf.Serialize (file, data);
		file.Close ();
		Debug.Log ("we saved hopefully");

	}

	
	public void Loadriff(){

		if(File.Exists(Application.persistentDataPath + "/riffInfo.data")){
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open(Application.persistentDataPath + "/riffInfo.dat", FileMode.Open);
		
			riffdata data = (riffdata)bf.Deserialize (file);
			file.Close ();
			Debug.Log ("we load hopefully");
		}
		
	}
	
}

 [Serializable]
class riffdata{
	
	public Riff riff;
	public int beat;


}
