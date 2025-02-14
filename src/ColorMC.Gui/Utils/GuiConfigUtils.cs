﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Media.Immutable;
using Avalonia.VisualTree;
using ColorMC.Core;
using ColorMC.Core.Net;
using ColorMC.Core.Objs;
using ColorMC.Core.Utils;
using ColorMC.Gui.Objs;
using ColorMC.Gui.Utils.LaunchSetting;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ColorMC.Gui.Utils;

public static class GuiConfigUtils
{
    public static GuiConfigObj Config { get; set; }

    private static string Name;

    public static void Init(string dir)
    {
        Name = dir + "gui.json";

        Load(Name);
    }

    public static bool Load(string name, bool quit = false)
    {
        if (File.Exists(name))
        {
            try
            {
                Config = JsonConvert.DeserializeObject<GuiConfigObj>(File.ReadAllText(name))!;
            }
            catch (Exception e)
            {
                Logs.Error(App.GetLanguage("Gui.Error17"), e);
            }

            if (Config == null)
            {
                if (quit)
                {
                    return false;
                }

                Config = MakeDefaultConfig();

                SaveNow();
            }

            if (Config.ServerCustom == null)
            {
                if (quit)
                {
                    return false;
                }

                Config.ServerCustom = MakeServerCustomConfig();

                Save();
            }

            if (Config.Render == null
                || Config.Render.Windows == null
                || Config.Render.X11 == null)
            {
                if (quit)
                {
                    return false;
                }

                Config.Render = MakeRenderConfig();

                Save();
            }

            if (Config.ColorLight == null)
            {
                Config.ColorLight = MakeColorLightConfig();
            }

            if (Config.ColorDark == null)
            {
                Config.ColorDark = MakeColorDarkConfig();
            }

            if (SystemInfo.Os == OsType.Linux && Config.WindowMode)
            {
                Config.WindowMode = false;
            }
        }
        else
        {
            Config = MakeDefaultConfig();

            SaveNow();
        }

        return true;
    }

    public static void SaveNow()
    {
        File.WriteAllText(Name,
                    JsonConvert.SerializeObject(Config, Formatting.Indented));
    }

    public static void Save()
    {
        ConfigSave.AddItem(new()
        {
            Name = "gui.json",
            Local = Name,
            Obj = Config
        });
    }

    public static Render MakeRenderConfig()
    {
        return new()
        {
            Windows = new()
            {
                UseWindowsUIComposition = null,
                UseWgl = null,
                AllowEglInitialization = null
            },
            X11 = new()
            {
                UseEGL = null,
                UseGpu = null,
                OverlayPopups = null
            }
        };
    }

    public static GuiConfigObj MakeDefaultConfig()
    {
        return new()
        {
            Version = ColorMCCore.Version,
            ColorMain = ColorSel.MainColorStr,
            ColorLight = MakeColorLightConfig(),
            ColorDark = MakeColorDarkConfig(),
            RGBS = 100,
            RGBV = 100,
            ServerCustom = MakeServerCustomConfig(),
            FontDefault = true,
            Render = MakeRenderConfig(),
            BackLimitValue = 50
        };
    }

    public static ColorSetting MakeColorLightConfig()
    {
        return new()
        {
            ColorBack = ColorSel.BackLigthColorStr,
            ColorTranBack = ColorSel.Back1LigthColorStr,
            ColorFont1 = ColorSel.ButtonLightFontStr,
            ColorFont2 = ColorSel.FontLigthColorStr,
        };
    }

    public static ColorSetting MakeColorDarkConfig()
    {
        return new()
        {
            ColorBack = ColorSel.BackDarkColorStr,
            ColorTranBack = ColorSel.Back1DarkColorStr,
            ColorFont1 = ColorSel.ButtonDarkFontStr,
            ColorFont2 = ColorSel.FontDarkColorStr,
        };
    }

    public static ServerCustom MakeServerCustomConfig()
    {
        return new()
        {
            MotdColor = "#FFFFFFFF",
            MotdBackColor = "#FF000000",
            Volume = 30
        };
    }
}