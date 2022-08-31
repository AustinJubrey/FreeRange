using System;

/// <summary>
/// Singleton
/// </summary>
/// <remarks> 
/// Any class extending this class becomes a Singleton.
/// T is the class that extends this Singleton.
/// This is a lazy Singleton, therefore it will only be created the first time someone tries to access it.
/// </remarks>
public abstract class Singleton<T> where T : class
{
    private static T s_instance;

    public static bool s_instanceExist = false;

    public static T Instance()
    {
        if (s_instance == null)
        {
            s_instance = CreateInstanceOfT();

            s_instanceExist = true;
        }

        return s_instance;
    }

    private static T CreateInstanceOfT()
    {
        return Activator.CreateInstance(typeof(T), true) as T;
    }
}