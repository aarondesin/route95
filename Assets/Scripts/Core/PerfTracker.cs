// PerfTracker.cs
// ©2016 Team 95

using System;
using System.IO;
using System.Collections.Generic;

using UnityEngine;

namespace Route95.Core {

    /// <summary>
    /// Class to track application performance across its
    /// various states.
    /// </summary>
    public class PerfTracker : MonoBehaviour {

        #region Nested Structs

        /// <summary>
        /// Struct to store performance info for a game state.
        /// </summary>
        struct Info {

            /// <summary>
            /// Number of seconds active.
            /// </summary>
            public float seconds;

            /// <summary>
            /// Number of frames active
            /// </summary>
            public float frames;

            /// <summary>
            /// Current average FPS
            /// </summary>
            public float avgFPS;

            /// <summary>
            /// Creates a PerfTracker.Info struct.
            /// </summary>
            /// <param name="startSeconds"></param>
            /// <param name="startFrames"></param>
            /// <param name="startFPS"></param>
            public Info(
                float startSeconds = 0f, 
                float startFrames = 0f, 
                float startFPS = 0f
            ) {
                seconds = startSeconds;
                frames = startFrames;
                avgFPS = startFPS;
            }

            /// <summary>
            /// String conversion.
            /// </summary>
            public override string ToString() {
                return
                    "Seconds: " + seconds.ToString("##.0000") + "\n" +
                    "Frames: " + frames.ToString("##.00") + "\n" +
                    "Average FPS: " + avgFPS.ToString("##.000") + "\n\n";
            }
        };

        #endregion
        #region PerfTracker Vars

        /// <summary>
        /// Dictionary to map game states to Info structs.
        /// </summary>
        Dictionary<GameManager.State, Info> _perfTracker;

        #endregion
        #region UnityCallbacks

        void Awake() {
            // Init dictionary
            _perfTracker = new Dictionary<GameManager.State, Info>();
            int numStates = System.Enum.GetValues(typeof(GameManager.State)).Length;
            for (int i=0; i<numStates; i++) {
                _perfTracker.Add ((GameManager.State)i, new Info());
            }
        }

        void Update() {
            Info perf = _perfTracker[GameManager.Instance.CurrentState];
            perf.seconds += Time.deltaTime;
            perf.frames += 1;
            perf.avgFPS += ((1f / Time.deltaTime) - perf.avgFPS) / perf.frames;
        }

        void OnApplicationQuit() {

            // Only save perf info in debug builds
            if (Debug.isDebugBuild) {

                // Get current time
                DateTime currTime = System.DateTime.Now;
                string log = currTime.ToString() + "\n";

                // Print info for each state
                foreach (GameManager.State state in Enum.GetValues(typeof(GameManager.State))) {
                    if (_perfTracker.ContainsKey(state))
                        log += state.ToString() + "\n-----\n" + _perfTracker[state].ToString();
                }

                // Create PerfInfo directory if possible
                try {
                    Directory.CreateDirectory(Application.persistentDataPath + "/PerfLogs");
                    System.IO.File.WriteAllText(PerfInfoPath(currTime), log);

                    // If fails, print to console
                }
                catch (UnauthorizedAccessException) {
                    Debug.Log(log);
                }
            }
        }

        #endregion
        #region PerfTracker Methods

        /// <summary>
        /// Returns a path for the PerfInfo file based on the
        /// current date and time.
        /// </summary>
        string PerfInfoPath(DateTime time) {
            return
                Application.persistentDataPath +
                "/PerfLogs/PerfLog_" +
                time.Year.ToString() +
                time.Month.ToString() +
                time.Day.ToString() + "_" +
                time.Hour.ToString() +
                time.Minute.ToString() +
                time.Second.ToString() +
                ".txt";
        }

        #endregion
    }
}
