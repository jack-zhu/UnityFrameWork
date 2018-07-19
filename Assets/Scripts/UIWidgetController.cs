using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWidgetController : MonoBehaviour {

    private string path = null;
    private GameEvent evt = null;
    private static Dictionary<string, GameObject> widgets = new Dictionary<string, GameObject>();

    public static GameObject Show(string path)
    {
        return Show(path, new GameEvent(0));
    }

    public static GameObject Show(string path, GameEvent evt)
    {
        GameObject parent = GameObject.Find("Canvas");
        return Show(parent.transform, path, evt);
    }

    public static GameObject Show(Transform parent, string path, GameEvent evt)
    {
        GameObject gameObject = null;
        if (widgets.ContainsKey(path))
        {
            widgets.TryGetValue(path, out gameObject);
        }
        if (gameObject == null)
        {
            gameObject = GameObectManager.GetInstance().CreateObject(path);
        }

        gameObject.transform.SetParent(parent);
        RectTransform rectTransform = gameObject.transform as RectTransform;
        rectTransform.anchoredPosition3D = Vector3.zero;
        rectTransform.localScale = Vector3.one;
        rectTransform.sizeDelta = Vector2.zero;
        UIWidgetController uIWidgetController = gameObject.GetComponent<UIWidgetController>();
        if (uIWidgetController == null)
        {
            uIWidgetController = gameObject.AddComponent<UIWidgetController>();
        }
        uIWidgetController.path = path;
        uIWidgetController.evt = evt;

        gameObject.name = path;
        widgets[path] = gameObject;

        return gameObject;
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
