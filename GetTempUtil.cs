using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class GetTempUtil
    {
        public float[] mTemp;
        private int imageWidth;
        private int imageHeight;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="width">图像宽</param>
        /// <param name="height">图像高</param>
        public void init(int width, int height)
        {
            this.imageHeight = height;
            this.imageWidth = width;
            this.mTemp = new float[width * height];
        }

        public void setTempDate(float[] tempdata)
        {
            this.mTemp = tempdata;
        }

        public void tempRotate(float[] oriTemp, float[] rotateTemp, int width, int height)
        {
            int count = 0;

            for (int j = width - 1; j >= 0; --j)
            {
                for (int i = 0; i < height; ++i)
                {
                    rotateTemp[count] = oriTemp[i * width + j];
                    ++count;
                }
            }

        }

        /// <summary>
        /// 获取最高温度及位置坐标
        /// </summary>
        /// <returns>maxTempAndPoint[0]：最高温度值*100   maxTempAndPoint[1]：最高温x坐标 maxTempAndPoint[2]：最高温y坐标</returns>
        public int[] GetMaxTempAndPoint()
        {
            int[] maxTempAndPoint = new int[3];

            float maxTemp = mTemp[0];

            for (int j = 0; j < imageHeight; j++)
            {
                for (int i = 0; i < imageWidth; i++)
                {
                    float temp = mTemp[i + j * imageWidth];
                    if (temp > maxTemp)
                    {
                        maxTemp = temp;
                        maxTempAndPoint[1] = i;
                        maxTempAndPoint[2] = j;
                    }
                }
            }


            //for (int i = 0; i < 384 * 288; i++)
            //{

            //    if (mTemp[i] > maxTemp)
            //    {
            //        maxTemp = mTemp[i];
            //        maxTempAndPoint[1] = i % 384;
            //        maxTempAndPoint[2] = i / 384;
            //    }
            //}

           
            maxTempAndPoint[0] = (int)(maxTemp * 100);

            return maxTempAndPoint;
        }

        /// <summary>
        /// 获取最低温度及位置坐标
        /// </summary>
        /// <returns>maxTempAndPoint[0]：最低温度值*100   minTempAndPoint[1]：最低温x坐标 minTempAndPoint[2]：最低温y坐标</returns>
        public int[] GetMinTempAndPoint()
        {
            int[] minTempAndPoint = new int[3];

            float minTemp = mTemp[0];
            for (int j = 0; j < imageHeight; j++)
            {
                for (int i = 0; i < imageWidth; i++)
                {
                    float temp = mTemp[i + j * imageWidth];
                    if (temp < minTemp)
                    {
                        minTemp = temp;
                        minTempAndPoint[1] = i;
                        minTempAndPoint[2] = j;
                    }
                }
            }

            minTempAndPoint[0] = (int)(minTemp * 100);
            return minTempAndPoint;
        }

        /// <summary>
        /// 获取区域最高温度点及位置
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public int[] GetRectMaxTempAndPoint(Point start,Point end)
        {
            int[] result = new int[3];
            int startX = start.X < end.X ? start.X : end.X;            
            int startY = start.Y < end.Y ? start.Y : end.Y;
            int endX = start.X < end.X ? end.X : start.X;
            int endY = start.Y < end.Y ? end.Y : start.Y;

            result[0] = (int)(this.mTemp[(int)(startX + startY * imageWidth)] * 100);
            result[1] = startX;
            result[2] = startY;

            for (int j = (int)startY; (float)j <= endY; ++j)
            {
                for (int i = (int)startX; (float)i <= endX; ++i)
                {                   
                    int index = (int)((double)i + (double)j * (double)imageWidth * 1.0D);
                    if ((int)(this.mTemp[index]*100) > result[0])
                    {
                        result[0] = (int)(this.mTemp[index] * 100);
                        result[1] = i;
                        result[2] = j;
                    }
                }
            }
            return result;
        }
    }
}
