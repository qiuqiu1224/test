using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class GDIHelper
    {
        [DllImport(@".\Render.dll", EntryPoint = "?initRender@@YA_NPEAUHWND__@@HH@Z")]
        public static extern bool initRender(IntPtr hwndex, int width, int height);
        [DllImport(@".\Render.dll", EntryPoint = "?releaseRender@@YA_NXZ")]
        public static extern bool releaseRender();
        [DllImport(@".\Render.dll", EntryPoint = "?RenderPlay@@YA_NPEAEH@Z")]
        public static extern bool RenderPlay(IntPtr buffer, int buffsize);
    }
}
