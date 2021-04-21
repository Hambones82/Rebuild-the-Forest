using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectPool
{
    private static class Pool<T> where T : IObjectPoolInterface, new()
    {
        public static Stack<T> objects = new Stack<T>();
        
        public static T GetObject()
        {
            //Debug.Log(objects.Count);
            if (objects.Count > 0)
            {
                return objects.Pop();
            }
            else
            {
                return new T();
            }
        }

        public static void ReturnObject(T retObject)
        {
            retObject.Reset();
            objects.Push(retObject);
        }
    }

    public static void Return<T>(T retObject) where T : IObjectPoolInterface, new()
    {
        Pool<T>.ReturnObject(retObject);
    }

    public static T Get<T>() where T : IObjectPoolInterface, new()
    {
        return Pool<T>.GetObject();
    }
}

