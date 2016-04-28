using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public struct PerfInfo {
	public float seconds;
	public float frames;
	public float avgFPS;

	public PerfInfo (float startSeconds, float startFrames, float startFPS) {
		seconds = startSeconds;
		frames = startFrames;
		avgFPS = startFPS;
	}

	public override string ToString() {
		return 
			"Seconds: " + seconds.ToString("##.0000") + "\n" +
			"Frames: " + frames.ToString ("##.00") + "\n" +
			"Average FPS: " + avgFPS.ToString ("##.000") + "\n\n";
	}
};

public class PerfTracker : MonoBehaviour {

	Dictionary<GameManager.Mode, PerfInfo> perfTracker;

	#region UnityCallbacks

	void Start () {
		perfTracker = new Dictionary<GameManager.Mode, PerfInfo>();
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!perfTracker.ContainsKey(GameManager.instance.currentMode)) {
			perfTracker.Add(GameManager.instance.currentMode, new PerfInfo());
		} else {
			PerfInfo perf = perfTracker[GameManager.instance.currentMode];
			perf.seconds += Time.deltaTime;
			perf.frames += 1;
			perf.avgFPS += ((1f/Time.deltaTime) - perf.avgFPS) / perf.frames;
		}
	
	}

	void OnApplicationQuit () {
		if (Debug.isDebugBuild) {
			DateTime currTime = System.DateTime.Now;
			string log = currTime.ToString() + "\n";
			foreach (GameManager.Mode mode in Enum.GetValues(typeof (GameManager.Mode))) {
				if (perfTracker.ContainsKey(mode))
					log += mode.ToString() + "\n-----\n" + perfTracker[mode].ToString();
			}
			try {
				Directory.CreateDirectory (Application.persistentDataPath+"/PerfLogs");
				System.IO.File.WriteAllText(Application.persistentDataPath+
					"/PerfLogs/PerfLog_" + currTime.Year.ToString() + currTime.Month.ToString() + currTime.Day.ToString() + "_" + currTime.Hour.ToString() + currTime.Minute.ToString() + currTime.Second.ToString() + ".txt", log);
				Debug.Log (log);
			} catch (UnauthorizedAccessException) {
				Debug.Log (log);
			}
		}
	}


	#endregion

}
