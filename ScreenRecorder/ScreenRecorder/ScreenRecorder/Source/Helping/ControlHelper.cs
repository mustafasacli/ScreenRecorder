using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ScreenRecorder.Source.Helping
{
    public static class ControlHelper
    {

        public static void SetEnableAndVisibility(this Control ctrl, bool state)
        {
            ctrl.Enabled = state;
            ctrl.Visible = state;
        }

        public static void SetEnableAndVisibility(this MenuItem ctrl, bool state)
        {
            ctrl.Enabled = state;
            ctrl.Visible = state;
        }
    }
}
