#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Seven.SaveSystem.Settings.Editor
{
    internal sealed class LocalDataSettingsProvider : AssetSettingsProvider
    {
        public const string CONFIG_NAME = "com.seven.local-save-data.settings";
        private const string SETTINGS_OBJECT_PATH = "Assets/Plugins/Seven/LocalSaveData/Settings.asset";

        private string _searchContext;
        private VisualElement _rootElement;

        internal static LocalDataSettings CurrentSettings
        {
            get => GetOrCreateSettings();
            set
            {
                if (value == null)
                {
                    EditorBuildSettings.RemoveConfigObject(CONFIG_NAME);
                }
                else
                {
                    EditorBuildSettings.AddConfigObject(CONFIG_NAME, value, overwrite: true);
                }
            }
        }

        private LocalDataSettingsProvider() : base("Seven/Local Save Data", () => CurrentSettings)
        {
            CurrentSettings = FindSettings();
            keywords = GetSearchKeywordsFromGUIContentProperties<LocalDataSettings>();
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            _rootElement = rootElement;
            _searchContext = searchContext;
            base.OnActivate(searchContext, rootElement);
        }

        public override void OnGUI(string searchContext)
        {
            DrawCurrentSettingsGUI();
            EditorGUILayout.Space();
            base.OnGUI(searchContext);
        }

        private void DrawCurrentSettingsGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUI.indentLevel++;
            var settings = EditorGUILayout.ObjectField("Current Settings", CurrentSettings, typeof(LocalDataSettings),
                allowSceneObjects: false) as LocalDataSettings;
            if (settings)
            {
                DrawValidSettingsMessage();
            }

            EditorGUI.indentLevel--;

            bool newSettings = EditorGUI.EndChangeCheck();
            if (newSettings)
            {
                CurrentSettings = settings;
                RefreshEditor();
            }
        }

        private void RefreshEditor() => base.OnActivate(_searchContext, _rootElement);

        private void DrawValidSettingsMessage()
        {
            const string message =
                "The current Local Save Data Settings will be automatically included into any builds.";
            EditorGUILayout.HelpBox(message, MessageType.Info, wide: true);
        }

        private static LocalDataSettings FindSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<LocalDataSettings>(SETTINGS_OBJECT_PATH);
            if (settings != null) return settings;

            Directory.CreateDirectory(Path.GetDirectoryName(SETTINGS_OBJECT_PATH));
            settings = ScriptableObject.CreateInstance<LocalDataSettings>();
            AssetDatabase.CreateAsset(settings, SETTINGS_OBJECT_PATH);

            return settings;
        }

        internal static LocalDataSettings GetOrCreateSettings()
        {
            if (!EditorBuildSettings.TryGetConfigObject(CONFIG_NAME, out LocalDataSettings settings))
                settings = FindSettings();
            return settings;
        }

        [SettingsProvider]
        private static SettingsProvider CreateProjectSettingsMenu() => new LocalDataSettingsProvider();
    }
}
#endif