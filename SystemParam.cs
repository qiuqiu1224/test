﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WindowsFormsApp1
{
    public class SystemParam
    {

        public SystemParam() { }

        [XmlElement(Type = typeof(int), ElementName = "CameraNum")]
        public int cameraNum = 2;

        #region 1# 左上
        /// <summary>
        /// 左上名称
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "Name_1")]
        public string name_1 = "轴1";

        /// <summary>
        /// 左上 IP地址
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "IP_1")]
        public string ip_1 = "192.168.1.20";

        /// <summary>
        /// 左上端口
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "Port_1")]
        public int port_1 = 54321;

        /// <summary>
        /// 左上报警温度
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "Alarm_1")]
        public float alarm_1 = 100.0F;

        /// <summary>
        /// 左上相对补偿
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "RCompensation_1")]
        public int relativeCompensation_1 = 0;

        /// <summary>
        /// 左上绝对补偿
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "ACompensation_1")]
        public float absoluteCompensation_1 = 0.0F;
        #endregion


        #region 2# 右上

        /// <summary>
        /// 右上名称
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "Name_2")]
        public string name_2 = "轴2";

        /// <summary>
        /// 右上 IP地址
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "IP_2")]
        public string ip_2 = "192.168.1.20";

        /// <summary>
        /// 右上端口
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "Port_2")]
        public int port_2 = 54321;

        /// <summary>
        /// 右上报警温度
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "Alarm_2")]
        public float alarm_2 = 100.0F;

        /// <summary>
        /// 右上相对补偿
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "RCompensation_2")]
        public int relativeCompensation_2 = 0;

        /// <summary>
        /// 右上相对补偿
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "ACompensation_2")]
        public float absoluteCompensation_2 = 0.0F;

        #endregion




        #region 3# 左下
        /// <summary>
        /// 左下名称
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "Name_3")]
        public string name_3 = "轴3";

        /// <summary>
        /// 左下 IP地址
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "IP_3")]
        public string ip_3 = "192.168.1.20";

        /// <summary>
        /// 左下端口
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "Port_3")]
        public int port_3 = 54321;

        /// <summary>
        /// 左下报警温度
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "Alarm_3")]
        public float alarm_3 = 100.0F;

        /// <summary>
        /// 左下相对补偿
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "RCompensation_3")]
        public int relativeCompensation_3 = 0;

        /// <summary>
        /// 左下相对补偿
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "ACompensation_3")]
        public float absoluteCompensation_3 = 0.0F;
        #endregion




        #region 4# 右下
        /// <summary>
        /// 右下名称
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "Name_4")]
        public string name_4 = "轴4";


        /// <summary>
        /// 右下 IP地址
        /// </summary>
        [XmlElement(Type = typeof(string), ElementName = "IP_4")]
        public string ip_4 = "192.168.1.20";

        /// <summary>
        /// 右下端口
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "Port_4")]
        public int port_4 = 54321;

        /// <summary>
        /// 右下报警温度
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "Alarm_4")]
        public float alarm_4 = 100.0F;

        /// <summary>
        /// 右下相对补偿
        /// </summary>
        [XmlElement(Type = typeof(int), ElementName = "RCompensation_4")]
        public int relativeCompensation_4 = 0;

        /// <summary>
        /// 右下相对补偿
        /// </summary>
        [XmlElement(Type = typeof(float), ElementName = "ACompensation_4")]
        public float absoluteCompensation_4 = 0.0F;
        #endregion


        ///// <summary>
        ///// 定期删除图像时间间隔
        ///// </summary>
        //[XmlElement(Type = typeof(int), ElementName = "Delete_Interval")]
        //public int delete_interval = 7200000;//2小时


        ///// <summary>
        ///// 图片保存期限 单位：天
        ///// </summary>
        //[XmlElement(Type = typeof(int), ElementName = "Image_Storage_Life")]
        //public int image_storage_life = 7;
    }
}
