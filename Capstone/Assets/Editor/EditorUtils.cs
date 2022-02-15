using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.BaseCommands;
using GameplayIngredients;
using UnityEngine;
using UnityEditor;

namespace Bladesmiths.Capstone.Editor
{
    public class EditorUtils
    {
        // Utility Methods
        public static string ConvertRelativePathToAbsolute(string relative)
        {
            return Application.dataPath + relative.Replace("Assets/", "/");
        }

        public static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            var guids = AssetDatabase.FindAssets("t:" +
                                                 typeof(T)
                                                     .Name); // FindAssets uses tags check documentation for more info
            T[] a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++) //probably could get optimized 
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return a;
        }

        public static bool GetGameLevelData(string fileName, string relativePath, ref GameLevel output)
        {
            // FindAssets uses tags check documentation for more info
            var guids = AssetDatabase.FindAssets(fileName + " t:" +
                                                 nameof(GameLevel), new[] { relativePath });
            if (guids.Length == 0)
                return false;

            GameLevel[] a = new GameLevel[guids.Length];
            for (int i = 0; i < guids.Length; i++) //probably could get optimized 
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<GameLevel>(path);
            }

            output = a[0];

            return true;
        }
    }

    public class EditorLogUtils
    {
        private enum Severity
        {
            Success,
            Info,
            Warning,
            Error
        }

        public static void Success(string msg)
        {
            Message(Severity.Success, ref msg);
        }
        
        public static void Info(string msg)
        {
            Message(Severity.Info, ref msg);
        }

        public static void Warning(string msg)
        {
            Message(Severity.Warning, ref msg);
        }

        public static void Error(string msg)
        {
            Message(Severity.Error, ref msg);
        }

        private static void Message(Severity severity, ref string msg)
        {
            string time = DateTime.Now.ToString("hh:mm:ss");
            
            string msgPrefix = "MSE";
            
            switch (severity)
            {
                case Severity.Success:
                    msgPrefix = (msgPrefix + " Success: ").Bold().Italic().Color("green");
                    break;
                case Severity.Info:
                    msgPrefix = (msgPrefix + " Info: ").Bold().Italic().Color("white");
                    break;
                case Severity.Warning:
                    msgPrefix = (msgPrefix + " Warning: ").Bold().Italic().Color("yellow");
                    break;
                case Severity.Error:
                    msgPrefix = (msgPrefix + " Error: ").Bold().Italic().Color("red");
                    break;
                default:
                    break;
            }
            Debug.LogFormat("[{0}] " + msgPrefix + msg, time);
        }
    }

    // This is the extension method.
    // The first parameter takes the "this" modifier
    // and specifies the type for which the method is defined.
    public static class StringUtils
    {
        public static string Bold(this string str) => "<b>" + str + "</b>";
        public static string Color(this string str, string clr) => string.Format("<color={0}>{1}</color>", clr, str);
        public static string Italic(this string str) => "<i>" + str + "</i>";
        public static string Size(this string str, int size) => string.Format("<size={0}>{1}</size>", size, str);
    }
    
    public static class Styles
    {
        public static GUIStyle buttonLeft;
        public static GUIStyle buttonMid;
        public static GUIStyle buttonRight;
        public static GUIStyle title;
        public static GUIStyle body;

        public static GUIStyle centeredTitle;
        public static GUIStyle centeredBody;
        public static GUIStyle helpBox;

        static Styles()
        {
            buttonLeft = new GUIStyle(EditorStyles.miniButtonLeft);
            buttonMid = new GUIStyle(EditorStyles.miniButtonMid);
            buttonRight = new GUIStyle(EditorStyles.miniButtonRight);
            buttonLeft.fontSize = 12;
            buttonMid.fontSize = 12;
            buttonRight.fontSize = 12;

            title = new GUIStyle(EditorStyles.label);
            title.fontSize = 22;

            centeredTitle = new GUIStyle(title);
            centeredTitle.alignment = TextAnchor.UpperCenter;

            body = new GUIStyle(EditorStyles.label);
            body.fontSize = 12;
            body.wordWrap = true;
            body.richText = true;

            centeredBody = new GUIStyle(body);
            centeredBody.alignment = TextAnchor.UpperCenter;

            helpBox = new GUIStyle(EditorStyles.helpBox);
            helpBox.padding = new RectOffset(12, 12, 12, 12);
        }
    }
    
    [InitializeOnLoadAttribute]
    public static class HierarchyMonitor
    {
        public static bool IsHierarchyDirty = false;
        
        static HierarchyMonitor()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        static void OnHierarchyChanged()
        {
            IsHierarchyDirty = true;
            // var all = Resources.FindObjectsOfTypeAll(typeof(GameObject));
            // var numberVisible =
            //     all.Where(obj => (obj.hideFlags & HideFlags.HideInHierarchy) != HideFlags.HideInHierarchy).Count();
            // Debug.LogFormat("There are currently {0} GameObjects visible in the hierarchy.", numberVisible);
        }
    }
}