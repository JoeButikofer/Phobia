using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

public class VideoRecorder : MonoBehaviour {

    public float recordIntervalle = 0.5f; // capture the screen twice per second

    public Session session;

    public bool UseCameraResolution = true;

    private float timer;

    private bool isStarted;

    public int resWidth = 800;
    public int resHeight = 600;

    private Camera cam;
    private Texture2D tex;
    private RenderTexture rt;

    // Use this for initialization
    void Awake () {

        timer = 0;
        isStarted = false;
    }

    public void StartRecord()
    {
        cam = Camera.main;

        if(UseCameraResolution)
        {
            resWidth = cam.pixelWidth;
            resHeight = cam.pixelHeight;
        }

        tex = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        rt = new RenderTexture(resWidth, resHeight, 24);

        isStarted = true;
    }

    public void EndRecord()
    {
        rt.Release();
        isStarted = false;
    }

    void LateUpdate()
    {
        if (isStarted)
        {
            if (session != null)
            {
                if (timer <= 0)
                {
                    timer = recordIntervalle;

                    ReadTexture();
                    // Application.CaptureScreenshot(path + ".png"); // The simple way directly implemented by Unity but not efficient

                    // reset active camera texture and render texture
                    cam.targetTexture = null;
                    RenderTexture.active = null;

                    // We must wait a bit before writing the pixels
                    StartCoroutine(WaitNFrames(2, () => WriteOnDisk()));

                }

                timer -= Time.unscaledDeltaTime;
            }
        }
    }

    IEnumerator WaitNFrames(int nFrame, Action action)
    {
        for (int i = 0; i > nFrame; i++)
            yield return new WaitForEndOfFrame();

        action.Invoke();
    }

    private void ReadTexture()
    {
        // Render to RenderTexture
        cam.targetTexture = rt;
        cam.Render();

        // Read pixels to texture
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
    }

    private void WriteOnDisk()
    {
        string path = session.GetAbsoluteVideoPath();

        // Read texture to array
        UnityEngine.Color32[] framebuffer = tex.GetPixels32();

        new Thread(() => {

            System.IO.Directory.CreateDirectory(path);
            long timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            path = Path.Combine(path, timestamp.ToString());

            //Create and fill the image
            Bitmap bmp = new Bitmap(resWidth, resHeight);

            for(int i = 0; i < resWidth;i++)
            {
                for (int j = 0; j < resHeight; j++)
                {
                    UnityEngine.Color32 color = framebuffer[i + (resHeight - j - 1) * resWidth];
                    bmp.SetPixel(i, j, System.Drawing.Color.FromArgb(color.r, color.g, color.b));
                }
            }

            //Save image to file
            FileStream fs = new FileStream(path + ".png", FileMode.Create);
            bmp.Save(fs, ImageFormat.Png);
            fs.Flush();
            fs.Close();

        }).Start();
    }
}
