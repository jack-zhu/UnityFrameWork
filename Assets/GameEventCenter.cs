using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent{
    public int code;
    public object data;
    public object arg0;
    public object arg1;
    public GameEvent(int code){
        this.code = code;
    }

    public GameEvent(int code, object data){
        this.code = code;
        this.data = data;
    }

    public GameEvent(int code, object data, object arg0)
    {
        this.code = code;
        this.data = data;
        this.arg0 = arg0;
    }

    public GameEvent(int code, object arg0, object arg1, object data){
        this.code = code;
        this.data = data;
        this.arg0 = arg0;
        this.arg1 = arg1;
    }
}

public delegate void GameEventDelegate(GameEvent evt);
public class GameEventHandler{
    private GameEventDelegate gameEventDelegate;

    public void process(GameEvent evt)
    {
        gameEventDelegate(evt);
    }

    public void Add(GameEventDelegate dele)
    {
        if (gameEventDelegate != null)
        {
            this.gameEventDelegate = dele;
        }else
        {
            gameEventDelegate += dele;
        }
    }

    public void Remove(GameEventDelegate dele)
    {
        if (gameEventDelegate != null)
        {
            gameEventDelegate -= dele;
        }
    }

}

public class GameEventCenter : MonoBehaviour {
    private static GameEventCenter mInstance;

    private Dictionary<int, GameEventHandler> values = new Dictionary<int, GameEventHandler>();

    public static GameEventCenter GetInstance(){
        return mInstance;
    }

    void Awake()
    {
        mInstance = this;    
    }

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RegisterEvent(int code, GameEventDelegate dele)
    {
        GameEventHandler handler;
        if (!values.TryGetValue(code,out handler))
        {
            handler = new GameEventHandler();
            values.Add(code,handler);
        }
        handler.Add(dele);
    }

    public void UnRegisterEvent(int code, GameEventDelegate dele)
    {
        GameEventHandler handler;
        if (values.TryGetValue(code,out handler))
        {
            handler.Remove(dele);
        }
    }

    public void ResetEvent(int code)
    {
        if (values.ContainsKey(code))
        {
            values.Remove(code);
        }
    }

    public void DispatchEvent(GameEvent evt)
    {
        GameEventHandler handler;
        if (values.TryGetValue(evt.code, out handler))
        {
            handler.process(evt);
        }
    }

    public void DispatchEvent(int code)
    {
        GameEvent evt = new GameEvent(code);
        DispatchEvent(evt);
    }

    public void DispatchEvent(int code, object data)
    {
        GameEvent evt = new GameEvent(code, data);
        DispatchEvent(evt);
    }


}
