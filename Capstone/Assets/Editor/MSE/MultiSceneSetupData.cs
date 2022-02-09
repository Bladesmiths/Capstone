using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bladesmiths.Capstone.Editor;
using GameplayIngredients;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using FilePath = Sirenix.OdinInspector.FilePathAttribute;

namespace Bladesmiths.Capstone.ScriptableObjects
{
    [CreateAssetMenu(fileName = "MultiSceneSetupData", menuName = "ScriptableObjects/MSE/Multi-scene Setup Data")]
    public class MultiSceneSetupData : ScriptableObject
    {
        // Fields
        [TableList(ShowIndexLabels = true)] public List<SceneSetupData> setupList;

        [ReadOnly] [LabelText("Corresponding GameLevel Data File")]
        public GameLevel gameLevelData;

        private static uint _activeSceneCount = 0;
        private ScriptableObject obj;

        // Unity Public Methods
        public void OnEnable()
        {
            CreateGameLevelData();
        }

        public void OnValidate()
        {
            if (!gameLevelData)
            {
                CreateGameLevelData();
            }

            // Update StartupScenes array
            gameLevelData.StartupScenes = (from SceneSetupData ssd in setupList
                select Path.GetFileNameWithoutExtension(ssd.scenePath)).ToArray();
        }

        public void OnDestroy()
        {
            UnityEngine.Object.DestroyImmediate(obj);
            EditorLogUtils.Info(name +
                                " has been destroyed along with its corresponding GameLevel data!");
        }

        // Public Methods (Buttons)
        [TitleGroup("Actions")]
        [ButtonGroup("Actions/Buttons")]
        public void LoadCurrentSetup()
        {
            if (ValidateSetupData())
            {
                EditorSceneManager.RestoreSceneManagerSetup(GetSetup());
                EditorLogUtils.Success("Scene setup is successfully loaded!");
            }
        }
        
        [ButtonGroup("Actions/Buttons")]
        public void AddAllScenesToBuildSettings()
        {
            // Find valid Scene paths and make a list of EditorBuildSettingsScene
            var editorBuildSettingsScenes = EditorBuildSettings.scenes.ToList();
            foreach (var ssd in setupList)
            {
                if (!File.Exists(EditorUtils.ConvertRelativePathToAbsolute(ssd.scenePath)))
                {
                    EditorLogUtils.Info(EditorUtils.ConvertRelativePathToAbsolute(ssd.scenePath));
                    EditorLogUtils.Error("Scene path not valid.");
                    return;
                }
                
                SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(ssd.scenePath);;
                
                string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                if (!string.IsNullOrEmpty(scenePath))
                    editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));
            }

            // Set the Build Settings window Scene list
            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
            
            EditorLogUtils.Success("All scenes have been added to the Build Settings!");
        }


        // Public Methods
        public SceneSetup[] GetSetup()
        {
            return setupList.Select(ConvertToSceneSetup).ToArray();
        }

        private bool ValidateSetupData()
        {
            _activeSceneCount = 0;

            foreach (var data in setupList)
            {
                if (data.scenePath == string.Empty)
                {
                    EditorLogUtils.Error("Scene path cannot be empty.");
                    return false;
                }

                if (data.isActive)
                {
                    if (!data.isLoaded)
                    {
                        EditorLogUtils.Error("The active scene must be loaded.");
                        return false;
                    }

                    _activeSceneCount++;
                }
                
                if (!File.Exists(EditorUtils.ConvertRelativePathToAbsolute(data.scenePath)))
                {
                    EditorLogUtils.Info(EditorUtils.ConvertRelativePathToAbsolute(data.scenePath));
                    EditorLogUtils.Error("Scene path not valid.");
                    return false;
                }
            }

            if (_activeSceneCount != 1)
            {
                EditorLogUtils.Error("There has to be 1 active scene.");
                return false;
            }

            return true;
        }

        #region Odin Utility Methods

        private void BeginDrawListElement(int index)
        {
            //SirenixEditorGUI.BeginBox(this.setup[index].path);
        }

        private void EndDrawListElement(int index)
        {
            SirenixEditorGUI.EndBox();
        }

        #endregion


        #region Misc Utility Methods

        private static SceneSetup ConvertToSceneSetup(SceneSetupData data)
        {
            var ss = new SceneSetup
            {
                path = data.scenePath,
                isLoaded = data.isLoaded,
                isActive = data.isActive
            };

            return ss;
        }

        private void CreateGameLevelData()
        {
            // Use this to update corresponding GameLevel object
            string gameLevelDataPath = EditorUtils.ConvertRelativePathToAbsolute("Assets/Levels/GameLevelData/") +
                                       this.name + ".asset";

            if (!EditorUtils.GetGameLevelData(this.name, "Assets/Levels/GameLevelData", ref gameLevelData))
            {
                // Create the GameLevel file
                obj = ScriptableObject.CreateInstance(typeof(GameLevel));

                if (!string.IsNullOrEmpty(gameLevelDataPath) &&
                    PathUtilities.TryMakeRelative(Path.GetDirectoryName(Application.dataPath), gameLevelDataPath,
                        out gameLevelDataPath))
                {
                    AssetDatabase.CreateAsset(obj, gameLevelDataPath);
                    AssetDatabase.Refresh();
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(obj);
                }

                EditorUtils.GetGameLevelData(this.name, "Assets/Levels/GameLevelData", ref gameLevelData);
            }
        }

        #endregion
    }

    [Serializable]
    public class SceneSetupData
    {
        [TableColumnWidth(200)] [FilePath(Extensions = "unity", RequireExistingPath = true)]
        public string scenePath;

        [TableColumnWidth(70, Resizable = false)]
        public bool isActive;

        [TableColumnWidth(70, Resizable = false)]
        public bool isLoaded;
    }
}