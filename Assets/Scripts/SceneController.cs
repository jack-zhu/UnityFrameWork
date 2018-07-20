using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour {

    public bool IsAsyncLoading = true;

    public string[] widgets;

    public GameObjectPool[] preLoad;

    private LoadingDialog loadingDialog;

    void Awake()
    {
        //是否异步加载
        if (IsAsyncLoading)
        {
            loadingDialog = LoadingDialog.Show();
            //异步加载
            StartCoroutine("OnLoad");
        }else
        {
            ShowWidget();
        }
    }

    //开启协成进行异步加载
    IEnumerator OnLoad()
    {
        //init
        float progressValue = 0;
        float progressMaxValue = (preLoad == null) ? widgets.Length : widgets.Length + preLoad.Length;
        loadingDialog.UpdateSlide("数据加载中...", progressValue, progressMaxValue);
        yield return new WaitForEndOfFrame();

        //load widgets里面的数据
        ResourceAsyncLoadingRequest<GameObject> request = null;
        foreach (var widget in widgets)
        {
            string title = "正在加载" + ResourceManager.GetFileName(widget);
            request = ResourceManager.AsyncLoadResourceUI(widget);
            do
            {
                loadingDialog.UpdateSlide(title, progressValue + request.progress + 1, progressMaxValue);
                yield return new WaitForEndOfFrame();
            } while (!request.isDone);
            progressValue++;
        }

        //load preload里面的数据
        GameObectManager manager = GameObectManager.GetInstance();
        if (preLoad != null)
        {
            foreach (var item in preLoad)
            {
                string title = "正在加载" + ResourceManager.GetFileName(item.name);
                loadingDialog.UpdateSlide(title, progressValue + 1, progressMaxValue);
                yield return new WaitForEndOfFrame();
                manager.StoreObject(item.getObject(), item.name);
                progressValue++;
            }
        }

        //加载完成
        loadingDialog.UpdateSlide("加载完毕...", progressMaxValue, progressMaxValue);
        yield return new WaitForEndOfFrame();
        ShowWidget();
        LoadingDialog.Hide(loadingDialog.gameObject);
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowWidget()
    {
        foreach (var item in widgets)
        {
            UIWidgetController.Show(item);
        }
    }

    private void OnDestroy()
    {
        foreach (var widget in widgets)
        {
            UIWidgetController.Hide(widget);
        }

        GameObectManager manager = GameObectManager.GetInstance();
        if (preLoad != null)
        {
            foreach (var item in preLoad)
            {
                manager.ClearObject(item.name);
            }
        }

        Resources.UnloadUnusedAssets();
    }
}
