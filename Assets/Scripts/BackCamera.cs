using UnityEngine;

public class BackCamera : MonoBehaviour
{
    public int resWidth = 1080; 
    public int resHeight = 1398;
    private static string prefix;
    private static bool takePic = false;
    [SerializeField]
    private Camera backCamera;
 
    public static string ScreenShotName(int width, int height) {
        return string.Format("{0}/Screenshots/{1}_{2}.png", 
            Application.streamingAssetsPath, 
            prefix, 
            System.DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss"));
    }
 
    public static void TakePic(string name)
    {
        prefix = name;
        takePic = true;
    }
    

    void Update() {
        if (takePic) {
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            backCamera.targetTexture= rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            backCamera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            backCamera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(resWidth, resHeight);
            System.IO.File.WriteAllBytes(filename, bytes);
            takePic = false;
        }
    }

}