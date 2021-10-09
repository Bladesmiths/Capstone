using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
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

        private static uint _activeSceneCount = 0;

        // Public Methods
        public SceneSetup[] GetSetup()
        {
            return setupList.Select(ConvertToSceneSetup).ToArray();
        }

        public bool ValidateSetupData()
        {
            _activeSceneCount = 0;

            foreach (var data in setupList)
            {
                if (data.scenePath == string.Empty)
                {
                    Debug.Log("<color=red>Error: </color>Scene path not valid.");
                    return false;
                }

                if (data.isActive)
                {
                    if (!data.isLoaded)
                    {
                        Debug.Log("<color=red>Error: </color>The active scene must be loaded.");
                        return false;
                    }

                    _activeSceneCount++;
                }
            }

            if (_activeSceneCount != 1)
            {
                Debug.Log("<color=red>Error: </color>There has to be 1 active scene.");
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