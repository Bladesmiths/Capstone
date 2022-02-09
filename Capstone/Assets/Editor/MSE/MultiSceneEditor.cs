using UnityEditor.Graphs;

#if UNITY_EDITOR
namespace Bladesmiths.Capstone.Editor
{
    using System;
    using Bladesmiths.Capstone.ScriptableObjects;
    using Sirenix.OdinInspector;
    using UnityEditor.SceneManagement;
    using Sirenix.OdinInspector.Editor;
    using System.IO;
    using System.Linq;
    using UnityEngine;
    using Sirenix.Utilities.Editor;
    using Sirenix.Serialization;
    using UnityEditor;
    using Sirenix.Utilities;

    public class MultiSceneEditor : OdinMenuEditorWindow
    {
        // Fields
        const string kShowOnStartupPreference = "MSE.ShowAtStartup";
        const int WindowWidth = 800;
        const int WindowHeight = 600;

        [SerializeField] private AboutPage aboutPage = new AboutPage();
        [SerializeField] private LoadSaveSettings settings = new LoadSaveSettings();

        // Properties
        static bool showOnStartup
        {
            get { return EditorPrefs.GetBool(kShowOnStartupPreference, true); }
            set
            {
                if (value != showOnStartup) EditorPrefs.SetBool(kShowOnStartupPreference, value);
            }
        }

        // Static Methods
        public static void Reload()
        {
            EditorApplication.update -= ShowAtStartup;
            InitShowAtStartup();
        }

        [InitializeOnLoadMethod]
        static void InitShowAtStartup()
        {
            if (showOnStartup)
                EditorApplication.update += ShowAtStartup;

            EditorApplication.quitting += EditorApplication_quitting;
        }

        const string kShowNextPreference = "MSE.ShowNextTime";

        private static void EditorApplication_quitting()
        {
            EditorPrefs.SetBool(kShowNextPreference, true);
        }

        static void ShowAtStartup()
        {
            if (!Application.isPlaying && EditorPrefs.GetBool(kShowNextPreference, true))
            {
                OpenWindow();
                EditorPrefs.SetBool(kShowNextPreference, false);
            }

            EditorApplication.update -= ShowAtStartup;
        }

        // Editor Window Methods
        [MenuItem("Tools/MSE")]
        private static void OpenWindow()
        {
            var window = GetWindow<MultiSceneEditor>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(WindowWidth, WindowHeight);
            window.titleContent = new GUIContent("Multi-Scene Editor (MSE)", EditorIcons.PacmanGhost.Raw);
        }

        private void OnDestroy()
        {
            EditorApplication.update -= ShowAtStartup;
        }


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
                Height = 23,
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

    public class AboutPage
    {
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