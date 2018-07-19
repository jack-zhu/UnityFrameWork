using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWidgetController : MonoBehaviour {

    private string path = null;
    private GameEvent evt = null;
    private static Dictionary<string, GameObject> widgets = new Dictionary<string, GameObject>();

    //ui 层次
    private Canvas mCanvas = null;
    private int layerIndex = 1;

    private bool isStart = false;

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

    public static void Hide(string path)
    {
        GameObject gameObject = null;
        if (widgets.TryGetValue(path,out gameObject))
        {
            widgets.Remove(path);
            Hide(gameObject);
        }
    }

    public static void Hide(GameObject gameObject)
    {
        UIWidgetController uIWidgetController = gameObject.GetComponent<UIWidgetController>();
        gameObject.SendMessage("OnHide",SendMessageOptions.DontRequireReceiver);
        //将对象存储
        GameObectManager.GetInstance().StoreObject(gameObject,"UI/"+uIWidgetController.path);
        widgets.Remove(gameObject.name);
        uIWidgetController.evt = new GameEvent(0);
    }


    public static void RegisterWidget(string path, int showCode, int hideCode)
    {
        GameEventCenter gameEventCenter = GameEventCenter.GetInstance();
        gameEventCenter.RegisterEvent(showCode,delegate(GameEvent evt) {
            Show(path);

        });
        gameEventCenter.RegisterEvent(hideCode, delegate(GameEvent evt) {
            Hide(path);

        });
    }

    public static void DispatchEvent(int code)
    {
        GameEventCenter.GetInstance().DispatchEvent(code);
    }

    public static void DispatchEvent(GameEvent evt)
    {
        GameEventCenter.GetInstance().DispatchEvent(evt);
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!isStart)
        {
            isStart = true;
            ReOrder();
            gameObject.SendMessage("OnShow", SendMessageOptions.DontRequireReceiver);
        }
	}

    public int GetUIZOrder()
    {
        if (mCanvas == null)
        {
            return 0;
        }else
        {
            return mCanvas.sortingOrder;
        }
    }

    private void ReOrder()
    {
        if (mCanvas == null)
        {
            Canvas canvas = gameObject.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
            }else
            {
                mCanvas = canvas;
            }
        }
        layerIndex += 1;
        mCanvas.overrideSorting = true;
        mCanvas.sortingOrder = layerIndex;
    }
}
