using UnityEngine;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading;
using System;

public class GifPlayer : MonoBehaviour {

    public float speed = 1;
    public Vector2 drawPosition;
    public int delay;
    public int frameCount;

    private Texture2D[] gifFrames;
    private bool[] gifFramesLock;
    private Stack<int> frameStack;
    private RawImage UIimage;

    private float savedProgress;
    private EventProcessor eventProcessor;
    private System.Drawing.Image gifImage;
    private FrameDimension dimension;

    public void Init()
    {
        UIimage = GetComponent<RawImage>();
        eventProcessor = GetComponent<EventProcessor>();
        if (eventProcessor == null)
            eventProcessor = gameObject.AddComponent<EventProcessor>();

        frameStack = new Stack<int>();
    }

    public void LoadGif(string gifPath)
    {
        gifImage = System.Drawing.Image.FromFile(gifPath);
        dimension = new FrameDimension(gifImage.FrameDimensionsList[0]);
        frameCount = gifImage.GetFrameCount(dimension);

        gifFrames = new Texture2D[frameCount];
        gifFramesLock = new bool[frameCount];

        for (int i = 0; i < frameCount; i++)
            gifFramesLock[i] = false;

        PropertyItem item = gifImage.GetPropertyItem(0x5100); // FrameDelay in libgdiplus

        delay = (item.Value[0] + item.Value[1] * 256) * 10; // Time is in 1/100ths of a second

        StartCoroutine(LoadFramesLoop());
    }

    IEnumerator LoadFramesLoop()
    {
        while(!CheckIfLoadingComplete())
        {
            if(frameStack.Count > 0)
            {
                LoadGifFrame(frameStack.Pop());
            }
            else
            {
                AddNextFrame();
            }

            yield return new WaitForSecondsRealtime(0.25f);
        }
    }

    private void LoadGifFrame(int frameIndex)
    {

        gifImage.SelectActiveFrame(dimension, frameIndex);
        var frame = new Bitmap(gifImage.Width, gifImage.Height);
        System.Drawing.Graphics.FromImage(frame).DrawImage(gifImage, Point.Empty);

        new Thread(() =>
        {
           
            Color32[] pixels = new Color32[frame.Width * frame.Height];

            for (int x = 0; x < frame.Width; x++)
                for (int y = 0; y < frame.Height; y++)
                {
                    System.Drawing.Color sourceColor = frame.GetPixel(x, y);
                    pixels[x + (frame.Height - 1 - y) * frame.Width] = new Color32(sourceColor.R, sourceColor.G, sourceColor.B, sourceColor.A);
                }

            eventProcessor.QueueEvent(delegate { CreateTexture(pixels, frame.Width, frame.Height, frameIndex); }); // Execute this on main thread

        }).Start();
    }

    private void CreateTexture(Color32[] pixels, int width, int height, int frameIndex)
    {
        var frameTexture = new Texture2D(width, height);
        frameTexture.SetPixels32(pixels);
        frameTexture.Apply();

        gifFrames[frameIndex] = frameTexture;

        DisplayNearestLoadedFrame();
    }

    private bool CheckIfLoadingComplete()
    {
        for (int i = 0; i < frameCount; i++)
        {
            if (gifFrames[i] == null)
            {
                return false;
            }
        }

        return true;
    }

    public void SetProgress(float progress)
    {
        int index = (int)(delay * (frameCount - 1) * progress) / delay;

        if (gifFrames[index] != null)
        {
            UIimage.texture = gifFrames[index];
        }
        else if(!gifFramesLock[index]) // Make sure the frame isn't already processing
        {
            gifFramesLock[index] = true;
            frameStack.Push(index);
        }
        
        savedProgress = progress;
    }

    private void AddNextFrame()
    {
        int index = (int)(delay * (frameCount - 1) * savedProgress) / delay;
        while (gifFramesLock[index] || gifFrames[index] != null)
        {
            index++;
            if (index >= gifFrames.Length)
                break;
        }

        if (index < gifFrames.Length)
        {
            gifFramesLock[index] = true;
            frameStack.Push(index);
        }

    }

    private void DisplayNearestLoadedFrame()
    {
        int index = (int)(delay * (frameCount - 1) * savedProgress) / delay;
        while (gifFrames[index] == null)
        {
            index--;
            if (index <= 0)
                break;
        }

        if(index >= 0)
        {
            UIimage.texture = gifFrames[index];
        }
    }

    public void Dispose()
    {
        UIimage.texture = null;
        gifImage.Dispose();

        for (int i = 0; i < frameCount; i++)
        {
            Destroy(gifFrames[i]);
            gifFrames[i] = null;
        }
     }
}
