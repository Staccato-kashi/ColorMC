﻿using Avalonia.Threading;
using ColorMC.Core.Game;
using ColorMC.Core.Helpers;
using ColorMC.Core.LaunchPath;
using ColorMC.Core.Net.Apis;
using ColorMC.Core.Net.Downloader;
using ColorMC.Core.Objs;
using ColorMC.Core.Objs.CurseForge;
using ColorMC.Core.Objs.Minecraft;
using ColorMC.Core.Objs.Modrinth;
using ColorMC.Core.Utils;
using ColorMC.Gui.Objs;
using ColorMC.Gui.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColorMC.Gui.UIBinding;

public static class WebBinding
{
    public static async Task<List<FileItemDisplayObj>?> GetPackList(SourceType type, string? version, string? filter, int page, int sort, string categoryId)
    {
        version ??= "";
        filter ??= "";
        if (type == SourceType.CurseForge)
        {
            var list = await CurseForgeAPI.GetModPackList(version, page, filter: filter,
                sortField: sort switch
                {
                    0 => 1,
                    1 => 2,
                    2 => 3,
                    3 => 4,
                    4 => 6,
                    _ => 2
                }, sortOrder: sort switch
                {
                    0 => 1,
                    1 => 1,
                    2 => 1,
                    3 => 0,
                    4 => 1,
                    _ => 1
                }, categoryId: categoryId);
            if (list == null)
                return null;
            var list1 = new List<FileItemDisplayObj>();
            list.data.ForEach(item =>
            {
                list1.Add(new()
                {
                    Name = item.name,
                    Summary = item.summary,
                    Author = item.authors.GetString(),
                    DownloadCount = item.downloadCount,
                    ModifiedDate = item.dateModified,
                    Logo = item.logo?.url,
                    FileType = FileType.ModPack,
                    SourceType = SourceType.CurseForge,
                    Data = item
                });
            });

            return list1;
        }
        else if (type == SourceType.Modrinth)
        {
            var list = await ModrinthAPI.GetModPackList(version, page, filter: filter, sortOrder: sort, categoryId: categoryId);
            if (list == null)
                return null;
            var list1 = new List<FileItemDisplayObj>();
            list.hits.ForEach(item =>
            {
                list1.Add(new()
                {
                    Name = item.title,
                    Summary = item.description,
                    Author = item.author,
                    DownloadCount = item.downloads,
                    ModifiedDate = item.date_modified,
                    Logo = item.icon_url,
                    FileType = FileType.ModPack,
                    SourceType = SourceType.Modrinth,
                    Data = item
                });
            });

            return list1;
        }

        return null;
    }

    public static async Task<List<FileDisplayObj>?> GetPackFile(SourceType type, string id,
        int page, string? mc, Loaders loader, FileType type1 = FileType.ModPack)
    {
        if (type == SourceType.CurseForge)
        {
            var list = await CurseForgeAPI.GetCurseForgeFiles(id, mc, page, loader);
            if (list == null)
                return null;

            var list1 = new List<FileDisplayObj>();
            list.data.ForEach(item =>
            {
                list1.Add(new()
                {
                    ID = item.modId.ToString(),
                    ID1 = item.id.ToString(),
                    Name = item.displayName,
                    Size = UIUtils.MakeFileSize1(item.fileLength),
                    Download = item.downloadCount,
                    Time = DateTime.Parse(item.fileDate).ToString(),
                    FileType = type1,
                    SourceType = SourceType.CurseForge,
                    Data = item
                });
            });

            return list1;
        }
        else if (type == SourceType.Modrinth)
        {
            var list = await ModrinthAPI.GetFileVersions(id, mc, loader);
            if (list == null)
                return null;

            var list1 = new List<FileDisplayObj>();
            list.ForEach(item =>
            {
                var file = item.files.FirstOrDefault(a => a.primary);
                if (file == null)
                {
                    file = item.files[0];
                }
                list1.Add(new()
                {
                    ID = item.project_id,
                    ID1 = item.id,
                    Name = item.name,
                    Size = UIUtils.MakeFileSize1(file.size),
                    Download = item.downloads,
                    Time = DateTime.Parse(item.date_published).ToString(),
                    FileType = type1,
                    SourceType = SourceType.Modrinth,
                    Data = item
                });
            });

            return list1;
        }

        return null;
    }

    public static List<SourceType> GetSourceList(FileType type)
    {
        switch (type)
        {
            case FileType.Mod:
            case FileType.DataPacks:
            case FileType.Resourcepack:
                return new()
                {
                    SourceType.CurseForge,
                    SourceType.Modrinth,
                };
            case FileType.Shaderpack:
                return new()
                {
                    SourceType.Modrinth,
                };
            case FileType.World:
                return new()
                {
                    SourceType.CurseForge,
                };
            default:
                return new();
        }
    }

    public static async Task<List<FileItemDisplayObj>?> GetList(FileType now, SourceType type, string? version, string? filter, int page, int sort, string categoryId, Loaders loader)
    {
        version ??= "";
        filter ??= "";
        if (type == SourceType.CurseForge)
        {
            var list = now switch
            {
                FileType.Mod => await CurseForgeAPI.GetModList(version, page, filter: filter,
                    sortField: sort switch
                    {
                        0 => 1,
                        1 => 2,
                        2 => 3,
                        3 => 4,
                        4 => 6,
                        _ => 2
                    }, sortOrder: sort switch
                    {
                        0 => 1,
                        1 => 1,
                        2 => 1,
                        3 => 0,
                        4 => 1,
                        _ => 1
                    }, categoryId: categoryId, loader: loader),
                FileType.World => await CurseForgeAPI.GetWorldList(version, page, filter: filter,
                    sortField: sort switch
                    {
                        0 => 1,
                        1 => 2,
                        2 => 3,
                        3 => 4,
                        4 => 6,
                        _ => 2
                    }, sortOrder: sort switch
                    {
                        0 => 1,
                        1 => 1,
                        2 => 1,
                        3 => 0,
                        4 => 1,
                        _ => 1
                    }, categoryId: categoryId),
                FileType.Resourcepack => await CurseForgeAPI.GetResourcepackList(version, page, filter: filter,
                    sortField: sort switch
                    {
                        0 => 1,
                        1 => 2,
                        2 => 3,
                        3 => 4,
                        4 => 6,
                        _ => 2
                    }, sortOrder: sort switch
                    {
                        0 => 1,
                        1 => 1,
                        2 => 1,
                        3 => 0,
                        4 => 1,
                        _ => 1
                    }, categoryId: categoryId),
                FileType.DataPacks => await CurseForgeAPI.GetDataPacksList(version, page, filter: filter,
                    sortField: sort switch
                    {
                        0 => 1,
                        1 => 2,
                        2 => 3,
                        3 => 4,
                        4 => 6,
                        _ => 2
                    }, sortOrder: sort switch
                    {
                        0 => 1,
                        1 => 1,
                        2 => 1,
                        3 => 0,
                        4 => 1,
                        _ => 1
                    }),
                _ => null
            };
            if (list == null)
                return null;
            var list1 = new List<FileItemDisplayObj>();
            list.data.ForEach(item =>
            {
                list1.Add(new()
                {
                    ID = item.id.ToString(),
                    Name = item.name,
                    Summary = item.summary,
                    Author = item.authors.GetString(),
                    DownloadCount = item.downloadCount,
                    ModifiedDate = item.dateModified,
                    Logo = item.logo?.url,
                    FileType = now,
                    SourceType = SourceType.CurseForge,
                    Data = item
                });
            });

            return list1;
        }
        else if (type == SourceType.Modrinth)
        {
            var list = now switch
            {
                FileType.Mod => await ModrinthAPI.GetModList(version, page, filter: filter, sortOrder: sort, categoryId: categoryId, loader: loader),
                FileType.Resourcepack => await ModrinthAPI.GetResourcepackList(version, page, filter: filter, sortOrder: sort, categoryId: categoryId),
                FileType.DataPacks => await ModrinthAPI.GetDataPackList(version, page, filter: filter, sortOrder: sort, categoryId: categoryId),
                FileType.Shaderpack => await ModrinthAPI.GetShaderpackList(version, page, filter: filter, sortOrder: sort, categoryId: categoryId),
                _ => null
            };
            if (list == null)
                return null;
            var list1 = new List<FileItemDisplayObj>();
            list.hits.ForEach(item =>
            {
                list1.Add(new()
                {
                    ID = item.project_id,
                    Name = item.title,
                    Summary = item.description,
                    Author = item.author,
                    DownloadCount = item.downloads,
                    ModifiedDate = item.date_modified,
                    Logo = item.icon_url,
                    FileType = FileType.ModPack,
                    SourceType = SourceType.Modrinth,
                    Data = item
                });
            });

            return list1;
        }

        return null;
    }

    public static async Task<(DownloadItemObj? Item, ModInfoObj? Info, List<DownloadModDisplayObj>? List)> DownloadMod(GameSettingObj obj, CurseForgeObjList.Data.LatestFiles? data)
    {
        if (data == null)
            return (null, null, null);

        var res = new Dictionary<string, DownloadModDisplayObj>();
        if (data.dependencies != null && data.dependencies.Count > 0)
        {
            var res1 = await CurseForgeAPI.GetModDependencies(data, obj.Version, obj.Loader);

            foreach (var item1 in res1)
            {
                if (res.ContainsKey(item1.Info.ModId) || obj.Mods.ContainsKey(item1.Info.ModId))
                    continue;

                List<string> version = new();
                List<(DownloadItemObj Item, ModInfoObj Info)> items = new();
                foreach (var item2 in item1.List)
                {
                    version.Add(item2.displayName);
                    items.Add((item2.MakeModDownloadObj(obj), item2.MakeModInfo()));
                }
                res.Add(item1.Info.ModId, new()
                {
                    Download = false,
                    Name = item1.Info.Name,
                    ModVersion = version,
                    Items = items,
                    SelectVersion = 0
                });
            }
        }

        return (data.MakeModDownloadObj(obj), data.MakeModInfo(), res.Values.ToList());
    }

    public static async Task<(DownloadItemObj? Item, ModInfoObj? Info, List<DownloadModDisplayObj>? List)> DownloadMod(GameSettingObj obj, ModrinthVersionObj? data)
    {
        if (data == null)
            return (null, null, null);

        var res = new Dictionary<string, DownloadModDisplayObj>();
        if (data.dependencies != null && data.dependencies.Count > 0)
        {
            var list2 = await ModrinthAPI.GetModDependencies(data, obj.Version, obj.Loader);
            foreach (var item1 in list2)
            {
                if (res.ContainsKey(item1.Info.ModId) || obj.Mods.ContainsKey(item1.Info.ModId))
                    continue;
                List<string> version = new();
                List<(DownloadItemObj Item, ModInfoObj Info)> items = new();
                foreach (var item2 in item1.List)
                {
                    version.Add(item2.name);
                    items.Add((item2.MakeModDownloadObj(obj), item2.MakeModInfo()));
                }
                res.Add(item1.Info.ModId, new()
                {
                    Download = false,
                    Name = item1.Info.Name,
                    ModVersion = version,
                    Items = items,
                    SelectVersion = 0
                });
            }
        }

        return (data.MakeModDownloadObj(obj), data.MakeModInfo(), res.Values.ToList());
    }

    public static async Task<bool> DownloadMod(GameSettingObj obj, IList<(DownloadItemObj Item, ModInfoObj Info)> list)
    {
        bool res;

        foreach (var (Item, Info) in list)
        {
            foreach (var item1 in obj.Mods)
            {
                if (item1.Key == Info.ModId)
                {
                    File.Delete(Path.GetFullPath(obj.GetModsPath() + '/' + item1.Value.File));
                    break;
                }
            }
        }

        if (list.Count == 1)
        {
            var (Item, Info) = list[0];
            res = await DownloadManager.Download(Item);
            if (res)
            {
                obj.AddModInfo(Info);
            }
        }
        else
        {
            var list1 = new List<DownloadItemObj>();
            foreach (var (Item, Info) in list)
            {
                Item.Later = (s) =>
                {
                    obj.AddModInfo(Info);
                };
                list1.Add(Item);
            }
            res = await DownloadManager.Start(list1);
        }
        return res;
    }

    public static async Task<bool> Download(FileType type, GameSettingObj obj, CurseForgeObjList.Data.LatestFiles? data)
    {
        if (data == null)
            return false;

        data.FixDownloadUrl();
        bool res;
        DownloadItemObj item;
        switch (type)
        {
            case FileType.World:
                item = new DownloadItemObj()
                {
                    Name = data.displayName,
                    Url = data.downloadUrl,
                    Local = Path.GetFullPath(DownloadManager.DownloadDir + "/" + data.fileName),
                    SHA1 = data.hashes.Where(a => a.algo == 1)
                        .Select(a => a.value).FirstOrDefault(),
                    Overwrite = true
                };

                res = await DownloadManager.Download(item);
                if (!res)
                {
                    return false;
                }

                return await GameBinding.AddWorld(obj, item.Local);
            case FileType.Resourcepack:
                item = new DownloadItemObj()
                {
                    Name = data.displayName,
                    Url = data.downloadUrl,
                    Local = Path.GetFullPath(obj.GetResourcepacksPath() + "/" + data.fileName),
                    SHA1 = data.hashes.Where(a => a.algo == 1)
                        .Select(a => a.value).FirstOrDefault(),
                    Overwrite = true
                };

                return await DownloadManager.Download(item);
            default:
                return false;
        }
    }

    public static async Task<bool> Download(FileType type, GameSettingObj obj, ModrinthVersionObj? data)
    {
        if (data == null)
            return false;

        var file = data.files.FirstOrDefault(a => a.primary);
        if (file == null)
        {
            file = data.files[0];
        }

        DownloadItemObj item;

        switch (type)
        {
            case FileType.Resourcepack:
                item = new DownloadItemObj()
                {
                    Name = data.name,
                    Url = file.url,
                    Local = Path.GetFullPath(obj.GetResourcepacksPath() + "/" + file.filename),
                    SHA1 = file.hashes.sha1,
                    Overwrite = true
                };

                return await DownloadManager.Download(item);
            case FileType.Shaderpack:
                item = new DownloadItemObj()
                {
                    Name = data.name,
                    Url = file.url,
                    Local = Path.GetFullPath(obj.GetShaderpacksPath() + "/" + file.filename),
                    SHA1 = file.hashes.sha1,
                    Overwrite = true
                };

                return await DownloadManager.Download(item);
            default:
                return false;
        }
    }

    public static async Task<bool> Download(WorldObj obj1, CurseForgeObjList.Data.LatestFiles? data)
    {
        if (data == null)
            return false;

        data.FixDownloadUrl();

        var item = new DownloadItemObj()
        {
            Name = data.displayName,
            Url = data.downloadUrl,
            Local = Path.GetFullPath(obj1.GetWorldDataPacksPath() + "/" + data.fileName),
            SHA1 = data.hashes.Where(a => a.algo == 1)
                .Select(a => a.value).FirstOrDefault(),
            Overwrite = true
        };

        return await DownloadManager.Download(item);
    }

    public static async Task<bool> Download(WorldObj obj1, ModrinthVersionObj? data)
    {
        if (data == null)
            return false;

        var file = data.files.FirstOrDefault(a => a.primary);
        if (file == null)
        {
            file = data.files[0];
        }

        var item = new DownloadItemObj()
        {
            Name = data.name,
            Url = file.url,
            Local = Path.GetFullPath(obj1.GetWorldDataPacksPath() + "/" + file.filename),
            SHA1 = file.hashes.sha1,
            Overwrite = true
        };

        return await DownloadManager.Download(item);
    }

    public static string? GetUrl(this FileItemDisplayObj obj)
    {
        if (obj.SourceType == SourceType.CurseForge)
        {
            return (obj.Data as CurseForgeObjList.Data)!.links.websiteUrl;
        }
        else if (obj.SourceType == SourceType.Modrinth)
        {
            var obj1 = (obj.Data as ModrinthSearchObj.Hit)!;
            return obj.FileType switch
            {
                FileType.ModPack => "https://modrinth.com/modpack/",
                FileType.Shaderpack => "https://modrinth.com/shaders/",
                FileType.Resourcepack => "https://modrinth.com/resourcepacks/",
                FileType.DataPacks => "https://modrinth.com/datapacks/",
                _ => "https://modrinth.com/mod/"
            } + obj1.project_id;
        }

        return null;
    }

    public static async Task<List<OptifineDisplayObj>?> GetOptifine()
    {
        var res = await OptifineAPI.GetOptifineVersion();
        if (res.Item1 == null)
            return null;

        var list = new List<OptifineDisplayObj>();
        res.Item2!.ForEach(item =>
        {
            list.Add(new()
            {
                MC = item.MCVersion,
                Version = item.Version,
                Forge = item.Forge,
                Date = item.Date,
                Data = item
            });
        });

        return list;
    }

    public static Task<(bool, string?)> DownloadOptifine(GameSettingObj obj, OptifineDisplayObj item)
    {
        return OptifineAPI.DownloadOptifine(obj, item.Data);
    }

    public static List<string> GetFTBTypeList()
    {
        return new()
        {
            FTBType.All.GetName(),
            FTBType.Featured.GetName(),
            FTBType.Popular.GetName(),
            FTBType.Installs.GetName(),
            FTBType.Search.GetName()
        };
    }

    public static async Task<List<(DownloadItemObj Item, ModInfoObj Info)>> CheckModUpdate(GameSettingObj game, List<ModDisplayObj> mods)
    {
        var list = new ConcurrentBag<(DownloadItemObj Item, ModInfoObj Info)>();
        await Parallel.ForEachAsync(mods, async (item, cancel) =>
        {
            try
            {
                if (string.IsNullOrWhiteSpace(item.PID) || string.IsNullOrWhiteSpace(item.FID))
                    return;

                var type1 = Funtcions.CheckNotNumber(item.PID) || Funtcions.CheckNotNumber(item.FID) ?
                   SourceType.Modrinth : SourceType.CurseForge;

                var list1 = await GetPackFile(type1, item.PID, 0,
                   game.Version, game.Loader, FileType.Mod);
                if (list1 == null || list1.Count == 0)
                    return;
                var item1 = list1.First();
                if (item1.ID1 != item.FID)
                {
                    switch (type1)
                    {
                        case SourceType.CurseForge:
                            if (item1.Data is CurseForgeObjList.Data.LatestFiles data)
                            {
                                list.Add((data.MakeModDownloadObj(game), data.MakeModInfo()));
                            }
                            break;
                        case SourceType.Modrinth:
                            if (item1.Data is ModrinthVersionObj data1)
                            {
                                list.Add((data1.MakeModDownloadObj(game), data1.MakeModInfo()));
                            }
                            break;
                    }
                    Dispatcher.UIThread.Post(() =>
                    {
                        item.New = true;
                    });
                }
            }
            catch (Exception e)
            {
                Logs.Error(App.GetLanguage("Gui.Error16"), e);
            }
        });

        return list.ToList();
    }

    public static void OpenMcmod(ModDisplayObj obj)
    {
        BaseBinding.OpUrl($"https://search.mcmod.cn/s?key={obj.Name}");
    }
}
