using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Seven.SaveSystem.Settings.Editor
{
    public class LocalDataSettingsBuildPlayer : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();

            LocalDataSettings currentSettings = LocalDataSettingsProvider.CurrentSettings;
            Type currentSettingsType = currentSettings.GetType();
            preloadedAssets.RemoveAll(asset => asset != null && asset.GetType() == currentSettingsType);
            preloadedAssets.Add(currentSettings);

            PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
        }
    }
}