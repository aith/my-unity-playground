using UnityEngine;
[CreateAssetMenu]
public class FloatVariable : ScriptableObject
{
    public float Value;
    public string DevDescription = "";

    public void SetValue(float input)
    {
        Value = input;
    }

    public float GetValue()
    {
        return Value;
    }

    public void ApplyChange(float input)
    {
        this.Value += input;
    }
    
    public void ApplyChange(FloatVariable input)
    {
        this.Value += input.Value;
    }
}
