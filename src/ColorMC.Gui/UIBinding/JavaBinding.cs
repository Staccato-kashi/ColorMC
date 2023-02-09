﻿using Avalonia.Platform.Storage;
using Avalonia.Platform.Storage.FileIO;
using ColorMC.Core.LaunchPath;
using ColorMC.Core.Objs;
using ColorMC.Core.Utils;
using ColorMC.Gui.Objs;
using ColorMC.Gui.Utils.LaunchSetting;
using System;
using System.Collections.Generic;
using System.IO;

namespace ColorMC.Gui.UIBinding;

public record JavaInfoObj
{
    public string Name { get; set; }
    public string Path { get; set; }
    public string Info { get; set; }
}

public static class JavaBinding
{
    private readonly static List<string> JavaType = 
        new() { "Adoptium", "Zulu", "Dragonwell", "OpenJ9" };
    private readonly static List<string> SystemType =
        new() { "", "Windows", "Linux", "MacOS" };
    private static JavaInfoObj MakeInfo(string name, JavaInfo item)
    {
        return new JavaInfoObj()
        {
            Name = name,
            Path = item.Path,
            Info = $"{item.Type} {item.Version} {item.Arch}"
        };
    }

    public static List<JavaInfoObj> GetJavaInfo()
    {
        List<JavaInfoObj> res = new();
        foreach (var item in JvmPath.Jvms)
        {
            res.Add(MakeInfo(item.Key, item.Value));
        }

        return res;
    }

    public static List<string> GetJavaName()
    {
        var list = new List<string>();
        foreach (var item in JvmPath.Jvms)
        {
            list.Add(item.Key);
        }

        return list;
    }

    public static (JavaInfoObj?, string?) AddJava(string name, string local)
    {
        var res = JvmPath.AddItem(name, local);
        if (res.Res == false)
        {
            return (null, res.Msg);
        }
        else
        {
            var info = JvmPath.GetInfo(res.Msg);
            if (info == null)
            {
                return (null, Localizer.Instance["Error5"]);
            }
            return (MakeInfo(res.Msg, info), null);
        }
    }

    public static JavaInfo? GetJavaInfo(string path)
    {
        return JvmPath.GetJavaInfo(path);
    }

    public static void RemoveJava(string name)
    {
        JvmPath.Remove(name);
    }

    public static List<string> GetGCTypes()
    {
        var list = new List<string>();
        Array values = Enum.GetValues(typeof(GCType));
        foreach (GCType value in values)
        {
            list.Add(value.GetName());
        }

        return list;
    }

    public static List<JavaDisplayObj> GetJavas()
    {
        List<JavaDisplayObj> res = new();
        foreach (var item in JvmPath.Jvms)
        {
            res.Add(new()
            {
                Name = item.Key,
                Path = item.Value.Path,
                MajorVersion = item.Value.MajorVersion.ToString(),
                Version = item.Value.Version,
                Type = item.Value.Type,
                Arch = item.Value.Arch.GetName()
            });
        }

        return res;
    }

    public static IStorageFolder? GetSuggestedStartLocation()
    {
        switch (SystemInfo.Os)
        {
            case OsType.Windows:
                if (Directory.Exists("C:\\Program Files\\java"))
                    return new BclStorageFolder(new DirectoryInfo("C:\\Program Files\\java"));

                break;
            case OsType.MacOS:
                if (Directory.Exists("/Library/Java/JavaVirtualMachines/"))
                    return new BclStorageFolder(new DirectoryInfo("/Library/Java/JavaVirtualMachines/"));

                break;
        }

        return null;
    }

    public static void RemoveAllJava()
    {
        JvmPath.RemoveAll();
    }

    public static List<string> GetJavaType()
    {
        return JavaType;
    }

    public static List<string> GetSystemType()
    {
        return SystemType;
    }
}
