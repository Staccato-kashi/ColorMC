﻿namespace ColorMC.Core.Objs;

public enum SourceType
{
    CurseForge, Modrinth
}

/// <summary>
/// ModPack信息
/// </summary>
public record ModPackInfoObj
{
    public string Path { get; set; }
    public string Name { get; set; }
    public string File { get; set; }
    public string SHA1 { get; set; }
    public string Url { get; set; }
    public string ModId { get; set; }
    public string FileId { get; set; }
    public SourceType Type { get; set; }
}
