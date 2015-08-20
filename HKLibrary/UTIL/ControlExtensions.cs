using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;


public static class ControlExtensions
{
    public static void InvokeIfNeeded(this Control control, System.Action action)
    {
        if (control.InvokeRequired)
            control.Invoke(action);
        else
            action();

    }

    public static void InvokeIfNeeded<T>(this Control control, Action<T> action, T arg)
    {
        if (control.InvokeRequired)
            control.Invoke(action, arg);
        else
            action(arg);

    }
}
