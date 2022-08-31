using UnityEngine;
using System;

[Serializable]
public class SerializableEnum<T> where T : struct, IConvertible
{
    public T Value
    {
        get
        {
#if UNITY_EDITOR
            ValidateSerializedValue();
#endif
            return m_EnumValue;
        }
        set { m_EnumValue = value; }
    }

    /// <summary>
    /// Attempts to parse the non-serialized string value.
    /// This resolves serialization issues, but is slower to execute.
    /// </summary>
    public T PersistentValue
    {
        get
        {
            T parsedValue = default(T);
            try
            {
                parsedValue = (T)System.Enum.Parse(typeof(T), m_EnumValueAsString);
            }
            catch (Exception)
            {
                Debug.LogError(string.Format("SerializableEnum: Could not convert value {0} to enum type {1}. Maybe the value was deleted?", m_EnumValueAsString, typeof(T).Name));
            }

            return parsedValue;
        }
    }

    [SerializeField]
    private string m_EnumValueAsString;
    [SerializeField]
    private T m_EnumValue;

#if UNITY_EDITOR
    private void ValidateSerializedValue()
    {
        Debug.Assert(m_EnumValue.ToString() == m_EnumValueAsString, string.Format("SerializableEnum: Serialized value = {0}; Internal string = {1}", m_EnumValue.ToString(), m_EnumValueAsString));
    }
#endif

    public override string ToString()
    {
        return m_EnumValueAsString;
    }
}
