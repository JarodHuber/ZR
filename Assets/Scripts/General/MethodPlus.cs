using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class MethodPlus
{
    /// <summary>
    /// Gets the components from the children with the specified tag
    /// </summary>
    /// <typeparam name="T">Component to return</typeparam>
    /// <param name="parent">Parent whose children are to be checked</param>
    /// <param name="tag">Tag the child must have</param>
    /// <param name="forceActive">Whether the child needs to be an active Gameobject</param>
    /// <returns>Returns the specified Component from all the children with the specified tag as an Array</returns>
    public static T[] GetComponentsInChildrenWithTag<T>(this GameObject parent, string tag, bool forceActive = false) where T : Component
    {
        if (parent == null) { throw new System.ArgumentNullException(); }
        if (string.IsNullOrEmpty(tag) == true) { throw new System.ArgumentNullException(); }
        List<T> list = new List<T>(parent.GetComponentsInChildren<T>(forceActive));
        if (list.Count == 0) { return null; }

        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i].CompareTag(tag) == false)
            {
                list.RemoveAt(i);
            }
        }
        return list.ToArray();
    }

    /// <summary>
    /// Gets a component from a child with the specified tag
    /// </summary>
    /// <typeparam name="T">Component to return</typeparam>
    /// <param name="parent">Parent whose children are to be checked</param>
    /// <param name="tag">Tag the child must have</param>
    /// <param name="forceActive">Whether the child needs to be an active Gameobject</param>
    /// <returns>Returns the specified component from the top child with the specified tag</returns>
    public static T GetComponentInChildrenWithTag<T>(this GameObject parent, string tag, bool forceActive = false) where T : Component
    {
        if (parent == null) { throw new System.ArgumentNullException(); }
        if (string.IsNullOrEmpty(tag) == true) { throw new System.ArgumentNullException(); }

        T[] list = parent.GetComponentsInChildren<T>(forceActive);
        foreach (T t in list)
        {
            if (t.CompareTag(tag) == true)
            {
                return t;
            }
        }
        return null;
    }

    /// <summary>
    /// Finds the component of the specified type in the GameObject with the specified tag
    /// </summary>
    /// <typeparam name="T">Component type to return</typeparam>
    /// <param name="tag">Tag the GameObject must have</param>
    /// <returns>Returns the specified component found in the first object with the specified tag</returns>
    public static T GetComponentInObjectByTag<T>(string tag) where T : Component
    {
        if (string.IsNullOrEmpty(tag) == true) { throw new System.ArgumentNullException(); }

        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
        if (objectsWithTag.Length == 0)
        {
            Debug.LogError("No objects with the tag: " + tag);
            return null;
        }

        foreach (GameObject g in objectsWithTag)
            if (g.GetComponent<T>() != null)
                return g.GetComponent<T>();

        Debug.LogError("No objects with the tag: " + tag + " has component the specified component type");
        return null;
    }

    /// <summary>
    /// Finds the components of the specified type in the GameObjects with the specified tag
    /// </summary>
    /// <typeparam name="T">Component type to return</typeparam>
    /// <param name="tag">Tag the GameObjects must have</param>
    /// <returns>Returns the specified component found in the first object with the specified tag</returns>
    public static List<T> GetComponentsInObjectsByTag<T>(string tag) where T : Component
    {
        if (string.IsNullOrEmpty(tag) == true) { throw new System.ArgumentNullException(); }

        List<T> componentList = new List<T>();
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);

        if (objectsWithTag.Length == 0)
        {
            Debug.LogError("No objects with the tag: " + tag);
            return null;
        }

        foreach (GameObject g in objectsWithTag)
            if (g.GetComponent<T>() != null)
                componentList.Add(g.GetComponent<T>());

        if(componentList.Count == 0)
            Debug.LogError("No objects with the tag: " + tag + " has component the specified component type");

        return componentList;
    }

    /// <summary>
    /// Gets a child of the specified parent with the specified name
    /// </summary>
    /// <param name="parent">Parent to pull the child from</param>
    /// <param name="name">Name of child you wish to pull</param>
    /// <returns>Returns a GameObject child in the specified parent with the specified name</returns>
    public static GameObject GetChildWithName(Transform parent, string name)
    {
        if (parent == null) { throw new System.ArgumentNullException(); }
        if (string.IsNullOrEmpty(name) == true) { throw new System.ArgumentNullException(); }

        foreach(Transform t in parent)
        {
            if (t.name == name)
                return t.gameObject;
        }

        return null;
    }

    /// <summary>
    /// Gets a child of the specified parent with the specified name
    /// </summary>
    /// <param name="parent">Parent to pull the child from</param>
    /// <param name="name">Name of child you wish to pull</param>
    /// <returns>Returns a GameObject child in the specified parent with the specified name</returns>
    public static T GetComponentInChildWithName<T>(Transform parent, string name) where T : Component
    {
        if (parent == null) { throw new System.ArgumentNullException(); }
        if (string.IsNullOrEmpty(name) == true) { throw new System.ArgumentNullException(); }

        foreach (Transform t in parent)
        {
            if (t.name == name)
                if (t.GetComponent<T>() != null)
                    return t.GetComponent<T>();
        }

        return null;
    }

    /// <summary>
    /// Returns an int based on a skewed chance
    /// </summary>
    /// <param name="values">Possible return values</param>
    /// <param name="probabilities">Array of chances for each value</param>
    /// <returns>Returns one of the inputed values based on the inputed probabilities</returns>
    public static int SkewedNum(int[] values, float[] probabilities)
    {
        float totalP = 0, miscHold = 0;

        float[] oneToHundred = new float[probabilities.Length+1]; 

        int[] valStorage = values;

        float[] augmentedP = probabilities;

        for (int counter = 0; counter < augmentedP.Length; counter++)
            if (augmentedP[counter] != -1)
                totalP += augmentedP[counter];

        for (int counter = 0; counter < augmentedP.Length; counter++)
            if (augmentedP[counter] != -1)
            {
                miscHold += augmentedP[counter];
                oneToHundred[counter + 1] = (miscHold / totalP) - 0.01f;
            }

        float hold = Random.Range(0f, 1f);
        for (int counter = 1; counter < oneToHundred.Length; counter++)
            if (oneToHundred[counter - 1] <= hold && hold < oneToHundred[counter])
                return valStorage[counter - 1];

        return valStorage[0];
    }

    /// <summary>
    /// Returns an int based on a skewed chance
    /// </summary>
    /// <param name="values">The number of values that could be returned</param>
    /// <param name="probabilities">Array of chances for each value</param>
    /// <returns>Returns value between 0 and values-1</returns>
    public static int SkewedNum(int values, float[] probabilities)
    {
        float totalP = 0, miscHold = 0;

        float[] oneToHundred = new float[probabilities.Length+1];

        int[] valStorage = new int[values];
        for(int x = 0; x < values; x++)
            valStorage[x] = x;

        float[] augmentedP = probabilities;

        for (int counter = 0; counter < augmentedP.Length; counter++)
            if (augmentedP[counter] != -1)
                totalP += augmentedP[counter];

        for (int counter = 0; counter < augmentedP.Length; counter++)
            if (augmentedP[counter] != -1)
            {
                miscHold += augmentedP[counter];
                oneToHundred[counter + 1] = (miscHold / totalP) - 0.01f;
            }

        float hold = Random.Range(0f, 1f);
        for (int counter = 1; counter < oneToHundred.Length; counter++)
            if (oneToHundred[counter - 1] <= hold && hold < oneToHundred[counter])
                return valStorage[counter - 1];

        return valStorage[0];
    }

    /// <summary>
    /// Converts a string into a value
    /// </summary>
    /// <typeparam name="T">Type of value to convert to</typeparam>
    /// <param name="String">String to convert</param>
    /// <returns>Returns a value of the specified type converted from a string</returns>
    public static T StringParse<T>(string String)
    {
        T Return = default;

        try
        {
            Return = (T)Convert.ChangeType(String, typeof(T));
        }
        catch
        {
            Debug.LogError("not convertable");
        }

        return Return;
    }

    /// <summary>
    /// Converts a string into a list of values
    /// </summary>
    /// <typeparam name="T">Type of value to convert to</typeparam>
    /// <param name="String">String to convert</param>
    /// <param name="splicer">Character to split string with</param>
    /// <returns>Returns a list of values of the specified type converted from a string</returns>
    public static List<T> StringParse<T>(string String, char splicer)
    {
        List<T> Return = default;
        string[] temp = String.Split(splicer);
        Converter<string, T> c = new Converter<string, T>(input => (T)Convert.ChangeType(input, typeof(T)));

        Return = Array.ConvertAll<string, T>(temp, c).ToList();

        return Return;
    }

    /// <summary>
    /// Converts an Array into a string of the values separated by a splicable char
    /// </summary>
    /// <param name="array">Array to be converted</param>
    /// <param name="spacer">Spacer put in-between values from the Array</param>
    /// <returns>Returns a string of the values separated by a splicable char</returns>
    public static string ToString<TInput>(TInput[] array, string spacer = "")
    {
        string Return = "";
        for (int count = 0; count < array.Length; count++)
            Return += (array[count]) + (count + 1 < array.Length ? spacer : "");

        return Return;
    }

    /// <summary>
    /// Converts a List into a string of the values separated by a splicable char
    /// </summary>
    /// <param name="list">List to be converted</param>
    /// <param name="spacer">Spacer put in-between values from the List</param>
    /// <returns>Returns a string of the values separated by a splicable char</returns>
    public static string ToString<TInput>(List<TInput> list, string spacer = "")
    {
        string Return = "";

        for (int count = 0; count < list.Count; count++)
            Return += (list[count]) + (count + 1 < list.Count ? spacer : "");

        return Return;
    }
}

public class Timer
{
    public float delay;
    float timer;

    public Timer(float delay)
    {
        this.timer = 0;
        this.delay = delay;
    }

    /// <summary>
    /// Reset the timer to the startPoint
    /// </summary>
    /// <param name="startPoint">The time in seconds you want the timer to get set to</param>
    public void Reset(float startPoint = 0)
    {
        timer = startPoint;
    }

    /// <summary>
    /// Increases timer by Time.deltaTime
    /// </summary>
    public void CountByTime()
    {
        timer += Time.deltaTime;
    }

    /// <summary>
    /// Increases timer by value
    /// </summary>
    /// <param name="value">The float value you want to add to timer</param>
    public void CountByValue(float value)
    {
        timer += value;
    }

    /// <summary>
    /// Checks to see if the timer has reached or passed the delay
    /// </summary>
    /// <param name="resetOnTrue">Whether you want the timer to reset when IsComplete() is true</param>
    /// <returns>Returns true if timer is greater than or equal to delay</returns>
    public bool IsComplete(bool resetOnTrue = true)
    {
        if (timer >= delay)
        {
            if (resetOnTrue)
                Reset();

            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks whether the timer has reached or passed the delay and if not count up
    /// </summary>
    /// <param name="resetOnTrue">Whether you want the timer to reset when IsComplete() is true</param>
    /// <param name="countByTime">Whether you want to increase the timer by Time.deltaTime or by value</param>
    /// <param name="value">The float value you want to add to timer if countByTime is false</param>
    /// <returns>Returns true if timer is greater than or equal to delay</returns>
    public bool Check(bool resetOnTrue = true, bool countByTime = true, float value = 0)
    {
        if (IsComplete(resetOnTrue))
            return true;

        if (countByTime)
            CountByTime();
        else
            CountByValue(value);

        return false;
    }

    /// <summary>
    /// Determines the percentage cmplete timer is
    /// </summary>
    /// <returns>Returns float percent timer is complete (not clamped)</returns>
    public float PercentComplete()
    {
        return timer / delay;
    }
}
