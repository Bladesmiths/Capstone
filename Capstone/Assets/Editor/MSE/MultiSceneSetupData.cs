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
            _activeSceneCount = 0;

            return setupList.Select(ConvertToSceneSetup).ToArray();
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
            };

            if (data.isActive)
                _activeSceneCount++;
            ss.isActive = _activeSceneCount <= 1 && data.isActive;

            if (_activeSceneCount == 2 && data.isActive)
                Debug.Log(
                    "<color=yellow>Warning: </color>Only 1 active scene is allowed! The 1st active scene in the setup list will be loaded in default.");

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