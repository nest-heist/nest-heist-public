using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
public class SceneTreeViewItem : TreeViewItem
{
    public string scenePath;

    public SceneTreeViewItem(int id, int depth, string displayName, string scenePath) : base(id, depth, displayName)
    {
        this.scenePath = scenePath;
    }
}

public class SceneTreeView : TreeView
{
    private const float RowHeight = 25f; // Adjusted row height

    public SceneTreeView(TreeViewState state) : base(state)
    {
        Reload();
    }

    protected override float GetCustomRowHeight(int row, TreeViewItem item)
    {
        return RowHeight;
    }

    protected override TreeViewItem BuildRoot()
    {
        var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };

        var allScenes = new List<TreeViewItem>();
        string scenesDirectory = "Assets/00.Develop";
        SearchScenes(scenesDirectory, 0, allScenes);

        SetupParentsAndChildrenFromDepths(root, allScenes);

        return root;
    }

    private void SearchScenes(string directory, int depth, List<TreeViewItem> items)
    {
        var directories = Directory.GetDirectories(directory);
        var files = Directory.GetFiles(directory, "*.unity");

        var directoryItems = new List<TreeViewItem>();

        foreach (var dir in directories)
        {
            string dirName = Path.GetFileName(dir);
            var dirItem = new SceneTreeViewItem(items.Count + 1, depth, dirName, null);
            var childItems = new List<TreeViewItem>();
            SearchScenes(dir, depth + 1, childItems);

            if (childItems.Count > 0)
            {
                directoryItems.Add(dirItem);
                directoryItems.AddRange(childItems);
            }
        }

        foreach (var file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            var item = new SceneTreeViewItem(items.Count + 1, depth, fileName, file);
            directoryItems.Add(item);
        }

        if (directoryItems.Count > 0)
        {
            items.AddRange(directoryItems);
        }
    }

    protected override void RowGUI(RowGUIArgs args)
    {
        var item = (SceneTreeViewItem)args.item;
        base.RowGUI(args);

        if (!string.IsNullOrEmpty(item.scenePath))
        {
            Rect buttonRect = new Rect(args.rowRect.xMax - 50, args.rowRect.y + 2, 50, args.rowRect.height - 4);
            if (GUI.Button(buttonRect, "Move"))
            {
                if (EditorApplication.isPlaying)
                {
                    SceneManager.LoadScene(Path.GetFileNameWithoutExtension(item.scenePath));
                }
                else
                {
                    EditorSceneManager.OpenScene(item.scenePath);
                }
            }
        }
    }

    protected override void SelectionChanged(IList<int> selectedIds)
    {
        // Override to disable selection
    }
}


public class SceneLoaderEditor : EditorWindow
{
    private TreeViewState treeViewState;
    private SceneTreeView treeView;

    [MenuItem("Tools/Scene Loader")]
    public static void ShowWindow()
    {
        GetWindow<SceneLoaderEditor>("Scene Loader");
    }

    private void OnEnable()
    {
        if (treeViewState == null)
            treeViewState = new TreeViewState();

        treeView = new SceneTreeView(treeViewState);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Refresh Scenes"))
        {
            treeView.Reload();
        }

        treeView.OnGUI(new Rect(0, 30, position.width, position.height - 30));
    }
}
