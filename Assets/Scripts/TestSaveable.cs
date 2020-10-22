using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Net.Security;

public class TestSaveable : MonoBehaviour, ISaveable
{
    private string str = "TestSaveable";
    private string key = "key";
    private string innerkey = "innerkey";
    public JsonData data = new JsonData();
    JsonData dic = new JsonData();

    void Start()
    {
        // Access data with JsonData[key]
        // TODO how to set jsondata's string?

        // data[key] = "contents";
    }

    // This is the same as having a 'get' property that has 'return str'
    public string SaveID => str;
    private int localX = 0;
    private int localY = 1;

    public JsonData SavedData
    {
        get
        {
            // Version with writer, which has pretty-print options if needed. But we already pretty-print in
            // SavingSystem.cs
            // var writer = new JsonWriter();
            // data.ToJson(writer);
            // return JsonMapper.ToObject(writer.ToString());
            
            // Version before I knew that JsonData counts as a dictionary itself
            // Debug.Log(JsonMapper.ToObject(JsonMapper.ToJson(data)).str);
            data[key] = "contents";
            data["nestedkey"] = new JsonData();
            data["nestedkey"]["innerkey"] = "hihi";
            return data;
        }
    }

    // public JsonData SavedData
    // {
    //     get
    //     {
    //         return dic;
    //     }
    // }

    public void LoadFromData(JsonData data)
    {
        // Debug.Log(data["nestedkey"]["innerkey"]);
    }
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
    }
}