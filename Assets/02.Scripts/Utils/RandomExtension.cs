using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Ramdon = UnityEngine.Random;

public static class RandomExtension
{
    /// <summary>
    /// 랜덤 시드 초기화
    /// </summary>
    public static void InitStateFromTicks()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
    }

    /// <summary>
    /// 리스트에서 순서 섞기
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void ShuffleList<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// 리스트에서 랜덤한 것 하나 뽑기
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T GetRandomT<T>(this IList<T> list)
    {
        int index = UnityEngine.Random.Range(0, list.Count);
        return list[index];
    }

    /// <summary>
    /// 딕셔너리에서 랜덤한 키 중 하나 뽑기
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dictionary"></param>
    /// <returns></returns>
    public static TKey GetRandomKey<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
    {
        List<TKey> keys = new List<TKey>(dictionary.Keys);
        int index = UnityEngine.Random.Range(0, keys.Count);
        return keys[index];
    }

    /// <summary>
    /// 딕셔너리에서 랜덤한 값 중 하나 뽑기
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dictionary"></param>
    /// <returns></returns>
    public static TValue GetRandomValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
    {
        List<TValue> dicValues = new List<TValue>(dictionary.Values);
        int index = UnityEngine.Random.Range(0, dicValues.Count);
        return dicValues[index];
    }
}
