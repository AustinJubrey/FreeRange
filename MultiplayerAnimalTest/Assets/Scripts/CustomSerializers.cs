using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Serializing;
using UnityEngine;

public static class CustomSerializers
{
    public static void WriteEPickUpID(this Writer writer, EPickUpID value)
    {
        writer.WriteString(value.ToString());
    }
    
    public static EPickUpID ReadEPickUpID(this Reader reader)
    {
        Enum.TryParse(reader.ReadString(), out EPickUpID id);

        return id;
    }

    public static void WriteHashSet(this Writer writer, HashSet<Transform> value)
    {
        foreach (var transform in value)
        {
            writer.WriteTransform(transform);
        }
    }

    public static HashSet<Transform> ReadHashSet(this Reader reader)
    {
        HashSet<Transform> newHashSet = new HashSet<Transform>();

        while (reader.Remaining < 0)
        {
            newHashSet.Add(reader.ReadTransform());
        }

        return newHashSet;
    }
}
