using System.Collections;
using System.Collections.Generic;
using MonsterLove.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;
using UIFramework;
using Unity.VisualScripting;
using Random = System.Random;

public static class PublicStaticMethod
{
    private static Random rng = new Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static void Release(this GameObject gameObject)
    {
        PoolManager.ReleaseObject(gameObject);
    }

    public static void InitLocalTransform(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public static void InitLocalTransformWithOutPosition(this Transform transform)
    {
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public static Dictionary<TKey, TValue> AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        dict[key] = value;
        return dict;
    }

    public static string GetTypeName<T>()
    {
        return GetTypeName(typeof(T));
    }

    private static string GetTypeName(Type type)
    {
        return type.ToString();
    }

    public static Transform FindRecursive(this Transform transform, string name)
    {
        return transform.FirstOrDefault(t => t.name == name);
    }

    public static Transform FirstOrDefault(this Transform transform, Func<Transform, bool> query)
    {
        if (query(transform))
        {
            return transform;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            var result = FirstOrDefault(transform.GetChild(i), query);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}