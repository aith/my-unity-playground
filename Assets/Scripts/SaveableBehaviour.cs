using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public abstract class SaveableBehaviour : MonoBehaviour, ISaveable, ISerializationCallbackReceiver
{
    public abstract JsonData SavedData { get; }
    public abstract void LoadFromData(JsonData data);

    public string SaveID
    {
        get
        {
            return _saveID;
        }
        set
        {
            _saveID = value;
        }
    }

    // TODO: Find out... is _saveID stored across games? It is (and needs to be, since that's how the objects are found) the same throughout saves. How?
    [HideInInspector] [SerializeField] private string _saveID;
    public void OnBeforeSerialize()
    {
        if (_saveID == null)
        {
            _saveID = System.Guid.NewGuid().ToString();
        }
    }

    public void OnAfterDeserialize()
    {
        
    }
}
