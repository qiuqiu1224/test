﻿
#define x64
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace WindowsFormsApp1
{
    public class XGNetSDK
    {
        public const int TILT_UP = 21;/* 云台以SS的速度上仰 */
        public const int TILT_DOWN = 22;/* 云台以SS的速度下俯 */
        public const int PAN_LEFT = 23;/* 云台以SS的速度左转 */
        public const int PAN_RIGHT = 24;/* 云台以SS的速度右转 */
        public const int UP_LEFT = 25;/* 云台以SS的速度上仰和左转 */
        public const int UP_RIGHT = 26;/* 云台以SS的速度上仰和右转 */
        public const int DOWN_LEFT = 27;/* 云台以SS的速度下俯和左转 */
        public const int DOWN_RIGHT = 28;/* 云台以SS的速度下俯和右转 */
        public const int PAN_AUTO = 29;/* 云台以SS的速度左右自动扫描 */
        public const int NET_DVR_DEV_ADDRESS_MAX_LEN = 129;
        public const int STREAM_ID_LEN = 32;
        public const int NET_DVR_LOGIN_USERNAME_MAX_LEN = 64;
        public const int NET_DVR_LOGIN_PASSWD_MAX_LEN = 64;
        public const int SERIALNO_LEN = 48;//序列号长度

        public const int NET_DVR_SYSHEAD = 1;//系统头数据
        public const int NET_DVR_STREAMDATA = 2;//视频流数据（包括复合流和音视频分开的视频流数据）
        public const int NET_DVR_AUDIOSTREAMDATA = 3;//音频流数据
        public const int NET_DVR_STD_VIDEODATA = 4;//标准视频流数据
        public const int NET_DVR_STD_AUDIODATA = 5;//标准音频流数据

        //预览V40接口
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_PREVIEWINFO
        {
            public Int32 lChannel;//通道号1-可见光，2-红外，3-可见光识别，4-红外识别
            public uint dwStreamType;   // 码流类型，0-主码流，1-子码流，2-码流3，3-码流4 等以此类推
            public uint dwLinkMode;// 0：TCP方式,1：UDP方式,2：多播方式,3 - RTP方式，4-RTP/RTSP,5-RSTP/HTTP 6- HRUDP（可靠传输） ,7-RTSP/HTTPS
            public IntPtr hPlayWnd;//播放窗口的句柄,为NULL表示不播放图象
            public uint bBlocked;  //0-非阻塞取流, 1-阻塞取流, 如果阻塞SDK内部connect失败将会有5s的超时才能够返回,不适合于轮询取流操作.
            public uint bPassbackRecord; //0-不启用录像回传,1启用录像回传
            public byte byPreviewMode;//预览模式，0-正常预览，1-延迟预览
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = STREAM_ID_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byStreamID;//流ID，lChannel为0xffffffff时启用此参数
            public byte byProtoType; //应用层取流协议，0-私有协议，1-RTSP协议
            public byte byRes1;
            public byte byVideoCodingType; //码流数据编解码类型 0-通用编码数据 1-热成像探测器产生的原始数据（温度数据的加密信息，通过去加密运算，将原始数据算出真实的温度值）
            public uint dwDisplayBufNum; //播放库播放缓冲区最大缓冲帧数，范围1-50，置0时默认为1 
            public byte byNPQMode;  //NPQ是直连模式，还是过流媒体 0-直连 1-过流媒体
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 215, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        public delegate void LOGINRESULTCALLBACK(int lUserID, int dwResult, IntPtr lpDeviceInfo, IntPtr pUser);

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_USER_LOGIN_INFO
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NET_DVR_DEV_ADDRESS_MAX_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sDeviceAddress;
            public byte byUseTransport;
            public ushort wPort;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NET_DVR_LOGIN_USERNAME_MAX_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sUserName;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NET_DVR_LOGIN_PASSWD_MAX_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sPassword;
            public LOGINRESULTCALLBACK cbLoginResult;
            public IntPtr pUser;
            public bool bUseAsynLogin;
            public byte byProxyType; //0:不使用代理，1：使用标准代理，2：使用EHome代理
            public byte byUseUTCTime;    //0-不进行转换，默认,1-接口上输入输出全部使用UTC时间,SDK完成UTC时间与设备时区的转换,2-接口上输入输出全部使用平台本地时间，SDK完成平台本地时间与设备时区的转换
            public byte byLoginMode; //0-Private, 1-ISAPI, 2-自适应
            public byte byHttps;    //0-不适用tls，1-使用tls 2-自适应
            public int iProxyID;    //代理服务器序号，添加代理服务器信息时，相对应的服务器数组下表值
            public byte byVerifyMode;  //认证方式，0-不认证，1-双向认证，2-单向认证；认证仅在使用TLS的时候生效;    
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 119, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes3;
        }
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_DEVICEINFO_V30
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = SERIALNO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sSerialNumber;  //序列号
            public byte byAlarmInPortNum;               //报警输入个数
            public byte byAlarmOutPortNum;              //报警输出个数
            public byte byDiskNum;                  //硬盘个数
            public byte byDVRType;                  //设备类型, 1:DVR 2:ATM DVR 3:DVS ......
            public byte byChanNum;                  //模拟通道个数
            public byte byStartChan;                    //起始通道号,例如DVS-1,DVR - 1
            public byte byAudioChanNum;                //语音通道数
            public byte byIPChanNum;                    //最大数字通道个数，低位  
            public byte byZeroChanNum;          //零通道编码个数 //2010-01-16
            public byte byMainProto;            //主码流传输协议类型 0-private, 1-rtsp,2-同时支持private和rtsp
            public byte bySubProto;             //子码流传输协议类型0-private, 1-rtsp,2-同时支持private和rtsp
            public byte bySupport;        //能力，位与结果为0表示不支持，1表示支持，
                                          //bySupport & 0x1, 表示是否支持智能搜索
                                          //bySupport & 0x2, 表示是否支持备份
                                          //bySupport & 0x4, 表示是否支持压缩参数能力获取
                                          //bySupport & 0x8, 表示是否支持多网卡
                                          //bySupport & 0x10, 表示支持远程SADP
                                          //bySupport & 0x20, 表示支持Raid卡功能
                                          //bySupport & 0x40, 表示支持IPSAN 目录查找
                                          //bySupport & 0x80, 表示支持rtp over rtsp
            public byte bySupport1;        // 能力集扩充，位与结果为0表示不支持，1表示支持
                                           //bySupport1 & 0x1, 表示是否支持snmp v30
                                           //bySupport1 & 0x2, 支持区分回放和下载
                                           //bySupport1 & 0x4, 是否支持布防优先级	
                                           //bySupport1 & 0x8, 智能设备是否支持布防时间段扩展
                                           //bySupport1 & 0x10, 表示是否支持多磁盘数（超过33个）
                                           //bySupport1 & 0x20, 表示是否支持rtsp over http	
                                           //bySupport1 & 0x80, 表示是否支持车牌新报警信息2012-9-28, 且还表示是否支持NET_DVR_IPPARACFG_V40结构体
            public byte bySupport2; /*能力，位与结果为0表示不支持，非0表示支持							
							bySupport2 & 0x1, 表示解码器是否支持通过URL取流解码
							bySupport2 & 0x2,  表示支持FTPV40
							bySupport2 & 0x4,  表示支持ANR
							bySupport2 & 0x8,  表示支持CCD的通道参数配置
							bySupport2 & 0x10,  表示支持布防报警回传信息（仅支持抓拍机报警 新老报警结构）
							bySupport2 & 0x20,  表示是否支持单独获取设备状态子项
							bySupport2 & 0x40,  表示是否是码流加密设备*/
            public ushort wDevType;              //设备型号
            public byte bySupport3; //能力集扩展，位与结果为0表示不支持，1表示支持
                                    //bySupport3 & 0x1, 表示是否多码流
                                    // bySupport3 & 0x4 表示支持按组配置， 具体包含 通道图像参数、报警输入参数、IP报警输入、输出接入参数、
                                    // 用户参数、设备工作状态、JPEG抓图、定时和时间抓图、硬盘盘组管理 
                                    //bySupport3 & 0x8为1 表示支持使用TCP预览、UDP预览、多播预览中的"延时预览"字段来请求延时预览（后续都将使用这种方式请求延时预览）。而当bySupport3 & 0x8为0时，将使用 "私有延时预览"协议。
                                    //bySupport3 & 0x10 表示支持"获取报警主机主要状态（V40）"。
                                    //bySupport3 & 0x20 表示是否支持通过DDNS域名解析取流

            public byte byMultiStreamProto;//是否支持多码流,按位表示,0-不支持,1-支持,bit1-码流3,bit2-码流4,bit7-主码流，bit-8子码流
            public byte byStartDChan;       //起始数字通道号,0表示无效
            public byte byStartDTalkChan;   //起始数字对讲通道号，区别于模拟对讲通道号，0表示无效
            public byte byHighDChanNum;     //数字通道个数，高位
            public byte bySupport4;
            public byte byLanguageType;// 支持语种能力,按位表示,每一位0-不支持,1-支持  
                                       //  byLanguageType 等于0 表示 老设备
                                       //  byLanguageType & 0x1表示支持中文
                                       //  byLanguageType & 0x2表示支持英文
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 9, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;       //保留
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DEVICEINFO_V40
        {
            public NET_DVR_DEVICEINFO_V30 struDeviceV30;
            public byte bySupportLock;        //设备支持锁定功能，该字段由SDK根据设备返回值来赋值的。bySupportLock为1时，dwSurplusLockTime和byRetryLoginTime有效
            public byte byRetryLoginTime;       //剩余可尝试登陆的次数，用户名，密码错误时，此参数有效
            public byte byPasswordLevel;      //admin密码安全等级0-无效，1-默认密码，2-有效密码,3-风险较高的密码。当用户的密码为出厂默认密码（12345）或者风险较高的密码时，上层客户端需要提示用户更改密码。      
            public byte byProxyType;//代理类型，0-不使用代理, 1-使用socks5代理, 2-使用EHome代理
            public uint dwSurplusLockTime;  //剩余时间，单位秒，用户锁定时，此参数有效
            public byte byCharEncodeType;     //字符编码类型（SDK所有接口返回的字符串编码类型，透传接口除外）：0- 无字符编码信息(老设备)，1- GB2312(简体中文)，2- GBK，3- BIG5(繁体中文)，4- Shift_JIS(日文)，5- EUC-KR(韩文)，6- UTF-8，7- ISO8859-1，8- ISO8859-2，9- ISO8859-3，…，依次类推，21- ISO8859-15(西欧) 
            public byte bySupportDev5;//支持v50版本的设备参数获取，设备名称和设备类型名称长度扩展为64字节
            public byte bySupport;  //能力集扩展，位与结果：0- 不支持，1- 支持
                                    // bySupport & 0x1:  保留
                                    // bySupport & 0x2:  0-不支持变化上报 1-支持变化上报
            public byte byLoginMode; //登录模式 0-Private登录 1-ISAPI登录
            public int dwOEMCode;
            public int iResidualValidity;   //该用户密码剩余有效天数，单位：天，返回负值，表示密码已经超期使用，例如“-3表示密码已经超期使用3天”
            public byte byResidualValidity; // iResidualValidity字段是否有效，0-无效，1-有效
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 243, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
        }

        public struct NET_DVR_JPEGPARA
        {
            /*注意：当图像压缩分辨率为VGA时，支持0=CIF, 1=QCIF, 2=D1抓图，
	        当分辨率为3=UXGA(1600x1200), 4=SVGA(800x600), 5=HD720p(1280x720),6=VGA,7=XVGA, 8=HD900p
	        仅支持当前分辨率的抓图*/
            public ushort wPicSize;/* 0=CIF, 1=QCIF, 2=D1 3=UXGA(1600x1200), 4=SVGA(800x600), 5=HD720p(1280x720),6=VGA*/
            public ushort wPicQuality;/* 图片质量系数 0-最好 1-较好 2-一般 */
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct LPVEDIO_DATA
        {
            public IntPtr pBuffer;
            public uint dwBufSize;
            public IntPtr pExtraBuffer;
            public uint dwExtraBufSize;
            public int nWidth;
            public int nHeight;
        }

        public delegate void REALDATACALLBACK(Int32 lRealHandle, UInt32 dwDataType, ref LPVEDIO_DATA lpVedioData, IntPtr pUser);
        public delegate void TEMRESULTCALLBACK(IntPtr pData, Int32 dataLen);
        
#if x86
        [DllImport(@".\XGNetSDK-x86.dll", EntryPoint = "_NET_DVR_RealPlay_V40@16")]
        public static extern int NET_DVR_RealPlay_V40(long lUserID, ref NET_DVR_PREVIEWINFO lpPreviewInfo, REALDATACALLBACK fRealDataCallBack_V30, IntPtr pUser);
        [DllImport(@".\XGNetSDK-x86.dll")]
        public static extern bool NET_DVR_Init();
        [DllImport(@".\XGNetSDK-x86.dll")]
        public static extern int NET_DVR_Login(ref NET_DVR_USER_LOGIN_INFO pLoginInfo, ref NET_DVR_DEVICEINFO_V40 lpDeviceInfo);
        [DllImport(@".\XGNetSDK-x86.dll")]
        public static extern uint NET_DVR_GetLastError();
        [DllImport(@".\XGNetSDK-x86.dll")]
        public static extern bool NET_DVR_Logout(int iUserID);
        //[DllImport("XGNetSDK-x64.dll", EntryPoint = "NET_DVR_StopRealPlay", CharSet = CharSet.Auto)]
        [DllImport(@".\XGNetSDK-x86.dll")]
        public static extern bool NET_DVR_StopRealPlay(long iRealHandle);
        [DllImport(@".\XGNetSDK-x86.dll")]
        public static extern bool NET_DVR_PTZControl_Other(Int32 lUserID, Int32 lChannel, uint dwPTZCommand, uint dwStop);
        [DllImport(@".\XGNetSDK-x86.dll")]
        public static extern bool NET_DVR_PTZControlWithSpeed(long lRealHandle, uint dwPTZCommand, uint dwStop, uint dwSpeed);
        [DllImport(@".\XGNetSDK-x86.dll")]
        public static extern bool NET_DVR_CaptureJPEGPicture(int lUserID, int lChannel, ref NET_DVR_JPEGPARA lpJpegPara, string sPicFileName);
        [DllImport(@".\XGNetSDK-x86.dll")]
        public static extern bool NET_DVR_Cleanup();
        [DllImport(@".\XGNetSDK-x86.dll")]
        public static extern bool NET_DVR_SetLogToFile(int bLogEnable, string strLogDir, bool bAutoDel);
#else
        [DllImport(@".\XGNetSDK-x64.dll",EntryPoint = "NET_DVR_RealPlay")]
        public static extern long NET_DVR_RealPlay(long lUserID, ref NET_DVR_PREVIEWINFO lpPreviewInfo, REALDATACALLBACK fRealDataCallBack_V30, IntPtr pUser);
        [DllImport(@".\XGNetSDK-x64.dll")]
        public static extern bool NET_DVR_Init();
        [DllImport(@".\XGNetSDK-x64.dll")]
        public static extern int NET_DVR_Login(ref NET_DVR_USER_LOGIN_INFO pLoginInfo, ref NET_DVR_DEVICEINFO_V40 lpDeviceInfo);
        [DllImport(@".\XGNetSDK-x64.dll", EntryPoint = "NET_DVR_GetLastError")]
        public static extern uint NET_DVR_GetLastError();
        [DllImport(@".\XGNetSDK-x64.dll")]
        public static extern bool NET_DVR_Logout(int iUserID);
        //[DllImport("XGNetSDK-x64.dll", EntryPoint = "NET_DVR_StopRealPlay", CharSet = CharSet.Auto)]
        [DllImport(@".\XGNetSDK-x64.dll")]
        public static extern bool NET_DVR_StopRealPlay(long iRealHandle);
        [DllImport(@".\XGNetSDK-x64.dll")]
        public static extern bool NET_DVR_PTZControl_Other(Int32 lUserID, Int32 lChannel, uint dwPTZCommand, uint dwStop);
        [DllImport(@".\XGNetSDK-x64.dll")]
        public static extern bool NET_DVR_PTZControlWithSpeed_Other(long lUserId, long lChannel, uint dwPTZCommand, uint dwStop, uint dwSpeed);
        [DllImport(@".\XGNetSDK-x64.dll")]
        public static extern bool NET_DVR_CaptureJPEGPicture(int lUserID, int lChannel, ref NET_DVR_JPEGPARA lpJpegPara, string sPicFileName);
        [DllImport(@".\XGNetSDK-x64.dll")]
        public static extern bool NET_DVR_Cleanup();
        [DllImport(@".\XGNetSDK-x64.dll")]
        public static extern bool NET_DVR_SetLogToFile(int bLogEnable, string strLogDir, bool bAutoDel);

        [DllImport(@".\XGNetSDK-x64.dll")]
        public static extern bool NET_DVR_GetIRTemResult(int bLogEnable, TEMRESULTCALLBACK temresultCb);

#endif
    }
    public enum PTZ_CTRL_CMD
    {
        PTZ_UP = 1,    //云台 上仰
        PTZ_DOWN = 2,     //云台 下俯
        PTZ_LEFT = 3,     //云台 左转
        PTZ_RIGHT = 4,    //云台 右转
        PTZ_STOP = 5,     //云台 停止转动

        PTZ_VL_ZOOM_NEAR = 6,  //可见光摄像机 镜头拉近
        PTZ_VL_ZOOM_FAR = 7,       //可见光摄像机 镜头拉远
        PTZ_VL_ZOOM_STOP = 8,      //可见光摄像机 镜头拉焦停止
        PTZ_VL_ZOOM_RAISE = 9,     //可见光摄像机 焦距增加
        PTZ_VL_ZOOM_REDUCE = 10,   //可见光摄像机 焦距减少
        PTZ_VL_ZOOM_AUTO = 11,     //可见光摄像机 自动聚焦
        PTZ_VL_ZOOM_RATION = 12,  //可见光摄像机 倍率值设置, 1-30倍
        PTZ_VL_ZOOM_FOCUS = 13,   //可见光摄像机 聚焦值设置, 聚集值为整数类型
        PTZ_VL_ZOOM_NEAR_FAR_STOP = 14,   //可见光摄像机 镜头拉远近stop

        PTZ_IR_ZOOM_NEAR_FAR_STOP = 16,      //红外热像仪 镜头拉远stop
        PTZ_IR_ZOOM_NEAR = 17,      //红外热像仪 镜头拉远
        PTZ_IR_ZOOM_FAR = 18,      //红外热像仪 镜头拉远
        PTZ_IR_ZOOM_STOP = 19,     //红外热像仪 镜头拉焦停止
        PTZ_IR_ZOOM_RAISE = 20,    //红外热像仪 焦距增加
        PTZ_IR_ZOOM_REDUCE = 21,   //红外热像仪 焦距减少
        PTZ_IR_ZOOM_AUTO = 22,     //红外热像仪 自动聚焦
        PTZ_IR_ZOOM_RATION = 27, //红外热像仪 倍率值设置, 1-30倍
        PTZ_IR_ZOOM_FOCUS = 28,       //红外热像仪 聚焦值设置, 聚集值为整数类型
    }
}
