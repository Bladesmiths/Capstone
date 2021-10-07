using System;
using Bladesmiths.Capstone.ScriptableObjects;
using Sirenix.OdinInspector;
using UnityEditor.SceneManagement;

#if UNITY_EDITOR
namespace Bladesmiths.Capstone.Editor
{
    using Sirenix.OdinInspector.Editor;
    using System.Linq;
    using UnityEngine;
    using Sirenix.Utilities.Editor;
    using Sirenix.Serialization;
    using UnityEditor;
    using Sirenix.Utilities;

    public class MultiSceneEditor : OdinMenuEditorWindow
    {
        [MenuItem("Tools/Bladesmiths/MSE")]
        private static void OpenWindow()
        {
            var window = GetWindow<MultiSceneEditor>();

            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        }

        [SerializeField] private GeneralSettings settings = new GeneralSettings();

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(supportsMultiSelect: true)
            {
                { "General Settings", this.settings, EditorIcons.SettingsCog },
                { "Multi-Scene Setup Data", null, EditorIcons.List },
            };

            var customMenuStyle = new OdinMenuStyle
            {
                BorderPadding = 0f,
                AlignTriangleLeft = true,
                TriangleSize = 16f,
                TrianglePadding = 0f,
                Offset = 20f,
                Height = 23,
                IconPadding = 0f,
                BorderAlpha = 0.323f
            };

            tree.DefaultMenuStyle = customMenuStyle;

            // Search tool bar
            tree.Config.DrawSearchToolbar = true;

            // Menu Item #2 - Loaded Multi-scene Setup Data
            //tree.MenuItems.Insert(2, new MSEGeneralMenuItem(tree, new SomeCustomClass()));

            tree.AddAllAssetsAtPath("Multi-Scene Setup Data", this.settings.pathToMSSetupData,
                    typeof(MultiSceneSetupData),
                    true, true)
                .AddThumbnailIcons();

            return tree;
        }


        protected override void OnBeginDrawEditors()
        {
            var selected = this.MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

            // Draws a toolbar with the name of the currently selected menu item.
            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null)
                {
                    GUILayout.Label(selected.Name);
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Save Current Scene Setup")))
                {
                }

                if (selected is { Name: "General Settings" })
                {
                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("Reload Setup Data List")))
                    {
                        base.ForceMenuTreeRebuild();
                    }
                }

                if (selected is { Parent: { FlatTreeIndex: 1 } })
                {
                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("Load Current Setup in Hierarchy")))
                    {
                        var selectedObj = selected.Value;
                        if (selectedObj.GetType() == typeof(MultiSceneSetupData))
                        {
                            var data = (MultiSceneSetupData)selectedObj;
                            EditorSceneManager.RestoreSceneManagerSetup(data.GetSetup());
                            Debug.Log("<color=green>Success: </color>Scene setup is successfully loaded!");
                        }
                    }
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }

    [Serializable]
    public class GeneralSettings
    {
        [ShowInInspector]
        [FolderPath(RequireExistingPath = true)]
        public string pathToMSSetupData
        {
            // Use EditorPrefs to hold persistent user-variables.
            get => EditorPrefs.GetString("MSE.MSSetupDataFolderPath", "Assets/Game/Scenes/");
            set => EditorPrefs.SetString("MSE.MSSetupDataFolderPath", value);
        }
    }
}

#endif