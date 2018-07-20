using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceAsyncLoadingRequest<T> where T : UnityEngine.Object
{
    private AssetBundleRequest abRequest;
    private List<AssetBundleCreateRequest> dependenciesRequest;
    private ResourceRequest resRequest;
    public LoadingType type;

    public ResourceAsyncLoadingRequest()
    {
        this.dependenciesRequest = new List<AssetBundleCreateRequest>();
    }

    public ResourceAsyncLoadingRequest(AssetBundleRequest req)
    {
        this.dependenciesRequest = new List<AssetBundleCreateRequest>();
        this.abRequest = req;
        this.type = LoadingType.ASSETBUNDLE_REQUEST;
    }

    public ResourceAsyncLoadingRequest(ResourceRequest req)
    {
        this.dependenciesRequest = new List<AssetBundleCreateRequest>();
        this.resRequest = req;
        this.type = LoadingType.RESOURCE_REQUEST;
    }

    public void AddDependency(AssetBundleCreateRequest request)
    {
        this.dependenciesRequest.Add(request);
    }

    public bool GetDepenenciesDone()
    {
        foreach (AssetBundleCreateRequest request in this.dependenciesRequest)
        {
            if (!request.isDone)
            {
                return false;
            }
        }
        return true;
    }

    public float GetDepenenciesProgress()
    {
        float count = this.dependenciesRequest.Count;
        float num2 = 0f;
        foreach (AssetBundleCreateRequest request in this.dependenciesRequest)
        {
            num2 += request.progress;
        }
        return ((count > 0f) ? (num2 / count) : 1f);
    }

    public T asset
    {
        get
        {
            switch (this.type)
            {
                case LoadingType.ASSETBUNDLE_REQUEST:
                    return (this.abRequest.asset as T);

                case LoadingType.RESOURCE_REQUEST:
                    return (this.resRequest.asset as T);
            }
            return default(T);
        }
    }

    public bool isDone
    {
        get
        {
            switch (this.type)
            {
                case LoadingType.ASSETBUNDLE_DEPENDENCIES_REQUEST:
                    this.GetDepenenciesDone();
                    break;

                case LoadingType.ASSETBUNDLE_REQUEST:
                    return this.abRequest.isDone;

                case LoadingType.RESOURCE_REQUEST:
                    return this.resRequest.isDone;
            }
            return false;
        }
    }

    public float progress
    {
        get
        {
            switch (this.type)
            {
                case LoadingType.ASSETBUNDLE_DEPENDENCIES_REQUEST:
                    return this.GetDepenenciesProgress();

                case LoadingType.ASSETBUNDLE_REQUEST:
                    return this.abRequest.progress;

                case LoadingType.RESOURCE_REQUEST:
                    return this.resRequest.progress;
            }
            return 0f;
        }
    }

    public enum LoadingType
    {
        ASSETBUNDLE_DEPENDENCIES_REQUEST,
        ASSETBUNDLE_REQUEST,
        RESOURCE_REQUEST
    };
}

