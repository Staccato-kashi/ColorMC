﻿using ColorMC.Core.Objs.Loader;
using Newtonsoft.Json;

namespace ColorMC.Core.Http;

public static class FabricHelper
{
    public static async Task<FabricMetaObj?> GetMeta(SourceLocal? local = null)
    {
        try
        {
            var data = await BaseClient.GetString(UrlHelp.FabricMeta(local));
            if (string.IsNullOrWhiteSpace(data))
                return null;
            return JsonConvert.DeserializeObject<FabricMetaObj>(data);
        }
        catch (Exception e)
        {
            Logs.Error("获取版本信息发生错误", e);
            return null;
        }
    }

    public static async Task<FabricLoaderObj?> GetLoader(string mc, string version, SourceLocal? local = null)
    {
        try
        {
            string url = $"{UrlHelp.FabricMeta(local)}/loader/{mc}/{version}/profile/json";
            var data = await BaseClient.GetString(url);
            if (string.IsNullOrWhiteSpace(data))
                return null;
            return JsonConvert.DeserializeObject<FabricLoaderObj>(data);
        }
        catch (Exception e)
        {
            Logs.Error("获取版本信息发生错误", e);
            return null;
        }
    }

    public static async Task<List<string>?> GetLoaders(string mc, SourceLocal? local = null)
    {
        try
        {
            string url = $"{UrlHelp.FabricMeta(local)}/loader/{mc}";
            var data = await BaseClient.GetString(url);
            if (string.IsNullOrWhiteSpace(data))
                return null;

            var list = JsonConvert.DeserializeObject<List<FabricLoaderObj1>>(data);
            if (list == null)
                return null;

            var list1 = new List<string>();
            foreach (var item in list)
            {
                list1.Add(item.loader.version);
            }
            return list1;
        }
        catch (Exception e)
        {
            Logs.Error("获取版本信息发生错误", e);
            return null;
        }
    }

    public static async Task<List<string>?> GetSupportVersion(SourceLocal? local = null)
    {
        try
        {
            string url = $"{UrlHelp.FabricMeta(local)}/game";
            var data = await BaseClient.GetString(url);
            if (string.IsNullOrWhiteSpace(data))
                return null;

            var list = JsonConvert.DeserializeObject<List<FabricMetaObj.Game>>(data);
            if (list == null)
                return null;

            var list1 = new List<string>();
            foreach(var item in list)
            {
                list1.Add(item.version);    
            }

            return list1;
        }
        catch (Exception e)
        {
            Logs.Error("获取版本信息发生错误", e);
            return null;
        }
    }
}
