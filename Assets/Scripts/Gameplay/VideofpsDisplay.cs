using UnityEngine;
using UnityEngine.Video;

public class VideoFPSDisplay : MonoBehaviour
{
    public VideoPlayer vp;

    private double lastTime;
    private long lastFrame;
    private float currentFps;

    void Update()
    {
        if (vp.isPlaying)
        {
            double time = vp.time;
            long frame = vp.frame;

            if (time > lastTime)
            {
                currentFps = (frame - lastFrame) / (float)(time - lastTime);
                lastTime = time;
                lastFrame = frame;
            }
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 40), $"🎬 Video FPS: {currentFps:F1}");
    }
}
