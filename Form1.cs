using FFmpeg.AutoGen.Example;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Sunny.UI;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{

    public partial class Form1 : Form
    {
        private bool flag = false;
        private int count = 0;
        private Int32 m_lPort = -1;
        public int m_lChannel = 1;
        private string str;
        private uint iLastErr = 0;
        private Int32 m_lUserID = -1;
        private long m_lRealHandle = -1;
        private IntPtr m_ptrRealHandle;
        private PlayCtrl.DECCBFUN m_fDisplayFun = null;
        public XGNetSDK.NET_DVR_USER_LOGIN_INFO struLogInfo;
        public XGNetSDK.NET_DVR_DEVICEINFO_V40 DeviceInfo;
        XGNetSDK.LOGINRESULTCALLBACK LoginCallBack = null;
        XGNetSDK.REALDATACALLBACK RealData = null;
        XGNetSDK.TEMRESULTCALLBACK temresultCallBack = null;//获取一帧红外测温结果回调函数
        public delegate void UpdateTextStatusCallback(string strLogStatus, IntPtr lpDeviceInfo);
        public delegate void MyDebugInfo(string str);

        OpenCvSharp.VideoCapture vcap = new VideoCapture();
        OpenCvSharp.Mat frame = new Mat();
        OpenCvSharp.Mat mgMatShow = new Mat();

     
        float fSx;
        float fSy;
        public int Width = 1920;
        public int Height = 1080;
        public static byte[] video_data_show = new byte[1920 * 1080 * 4];//image data

        [DllImport("user32.dll", EntryPoint = "PostMessage")]
        private static extern int PostMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        CancellationTokenSource cts;
        VideoCapture capture =new VideoCapture();
        GetTempUtil getTempUtil;

        public Form1()
        {
            InitializeComponent();

            Control.CheckForIllegalCrossThreadCalls = false;

            XGNetSDK.NET_DVR_Init();
            //保存SDK日志 To save the SDK log
            XGNetSDK.NET_DVR_SetLogToFile(3, "C:\\SdkLog\\", true);
            //FFHelper.Init(RealPlayWnd);


            //float[] tempData = { 1,2,3,4
            //                    ,5,6,7,8
            //                    ,9,10,11,12};
            //GetTempUtil tempUtil = new GetTempUtil();
            //tempUtil.init(4, 3);
            //tempUtil.setTempDate(tempData);
            //int[] result = tempUtil.GetMaxTempAndPoint();
            //int[] result1 = tempUtil.GetMinTempAndPoint();
            //Rect rect = new Rect();
            //rect.Left = 0;
            //rect.Top = 0;

            //OpenCvSharp.Point startPoint = new OpenCvSharp.Point(0, 0);
            //OpenCvSharp.Point endPoint = new OpenCvSharp.Point(2, 1);

            //int[] result2 = tempUtil.GetRectMaxTempAndPoint(startPoint, endPoint);
            //MessageBox.Show("");

            getTempUtil = new GetTempUtil();
            getTempUtil.init(384, 288);

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (textBoxIP.Text == "" || textBoxPort.Text == "" ||
                textBoxUserName.Text == "" || textBoxPassword.Text == "")
            {
                MessageBox.Show("Please input IP, Port, User name and Password!");
                return;
            }
            if (m_lUserID < 0)
            {

                struLogInfo = new XGNetSDK.NET_DVR_USER_LOGIN_INFO();

                //设备IP地址或者域名
                byte[] byIP = System.Text.Encoding.Default.GetBytes(textBoxIP.Text);
                struLogInfo.sDeviceAddress = new byte[129];
                byIP.CopyTo(struLogInfo.sDeviceAddress, 0);

                //设备用户名
                byte[] byUserName = System.Text.Encoding.Default.GetBytes(textBoxUserName.Text);
                struLogInfo.sUserName = new byte[64];
                byUserName.CopyTo(struLogInfo.sUserName, 0);

                //设备密码
                byte[] byPassword = System.Text.Encoding.Default.GetBytes(textBoxPassword.Text);
                struLogInfo.sPassword = new byte[64];
                byPassword.CopyTo(struLogInfo.sPassword, 0);

                struLogInfo.wPort = ushort.Parse(textBoxPort.Text);//设备服务端口号

                if (LoginCallBack == null)
                {
                    LoginCallBack = new XGNetSDK.LOGINRESULTCALLBACK(cbLoginCallBack);//注册回调函数                    
                }
                struLogInfo.cbLoginResult = LoginCallBack;
                struLogInfo.bUseAsynLogin = false; //是否异步登录：0- 否，1- 是 


                if (temresultCallBack == null)
                {
                    temresultCallBack = new XGNetSDK.TEMRESULTCALLBACK(TemResultCallBack);

                }

                DeviceInfo = new XGNetSDK.NET_DVR_DEVICEINFO_V40();

                //登录设备 Login the device
                m_lUserID = XGNetSDK.NET_DVR_Login(ref struLogInfo, ref DeviceInfo);
                if (m_lUserID < 0)
                {
                    iLastErr = XGNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_Login_V40 failed, error code= " + iLastErr; //登录失败，输出错误号
                    MessageBox.Show(str);
                    return;
                }
                else
                {
                    //登录成功
                    MessageBox.Show("Login Success!");
                    btnLogin.Text = "Logout";

                  
                }

            }
            else
            {
                //注销登录 Logout the device
                if (m_lRealHandle >= 0)
                {
                    MessageBox.Show("Please stop live view firstly");
                    return;
                }

                if (!XGNetSDK.NET_DVR_Logout(m_lUserID))
                {
                    iLastErr = XGNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_Logout failed, error code= " + iLastErr;
                    MessageBox.Show(str);
                    return;
                }
                m_lUserID = -1;
                btnLogin.Text = "Login";
            }
            return;
        }
        public void cbLoginCallBack(int lUserID, int dwResult, IntPtr lpDeviceInfo, IntPtr pUser)
        {
            string strLoginCallBack = "登录设备，lUserID：" + lUserID + "，dwResult：" + dwResult;

            if (dwResult == 0)
            {
                uint iErrCode = XGNetSDK.NET_DVR_GetLastError();
                strLoginCallBack = strLoginCallBack + "，错误号:" + iErrCode;
            }

            //下面代码注释掉也会崩溃
            if (InvokeRequired)
            {
                object[] paras = new object[2];
                paras[0] = strLoginCallBack;
                paras[1] = lpDeviceInfo;
                labelLogin.BeginInvoke(new UpdateTextStatusCallback(UpdateClientList), paras);
            }
            else
            {
                //创建该控件的主线程直接更新信息列表 
                UpdateClientList(strLoginCallBack, lpDeviceInfo);
            }

        }
        public void UpdateClientList(string strLogStatus, IntPtr lpDeviceInfo)
        {
            //列表新增报警信息
            labelLogin.Text = "登录状态（异步）：" + strLogStatus;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (m_lUserID < 0)
            {
                MessageBox.Show("Please login the device firstly");
                return;
            }

            if (m_lRealHandle < 0)
            {
                XGNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new XGNetSDK.NET_DVR_PREVIEWINFO();
                m_ptrRealHandle = RealPlayWnd.Handle;
                //lpPreviewInfo.hPlayWnd = RealPlayWnd.Handle;//预览窗口
                lpPreviewInfo.lChannel = Int16.Parse(textBoxChannel.Text);//预te览的设备通道
                lpPreviewInfo.dwStreamType = 0;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                lpPreviewInfo.bBlocked = 0; //0- 非阻塞取流，1- 阻塞取流
                lpPreviewInfo.dwDisplayBufNum = 5; //播放库播放缓冲区最大缓冲帧数
                lpPreviewInfo.byVideoCodingType = 0;
                lpPreviewInfo.byProtoType = 0;
                lpPreviewInfo.byPreviewMode = 0;

                if (textBoxID.Text != "")
                {
                    lpPreviewInfo.lChannel = -1;
                    byte[] byStreamID = System.Text.Encoding.Default.GetBytes(textBoxID.Text);
                    lpPreviewInfo.byStreamID = new byte[32];
                    byStreamID.CopyTo(lpPreviewInfo.byStreamID, 0);
                }


                if (RealData == null)
                {
                    RealData = new XGNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数
                }

                IntPtr pUser = new IntPtr();//用户数据

                //打开预览 Start live view 
                m_lRealHandle = XGNetSDK.NET_DVR_RealPlay(m_lUserID, ref lpPreviewInfo, RealData, pUser);
                if (m_lRealHandle < 0)
                {
                    iLastErr = XGNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_RealPlay_V40 failed, error code= " + iLastErr; //预览失败，输出错误号
                    MessageBox.Show(str);
                    return;
                }
                else
                {
                    //预览成功
                    btnPreview.Text = "Stop Live View";
                }
            }
            else
            {
                //停止预览 Stop live view 
                if (!XGNetSDK.NET_DVR_StopRealPlay(m_lRealHandle))
                {
                    iLastErr = XGNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_StopRealPlay failed, error code= " + iLastErr;
                    MessageBox.Show(str);
                    return;
                }
                flag = false;
                m_lRealHandle = -1;
                btnPreview.Text = "Live View";

            }
            return;
        }
        public void DebugInfo(string str)
        {
            if (str.Length > 0)
            {
                str += "\n";
                TextBoxInfo.AppendText(str);
            }
        }
        //public void RealDataCallBack(Int32 lRealHandle, UInt32 dwDataType, ref XGNetSDK.LPVEDIO_DATA lpVedioData, IntPtr pUser)
        //{

        //    //下面数据处理建议使用委托的方式
        //    MyDebugInfo AlarmInfo = new MyDebugInfo(DebugInfo);
        //    switch (dwDataType)
        //    {
        //        case XGNetSDK.NET_DVR_SYSHEAD:     // sys head
        //            if (lpVedioData.dwBufSize > 0)
        //            {
        //                if (m_lPort >= 0)
        //                {
        //                    return; //同一路码流不需要多次调用开流接口
        //                }

        //                //获取播放句柄 Get the port to play
        //                if (!PlayCtrl.PlayM4_GetPort(ref m_lPort))
        //                {
        //                    iLastErr = PlayCtrl.PlayM4_GetLastError(m_lPort);
        //                    str = "PlayM4_GetPort failed, error code= " + iLastErr;
        //                    this.BeginInvoke(AlarmInfo, str);
        //                    break;
        //                }

        //                //设置流播放模式 Set the stream mode: real-time stream mode
        //                if (!PlayCtrl.PlayM4_SetStreamOpenMode(m_lPort, PlayCtrl.STREAME_REALTIME))
        //                {
        //                    iLastErr = PlayCtrl.PlayM4_GetLastError(m_lPort);
        //                    str = "Set STREAME_REALTIME mode failed, error code= " + iLastErr;
        //                    this.BeginInvoke(AlarmInfo, str);
        //                }

        //                //打开码流，送入头数据 Open stream
        //                if (!PlayCtrl.PlayM4_OpenStream(m_lPort, lpVedioData.pBuffer, lpVedioData.dwBufSize, 2 * 1024 * 1024))
        //                {
        //                    iLastErr = PlayCtrl.PlayM4_GetLastError(m_lPort);
        //                    str = "PlayM4_OpenStream failed, error code= " + iLastErr;
        //                    this.BeginInvoke(AlarmInfo, str);
        //                    break;
        //                }


        //                //设置显示缓冲区个数 Set the display buffer number
        //                if (!PlayCtrl.PlayM4_SetDisplayBuf(m_lPort, 15))
        //                {
        //                    iLastErr = PlayCtrl.PlayM4_GetLastError(m_lPort);
        //                    str = "PlayM4_SetDisplayBuf failed, error code= " + iLastErr;
        //                    this.BeginInvoke(AlarmInfo, str);
        //                }

        //                //设置显示模式 Set the display mode
        //                if (!PlayCtrl.PlayM4_SetOverlayMode(m_lPort, 0, 0/* COLORREF(0)*/)) //play off screen 
        //                {
        //                    iLastErr = PlayCtrl.PlayM4_GetLastError(m_lPort);
        //                    str = "PlayM4_SetOverlayMode failed, error code= " + iLastErr;
        //                    this.BeginInvoke(AlarmInfo, str);
        //                }

        //                //设置解码回调函数，获取解码后音视频原始数据 Set callback function of decoded data
        //                m_fDisplayFun = new PlayCtrl.DECCBFUN(DecCallbackFUN);
        //                if (!PlayCtrl.PlayM4_SetDecCallBackEx(m_lPort, m_fDisplayFun, IntPtr.Zero, 0))
        //                {
        //                    this.BeginInvoke(AlarmInfo, "PlayM4_SetDisplayCallBack fail");
        //                }

        //                //开始解码 Start to play                       
        //                if (!PlayCtrl.PlayM4_Play(m_lPort, m_ptrRealHandle))
        //                {
        //                    iLastErr = PlayCtrl.PlayM4_GetLastError(m_lPort);
        //                    str = "PlayM4_Play failed, error code= " + iLastErr;
        //                    this.BeginInvoke(AlarmInfo, str);
        //                    break;
        //                }
        //            }
        //            break;
        //        case XGNetSDK.NET_DVR_STREAMDATA:     // video stream data
        //            if (lpVedioData.dwBufSize > 0 && m_lPort != -1)
        //            {
        //                for (int i = 0; i < 999; i++)
        //                {
        //                    //送入码流数据进行解码 Input the stream data to decode
        //                    if (!PlayCtrl.PlayM4_InputData(m_lPort, lpVedioData.pBuffer, lpVedioData.dwBufSize))
        //                    {
        //                        iLastErr = PlayCtrl.PlayM4_GetLastError(m_lPort);
        //                        str = "PlayM4_InputData failed, error code= " + iLastErr;
        //                        this.BeginInvoke(AlarmInfo, str);
        //                        Thread.Sleep(2);
        //                    }
        //                    else
        //                    {
        //                        break;
        //                    }
        //                }
        //            }
        //            break;
        //        default:
        //            if (lpVedioData.dwBufSize > 0 && m_lPort != -1)
        //            {
        //                //送入其他数据 Input the other data
        //                for (int i = 0; i < 999; i++)
        //                {
        //                    if (!PlayCtrl.PlayM4_InputData(m_lPort, lpVedioData.pBuffer, lpVedioData.dwBufSize))
        //                    {
        //                        iLastErr = PlayCtrl.PlayM4_GetLastError(m_lPort);
        //                        str = "PlayM4_InputData failed, error code= " + iLastErr;
        //                        this.BeginInvoke(AlarmInfo, str);
        //                        Thread.Sleep(2);
        //                    }
        //                    else
        //                    {
        //                        break;
        //                    }
        //                }
        //            }
        //            break;
        //    }
        //}

        public void TemResultCallBack(IntPtr pData, int dataLen)
        {
            Console.WriteLine(dataLen);
            try
            {
                //string s = System.DateTime.Now.ToString();
                //// s = s.Substring(s.Length - 8, 8);


                //byte[] ImageData1 = new byte[dataLen];
                //float[] tempData = new float[dataLen / 2];
                //Marshal.Copy(pData, ImageData1, 0, dataLen);

                //int a = ImageData1[1] * 256 + ImageData1[0];

                ////for (int j = 0; j < 288; j++)
                ////{
                ////    for (int i = 0; i < 384; i++)
                ////    {
                ////        tempData[j * 384 + i] = (ushort)((ImageData1[j * 384 + (i * 2 + 1)] << 8) + ImageData1[j * 384 + (i * 2)]);
                ////        //判断温度区间0~7300，则温度换算公式（换算为摄氏度）(Value+7000)/30-273.2
                ////        //7301~16383， (Value-3300)/15-273.2
                ////        tempData[j * 384 + i] = (tempData[j * 384 + i] - 10000) / 100;
                ////    }
                ////}

                ////getTempUtil.setTempDate(tempData);

                ////Console.WriteLine(s +" " + Math.Round(getTempUtil.GetMaxTempAndPoint()[0]/100d,1));
                //Console.WriteLine(s + " " + a);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
           
        }

        public void RealDataCallBack(Int32 lRealHandle, UInt32 dwDataType, ref XGNetSDK.LPVEDIO_DATA lpVedioData, IntPtr pUser)
        {
            Width = 1920;
            Height = 1080;

            byte[] buf = new byte[Width * Height * 2];

            //unsafe
            //{
            //    fixed (byte* p = &(byte)(lpVedioData.pBuffer))
            //    {
            //        using (UnmanagedMemoryStream ms = new UnmanagedMemoryStream((byte*)p, Width * Height * 2))
            //        {
            //            ms.Read(buf, 0, buf.Length);
            //        }
            //    }
            //}
            int bytes1 = 1920 * 1080 * 4;
            byte[] ImageData1 = new byte[bytes1];

            //Buffer.BlockCopy(lpVedioData.pBuffer.ToBytes(), 0, video_data_show, 0, (int)Width * Height*4);

            //SendMessage(this.Handle, 2000, 0, 0);

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

                        OpenCvSharp.Mat mgMat;
                        OpenCvSharp.Mat mgMatShow = new OpenCvSharp.Mat();
                        mgMat = new OpenCvSharp.Mat((int)lpVedioData.nHeight, (int)lpVedioData.nWidth, OpenCvSharp.MatType.CV_8UC4, lpVedioData.pBuffer, 0);

                        OpenCvSharp.Size size = new OpenCvSharp.Size(RealPlayWnd.Width, RealPlayWnd.Height);            
                        Cv2.Resize(mgMat, mgMatShow, size, 0, 0, InterpolationFlags.Cubic);


                        OpenCvSharp.Point cor;
                        cor.X = 100;
                        cor.Y = 100;
                        Cv2.PutText(mgMatShow, "haha", cor, OpenCvSharp.HersheyFonts.HersheyPlain, 1.5, OpenCvSharp.Scalar.Blue, 1);

                        RealPlayWnd.Image = mgMatShow.ToBitmap();
                        //Marshal.Copy(lpVedioData.pBuffer, ImageData1, 0, bytes1);                      
                        // File.WriteAllBytes("1.bmp",ImageData1);
                        //MemoryStream datastream = new MemoryStream(ImageData1);
                        //Image image = Image.FromStream(datastream);//该句报错，参数无效，当向方法提供的参数之一无效时引发的异常。
                        //RealPlayWnd.Image = image;
                        //datastream.Dispose();
                        //if (!flag)
                        //{
                        //    flag = GDIHelper.initRender(m_ptrRealHandle, (int)lpVedioData.nWidth, (int)lpVedioData.nHeight);
                        //}
                        //GDIHelper.RenderPlay(lpVedioData.pBuffer, (int)lpVedioData.dwBufSize);
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

        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case 2000:   //处理消息
                    ImageShow();
                    break;
                default:
                    base.DefWndProc(ref m);   //调用基类函数处理非自定义消息。
                    break;
            }
        }

        protected void ImageShow()//绿色
        {
            fSx = (float)Width / (float)RealPlayWnd.Width;
            fSy = (float)Height / (float)RealPlayWnd.Height;
            OpenCvSharp.Mat mgMat;
            OpenCvSharp.Mat mgMatShow = new OpenCvSharp.Mat();

            mgMat = new OpenCvSharp.Mat(Height, Width, OpenCvSharp.MatType.CV_8UC2, video_data_show, 0);

            OpenCvSharp.Size size = new OpenCvSharp.Size(RealPlayWnd.Width, RealPlayWnd.Height);
            Cv2.Resize(mgMatShow, mgMatShow, size, 0, 0, InterpolationFlags.Cubic);

            RealPlayWnd.Image = mgMatShow.ToBitmap();
        }


        private void DecCallbackFUN(int nPort, IntPtr pBuf, int nSize, ref PlayCtrl.FRAME_INFO pFrameInfo, int nReserved1, int nReserved2)
        {

        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            bool flag = false;
            if (checkBoxPreview.Checked)
            {
                flag = XGNetSDK.NET_DVR_PTZControlWithSpeed_Other(m_lUserID,1, (uint)PTZ_CTRL_CMD.PTZ_UP, 0, (uint)comboBoxSpeed.SelectedIndex + 1);
            }
            else
            {
                flag = XGNetSDK.NET_DVR_PTZControl_Other(m_lUserID, m_lChannel, (uint)PTZ_CTRL_CMD.PTZ_UP, 0);
            }
        }

        private void button1_MouseUp(object sender, MouseEventArgs e)
        {
            bool flag = false;
            if (checkBoxPreview.Checked)
            {
                flag = XGNetSDK.NET_DVR_PTZControlWithSpeed_Other(m_lUserID,1, (uint)PTZ_CTRL_CMD.PTZ_UP, 1, (uint)comboBoxSpeed.SelectedIndex + 1);
            }
            else
            {
                flag = XGNetSDK.NET_DVR_PTZControl_Other(m_lUserID, m_lChannel, (uint)PTZ_CTRL_CMD.PTZ_UP, 1);
            }
        }

        private void button3_MouseDown(object sender, MouseEventArgs e)
        {
            bool flag = false;
            if (checkBoxPreview.Checked)
            {
                flag = XGNetSDK.NET_DVR_PTZControlWithSpeed_Other(m_lUserID,1, (uint)PTZ_CTRL_CMD.PTZ_DOWN, 0, (uint)comboBoxSpeed.SelectedIndex + 1);
            }
            else
            {
                flag = XGNetSDK.NET_DVR_PTZControl_Other(m_lUserID, m_lChannel, (uint)PTZ_CTRL_CMD.PTZ_DOWN, 0);
            }
        }

        private void button3_MouseUp(object sender, MouseEventArgs e)
        {
            bool flag = false;
            if (checkBoxPreview.Checked)
            {
                flag = XGNetSDK.NET_DVR_PTZControlWithSpeed_Other(m_lUserID,1, (uint)PTZ_CTRL_CMD.PTZ_DOWN, 1, (uint)comboBoxSpeed.SelectedIndex + 1);
            }
            else
            {
                flag = XGNetSDK.NET_DVR_PTZControl_Other(m_lUserID, m_lChannel, (uint)PTZ_CTRL_CMD.PTZ_DOWN, 1);
            }
        }

        private void btnJPEG_Click(object sender, EventArgs e)
        {
            int lChannel = Int16.Parse(textBoxChannel.Text); //通道号 Channel number

            XGNetSDK.NET_DVR_JPEGPARA lpJpegPara = new XGNetSDK.NET_DVR_JPEGPARA();
            lpJpegPara.wPicQuality = 0; //图像质量 Image quality
            lpJpegPara.wPicSize = 0xff; //抓图分辨率 Picture size: 0xff-Auto(使用当前码流分辨率) 
            //抓图分辨率需要设备支持，更多取值请参考SDK文档

            //JPEG抓图保存成文件 Capture a JPEG picture
            string sJpegPicFileName;
            sJpegPicFileName = Application.StartupPath + "\\filetest.jpg";//图片保存路径和文件名 the path and file name to save

            if (!XGNetSDK.NET_DVR_CaptureJPEGPicture(m_lUserID, lChannel, ref lpJpegPara, sJpegPicFileName))
            {
                iLastErr = XGNetSDK.NET_DVR_GetLastError();
                str = "NET_DVR_CaptureJPEGPicture failed, error code= " + iLastErr;
                MessageBox.Show(str);
                return;
            }
            else
            {
                str = "NET_DVR_CaptureJPEGPicture succ and the saved file is " + sJpegPicFileName;
                MessageBox.Show(str);
            }
            return;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            XGNetSDK.NET_DVR_Cleanup();
        }

        private void BtnBMP_Click(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {

        }

        private void PreSet_Click(object sender, EventArgs e)
        {

        }

        private void BtnRecord_Click(object sender, EventArgs e)
        {

        }

        private void Button3_Click(object sender, EventArgs e)
        {

        }

        private void Button2_Click(object sender, EventArgs e)
        {

        }

        private void Button5_Click(object sender, EventArgs e)
        {

            //cts = new CancellationTokenSource();
            //Task task = new Task(() =>
            //{
            //    capture.Open("rtsp://admin:admin@192.168.2.16:8557/streaming/2/main");
            //    if (capture.IsOpened())
            //    {
            //        //var dsize = new System.Windows.Size(capture.FrameWidth, capture.FrameHeight);
            //        Mat frame = new Mat();
            //        while (true)
            //        {
            //            Thread.Sleep(10);
            //            //判断是否被取消;
            //            if (cts.Token.IsCancellationRequested)
            //            {
            //                RealPlayWnd.Image = null;
            //                return;
            //            }
            //            //Read image
            //            capture.Read(frame);
            //            if (frame.Empty())
            //                continue;

            //            if (RealPlayWnd.Image != null)
            //            {
            //                RealPlayWnd.Image.Dispose();
            //            }
            //            RealPlayWnd.Image = BitmapConverter.ToBitmap(frame);
            //        }
            //    }

            //}, cts.Token);
            //task.Start();

            ThreadStart thStart = new ThreadStart(ShowImageThreadProc);//threadStart委托 
            Thread thread = new Thread(thStart);
            thread.Priority = ThreadPriority.Highest;
            thread.IsBackground = false; //关闭窗体继续执行

            thread.Start();
        }


        public void ShowImageThreadProc()
        {

            string rtspUrl = "rtsp://admin:admin@192.168.2.16:8557/streaming/2/main";//红外图像
            //string videoStreamAddress = "rtsp://" + "192.168.2.16" + ":8903";
            //Emgu.CV.Capture vcap = new Emgu.CV.Capture(videoStreamAddress);

            vcap.Open(rtspUrl);

            while (vcap.IsOpened())
            {
                OpenCvSharp.Mat mgMat;
                vcap.Read(frame);
                OpenCvSharp.Size size = new OpenCvSharp.Size(RealPlayWnd.Width, RealPlayWnd.Height);

                OpenCvSharp.Cv2.CvtColor(frame, mgMatShow, OpenCvSharp.ColorConversionCodes.RGBA2RGB);

                Cv2.Resize(mgMatShow, mgMatShow, size, 0, 0, InterpolationFlags.Cubic);

                OpenCvSharp.Point cor;
                cor.X = 100;
                cor.Y = 100;
                // strTemp = "hello";

               // Cv2.PutText(mgMatShow, "haha", cor, OpenCvSharp.HersheyFonts.HersheyPlain, 1.5, OpenCvSharp.Scalar.Blue, 1);

                
                RealPlayWnd.Image = mgMatShow.ToBitmap();

                //MessageBox.Show("打开成功");
                //Thread.Sleep(50);
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
           // XGNetSDK.NET_DVR_GetIRTemResult(m_lUserID, temresultCallBack);

        }

        private void Button4_Click(object sender, EventArgs e)
        {

        }
    }
}