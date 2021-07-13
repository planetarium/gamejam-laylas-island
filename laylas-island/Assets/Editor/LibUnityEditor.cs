using System.IO;
using LaylasIsland.Frontend.BlockChain;
using UnityEditor;
using UnityEngine;

namespace LaylasIsland.Editor
{
    public static class LaylasIslandEditor
    {
        [MenuItem("Tools/LaylasIsland/Delete Blockchain Store(dev) - Make Genesis Block For Dev To StreamingAssets Folder")]
        public static void DeleteAllEditorAndMakeGenesisBlock()
        {
            DeleteAll(StorePath.GetDefaultStoragePath(StorePath.Env.Development));
            MakeGenesisBlock(BlockManager.GenesisBlockPath);
        }

        [MenuItem("Tools/LaylasIsland/Delete Blockchain Store(prod) - Make Genesis Block For Prod To StreamingAssets Folder")]
        public static void DeleteAllPlayerAndMakeGenesisBlock()
        {
            DeleteAll(StorePath.GetDefaultStoragePath(StorePath.Env.Production));
            MakeGenesisBlock(BlockManager.GenesisBlockPath);
        }

        [MenuItem("Tools/LaylasIsland/Make Genesis Block")]
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
        
        [MenuItem("Tools/LaylasIsland/Delete All Of PlayerPrefs")]
        public static void DeleteAllOfPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        private static void DeleteAll(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                Debug.Log($"Blockchain Store deleted at {path}");
            }
        }

        private static void MakeGenesisBlock(string path)
        {
            var block = BlockManager.MineGenesisBlock();
            BlockManager.ExportBlock(block, path);
            Debug.Log($"GenesisBlock created at {path}");
        }
    }
}
