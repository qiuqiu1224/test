using Sunny.UI;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Threading;
using System.Media;

namespace WindowsFormsApp1
{

    public partial class FormMain : UIForm
    {

        [DllImport("user32.dll", EntryPoint = "PostMessage")]
        private static extern int PostMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        const uint DISPLAYWND_GAP = 1;//监控画面的间隙
        const uint DISPLAYWND_MARGIN_LEFT = 20;//监控画面距离左边控件的距离
        const uint DISPLAYWND_MARGIN_TOP = 20; //监控画面距离上边的距离
        const int PAGE_INDEX = 1000;

        private PictureBox[] pics;
        public static TransparentLabel[] labels;

        private UIPage fmonitor;//监控界面

        private string str;
        private uint iLastErr = 0;
        //private Int32 m_lUserID = -1;
        //private long m_lRealHandle = -1;
        //private long m_lRealHandle1 = -1;
        //private IntPtr m_ptrRealHandle;
        //private IntPtr m_ptrRealHandle1;
        public XGNetSDK.NET_DVR_USER_LOGIN_INFO struLogInfo;
        XGNetSDK.LOGINRESULTCALLBACK LoginCallBack = null;
        XGNetSDK.TEMRESULTCALLBACK temresultCallBack = null;//获取一帧红外测温结果回调函数
        XGNetSDK.TEMRESULTCALLBACK temresultCallBack1 = null;//获取一帧红外测温结果回调函数
        public XGNetSDK.NET_DVR_DEVICEINFO_V40 DeviceInfo;
        //XGNetSDK.REALDATACALLBACK RealData = null;
        //XGNetSDK.REALDATACALLBACK RealData1 = null;

        public static Int32[] m_UserIDs;
        XGNetSDK.REALDATACALLBACK[] RealDatas;
        private long[] m_RealHandles;
        GetTempUtil getTempUtil = new GetTempUtil();
        GetTempUtil getTempUtil1 = new GetTempUtil();

        public Boolean saveImageFlag = false;


        public FormMain()
        {
            InitializeComponent();

            Control.CheckForIllegalCrossThreadCalls = false;

            XGNetSDK.NET_DVR_Init();  //SDK初始化
            string logPath = Application.StartupPath + "\\" + "Log";
            XGNetSDK.NET_DVR_SetLogToFile(3, logPath, true);//保存SDK日志

            int pageIndex = PAGE_INDEX;

            //设置关联
            uiNavBar1.TabControl = uiTabControl1;

            //uiNavBar1设置节点，也可以在Nodes属性里配置
            uiNavBar1.Nodes.Add("红外监控");
            uiNavBar1.SetNodeSymbol(uiNavBar1.Nodes[0], 61501);

            //添加实时监控界面
            fmonitor = new FMonitor();
            AddPage(fmonitor, pageIndex);
            uiNavBar1.SetNodePageIndex(uiNavBar1.Nodes[0], pageIndex);

            uiNavBar1.Nodes.Add("图像浏览");
            uiNavBar1.SetNodeSymbol(uiNavBar1.Nodes[1], 61502);

            uiNavBar1.Nodes.Add("系统设置");
            uiNavBar1.SetNodeSymbol(uiNavBar1.Nodes[2], 61459);
            StyleManager.Style = UIStyle.Blue;

            //设置默认显示界面
            uiNavBar1.SelectedIndex = 0;

            //读取配置文件
            Globals.ReadInfoXml<SystemParam>(ref Globals.systemParam, Globals.systemXml);

            initDatas();

            SetFmonitorDisplayWnds(2, 2);

            //labels[0].Text = Globals.systemParam.name_1;
            //labels[1].Text = Globals.systemParam.name_2;
            //labels[2].Text = Globals.systemParam.name_3;
            //labels[3].Text = Globals.systemParam.name_4;


            LoginDevice(1, "10.19.221.14", "admin", "admin", "8903", cbLoginCallBack1);
            Thread.Sleep(100);


            previewImage(1, 2, 1, pics[2], RealDataCallBack2);
            previewImage(1, 3, 2, pics[3], RealDataCallBack3);


            LoginDevice(0, "10.19.221.138", "admin", "admin", "8903", cbLoginCallBack);
            Thread.Sleep(100);

            previewImage(0, 0, 1, pics[0], RealDataCallBack);
            previewImage(0, 1, 2, pics[1], RealDataCallBack1);


        }

        /// <summary>
        /// 登录设备
        /// </summary>
        /// <param name="deviceNum">设备号 从0开始</param>
        /// <param name="ip">ip地址</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="port">端口号</param>
        /// <param name="loginCallBack">登录回调函数</param>
        private void LoginDevice(Int16 deviceNum, String ip, String userName, String password, String port, XGNetSDK.LOGINRESULTCALLBACK loginCallBack)
        {
            if (m_UserIDs[deviceNum] < 0)
            {
                struLogInfo = new XGNetSDK.NET_DVR_USER_LOGIN_INFO();

                //设备IP地址或者域名
                byte[] byIP = System.Text.Encoding.Default.GetBytes(ip);
                struLogInfo.sDeviceAddress = new byte[129];
                byIP.CopyTo(struLogInfo.sDeviceAddress, 0);

                //设备用户名
                byte[] byUserName = System.Text.Encoding.Default.GetBytes(userName);
                struLogInfo.sUserName = new byte[64];
                byUserName.CopyTo(struLogInfo.sUserName, 0);

                //设备密码
                byte[] byPassword = System.Text.Encoding.Default.GetBytes(password);
                struLogInfo.sPassword = new byte[64];
                byPassword.CopyTo(struLogInfo.sPassword, 0);

                struLogInfo.wPort = ushort.Parse(port);//设备服务端口号

                if (LoginCallBack == null)
                {
                    LoginCallBack = new XGNetSDK.LOGINRESULTCALLBACK(loginCallBack);//注册回调函数                    
                }

                struLogInfo.cbLoginResult = LoginCallBack;
                struLogInfo.bUseAsynLogin = false; //是否异步登录：0- 否，1- 是 

                DeviceInfo = new XGNetSDK.NET_DVR_DEVICEINFO_V40();

                //登录设备 Login the device
                m_UserIDs[deviceNum] = XGNetSDK.NET_DVR_Login(ref struLogInfo, ref DeviceInfo);
                if (m_UserIDs[deviceNum] < 0)
                {
                    iLastErr = XGNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_Login_V40 failed, error code= " + iLastErr; //登录失败，输出错误号
                    MessageBox.Show(str);
                    return;
                }
                else
                {
                    //登录成功
                    //MessageBox.Show("Login Success!");
                    //btnLogin.Text = "Logout";
                    //if ("192.168.2.10".Equals(ip))
                    //{
                    //    XGNetSDK.NET_DVR_Logout(m_UserIDs[deviceNum]);
                    //    m_UserIDs[deviceNum] = XGNetSDK.NET_DVR_Login(ref struLogInfo, ref DeviceInfo);
                    //}
                }
            }
        }

        /// <summary>
        /// 预览画面
        /// </summary>
        /// <param name="deviceNum">设备号 从0开始</param>
        /// <param name="channelNum">通道号 通道号1-可见光，2-红外</param>
        /// <param name="pictureBox">图像显示控件</param>
        /// <param name="real">图像回调函数</param>
        private void previewImage(Int32 deviceNum, Int32 realHandleNum, Int32 channelNum, PictureBox pictureBox, XGNetSDK.REALDATACALLBACK real)
        {

            if (m_UserIDs[deviceNum] < 0)
            {
                MessageBox.Show("Please login the device firstly");
                return;
            }

            if (m_RealHandles[realHandleNum] < 0)
            {
                XGNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new XGNetSDK.NET_DVR_PREVIEWINFO();
                //m_ptrRealHandle = pictureBox.Handle;
                //lpPreviewInfo.hPlayWnd = pictureBox.Handle;//预览窗口
                lpPreviewInfo.lChannel = channelNum;//预te览的设备通道
                lpPreviewInfo.dwStreamType = 0;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                lpPreviewInfo.bBlocked = 0; //0- 非阻塞取流，1- 阻塞取流
                lpPreviewInfo.dwDisplayBufNum = 5; //播放库播放缓冲区最大缓冲帧数
                lpPreviewInfo.byVideoCodingType = 0;
                lpPreviewInfo.byProtoType = 0;
                lpPreviewInfo.byPreviewMode = 0;

                if (RealDatas[realHandleNum] == null)
                {
                    RealDatas[realHandleNum] = new XGNetSDK.REALDATACALLBACK(real);//预览实时流回调函数
                }

                IntPtr pUser = new IntPtr();//用户数据

                //打开预览 Start live view 
                m_RealHandles[realHandleNum] = XGNetSDK.NET_DVR_RealPlay(m_UserIDs[deviceNum], ref lpPreviewInfo, RealDatas[realHandleNum], pUser);
                if (m_RealHandles[realHandleNum] < 0)
                {
                    iLastErr = XGNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_RealPlay_V40 failed, error code= " + iLastErr; //预览失败，输出错误号
                    MessageBox.Show(str);
                    return;
                }
                else
                {
                    //预览成功
                    // btnPreview.Text = "Stop Live View";
                }
            }
            else
            {
                //停止预览 Stop live view 
                if (!XGNetSDK.NET_DVR_StopRealPlay(m_RealHandles[realHandleNum]))
                {
                    iLastErr = XGNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_StopRealPlay failed, error code= " + iLastErr;
                    MessageBox.Show(str);
                    return;
                }
                //flag = false;
                m_RealHandles[realHandleNum] = -1;
                //btnPreview.Text = "Live View";

            }
            return;
            //if (channelNum == 1)
            //{

            //    if (m_lRealHandle < 0)
            //    {
            //        XGNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new XGNetSDK.NET_DVR_PREVIEWINFO();
            //        m_ptrRealHandle = pictureBox.Handle;
            //        //lpPreviewInfo.hPlayWnd = RealPlayWnd.Handle;//预览窗口
            //        lpPreviewInfo.lChannel = channelNum;//预te览的设备通道
            //        lpPreviewInfo.dwStreamType = 0;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
            //        lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
            //        lpPreviewInfo.bBlocked = 0; //0- 非阻塞取流，1- 阻塞取流
            //        lpPreviewInfo.dwDisplayBufNum = 5; //播放库播放缓冲区最大缓冲帧数
            //        lpPreviewInfo.byVideoCodingType = 0;
            //        lpPreviewInfo.byProtoType = 0;
            //        lpPreviewInfo.byPreviewMode = 0;

            //        //if (textBoxID.Text != "")
            //        //{
            //        //    lpPreviewInfo.lChannel = -1;
            //        //    byte[] byStreamID = System.Text.Encoding.Default.GetBytes(textBoxID.Text);
            //        //    lpPreviewInfo.byStreamID = new byte[32];
            //        //    byStreamID.CopyTo(lpPreviewInfo.byStreamID, 0);
            //        //}



            //        if (RealData == null)
            //        {

            //            RealData = new XGNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数

            //        }

            //        IntPtr pUser = new IntPtr();//用户数据

            //        //打开预览 Start live view 
            //        m_lRealHandle = XGNetSDK.NET_DVR_RealPlay(m_UserIDs[deviceNum], ref lpPreviewInfo, RealData, pUser);
            //        if (m_lRealHandle < 0)
            //        {
            //            iLastErr = XGNetSDK.NET_DVR_GetLastError();
            //            str = "NET_DVR_RealPlay_V40 failed, error code= " + iLastErr; //预览失败，输出错误号
            //            MessageBox.Show(str);
            //            return;
            //        }
            //        else
            //        {
            //            //预览成功
            //            // btnPreview.Text = "Stop Live View";
            //        }
            //    }
            //    else
            //    {
            //        //停止预览 Stop live view 
            //        if (!XGNetSDK.NET_DVR_StopRealPlay(m_lRealHandle))
            //        {
            //            iLastErr = XGNetSDK.NET_DVR_GetLastError();
            //            str = "NET_DVR_StopRealPlay failed, error code= " + iLastErr;
            //            MessageBox.Show(str);
            //            return;
            //        }
            //        //flag = false;
            //        m_lRealHandle = -1;
            //        //btnPreview.Text = "Live View";

            //    }
            //    return;
            //}


            //if (channelNum == 2)
            //{

            //    if (m_lRealHandle1 < 0)
            //    {
            //        XGNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new XGNetSDK.NET_DVR_PREVIEWINFO();
            //        m_ptrRealHandle1 = pictureBox.Handle;
            //        //lpPreviewInfo.hPlayWnd = RealPlayWnd.Handle;//预览窗口
            //        lpPreviewInfo.lChannel = channelNum;//预te览的设备通道
            //        lpPreviewInfo.dwStreamType = 0;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
            //        lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
            //        lpPreviewInfo.bBlocked = 0; //0- 非阻塞取流，1- 阻塞取流
            //        lpPreviewInfo.dwDisplayBufNum = 5; //播放库播放缓冲区最大缓冲帧数
            //        lpPreviewInfo.byVideoCodingType = 0;
            //        lpPreviewInfo.byProtoType = 0;
            //        lpPreviewInfo.byPreviewMode = 0;

            //        //if (textBoxID.Text != "")
            //        //{
            //        //    lpPreviewInfo.lChannel = -1;
            //        //    byte[] byStreamID = System.Text.Encoding.Default.GetBytes(textBoxID.Text);
            //        //    lpPreviewInfo.byStreamID = new byte[32];
            //        //    byStreamID.CopyTo(lpPreviewInfo.byStreamID, 0);
            //        //}



            //        if (RealData1 == null)
            //        {

            //            RealData1 = new XGNetSDK.REALDATACALLBACK(RealDataCallBack1);//预览实时流回调函数

            //        }

            //        IntPtr pUser = new IntPtr();//用户数据

            //        //打开预览 Start live view 
            //        m_lRealHandle1 = XGNetSDK.NET_DVR_RealPlay(m_UserIDs[deviceNum], ref lpPreviewInfo, RealData1, pUser);
            //        if (m_lRealHandle1 < 0)
            //        {
            //            iLastErr = XGNetSDK.NET_DVR_GetLastError();
            //            str = "NET_DVR_RealPlay_V40 failed, error code= " + iLastErr; //预览失败，输出错误号
            //            MessageBox.Show(str);
            //            return;
            //        }
            //        else
            //        {
            //            //预览成功
            //            // btnPreview.Text = "Stop Live View";
            //        }
            //    }
            //    else
            //    {
            //        //停止预览 Stop live view 
            //        if (!XGNetSDK.NET_DVR_StopRealPlay(m_lRealHandle1))
            //        {
            //            iLastErr = XGNetSDK.NET_DVR_GetLastError();
            //            str = "NET_DVR_StopRealPlay failed, error code= " + iLastErr;
            //            MessageBox.Show(str);
            //            return;
            //        }
            //        //flag = false;
            //        m_lRealHandle1 = -1;
            //        //btnPreview.Text = "Live View";

            //    }
            //    return;
            //}

        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x112)
            {
                switch ((int)m.WParam)
                {
                    case 0xf122:
                        m.WParam = IntPtr.Zero;
                        break;

                    case 0xF012:
                    case 0xF010:
                        m.WParam = IntPtr.Zero;
                        break;
                }
            }

            base.WndProc(ref m);
        }


        public void RealDataCallBack(Int32 lRealHandle, UInt32 dwDataType, ref XGNetSDK.LPVEDIO_DATA lpVedioData, IntPtr pUser)
        {

            switch (dwDataType)
            {
                case XGNetSDK.NET_DVR_SYSHEAD:     // sys head

                    if (lpVedioData.dwBufSize > 0)
                    {
                        //flag = GDIHelper.initRender(m_ptrRealHandle, (int)lpVedioData.nWidth, (int)lpVedioData.nHeight);
                    }
                    break;
                case XGNetSDK.NET_DVR_STREAMDATA:     // video stream data


                    if (lpVedioData.dwBufSize > 0)
                    {
                        try
                        {
                            OpenCvSharp.Mat mgMat;
                            OpenCvSharp.Mat mgMatShow = new OpenCvSharp.Mat();
                            mgMat = new OpenCvSharp.Mat((int)lpVedioData.nHeight, (int)lpVedioData.nWidth, OpenCvSharp.MatType.CV_8UC4, lpVedioData.pBuffer, 0);
                            OpenCvSharp.Cv2.CvtColor(mgMat, mgMatShow, OpenCvSharp.ColorConversionCodes.BGRA2BGR);
                            OpenCvSharp.Size size = new OpenCvSharp.Size(pics[0].Width, pics[0].Height);
                            Cv2.Resize(mgMatShow, mgMatShow, size, 0, 0, InterpolationFlags.Cubic);

                            pics[0].Image = mgMatShow.ToBitmap();

                            //Graphics g = Graphics.FromImage(pics[0].Image);
                            //g.DrawString("可见光1", new Font("楷体", 14), new SolidBrush(Color.White), 5, 5);

                            //if (saveImageFlag)
                            //{
                            //    string strFileName = "test1.jpg";
                            //    Cv2.ImWrite(strFileName, mgMatShow);
                            //    //saveImageFlag = false;
                            //}
                        }
                        catch(Exception ex)//设备0可见光图像异常捕获
                        {
                            Globals.Log("设备0可见光图像显示异常" + ex.ToString());
                        }

                    }
                    break;
                default:
                    //if (lpVedioData.dwBufSize > 0)
                    //{
                    //    GDIHelper.RenderPlay(lpVedioData.pBuffer, (int)lpVedioData.dwBufSize);
                    //}
                    break;
            }
        }


        public void RealDataCallBack1(Int32 lRealHandle, UInt32 dwDataType, ref XGNetSDK.LPVEDIO_DATA lpVedioData, IntPtr pUser)
        {

            switch (dwDataType)
            {
                case XGNetSDK.NET_DVR_SYSHEAD:     // sys head

                    if (lpVedioData.dwBufSize > 0)
                    {

                        //flag = GDIHelper.initRender(m_ptrRealHandle, (int)lpVedioData.nWidth, (int)lpVedioData.nHeight);
                    }

                    break;
                case XGNetSDK.NET_DVR_STREAMDATA:     // video stream data

                    if (lpVedioData.dwBufSize > 0)
                    {
                        XGNetSDK.NET_DVR_GetIRTemResult(m_UserIDs[0], temresultCallBack);
                        //flag = true;
                        try//设备0红外图像异常捕获
                        {
                            OpenCvSharp.Mat mgMat;
                            OpenCvSharp.Mat mgMatShow = new OpenCvSharp.Mat();
                            mgMat = new OpenCvSharp.Mat((int)lpVedioData.nHeight, (int)lpVedioData.nWidth, OpenCvSharp.MatType.CV_8UC4, lpVedioData.pBuffer, 0);
                            OpenCvSharp.Cv2.CvtColor(mgMat, mgMatShow, OpenCvSharp.ColorConversionCodes.BGRA2BGR);
                            OpenCvSharp.Size size = new OpenCvSharp.Size(pics[1].Width, pics[1].Height);
                            Cv2.Resize(mgMatShow, mgMatShow, size, 0, 0, InterpolationFlags.Cubic);

                            float scaleX = pics[1].Width * 1.0f / 384;
                            float scaleY = pics[1].Height * 1.0f / 288;

                            int[] maxTempData = getTempUtil.GetMaxTempAndPoint();
                            OpenCvSharp.Point cor;
                            cor.X = (int)(maxTempData[1] * scaleX);
                            cor.Y = (int)(maxTempData[2] * scaleY) + 10;

                            double maxTemp = Math.Round(getTempUtil.GetMaxTempAndPoint()[0] / 100d, 2);
                            Cv2.Line(mgMatShow, (int)(maxTempData[1] * scaleX - 10), (int)(maxTempData[2] * scaleY), (int)(maxTempData[1] * scaleX + 10), (int)(maxTempData[2] * scaleY), OpenCvSharp.Scalar.Red);
                            Cv2.Line(mgMatShow, (int)(maxTempData[1] * scaleX), (int)(maxTempData[2] * scaleY - 10), (int)(maxTempData[1] * scaleX), (int)(maxTempData[2] * scaleY + 10), OpenCvSharp.Scalar.Red);
                            Cv2.PutText(mgMatShow, maxTemp.ToString(), cor, OpenCvSharp.HersheyFonts.HersheyPlain, 1.5, OpenCvSharp.Scalar.Red, 2);
                            pics[1].Image = mgMatShow.ToBitmap();

                            Graphics g = Graphics.FromImage(pics[1].Image);
                            g.DrawString("红外1", new Font("楷体", 14), new SolidBrush(Color.White), 5, 5);
                            //labels[1].BringToFront();

                            if (saveImageFlag)
                            {
                                string strFileName = "test.jpg";
                                Cv2.ImWrite(strFileName, mgMatShow);
                                saveImageFlag = false;
                            }
                        }
                        catch(Exception ex)//设备0红外图像异常捕获
                        {
                            Globals.Log("设备0红外图像显示异常" + ex.ToString());
                        }

                    }
                    break;
                default:
                    //if (lpVedioData.dwBufSize > 0)
                    //{
                    //    GDIHelper.RenderPlay(lpVedioData.pBuffer, (int)lpVedioData.dwBufSize);
                    //}
                    break;
            }
        }


        public void RealDataCallBack2(Int32 lRealHandle, UInt32 dwDataType, ref XGNetSDK.LPVEDIO_DATA lpVedioData, IntPtr pUser)
        {

            switch (dwDataType)
            {
                case XGNetSDK.NET_DVR_SYSHEAD:     // sys head

                    if (lpVedioData.dwBufSize > 0)
                    {
                        //flag = GDIHelper.initRender(m_ptrRealHandle, (int)lpVedioData.nWidth, (int)lpVedioData.nHeight);
                    }
                    break;
                case XGNetSDK.NET_DVR_STREAMDATA:     // video stream data

                    if (lpVedioData.dwBufSize > 0)
                    {
                        try { 
                        OpenCvSharp.Mat mgMat;
                        OpenCvSharp.Mat mgMatShow = new OpenCvSharp.Mat();
                        mgMat = new OpenCvSharp.Mat((int)lpVedioData.nHeight, (int)lpVedioData.nWidth, OpenCvSharp.MatType.CV_8UC4, lpVedioData.pBuffer, 0);
                        OpenCvSharp.Cv2.CvtColor(mgMat, mgMatShow, OpenCvSharp.ColorConversionCodes.BGRA2BGR);
                        OpenCvSharp.Size size = new OpenCvSharp.Size(pics[2].Width, pics[2].Height);
                        Cv2.Resize(mgMatShow, mgMatShow, size, 0, 0, InterpolationFlags.Cubic);

                        pics[2].Image = mgMatShow.ToBitmap();

                        Graphics g = Graphics.FromImage(pics[2].Image);
                        g.DrawString("可见光2", new Font("楷体", 14), new SolidBrush(Color.White), 5, 5);

                            //if (saveImageFlag)//保存图像
                            //{
                            //    string IrImagePath = Globals.ImageDirectoryPath + 1;
                            //    if (!Directory.Exists(IrImagePath))
                            //    {
                            //        Directory.CreateDirectory(IrImagePath);
                            //    }

                            //    IrImagePath += "\\" + strTime + "_Visual.jpg";

                            //    Cv2.ImWrite(IrImagePath, mgMatShow);
                            //    //saveImageFlag = false;                                                                                 
                            //}

                            //double maxTemp = Math.Round(getTempUtil.GetMaxTempAndPoint()[0] / 100d, 2);

                            //if (maxTemp > Globals.systemParam.alarm_1)//保存报警图像
                            //{
                            //    string AlarmIrImagePath = Globals.AlarmImageDirectoryPath + 1;
                            //    if (!Directory.Exists(AlarmIrImagePath))
                            //    {
                            //        Directory.CreateDirectory(AlarmIrImagePath);
                            //    }

                            //    AlarmIrImagePath += "\\" + strTime + "_Visual.jpg";

                            //    Cv2.ImWrite(AlarmIrImagePath, mgMatShow);
                            //}
                        }
                        catch (Exception ex)
                        {
                            Globals.Log("设备1可见光图像显示异常"  + ex.ToString());
                        }
                    }
                    break;
                default:
                    //if (lpVedioData.dwBufSize > 0)
                    //{
                    //    GDIHelper.RenderPlay(lpVedioData.pBuffer, (int)lpVedioData.dwBufSize);
                    //}
                    break;
            }
        }

        public void RealDataCallBack3(Int32 lRealHandle, UInt32 dwDataType, ref XGNetSDK.LPVEDIO_DATA lpVedioData, IntPtr pUser)
        {

            switch (dwDataType)
            {
                case XGNetSDK.NET_DVR_SYSHEAD:     // sys head

                    if (lpVedioData.dwBufSize > 0)
                    {

                        //flag = GDIHelper.initRender(m_ptrRealHandle, (int)lpVedioData.nWidth, (int)lpVedioData.nHeight);
                    }

                    break;
                case XGNetSDK.NET_DVR_STREAMDATA:     // video stream data

                    if (lpVedioData.dwBufSize > 0)
                    {
                        try { 
                        XGNetSDK.NET_DVR_GetIRTemResult(m_UserIDs[1], temresultCallBack1);
                        //flag = true;
                        OpenCvSharp.Mat mgMat;
                        OpenCvSharp.Mat mgMatShow = new OpenCvSharp.Mat();
                        mgMat = new OpenCvSharp.Mat((int)lpVedioData.nHeight, (int)lpVedioData.nWidth, OpenCvSharp.MatType.CV_8UC4, lpVedioData.pBuffer, 0);
                        OpenCvSharp.Cv2.CvtColor(mgMat, mgMatShow, OpenCvSharp.ColorConversionCodes.BGRA2BGR);
                        OpenCvSharp.Size size = new OpenCvSharp.Size(pics[3].Width, pics[3].Height);
                        Cv2.Resize(mgMatShow, mgMatShow, size, 0, 0, InterpolationFlags.Cubic);

                        float scaleX = pics[3].Width * 1.0f / 384;
                        float scaleY = pics[3].Height * 1.0f / 288;

                        int[] maxTempData = getTempUtil.GetMaxTempAndPoint();
                        OpenCvSharp.Point cor;
                        cor.X = (int)(maxTempData[1] * scaleX);
                        cor.Y = (int)(maxTempData[2] * scaleY) + 10;

                        double maxTemp = Math.Round(getTempUtil.GetMaxTempAndPoint()[0] / 100d, 2);
                        //Cv2.Line(mgMatShow, (int)(maxTempData[1] * scaleX - 10), (int)(maxTempData[2] * scaleY), (int)(maxTempData[1] * scaleX + 10), (int)(maxTempData[2] * scaleY), OpenCvSharp.Scalar.Red);
                        //Cv2.Line(mgMatShow, (int)(maxTempData[1] * scaleX), (int)(maxTempData[2] * scaleY - 10), (int)(maxTempData[1] * scaleX), (int)(maxTempData[2] * scaleY + 10), OpenCvSharp.Scalar.Red);
                        //Cv2.PutText(mgMatShow, maxTemp.ToString(), cor, OpenCvSharp.HersheyFonts.HersheyPlain, 1.5, OpenCvSharp.Scalar.Red, 2);
                        pics[3].Image = mgMatShow.ToBitmap();

                            //Graphics g = Graphics.FromImage(pics[3].Image);
                            //g.DrawString("红外2", new Font("楷体", 14), new SolidBrush(Color.White), 5, 5);
                            //labels[1].BringToFront();


                            //if (saveImageFlag)//保存图像
                            //{
                            //    string IrImagePath = Globals.ImageDirectoryPath + 1;
                            //    if (!Directory.Exists(IrImagePath))
                            //    {
                            //        Directory.CreateDirectory(IrImagePath);
                            //    }

                            //    IrImagePath += "\\" + strTime + "_IR.jpg";

                            //    Cv2.ImWrite(IrImagePath, mgMatShow);
                            //    //saveImageFlag = false;                                                                                 
                            //}

                            //if (maxTemp > Globals.systemParam.alarm_1)//保存报警图像
                            //{
                            //    string AlarmIrImagePath = Globals.AlarmImageDirectoryPath + 1;
                            //    if (!Directory.Exists(AlarmIrImagePath))
                            //    {
                            //        Directory.CreateDirectory(AlarmIrImagePath);
                            //    }

                            //    AlarmIrImagePath += "\\" + strTime + "_IR.jpg";

                            //    Cv2.ImWrite(AlarmIrImagePath, mgMatShow);
                            //}
                        }
                        catch (Exception ex)
                        {
                            Globals.Log("设备2红外图像显示异常" + ex.ToString());
                        }

                    }
                    break;
                default:
                    //if (lpVedioData.dwBufSize > 0)
                    //{
                    //    GDIHelper.RenderPlay(lpVedioData.pBuffer, (int)lpVedioData.dwBufSize);
                    //}
                    break;
            }
        }

        public void TemResultCallBack(IntPtr pData, int dataLen)
        {
            try
            {
                string s = System.DateTime.Now.ToString();
                s = s.Substring(s.Length - 8, 8);

                byte[] ImageData1 = new byte[dataLen];
                float[] tempData = new float[dataLen / 2];
                Marshal.Copy(pData, ImageData1, 0, dataLen);

                //int a = ImageData1[1] * 256 + ImageData1[0];

                for (int i = 0; i < 384 * 288; i++)
                {
                    tempData[i] = (ImageData1[i * 2 + 1] << 8) + ImageData1[i * 2];
                    tempData[i] = (tempData[i] - 10000) / 100;
                }
                //getTempUtil.tempRotate(tempData, rotateTempData, 384, 288);
                getTempUtil.setTempDate(tempData);

                //Console.WriteLine(s + " " + Math.Round(getTempUtil.GetMaxTempAndPoint()[0] / 100d, 2));
                //Console.WriteLine(s + " " + a);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        public void TemResultCallBack1(IntPtr pData, int dataLen)
        {
            try
            {
                string s = System.DateTime.Now.ToString();
                s = s.Substring(s.Length - 8, 8);

                byte[] ImageData1 = new byte[dataLen];
                float[] tempData = new float[dataLen / 2];
                Marshal.Copy(pData, ImageData1, 0, dataLen);

                //int a = ImageData1[1] * 256 + ImageData1[0];

                for (int i = 0; i < 384 * 288; i++)
                {
                    tempData[i] = (ImageData1[i * 2 + 1] << 8) + ImageData1[i * 2];
                    tempData[i] = (tempData[i] - 10000) / 100;
                }
                //getTempUtil.tempRotate(tempData, rotateTempData, 384, 288);
                getTempUtil1.setTempDate(tempData);

                // Console.WriteLine(s + " " + Math.Round(getTempUtil.GetMaxTempAndPoint()[0] / 100d, 2));
                //Console.WriteLine(s + " " + a);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        public void cbLoginCallBack(int lUserID, int dwResult, IntPtr lpDeviceInfo, IntPtr pUser)
        {
            //string strLoginCallBack = "登录设备，lUserID：" + lUserID + "，dwResult：" + dwResult;

            //if (dwResult == 0)
            //{
            //    uint iErrCode = XGNetSDK.NET_DVR_GetLastError();
            //    strLoginCallBack = strLoginCallBack + "，错误号:" + iErrCode;
            //}

            ////下面代码注释掉也会崩溃
            //if (InvokeRequired)
            //{
            //    object[] paras = new object[2];
            //    paras[0] = strLoginCallBack;
            //    paras[1] = lpDeviceInfo;
            //    labelLogin.BeginInvoke(new UpdateTextStatusCallback(UpdateClientList), paras);
            //}
            //else
            //{
            //    //创建该控件的主线程直接更新信息列表 
            //    UpdateClientList(strLoginCallBack, lpDeviceInfo);
            //}

        }

        public void cbLoginCallBack1(int lUserID, int dwResult, IntPtr lpDeviceInfo, IntPtr pUser)
        {
            //string strLoginCallBack = "登录设备，lUserID：" + lUserID + "，dwResult：" + dwResult;

            //if (dwResult == 0)
            //{
            //    uint iErrCode = XGNetSDK.NET_DVR_GetLastError();
            //    strLoginCallBack = strLoginCallBack + "，错误号:" + iErrCode;
            //}

            ////下面代码注释掉也会崩溃
            //if (InvokeRequired)
            //{
            //    object[] paras = new object[2];
            //    paras[0] = strLoginCallBack;
            //    paras[1] = lpDeviceInfo;
            //    labelLogin.BeginInvoke(new UpdateTextStatusCallback(UpdateClientList), paras);
            //}
            //else
            //{
            //    //创建该控件的主线程直接更新信息列表 
            //    UpdateClientList(strLoginCallBack, lpDeviceInfo);
            //}

        }



        private void initDatas()
        {
            int cameraNum = Globals.systemParam.cameraNum;
            pics = new PictureBox[cameraNum * 2];//定义显示图像控件
            labels = new TransparentLabel[cameraNum * 2];//显示控件说明Label

            m_UserIDs = new Int32[cameraNum];
            m_RealHandles = new long[cameraNum * 2];
            RealDatas = new XGNetSDK.REALDATACALLBACK[cameraNum * 2];

            for (int i = 0; i < cameraNum; i++)
            {
                m_UserIDs[i] = -1; //初始化userID 


            }
            for (int i = 0; i < cameraNum * 2; i++)
            {
                m_RealHandles[i] = -1;
                RealDatas[i] = null;
            }

            if (temresultCallBack == null)
            {
                temresultCallBack = new XGNetSDK.TEMRESULTCALLBACK(TemResultCallBack);
            }

            if (temresultCallBack1 == null)
            {
                temresultCallBack1 = new XGNetSDK.TEMRESULTCALLBACK(TemResultCallBack1);
            }

            getTempUtil.init(384, 288);
            getTempUtil1.init(384, 288);
        }

        ///// <summary>
        ///// 声音报警线程，只需要一个线程
        ///// </summary>
        //public void ThreadAlert()
        //{
        //    while (true)
        //    {
        //        Thread.Sleep(10);

        //        if (Globals.bSoundAlert)
        //        {
        //            try
        //            {
        //                SoundPlayer player = new SoundPlayer();
        //                player.SoundLocation = Application.StartupPath + "\\Alert.wav";
        //                player.Load();
        //                player.Play();
        //                Thread.Sleep(5000);

        //                Globals.bSoundAlert = false;
        //            }
        //            catch (Exception e)
        //            {
        //                Globals.Log("ThreadAlert" + e.Message);
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// 设置实时监控界面相关控件大小及显示位置
        /// </summary>
        /// <param name="rowNum">行数</param>
        /// <param name="colNum">列数</param>

        private void SetFmonitorDisplayWnds(uint rowNum, uint colNum)
        {
            //uint w = (uint)(Screen.PrimaryScreen.Bounds.Width - fmonitor.GetControl("uiPanel1").Width);
            uint w = (uint)(Screen.PrimaryScreen.Bounds.Width);
            uint h = (uint)(Screen.PrimaryScreen.Bounds.Height - this.TitleHeight - uiNavBar1.Height);


            //先计算显示窗口的位置和大小，依据为：在不超过主窗口大小的情况下尽可能大，同时严格保持4:3的比例显示
            uint real_width = w;
            uint real_height = h;

            uint display_width = (real_width - DISPLAYWND_MARGIN_LEFT * 2 - (colNum - 1) * DISPLAYWND_GAP) / colNum;//单个相机显示区域的宽度(还未考虑比例)
            uint display_height = (real_height - DISPLAYWND_MARGIN_TOP * 2 - (rowNum - 1) * DISPLAYWND_GAP) / rowNum;//单个相机显示区域的高度(还未考虑比例)

            if (display_width * 3 >= display_height * 4)//考虑比例
            {
                uint ret = display_height % 3;
                if (ret != 0)
                {
                    display_height -= ret;
                }
                display_width = display_height * 4 / 3;
            }
            else
            {
                uint ret = display_width % 4;
                if (ret != 0)
                {
                    display_width -= ret;
                }
                display_height = display_width * 3 / 4;
            }

            for (uint i = 0; i < rowNum; i++)
            {
                uint y = (real_height - rowNum * display_height - DISPLAYWND_GAP * (rowNum - 1)) / 2 + (display_height + DISPLAYWND_GAP) * i;
                for (uint j = 0; j < colNum; j++)
                {
                    uint x = (real_width - colNum * display_width - DISPLAYWND_GAP * (colNum - 1)) / 2 + (display_width + DISPLAYWND_GAP) * j;

                    pics[i * 2 + j] = new PictureBox();
                    pics[i * 2 + j].Left = (int)x;
                    pics[i * 2 + j].Top = (int)y;
                    pics[i * 2 + j].Width = (int)display_width;
                    pics[i * 2 + j].Height = (int)display_height;
                    pics[i * 2 + j].Show();
                    pics[i * 2 + j].BackColor = Color.Black;
                    //pics[i * 2 + j].Image = Image.FromFile(@"D:\C#\IRAY_Test\IR_Tmp_Measurement\bin\Debug\AlarmImage\20230330105027_Visual.jpg");
                    //pics[i * 2 + j].Name = "pic" + (i * 2 + j).ToString();
                    pics[i * 2 + j].SizeMode = PictureBoxSizeMode.StretchImage;

                    fmonitor.Controls.Add(pics[i * 2 + j]);

                    //labels[i * 2 + j] = new TransparentLabel();
                    //labels[i * 2 + j].Left = (int)x;
                    //labels[i * 2 + j].Top = (int)y;
                    ////labels[i * 2 + j].Text = "轴" + (i * 2 + j + 1).ToString();
                    //labels[i * 2 + j].ForeColor = Color.WhiteSmoke;
                    //labels[i * 2 + j].BackColor = Color.Transparent;
                    //// label.Show();

                    //fmonitor.Controls.Add(labels[i * 2 + j]);
                    ////pic[i * 2 + j].Controls.Add(label);
                    ////label.Parent = fmonitor;
                    //labels[i * 2 + j].BringToFront();

                }
            }


            //foreach (PictureBox p in fmonitor.GetControls<PictureBox>())
            //{
            //    Console.WriteLine(p.Name);
            //}
        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
            string s = System.DateTime.Now.ToString();
            s = s.Substring(s.Length - 8, 8);
            timeLabel.Text = s;

        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            XGNetSDK.NET_DVR_Cleanup();
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            //if (flag == true)
            //{
            //    XGNetSDK.NET_DVR_GetIRTemResult(m_lUserID, temresultCallBack);
            //}

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            XGNetSDK.NET_DVR_JPEGPARA jpegPara = new XGNetSDK.NET_DVR_JPEGPARA();
            jpegPara.wPicSize = 0;
            jpegPara.wPicQuality = 0xff;
            XGNetSDK.NET_DVR_CaptureJPEGPicture(m_UserIDs[0], 2, ref jpegPara, "ir.jpg");

        }

        private void UiNavBar1_MenuItemClick(string itemText, int menuIndex, int pageIndex)
        {

        }
    }
}
