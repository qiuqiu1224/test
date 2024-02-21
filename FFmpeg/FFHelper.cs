using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;

namespace FFmpeg.AutoGen.Example
{

    internal class FFHelper
    {
        private static AVHWDeviceType HWDevice;
        private static System.Windows.Forms.PictureBox pb;
        public static void Init(System.Windows.Forms.PictureBox Picture)
        {
            FFmpegBinariesHelper.RegisterFFmpegBinaries();
            DynamicallyLoadedBindings.Initialize();
            SetupLogging();
            ConfigureHWDecoder(out HWDevice);
            pb = Picture;
        }

        private static void ConfigureHWDecoder(out AVHWDeviceType HWtype)
        {
            HWtype = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE;
           
            var availableHWDecoders = new Dictionary<int, AVHWDeviceType>();
           
  
            var type = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE;
            var number = 0;

            while ((type = ffmpeg.av_hwdevice_iterate_types(type)) != AVHWDeviceType.AV_HWDEVICE_TYPE_NONE)
            {
                ++number;
                availableHWDecoders.Add(number, type);
            }

            if (availableHWDecoders.Count == 0)
            {
                HWtype = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE;
                return;
            }

            var decoderNumber = availableHWDecoders.SingleOrDefault(t => t.Value == AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2).Key;
            if (decoderNumber == 0)
                decoderNumber = availableHWDecoders.First().Key;
            availableHWDecoders.TryGetValue(decoderNumber,out HWtype);
        }

        private static unsafe void SetupLogging()
        {
            ffmpeg.av_log_set_level(ffmpeg.AV_LOG_VERBOSE);

            // do not convert to local function
            av_log_set_callback_callback logCallback = (p0, level, format, vl) =>
            {
                if (level > ffmpeg.av_log_get_level()) return;

                var lineSize = 1024;
                var lineBuffer = stackalloc byte[lineSize];
                var printPrefix = 1;
                ffmpeg.av_log_format_line(p0, level, format, vl, lineBuffer, lineSize, &printPrefix);
                var line = Marshal.PtrToStringAnsi((IntPtr)lineBuffer);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(line);
                Console.ResetColor();
            };

            ffmpeg.av_log_set_callback(logCallback);
        }

        public static unsafe void DecodeAllFramesToImages()
        {
            // decode all frames from url, please not it might local resorce, e.g. string url = "../../sample_mpeg4.mp4";

            var url = "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4"; // be advised this file holds 1440 frames
            using (var vsd = new VideoStreamDecoder(url, HWDevice))
            {
                var info = vsd.GetContextInfo();
                info.ToList().ForEach(x => Console.WriteLine($"{x.Key} = {x.Value}"));

                var sourceSize = vsd.FrameSize;
                var sourcePixelFormat = HWDevice == AVHWDeviceType.AV_HWDEVICE_TYPE_NONE
                    ? vsd.PixelFormat
                    : GetHWPixelFormat(HWDevice);
                var destinationSize = sourceSize;
                var destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24;
                using (var vfc = new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat))
                {
                    var frameNumber = 0;

                    while (vsd.TryDecodeNextFrame(out var frame))
                    {
                        var convertedFrame = vfc.Convert(frame);

                        using (var bitmap = new Bitmap(convertedFrame.width,
                                   convertedFrame.height,
                                   convertedFrame.linesize[0],
                                   PixelFormat.Format24bppRgb,
                                   (IntPtr)convertedFrame.data[0]))
                            if (pb.InvokeRequired)
                            {
                                pb.BeginInvoke(new Action(() => { pb.Image = bitmap; }));
                            }

                        frameNumber++;
                    }
                }
            }
        }

        private static AVPixelFormat GetHWPixelFormat(AVHWDeviceType hWDevice)
        {
            AVPixelFormat dtype;
            switch (hWDevice) 
            {
                case AVHWDeviceType.AV_HWDEVICE_TYPE_NONE:
                    dtype = AVPixelFormat.AV_PIX_FMT_NONE;
                    break;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_VDPAU:
                    dtype = AVPixelFormat.AV_PIX_FMT_VDPAU;
                    break;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_CUDA:
                    dtype = AVPixelFormat.AV_PIX_FMT_CUDA;
                    break;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_VAAPI:
                    dtype = AVPixelFormat.AV_PIX_FMT_VAAPI;
                    break;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2:
                    dtype = AVPixelFormat.AV_PIX_FMT_NV12;
                    break;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_QSV:
                    dtype = AVPixelFormat.AV_PIX_FMT_QSV;
                    break;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_VIDEOTOOLBOX:
                    dtype = AVPixelFormat.AV_PIX_FMT_VIDEOTOOLBOX;
                    break;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_D3D11VA:
                    dtype = AVPixelFormat.AV_PIX_FMT_NV12;
                    break;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_DRM:
                    dtype = AVPixelFormat.AV_PIX_FMT_DRM_PRIME;
                    break;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_OPENCL:
                    dtype = AVPixelFormat.AV_PIX_FMT_OPENCL;
                    break;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_MEDIACODEC:
                    dtype = AVPixelFormat.AV_PIX_FMT_MEDIACODEC;
                    break;
                default:
                    dtype = AVPixelFormat.AV_PIX_FMT_NONE;
                    break;
            }
            return dtype;
        }

        private static unsafe void EncodeImagesToH264()
        {
            var frameFiles = Directory.GetFiles("./frames", "frame.*.jpg").OrderBy(x => x).ToArray();
            var fistFrameImage = Image.FromFile(frameFiles.First());

            var outputFileName = "frames/out.h264";
            var fps = 25;
            var sourceSize = fistFrameImage.Size;
            var sourcePixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24;
            var destinationSize = sourceSize;
            var destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_YUV420P;
            using (var vfc = new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat))
            {
                using (var fs = File.Open(outputFileName, FileMode.Create))
                {
                    using (var vse = new H264VideoStreamEncoder(fs, fps, destinationSize))
                    {
                        var frameNumber = 0;

                        foreach (var frameFile in frameFiles)
                        {
                            byte[] bitmapData;

                            using (var frameImage = Image.FromFile(frameFile))
                            using (var frameBitmap = frameImage is Bitmap bitmap ? bitmap : new Bitmap(frameImage))
                                bitmapData = GetBitmapData(frameBitmap);

                            fixed (byte* pBitmapData = bitmapData)
                            {
                                var data = new byte_ptrArray8 { [0] = pBitmapData };
                                var linesize = new int_array8 { [0] = bitmapData.Length / sourceSize.Height };
                                var frame = new AVFrame
                                {
                                    data = data,
                                    linesize = linesize,
                                    height = sourceSize.Height
                                };
                                var convertedFrame = vfc.Convert(frame);
                                convertedFrame.pts = frameNumber * fps;
                                vse.Encode(convertedFrame);
                            }

                            Console.WriteLine($"frame: {frameNumber}");
                            frameNumber++;
                        }
                        vse.Drain();
                    }                   
                }
            }
        }

        private static byte[] GetBitmapData(Bitmap frameBitmap)
        {
            var bitmapData = frameBitmap.LockBits(new Rectangle(Point.Empty, frameBitmap.Size),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);

            try
            {
                var length = bitmapData.Stride * bitmapData.Height;
                var data = new byte[length];
                Marshal.Copy(bitmapData.Scan0, data, 0, length);
                return data;
            }
            finally
            {
                frameBitmap.UnlockBits(bitmapData);
            }
        }
    }
}