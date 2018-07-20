using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WidgetConfig {

    private static bool isInit = false;

    private static Dictionary<string, int> widgetEvents = new Dictionary<string, int>();

    public static void Init()
    {
        if (!isInit)
        {
            isInit = true;
            LoadConfig();
        }
    }

    public static void Register(string path, int showCode, int hideCode)
    {
        UIWidgetController.RegisterWidget(path, showCode, hideCode);
    }

    public static void LoadConfig()
    {
        string[] props;
        int showCode, hideCode;
        TextAsset textAsset = ResourceManager.LoadResource<TextAsset>("WidgetConfig", "WidgetConfig");
        string[] lines = textAsset.text.Split(new char[] {'\r', '\n'}, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            props = line.Split('=');
            if (props.Length > 1)
            {
                showCode = GetEventCode("EVENT_SHOW_" + props[0].ToUpper() + "_WIDGET");
                hideCode = GetEventCode("EVENT_HIDE_" + props[0].ToUpper() + "_WIDGET");
                Register(props[1], showCode, hideCode);
            }
        }
        Resources.UnloadAsset(textAsset);
        ResourceManager.DestroyResource("WidgetConfig");
    }


    public static int GetEventCode(string name)
    {
        return GameEventCenter.GetInstance().GetEventCode(name);
    }
}
