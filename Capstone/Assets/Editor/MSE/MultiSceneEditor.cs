using Sirenix.OdinInspector.Editor;

#if UNITY_EDITOR
namespace Bladesmiths.Capstone.Editor
{
    using UnityEngine;
    using UnityEditor;
    using Sirenix.Utilities.Editor;
    using Sirenix.Utilities;

    public partial class MultiSceneEditor : OdinMenuEditorWindow
    {
        // Fields
        const string kShowOnStartupPreference = "MSE.ShowAtStartup";
        const int WindowWidth = 800;
        const int WindowHeight = 600;

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
    }
}

#endif