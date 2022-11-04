using SkiaSharp;
using System;
using System.Runtime.InteropServices;

namespace LinCms.Common
{
    /// <summary>
    /// https://cloud.tencent.com/developer/article/2142750
    /// </summary>
    public class ImgHelper
    {
        /// <summary>
        /// 获取图像数字验证码
        /// </summary>
        /// <param name="text">验证码内容，如4为数字</param>
        /// <returns></returns>
        public static byte[] GetVerifyCode(string text)
        {
            int width = 128;
            int height = 45;

            Random random = new();

            //创建bitmap位图
            using SKBitmap image = new(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
            //创建画笔
            using SKCanvas canvas = new(image);
            //填充背景颜色为白色
            canvas.DrawColor(SKColors.White);

            //画图片的背景噪音线
            for (int i = 0; i < width * height * 0.015; i++)
            {
                using SKPaint drawStyle = new();
                drawStyle.Color = new(Convert.ToUInt32(random.Next(int.MaxValue)));
                canvas.DrawLine(random.Next(0, width), random.Next(0, height), random.Next(0, width), random.Next(0, height), drawStyle);
            }
            //将文字写到画布上
            var fonts = new[] { "Cantarell", "Karla" };
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                fonts = new[] { "宋体" };
            }
            using (SKPaint drawStyle = new())
            {
                var font = SKTypeface.FromFamilyName(fonts[random.Next(0, fonts.Length - 1)], SKFontStyleWeight.SemiBold, SKFontStyleWidth.ExtraCondensed, SKFontStyleSlant.Upright);
                drawStyle.Color = SKColors.Red;
                drawStyle.TextSize = height;
                drawStyle.StrokeWidth = 2;
                drawStyle.IsAntialias = true;
                drawStyle.Typeface = font;

                float emHeight = height - height * (float)0.14;
                float emWidth = (float)width / text.Length - width * (float)0.13;
                canvas.DrawText(text, emWidth, emHeight, drawStyle);
            }

            //画图片的前景噪音点
            for (int i = 0; i < width * height * 0.78; i++)
            {
                image.SetPixel(random.Next(0, width), random.Next(0, height), new SKColor(Convert.ToUInt32(random.Next(int.MaxValue))));
            }

            using var img = SKImage.FromBitmap(image);
            using SKData p = img.Encode(SKEncodedImageFormat.Png, 100);
            return p.ToArray();
        }

    }
}
