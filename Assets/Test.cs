using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Singleton{

    private static Singleton mInstance;
    private Singleton(){}
    private static readonly object mLock = new object();

    public Singleton GetInstance(){
        if (mInstance == null)
        {
            lock(mLock){
                if (mInstance == null)
                {
                    mInstance = new Singleton();
                }
            }
        }
        return mInstance;
    }
	
}
