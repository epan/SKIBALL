using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

namespace BrokenVector.TreeRandomizer
{
    public class TreeRandomizerWindow : EditorWindow
    {

        private const string ROOT_PROPERTY = "root.seed";
        private const string BARK_MATERIAL = "optimizedSolidMaterial";
        private const string LEAF_MATERIAL = "optimizedCutoutMaterial";
        private const string TREE_UPDATE_METHOD = "UpdateMesh";
        private const int MAX_SEED = 9999998;

        private Tree treeTemplate = null;
        private int treeCount = 5;

        [MenuItem(Constants.WINDOW_PATH), MenuItem(Constants.WINDOW_PATH_ALT)]
        private static void ShowWindow()
        {
            var window = GetWindow(typeof(TreeRandomizerWindow));
            window.titleContent = new GUIContent(Constants.ASSET_NAME);
            window.Show();
        }

        private void OnGUI()
        {
            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("You can not generate trees while in playmode!", MessageType.Info);
                return;
            }

            treeTemplate = EditorGUILayout.ObjectField("Tree Template", treeTemplate, typeof(Tree), true) as Tree;
            treeCount = EditorGUILayout.IntSlider("Count", treeCount, 1, Constants.SLIDER_TREE_COUNT_MAX);

            GUILayout.Space(20);

            if (treeTemplate == null)
            {
                EditorGUILayout.HelpBox("Drag a tree from your project window to the field above.", MessageType.Warning);
            }
            else
            {
                if (GUILayout.Button("Generate Trees"))
                {
                    Generate(treeTemplate, treeCount);
                }
            }
        }

        private void Generate(Tree treeTemplate, int treeCount)
        {
            Debug.Log("Starting generation of " + treeCount + " trees.");
            Progress(0);

            if (!AssetDatabase.Contains(treeTemplate))
            {
                Debug.LogError("The tree you want to duplicate has to be a prefab and must not be a scene object.", treeTemplate);
                return;
            }

            var treeobj = new SerializedObject(treeTemplate.data);
            var barkMat = FindMaterial(treeobj, BARK_MATERIAL);
            var leafMat = FindMaterial(treeobj, LEAF_MATERIAL);
            if (barkMat == null)
            {
                Debug.LogError("Bark material not found.");
                return;
            }
            if (leafMat == null)
            {
                Debug.LogError("Leaf material not found.");
                return;
            }

            string path = AssetDatabase.GetAssetPath(treeTemplate);
            string dir = Path.GetDirectoryName(path);
            string filename = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);

            string copyFolder = dir + Path.DirectorySeparatorChar + Constants.OUTPUT_FOLDER;
            string copyFolderCotent = copyFolder + Path.DirectorySeparatorChar;
            string copyFile = filename + " {0}" + ext;

            if (!AssetDatabase.IsValidFolder(copyFolder))
                AssetDatabase.CreateFolder(dir, Constants.OUTPUT_FOLDER);

            List<Tree> generatedTrees = new List<Tree>();
            for (int i = 0; i < treeCount; i++)
            {
                string copyPath = copyFolderCotent + String.Format(copyFile, i);

                if (!AssetDatabase.CopyAsset(path, copyPath))
                {
                    Debug.LogError("Couldn't copy the tree from " + path + " to " + copyPath);
                    return;
                }
                AssetDatabase.Refresh();
                AssetDatabase.ImportAsset(copyPath);

                Tree tree = AssetDatabase.LoadAssetAtPath(copyPath, typeof(Tree)) as Tree;
                if (tree == null)
                {
                    Debug.LogError("Couldn't load tree.");
                    return;
                }

                foreach (Material mat in tree.GetComponent<MeshRenderer>().sharedMaterials)
                {
                    DestroyImmediate(mat, true);
                }

                AssetDatabase.SaveAssets();
                tree.GetComponent<MeshRenderer>().sharedMaterials = new[] { barkMat, leafMat };

                var obj = new SerializedObject(tree.data);

                int randomSeed = Random.Range(0, MAX_SEED);
                obj.FindProperty(ROOT_PROPERTY).intValue = randomSeed;
                obj.FindProperty(BARK_MATERIAL).objectReferenceValue = barkMat;
                obj.FindProperty(LEAF_MATERIAL).objectReferenceValue = leafMat;
                obj.ApplyModifiedProperties();

                UpdateMesh(tree);

                AssetDatabase.DeleteAsset(copyFolderCotent + filename + " " + i + "_Textures");
                Reimport(tree);

                generatedTrees.Add(tree);
                Progress((float) i / treeCount);
            }

            Progress(1);
            Debug.Log("Generated " + treeCount + " Trees!");
            
            // highlight first generated tree
            Selection.activeObject = generatedTrees[0];
        }

        private void Progress(float percent)
        {
            EditorUtility.DisplayProgressBar(Constants.ASSET_NAME, "Generating Trees...", percent);

            if (percent >= 1.0f)
                EditorUtility.ClearProgressBar();
        }

        private Material FindMaterial(SerializedObject obj, string materialName)
        {
            return obj.FindProperty(materialName).objectReferenceValue as Material;
        }

        private void UpdateMesh(Tree tree)
        {
            MethodInfo method = tree.data.GetType().GetMethod(TREE_UPDATE_METHOD, new[] { typeof(Matrix4x4), typeof(Material[]).MakeByRefType() });
            object[] arguments = { tree.transform.worldToLocalMatrix, null };
            method.Invoke(tree.data, arguments);
        }

        private void Reimport(UnityEngine.Object obj)
        {
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(obj));
        }

    }
}