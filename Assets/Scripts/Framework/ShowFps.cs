using UnityEngine;
using System.Collections;

public class ShowFps : MonoBehaviour
{

    public float updateInterval = 0.5F;

    private float lastInterval;

    private int frames = 0;

    private float fps;

    void Start()
    {
        //Application.targetFrameRate=60;

        lastInterval = Time.realtimeSinceStartup;

        frames = 0;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, 100, 200, 200), "FPS:" + fps.ToString("f2"));
    }

    void Update()
    {
        ++frames;

        if (Time.realtimeSinceStartup > lastInterval + updateInterval)
        {
            fps = frames / (Time.realtimeSinceStartup - lastInterval);

            frames = 0;

            lastInterval = Time.realtimeSinceStartup;
        }
    }
}
