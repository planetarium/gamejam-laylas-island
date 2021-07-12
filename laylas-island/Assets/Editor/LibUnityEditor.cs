using System.IO;
using LibUnity.Frontend.BlockChain;
using UnityEditor;
using UnityEngine;

namespace LibUnity.Editor
{
    public static class LibUnityEditor
    {
        [MenuItem("Tools/LibUnity/Delete All(Editor) - Make Genesis Block For Dev To StreamingAssets Folder")]
        public static void DeleteAllEditorAndMakeGenesisBlock()
        {
            DeleteAll(StorePath.GetDefaultStoragePath(StorePath.Env.Development));
            MakeGenesisBlock(BlockManager.GenesisBlockPath);
        }

        [MenuItem("Tools/LibUnity/Delete All(Player) - Make Genesis Block For Prod To StreamingAssets Folder")]
        public static void DeleteAllPlayerAndMakeGenesisBlock()
        {
            DeleteAll(StorePath.GetDefaultStoragePath(StorePath.Env.Production));
            MakeGenesisBlock(BlockManager.GenesisBlockPath);
        }

        [MenuItem("Tools/LibUnity/Make Genesis Block")]
        public static void MakeGenesisBlock()
        {
            var path = EditorUtility.SaveFilePanel(
                "Choose path to export the new genesis block",
                Application.streamingAssetsPath,
                BlockManager.GenesisBlockName,
                ""
            );

            if (path == "")
            {
                return;
            }

            MakeGenesisBlock(path);
        }
        
        [MenuItem("Tools/Delete All Of PlayerPrefs")]
        public static void DeleteAllOfPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        private static void DeleteAll(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }

        private static void MakeGenesisBlock(string path)
        {
            var block = BlockManager.MineGenesisBlock();
            BlockManager.ExportBlock(block, path);
        }
    }
}
