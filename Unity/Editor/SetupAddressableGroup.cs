using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

public static class SetupAddressableGroup
{
    const string SourceFolder = "Assets/ClosingBattle";
    const string GroupName = "ClosingBattle";

    [MenuItem("Setup/Create ClosingBattle Addressable Group")]
    public static void AddFolderToAddressablesMenu()
    {
        AddFolderToAddressables(SourceFolder, GroupName);
    }

    public static void AddFolderToAddressables(string folderPath, string groupName)
    {
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogError($"Folder not found: {folderPath}");
            return;
        }

        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("AddressableAssetSettings not found.");
            return;
        }

        // Find or create group
        var group = settings.FindGroup(groupName);
        if (group == null)
        {
            group = settings.CreateGroup(groupName, false, false, false,
                null,
                typeof(BundledAssetGroupSchema),
                typeof(ContentUpdateGroupSchema));
        }

        var bundleSchema = group.GetSchema<BundledAssetGroupSchema>();
        if (bundleSchema != null)
        {
            bundleSchema.InternalIdNamingMode = BundledAssetGroupSchema.AssetNamingMode.FullPath;
            bundleSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
            bundleSchema.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZ4;
        }

        var updateSchema = group.GetSchema<ContentUpdateGroupSchema>();
        if (updateSchema != null)
        {
            updateSchema.StaticContent = false;
        }

        // mark dirty
        settings.SetDirty(AddressableAssetSettings.ModificationEvent.GroupSchemaModified, group, true);
        AssetDatabase.SaveAssets();

        // Find all assets under folder (recursively)
        var guids = AssetDatabase.FindAssets("", new[] { folderPath });
        int added = 0;

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);

            // skip folders
            if (AssetDatabase.IsValidFolder(path))
                continue;

            // Create or move entry into the target group
            var entry = settings.CreateOrMoveEntry(guid, group);
            entry.address = path.Replace("\\", "/");

            added++;
        }

        // mark settings dirty and save
        settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true);
        AssetDatabase.SaveAssets();

        Debug.Log($"Addressables: added {added} asset(s) from '{folderPath}' to group '{groupName}'.");
        EditorUtility.DisplayDialog("Added ", GroupName + " as an AddressableGroup successfully.", "Ok");
    }
}
