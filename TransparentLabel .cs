using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
/// <summary>
/// 背景透明的Label控件
/// </summary>
namespace WindowsFormsApp1
{
    public class TransparentLabel :Label

    {
        protected override CreateParams CreateParams
        {
            get
            {
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);

                const int WS_EX_TRANSPARENT = 0x20;
                CreateParams result = base.CreateParams;
                result.ExStyle = result.ExStyle | WS_EX_TRANSPARENT;
                return result;
            }
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // do nothing
        }

    }
}
