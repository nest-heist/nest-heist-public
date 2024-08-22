using System.Collections;
using System.IO;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class Capture : MonoBehaviour
{
    public Camera Cam;
    public RenderTexture RT;
    public Image BG;
    public string Id;
    void Start()
    {
        Cam = Camera.main;
    }

    public void Create()
    {
        StartCoroutine(CaptureImage());
    }

    IEnumerator CaptureImage()
    {
        yield return null;
        Texture2D tex = new Texture2D(RT.width, RT.height, TextureFormat.ARGB32, false, true);
        RenderTexture.active = RT;
        tex.ReadPixels(new Rect(0, 0, RT.width, RT.height), 0, 0);

        yield return null;
        var data = tex.EncodeToPNG();
        string name = "MON001";
        string extention = ".png";
        string path = Application.persistentDataPath + "/Thumbnail/";
        Debug.Log(path);

        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        File.WriteAllBytes(path + name + Id + extention, data);
        yield return null;
    }
}
