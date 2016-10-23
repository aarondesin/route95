// SaveLoad.cs
// ©2016 Team 95

using Route95.Music;
using Route95.UI;

using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using UnityEngine;

namespace Route95.Core {

    /// <summary>
    /// Class to handle I/O operations.
    /// </summary>
    public static class SaveLoad {

        #region Exceptions

        /// <summary>
        /// Exception thrown whenever a file fails to save.
        /// </summary>
        [Serializable]
        public class FailedToSaveException : ApplicationException {
            public FailedToSaveException(string info, Exception standard) : base(info, standard) { }
            public FailedToSaveException(string info) : base(info) { }
            public FailedToSaveException() { }
        }

        /// <summary>
        /// Exception thrown whenever a file fails to load.
        /// </summary>
        [Serializable]
        public class FailedToLoadException : ApplicationException {
            public FailedToLoadException(string info, Exception standard) : base(info, standard) { }
            public FailedToLoadException(string info) : base(info) { }
            public FailedToLoadException() { }
        }

        #endregion
        #region SaveLoad Vars

        /// <summary>
        /// File extension for projects.
        /// </summary>
        public const string PROJECT_SAVE_EXT = ".r95p";

        /// <summary>
        /// File extension for songs.
        /// </summary>
        public const string SONG_SAVE_EXT = ".r95s";

        #endregion
        #region Save Methods

        /// <summary>
        /// Saves an item to a particular path with a certain name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSave">Object to save (must be serializable).</param>
        /// <param name="dirPath">Full path to save directory.</param>
        /// <param name="filePath">Full save path, including file name, extension, and directory.</param>
        public static void Save<T>(T toSave, string dirPath, string filePath) where T : new() {

            // Check if type is serializable
            if (!typeof(T).IsSerializable)
                throw new FailedToSaveException("SaveLoad.Save(): type " + 
                    toSave.GetType().ToString() + " is not serializable!");

            // Init BinaryFormatter
            BinaryFormatter bf = new BinaryFormatter();

            // Create directory
            Directory.CreateDirectory(dirPath);

            // Open file
            FileStream file = File.Open(filePath, FileMode.Create);

            // Serialize object
            bf.Serialize(file, toSave);

            // Close file
            file.Close();
        }

        /// <summary>
        /// Saves the current open project.
        /// </summary>
        public static void SaveCurrentProject() {

            // Build paths
            string directoryPath = GameManager.Instance.ProjectSavePath;
            string filePath = directoryPath + MusicManager.Instance.CurrentProject.Name + 
                PROJECT_SAVE_EXT;

            try {

                // Save project
                Save<Project>(MusicManager.Instance.CurrentProject, directoryPath, filePath);

                // Prompt
                Prompt.Instance.PromptMessage("Save Project", "Successfully saved project to \"" + filePath + "\".", "Okay");

                // Catch exceptions
            }
            catch (FailedToSaveException e) {
                Debug.LogError(e.Message);
                return;
            }
        }

        /// <summary>
        /// Saves the indicated song.
        /// </summary>
        /// <param name="song">Song to save.</param>
        public static void SaveSong(Song song) {

            // Build paths
            string directoryPath = Application.dataPath + "/Songs/";
            string filePath = directoryPath + song.Name + SONG_SAVE_EXT;

            // Save song
            Save<Song>(song, directoryPath, filePath);

            // Prompt
            Prompt.Instance.PromptMessage("Save Project", "Successfully saved Song!", "Okay");
        }

        /// <summary>
        /// Saves the currently open song.
        /// </summary>
        public static void SaveCurrentSong() {
            Song song = MusicManager.Instance.CurrentSong;

            // Check if song is null
            if (song == null) {
                Debug.LogError("SaveLoad.SaveCurrentSong(): tried to save null song!");
                return;
            }

            // Save song
            SaveSong(song);
        }

        #endregion
        #region Load Methods

        /// <summary>
        /// Loads the specified object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dirPath">Full path to load directory.</param>
        /// <param name="filePath">Full load path, including file name, extension, and directory.</param>
        public static T Load<T>(string filePath) where T : new() {

            // Check if file exists
            if (!File.Exists(filePath))
                throw new FailedToLoadException("SaveLoad.Load(): invalid path \"" + filePath + "\"");

            try {

                Debug.Log("SaveLoad.Load<" + typeof(T).ToString() + ">(): loading path \"" + filePath + "\".");

                // Init BinaryFormatter
                BinaryFormatter bf = new BinaryFormatter();

                // Open file
                FileStream file = File.Open(filePath, FileMode.Open);

                // Deserialize object
                T result = (T)bf.Deserialize(file);

                // Check if valid result
                if (result == null) throw new FailedToLoadException(
                    "SaveLoad.Load(): loaded file \"" + filePath + "\" was null.\n"
                    );

                // Close file
                file.Close();

                // Return object
                return result;

                // Catch SerializationExceptions
            }
            catch (SerializationException e) {
                throw new FailedToLoadException(
                    "SaveLoad.Load(): failed to deserialize file \"" + filePath + "\".\n" + e.Message
                );

                // Catch EndOfStreamExceptions
            }
            catch (EndOfStreamException e) {
                throw new FailedToLoadException(
                    "SaveLoad.Load(): hit unexpected end of stream in file \"" + filePath + "\".\n" + e.Message
                );
            }
        }

        /// <summary>
        /// Loads the project at the specified path.
        /// </summary>
        /// <param name="path">Project path.</param>
        public static void LoadProject(string path) {

            // Backup project and song
            Project backupProject = MusicManager.Instance.CurrentProject;
            Song backupSong = MusicManager.Instance.CurrentSong;

            try {

                // Load project
                Project project = Load<Project>(path);
                MusicManager.Instance.CurrentProject = project;

                // Load first song, if available
                if (!project.Empty)
                    MusicManager.Instance.CurrentSong = project.Songs[0];

                // Catch and print any FailedToLoadExceptions
            }
            catch (FailedToLoadException f) {

                // Restore backups
                MusicManager.Instance.CurrentProject = backupProject;
                MusicManager.Instance.CurrentSong = backupSong;

                throw f;
            }
        }

        /// <summary>
        /// Loads the specified song to the project.
        /// </summary>
        /// <param name="path">Song path.</param>
        public static void LoadSongToProject(string path) {

            // Load song
            Song song = LoadSong(path);

            // Add song to project
            MusicManager.Instance.CurrentProject.AddSong(song);

            // Set current song to song
            MusicManager.Instance.CurrentSong = song;
        }

        /// <summary>
        /// Loads a song.
        /// </summary>
        /// <param name="path"></param>
        public static Song LoadSong(string path) {

            try {

                // Load song
                Song result = Load<Song>(path);

                //Prompt.Instance.PromptMessage("Load Song", "Successfully loaded Song!", "Okay");

                return result;

                // Catch and print FailedToLoadExceptions
            }
            catch (FailedToLoadException e) {

                // Print exception
                Debug.LogError(e.Message);

                // Prompt
                Prompt.Instance.PromptMessage("Failed to load song",
                    "Failed to load song \"" + path + "\". It may be in an older format or corrupt.",
                    "Okay");

                return null;
            }
        }

        #endregion
    }
}
