using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class TestSaveable2 : SaveableBehaviour
{
    JsonData data = new JsonData();
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(SaveID);
        data["key"] = "contents";
    }

    public override JsonData SavedData
    {
        get
        {
            JsonData thing = new JsonData();
            thing["thing"] = "ok";
            return thing;
        }
    }

    public override void LoadFromData(JsonData data)
    {
        Debug.Log(data["thing"]);
    }
}
