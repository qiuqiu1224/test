using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sunny.UI;

namespace WindowsFormsApp1
{
    public partial class FMonitor : UIPage
    {
        public FMonitor()
        {
            InitializeComponent();
        }

        private void UpButton_MouseDown(object sender, MouseEventArgs e)
        {
            XGNetSDK.NET_DVR_PTZControl_Other(FormMain.m_UserIDs[0], 1, (uint)PTZ_CTRL_CMD.PTZ_UP, 0);
            //XGNetSDK.NET_DVR_PTZControlWithSpeed_Other(FormMain.m_UserIDs[0], 1, (uint)PTZ_CTRL_CMD.PTZ_UP, 0,4);
        }

        private void UpButton_MouseUp(object sender, MouseEventArgs e)
        {
            XGNetSDK.NET_DVR_PTZControl_Other(FormMain.m_UserIDs[0], 1, (uint)PTZ_CTRL_CMD.PTZ_UP, 1);
            //XGNetSDK.NET_DVR_PTZControlWithSpeed_Other(FormMain.m_UserIDs[0], 1, (uint)PTZ_CTRL_CMD.PTZ_UP, 1,4);
        }

        private void DownButton_MouseDown(object sender, MouseEventArgs e)
        {
            XGNetSDK.NET_DVR_PTZControl_Other(FormMain.m_UserIDs[0], 1, (uint)PTZ_CTRL_CMD.PTZ_DOWN, 0);
        }

        private void DownButton_MouseUp(object sender, MouseEventArgs e)
        {
            XGNetSDK.NET_DVR_PTZControl_Other(FormMain.m_UserIDs[0], 1, (uint)PTZ_CTRL_CMD.PTZ_DOWN, 1);
        }

        private void LeftButton_MouseDown(object sender, MouseEventArgs e)
        {
            XGNetSDK.NET_DVR_PTZControl_Other(FormMain.m_UserIDs[0], 1, (uint)PTZ_CTRL_CMD.PTZ_LEFT, 0);
        }

        private void LeftButton_MouseUp(object sender, MouseEventArgs e)
        {
            XGNetSDK.NET_DVR_PTZControl_Other(FormMain.m_UserIDs[0], 1, (uint)PTZ_CTRL_CMD.PTZ_LEFT, 1);
        }

        private void RightButton_MouseDown(object sender, MouseEventArgs e)
        {
            XGNetSDK.NET_DVR_PTZControl_Other(FormMain.m_UserIDs[0], 1, (uint)PTZ_CTRL_CMD.PTZ_RIGHT, 0);
        }

        private void RightButton_MouseUp(object sender, MouseEventArgs e)
        {
            XGNetSDK.NET_DVR_PTZControl_Other(FormMain.m_UserIDs[0], 1, (uint)PTZ_CTRL_CMD.PTZ_RIGHT, 1);
        }
    }
}
