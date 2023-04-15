using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ML_Agents.PF.NN_Models.Editor
{
    //Note: this script will not order correctly the NN models IF the first model name is "PF" without the number extension ("PF 1")
    public class NNModelsTool : MonoBehaviour
    {
        private const string ROOT_PATH = "Assets/ML-Agents/PF/NN_Models";
        private const string ADVANCED_NN_NAME = "_Advanced";
        private const string SIMPLE_NN_NAME = "_Simple";

        private const string FILE_TAG = "*.onnx";

        [MenuItem("RL/Rename NN-Models")]
        private static void RenameModels()
        {
            Debug.Log("Starting Renaming...");
            RenameAdvancedNnModels();
            RenameSimpleNnModels();
        }

        private static void RenameAdvancedNnModels()
        {
            var advancedPath = $"{ROOT_PATH}/{ADVANCED_NN_NAME}";
            var subFolders = AssetDatabase.GetSubFolders(advancedPath);
            RenameAssets(subFolders, ADVANCED_NN_NAME);
        }

        private static void RenameSimpleNnModels()
        {
            var path = $"{ROOT_PATH}/{SIMPLE_NN_NAME}";
            var subFolders = AssetDatabase.GetSubFolders(path);
            RenameAssets(subFolders, SIMPLE_NN_NAME);
        }

        private static void RenameAssets(string[] subFolders, string typePath)
        {
            var counter = 0;

            foreach (var folder in subFolders)
            {
                var files = Directory.GetFiles(folder, FILE_TAG, SearchOption.TopDirectoryOnly);
                var reversedFiles = files.Reverse().ToArray();

                if (!reversedFiles.Any()) return;

                var folderName = subFolders[counter].Substring(subFolders[0].Length - 2);
                counter++;
                // Debug.Log($"Folder : {folderName}");

                for (var i = 0; i < reversedFiles.Length; i++)
                {
                    var newName = $"{folderName}_{i}";
                    var lc = reversedFiles[i].ToLower();

                    if (lc.Contains("pf.onnx"))
                    {
                        AssetDatabase.RenameAsset(reversedFiles[i], $"_{newName}");
                        AssetDatabase.DeleteAsset($"{ROOT_PATH}/{typePath}/_{folderName}/PF.onnx");
                        continue;
                    }

                    if (reversedFiles[i].Contains(folderName)) continue;
                    if (string.Equals(reversedFiles[i], newName, StringComparison.CurrentCultureIgnoreCase)) continue;

                    AssetDatabase.RenameAsset(reversedFiles[i], $"_{newName}");

                }

            }
        }

    }
}
