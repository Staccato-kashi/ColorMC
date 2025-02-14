﻿using ColorMC.Core.Helpers;
using ColorMC.Core.Net;
using ColorMC.Core.Net.Downloader;
using ColorMC.Core.Objs;
using ColorMC.Core.Utils;
using ColorMC.Gui.Objs;
using ColorMC.Gui.Utils;
using ColorMC.Gui.Utils.LaunchSetting;
using System.Threading.Tasks;

namespace ColorMC.Gui.UIBinding;

public static class ConfigBinding
{
    /// <summary>
    /// 加载账户数据库
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static bool LoadAuthDatabase(string dir)
    {
        return AuthDatabase.LoadData(dir);
    }

    /// <summary>
    /// 加载配置文件
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static bool LoadConfig(string dir)
    {
        return ConfigUtils.Load(dir, true);
    }
    /// <summary>
    /// 加载图形配置文件
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static bool LoadGuiConfig(string dir)
    {
        var res = GuiConfigUtils.Load(dir, true);
        if (res)
        {
            ColorSel.Instance.Load();
            FontSel.Instance.Load();
        }

        return res;
    }

    /// <summary>
    /// 获取所有配置文件
    /// </summary>
    /// <returns></returns>
    public static (ConfigObj, GuiConfigObj) GetAllConfig()
    {
        return (ConfigUtils.Config, GuiConfigUtils.Config);
    }

    /// <summary>
    /// 设置RGB模式
    /// </summary>
    /// <param name="enable"></param>
    public static void SetRgb(bool enable)
    {
        GuiConfigUtils.Config.RGB = enable;

        GuiConfigUtils.Save();
        ColorSel.Instance.Load();
    }

    /// <summary>
    /// 设置RGB值
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    public static void SetRgb(int v1, int v2)
    {
        GuiConfigUtils.Config.RGBS = v1;
        GuiConfigUtils.Config.RGBV = v2;
        GuiConfigUtils.Save();
        ColorSel.Instance.Load();
    }

    /// <summary>
    /// 设置启动器颜色
    /// </summary>
    /// <param name="main"></param>
    /// <param name="back"></param>
    /// <param name="back1"></param>
    /// <param name="font1"></param>
    /// <param name="font2"></param>
    /// <param name="back2"></param>
    /// <param name="back3"></param>
    /// <param name="font3"></param>
    /// <param name="font4"></param>
    public static void SetColor(string main, string back, string back1, string font1, string font2, string back2, string back3, string font3, string font4)
    {
        GuiConfigUtils.Config.ColorMain = main;
        GuiConfigUtils.Config.ColorLight.ColorBack = back;
        GuiConfigUtils.Config.ColorLight.ColorTranBack = back1;
        GuiConfigUtils.Config.ColorLight.ColorFont1 = font1;
        GuiConfigUtils.Config.ColorLight.ColorFont2 = font2;
        GuiConfigUtils.Config.ColorDark.ColorBack = back2;
        GuiConfigUtils.Config.ColorDark.ColorTranBack = back3;
        GuiConfigUtils.Config.ColorDark.ColorFont1 = font3;
        GuiConfigUtils.Config.ColorDark.ColorFont2 = font4;
        GuiConfigUtils.Save();
        ColorSel.Instance.Load();
    }

    /// <summary>
    /// 重置启动器颜色
    /// </summary>
    public static void ResetColor()
    {
        SetColor(
            ColorSel.MainColorStr,
            ColorSel.BackLigthColorStr,
            ColorSel.Back1LigthColorStr,
            ColorSel.ButtonLightFontStr,
            ColorSel.FontLigthColorStr,
            ColorSel.BackDarkColorStr,
            ColorSel.Back1DarkColorStr,
            ColorSel.ButtonDarkFontStr,
            ColorSel.FontDarkColorStr
        );
    }

    /// <summary>
    /// 删除背景图片
    /// </summary>
    public static void DeleteGuiImageConfig()
    {
        App.RemoveImage();
        GuiConfigUtils.Config.BackImage = null;
        GuiConfigUtils.Save();
        App.OnPicUpdate();
    }

    /// <summary>
    /// 设置背景图片
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static async Task SetBackPic(string dir, int data)
    {
        GuiConfigUtils.Config.BackEffect = data;
        GuiConfigUtils.Config.BackImage = dir;
        GuiConfigUtils.Save();

        await App.LoadImage();

        App.OnPicUpdate();
    }

    /// <summary>
    /// 设置背景图片分辨率限制
    /// </summary>
    /// <param name="enable"></param>
    /// <param name="pix"></param>
    /// <returns></returns>
    public static async Task SetBackLimit(bool enable, int pix)
    {
        GuiConfigUtils.Config.BackLimitValue = pix;
        GuiConfigUtils.Config.BackLimit = enable;
        GuiConfigUtils.Save();

        await App.LoadImage();

        App.OnPicUpdate();
    }

    /// <summary>
    /// 设置背景图片透明度
    /// </summary>
    /// <param name="data"></param>
    public static void SetBackTran(int data)
    {
        GuiConfigUtils.Config.BackTran = data;
        GuiConfigUtils.Save();

        App.OnPicUpdate();
    }

    /// <summary>
    /// 设置窗口透明
    /// </summary>
    /// <param name="open"></param>
    /// <param name="type"></param>
    public static void SetWindowTran(bool open, int type)
    {
        GuiConfigUtils.Config.WindowTranType = type;
        GuiConfigUtils.Config.WindowTran = open;
        GuiConfigUtils.Save();

        App.OnPicUpdate();
    }

    /// <summary>
    /// 设置网络参数
    /// </summary>
    /// <param name="obj"></param>
    public static void SetHttpConfig(HttpObj obj)
    {
        if (DownloadManager.State != CoreRunState.End)
            return;

        ConfigUtils.Config.Http = obj;
        ConfigUtils.Save();

        BaseClient.Init();
    }

    /// <summary>
    /// 设置Jvm参数
    /// </summary>
    /// <param name="obj"></param>
    public static void SetJvmArgConfig(JvmArgObj obj)
    {
        ConfigUtils.Config.DefaultJvmArg = obj;
        ConfigUtils.Save();

        BaseClient.Init();
    }

    /// <summary>
    /// 设置窗口设置
    /// </summary>
    /// <param name="obj"></param>
    public static void SetWindowSettingConfig(WindowSettingObj obj)
    {
        ConfigUtils.Config.Window = obj;
        ConfigUtils.Save();

        BaseClient.Init();
    }

    /// <summary>
    /// 设置服务器自定义参数
    /// </summary>
    /// <param name="obj"></param>
    public static void SetServerCustom(ServerCustom obj)
    {
        GuiConfigUtils.Config.ServerCustom = obj;
        GuiConfigUtils.Save();

        App.MainWindow?.Load();

        ColorSel.Instance.Load();
    }

    /// <summary>
    /// 设置游戏检查选项
    /// </summary>
    /// <param name="obj"></param>
    public static void SetGameCheckConfig(GameCheckObj obj)
    {
        ConfigUtils.Config.GameCheck = obj;
        ConfigUtils.Save();
    }

    /// <summary>
    /// 设置字体设置
    /// </summary>
    /// <param name="name"></param>
    /// <param name="def"></param>
    public static void SetFont(string? name, bool def)
    {
        GuiConfigUtils.Config.FontName = name;
        GuiConfigUtils.Config.FontDefault = def;

        GuiConfigUtils.Save();

        FontSel.Instance.Load();
    }

    /// <summary>
    /// 重置配置文件
    /// </summary>
    public static void ResetConfig()
    {
        GuiConfigUtils.Config = GuiConfigUtils.MakeDefaultConfig();

        GuiConfigUtils.Save();
    }

    /// <summary>
    /// 设置启动后关闭启动器
    /// </summary>
    /// <param name="value"></param>
    public static void SetLaunchCloseConfig(bool value)
    {
        GuiConfigUtils.Config.CloseBeforeLaunch = value;

        GuiConfigUtils.Save();
    }

    /// <summary>
    /// 设置窗口模式
    /// </summary>
    /// <param name="value"></param>
    public static void SetWindowMode(bool value)
    {
        GuiConfigUtils.Config.WindowMode = value;

        GuiConfigUtils.Save();
    }

    /// <summary>
    /// 获取窗口模式
    /// </summary>
    /// <returns>true为单窗口false为多窗口</returns>
    public static bool WindowMode()
    {
        return GuiConfigUtils.Config.WindowMode || SystemInfo.Os == OsType.Android;
    }

    /// <summary>
    /// 设置语言
    /// </summary>
    /// <param name="type"></param>
    public static void SetLanguage(LanguageType type)
    {
        ConfigUtils.Config.Language = type;
        ConfigUtils.Save();

        LanguageHelper.Change(type);
    }

    /// <summary>
    /// 设置主题色
    /// </summary>
    /// <param name="type"></param>
    public static void SetColorType(ColorType type)
    {
        GuiConfigUtils.Config.ColorType = type;
        GuiConfigUtils.Save();

        App.ColorChange();
        ColorSel.Instance.Load();
        App.OnPicUpdate();
    }

    /// <summary>
    /// 设置上次启动的游戏实例
    /// </summary>
    /// <param name="uuid"></param>
    public static void SetLastLaunch(string uuid)
    {
        GuiConfigUtils.Config.LastLaunch = uuid;
        GuiConfigUtils.Save();
    }

    /// <summary>
    /// 获取上次运行的游戏实例
    /// </summary>
    /// <returns></returns>
    public static string? GetLastLaunch()
    {
        return GuiConfigUtils.Config.LastLaunch;
    }
}
