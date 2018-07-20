using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadingDialog : UIWidgetController
{
    private Slider mSlider;
    private Text mTitle;
    private Text mContent;

    void Awake()
    {
        mSlider = this.GetComponent<Slider>();
        mTitle = mSlider.transform.Find("mProgressTitle").GetComponent<Text>();
        mContent = mSlider.transform.Find("mProgressContent").GetComponent<Text>();
    }

    private void OnEnable()
    {
        mSlider.onValueChanged.AddListener(OnUpdateSlideValue);
    }

    // Use this for initialization
    void Start()
    {

    }


    void OnShow(GameEvent evt)
    {
        mSlider.value = 0;
        mContent.text = string.Empty;
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void OnUpdateSlideValue(float value)
    {
        
    }

    public void UpdateSlide(string title, float curValue, float maxValue)
    {
        if (mSlider != null)
        {
            mSlider.value = curValue;
            mSlider.maxValue = maxValue;
            mTitle.text = title;
        }
    }

    void OnHide()
    {
        
    }

    private void OnDisable()
    {
        if (mSlider != null)
        {
            mSlider.onValueChanged.RemoveAllListeners();
        }
    }

    public static LoadingDialog Show()
    {
        //加载并显示loadingDialog widget
        return Show("LoadingDialog").GetComponent<LoadingDialog>();
    }

}
