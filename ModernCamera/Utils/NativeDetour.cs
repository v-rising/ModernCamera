using BepInEx.IL2CPP.Hook;
using System;
using System.Linq;
using System.Reflection;

namespace ModernCamera.Utils;

internal static class NativeDetour
{
    private static readonly BindingFlags bindingFlags = BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetField | BindingFlags.SetProperty | BindingFlags.Static;

    internal static FastNativeDetour Create<T>(string typeName, string methodName, T to, out T original) where T : System.Delegate
    {
        return Create(Type.GetType(typeName), methodName, to, out original);
    }

    internal static FastNativeDetour Create<T>(string typeName, string innerTypeName, string methodName, T to, out T original) where T : System.Delegate
    {
        return Create(GetInnerType(Type.GetType(typeName), innerTypeName), methodName, to, out original);
    }

    internal static FastNativeDetour Create<T>(Type type, string innerTypeName, string methodName, T to, out T original) where T : System.Delegate
    {
        return Create(GetInnerType(type, innerTypeName), methodName, to, out original);
    }

    internal static FastNativeDetour Create<T>(Type type, string methodName, T to, out T original) where T : System.Delegate
    {
        return Create(type.GetMethod(methodName, bindingFlags), to, out original);
    }

    internal static FastNativeDetour Create<T>(MethodInfo method, T to, out T original) where T : System.Delegate
    {
        var address = Il2CppMethodResolver.ResolveFromMethodInfo(method!);
        Plugin.Logger.LogInfo($"Detouring {method.DeclaringType.FullName}.{method.Name} at {address.ToString("X")}");
        return FastNativeDetour.CreateAndApply(address, to, out original);
    }

    private static Type GetInnerType(Type type, string innerTypeName)
    {
        return type.GetNestedTypes().First(x => x.Name.Contains(innerTypeName));
    }
}