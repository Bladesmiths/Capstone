#if UNITY_EDITOR

namespace Bladesmiths.Capstone.Editor
{
    using System;
    using System.Linq;
    using System.IO;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using UnityEngine;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using Sirenix.Utilities;

    public partial class MultiSceneEditor
    {
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(supportsMultiSelect: true)
            {
                { "About", null, EditorIcons.Info },
                { "Load & Save", this.settings, EditorIcons.SettingsCog },
                { "Multi-Scene Setup Data", null, EditorIcons.List },
            };

            var customMenuStyle = new OdinMenuStyle
            {
                BorderPadding = 0f,
                AlignTriangleLeft = true,
                TriangleSize = 16f,
                TrianglePadding = 0f,
                Offset = 20f,
                Height = 25,
                IconPadding = 0f,
                BorderAlpha = 0.323f
            };

            tree.DefaultMenuStyle = customMenuStyle;

            // Search tool bar
            tree.Config.DrawSearchToolbar = true;

            // Menu Item #2 - Loaded Multi-scene Setup Data
            //tree.MenuItems.Insert(2, new MSEGeneralMenuItem(tree, new SomeCustomClass()));

            tree.AddAllAssetsAtPath("Multi-Scene Setup Data", this.settings.pathToLoad,
                    typeof(MultiSceneSetupData),
                    true, false)
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

                if (selected is { FlatTreeIndex: 0 })
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        showOnStartup = GUILayout.Toggle(showOnStartup, " Show MSE on Startup");
                    }
                }

                if (selected is { FlatTreeIndex: 1 })
                {
                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("Reload Setup Data List")))
                    {
                        base.ForceMenuTreeRebuild();
                    }
                }

                if (selected is { Parent: { FlatTreeIndex: 2 } })
                {
                    if (SirenixEditorGUI.ToolbarButton((new GUIContent("Show Build Settings"))))
                    {
                        EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
                    }
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();

            if (selected is { FlatTreeIndex: 0 })
            {
                using (new GUILayout.VerticalScope())
                {
                    GUILayout.Label("Multi-Scene Editor", Styles.centeredTitle);
                    GUILayout.Label(@"(or MSE)

The editor allows developers to manipulate Unity hierarchy setup with multiple scenes.
The purposes are to allow to create large streaming worlds and to improve the workflow when collaborating on scene editing.

<b>This editor makes use of the following third party component(s):</b>
- <i>Odin Inspector</i> by Sirenix (https://odininspector.com/) 
", Styles.centeredBody);

                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(@"Copyright @ 2022 Bladesmiths", Styles.centeredBody);

                        GUILayout.FlexibleSpace();
                    }

                    GUILayout.FlexibleSpace();
                }
            }
        }
    }

    [Serializable]
    public class LoadSaveSettings
    {
        [ShowInInspector]
        [FolderPath(RequireExistingPath = true)]
        [TitleGroup("Folder Paths", "$folderPathsSubtitle", horizontalLine: true)]
        [BoxGroup("Folder Paths/Multi-Scene Setup Data")]
        public string pathToLoad
        {
            // Use EditorPrefs to hold persistent user-variables.
            get => EditorPrefs.GetString("MSE.LoadPath", "Assets/Game/Scenes/");
            set => EditorPrefs.SetString("MSE.LoadPath", value);
        }

        [ShowInInspector]
        [FolderPath(RequireExistingPath = true)]
        [TitleGroup("Folder Paths")]
        [BoxGroup("Folder Paths/Multi-Scene Setup Data")]
        public string PathToSave
        {
            // Use EditorPrefs to hold persistent user-variables.
            get => EditorPrefs.GetString("MSE.SavePath", "Assets/Game/Scenes/");
            set => EditorPrefs.SetString("MSE.SavePath", value);
        }


        // Actions Section
        [TitleGroup("Actions")]
        [ButtonGroup("Actions/Buttons")]
        public void CreateNewMultiSceneSetupData()
        {
            var obj = ScriptableObject.CreateInstance(typeof(MultiSceneSetupData));

            string dest = EditorUtils.ConvertRelativePathToAbsolute(PathToSave).TrimEnd('/');

            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
                AssetDatabase.Refresh();
            }

            dest = EditorUtility.SaveFilePanel("Save object as", dest,
                "New " + typeof(MultiSceneSetupData).GetNiceName(), "asset");

            if (!string.IsNullOrEmpty(dest) &&
                PathUtilities.TryMakeRelative(Path.GetDirectoryName(Application.dataPath), dest, out dest))
            {
                AssetDatabase.CreateAsset(obj, dest);
                AssetDatabase.Refresh();
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(obj);
            }
        }

        private string folderPathsSubtitle = "The folder paths are persistent values, even across Unity projects.";
    }
}
#endif