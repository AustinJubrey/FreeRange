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
}
