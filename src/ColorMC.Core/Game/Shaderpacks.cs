using ColorMC.Core.LaunchPath;
using ColorMC.Core.Objs;
using ColorMC.Core.Objs.Minecraft;
using ColorMC.Core.Utils;

namespace ColorMC.Core.Game;

public static class Shaderpacks
{
    /// <summary>
    /// 获取游戏实例资源包
    /// </summary>
    /// <param name="game">游戏实例</param>
    /// <returns>资源包列表</returns>
    public static List<ShaderpackObj> GetShaderpacks(this GameSettingObj game)
    {
        var list = new List<ShaderpackObj>();
        var dir = game.GetResourcepacksPath();

        DirectoryInfo info = new(dir);
        if (!info.Exists)
        {
            info.Create();
            return list;
        }

        Parallel.ForEach(info.GetFiles(), (item) =>
        {
            if (item.Extension is not ".zip")
                return;
            var obj1 = new ShaderpackObj()
            {
                Local = Path.GetFullPath(item.FullName)
            };
            list.Add(obj1);
        });

        return list;
    }

    public static void Delete(this ShaderpackObj pack)
    {
        File.Delete(pack.Local);
    }

    public static async Task<bool> AddShaderpack(this GameSettingObj obj, List<string> file)
    {
        var dir = obj.GetResourcepacksPath();
        Directory.CreateDirectory(dir);
        bool ok = true;

        foreach (var item in file)
        {
            var name = Path.GetFileName(item);
            var name1 = Path.GetFullPath(dir + "/" + name);

            if (File.Exists(name1))
                return false;

            await Task.Run(() =>
            {
                try
                {
                    File.Copy(item, name1);
                }
                catch (Exception e)
                {
                    Logs.Error(LanguageHelper.GetName("Core.Game.Error3"), e);
                    ok = false;
                }
            });
            if (!ok)
                return false;
        }
        return true;
    }

    //public static void Disable(this ShaderpackObj pack)
    //{
    //    if (pack.Disable)
    //        return;

    //    var file = new FileInfo(pack.Local);
    //    pack.Disable = true;
    //    pack.Local = Path.GetFullPath($"{file.DirectoryName}/{file.Name
    //        .Replace(".zip", ".disable")}");
    //    File.Move(file.FullName, pack.Local);
    //}

    //public static void Enable(this ShaderpackObj pack)
    //{
    //    if (!pack.Disable)
    //        return;

    //    var file = new FileInfo(pack.Local);
    //    pack.Disable = false;
    //    pack.Local = Path.GetFullPath($"{file.DirectoryName}/{file.Name
    //        .Replace(".disable", ".zip")}");
    //    File.Move(file.FullName, pack.Local);
    //}
}
