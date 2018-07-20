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
        float progressValue = 0;
        float progressMaxValue = (preLoad == null) ? widgets.Length : widgets.Length + preLoad.Length;
        loadingDialog.UpdateSlide("数据加载中...", progressValue, progressMaxValue);
        yield return new WaitForEndOfFrame();
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
}
