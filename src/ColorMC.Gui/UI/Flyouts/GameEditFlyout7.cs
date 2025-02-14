﻿using ColorMC.Gui.Objs;
using ColorMC.Gui.UI.Controls.GameEdit;
using ColorMC.Gui.UIBinding;

namespace ColorMC.Gui.UI.Flyouts;

public class GameEditFlyout7
{
    private readonly SchematicDisplayObj Obj;
    private readonly Tab12Control Con;
    public GameEditFlyout7(Tab12Control con, SchematicDisplayObj obj)
    {
        Con = con;
        Obj = obj;

        var fy = new FlyoutsControl(new()
        {
            (App.GetLanguage("Button.OpFile"), true, Button1_Click),
            (App.GetLanguage("Button.Delete"), true, Button2_Click)
        }, con);
    }

    private void Button1_Click()
    {
        BaseBinding.OpFile(Obj.Local);
    }

    private void Button2_Click()
    {
        Con.Delete(Obj);
    }
}