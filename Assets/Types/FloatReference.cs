using System;

// References a FloatVariable, if given one via the Inspector.
// Otherwise, provides a constant value.

// UseConstant is changed by the EditorGUI changes made by FloatReferenceDrawer.cs

namespace aith.CustomVariables
{
    [Serializable]
    public class FloatReference
    {
        public FloatVariable Variable;
        public float ConstantValue;
        public bool UseConstant = true;

        public FloatReference()
        {
        }

        // constuctor
        public FloatReference(float value)
        {
            UseConstant = true;
            ConstantValue = value;
        }

        public float Value
        {
            get { return UseConstant ? ConstantValue : Variable.Value; }
        }

        public static implicit operator float(FloatReference reference)
        {
            return reference.Value;
        }
    }
}