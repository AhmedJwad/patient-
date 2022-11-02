using HealthCare.Common.Enums;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace HealthCare.API.Models
{
    public static class ColorCalculator
    {
        public static System.Drawing.Bitmap ArithmeticBlend(this System.Drawing.Bitmap sourceBitmap, System.Drawing.Bitmap blendBitmap,
                              ColorCalculationType calculationType)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new System.Drawing.Rectangle(0, 0,
                                    sourceBitmap.Width, sourceBitmap.Height),
                                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);


            BitmapData blendData = blendBitmap.LockBits(new System.Drawing.Rectangle(0, 0,
                                   blendBitmap.Width, blendBitmap.Height),
                                   ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


            byte[] blendBuffer = new byte[blendData.Stride * blendData.Height];
            Marshal.Copy(blendData.Scan0, blendBuffer, 0, blendBuffer.Length);
            blendBitmap.UnlockBits(blendData);


            for (int k = 0; (k + 4 < pixelBuffer.Length) &&
                            (k + 4 < blendBuffer.Length); k += 4)
            {
                pixelBuffer[k] = Calculate(pixelBuffer[k],
                                    blendBuffer[k], calculationType);


                pixelBuffer[k + 1] = Calculate(pixelBuffer[k + 1],
                                        blendBuffer[k + 1], calculationType);


                pixelBuffer[k + 2] = Calculate(pixelBuffer[k + 2],
                                        blendBuffer[k + 2], calculationType);
            }


            System.Drawing.Bitmap resultBitmap = new System.Drawing.Bitmap(sourceBitmap.Width, sourceBitmap.Height);


            BitmapData resultData = resultBitmap.LockBits(new System.Drawing.Rectangle(0, 0,
                                    resultBitmap.Width, resultBitmap.Height),
                                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);


            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);


            return resultBitmap;
        }

        public static byte Calculate(byte color1, byte color2,
                       ColorCalculationType calculationType)
        {
            byte resultValue = 0;
            int intResult = 0;

            if (calculationType == ColorCalculationType.Add)
            {
                intResult = color1 + color2;
            }
            else if (calculationType == ColorCalculationType.Average)
            {
                intResult = (color1 + color2) / 2;
            }
            else if (calculationType == ColorCalculationType.SubtractLeft)
            {
                intResult = color1 - color2;
            }
            else if (calculationType == ColorCalculationType.SubtractRight)
            {
                intResult = color2 - color1;
            }
            else if (calculationType == ColorCalculationType.Difference)
            {
                intResult = Math.Abs(color1 - color2);
            }
            else if (calculationType == ColorCalculationType.Multiply)
            {
                intResult = (int)((color1 / 255.0 * color2 / 255.0) * 255.0);
            }
            else if (calculationType == ColorCalculationType.Min)
            {
                intResult = (color1 < color2 ? color1 : color2);
            }
            else if (calculationType == ColorCalculationType.Max)
            {
                intResult = (color1 > color2 ? color1 : color2);
            }
            else if (calculationType == ColorCalculationType.Amplitude)
            {
                intResult = (int)(Math.Sqrt(color1 * color1 + color2 * color2)
                                                             / Math.Sqrt(2.0));
            }

            if (intResult < 0)
            {
                resultValue = 0;
            }
            else if (intResult > 255)
            {
                resultValue = 255;
            }
            else
            {
                resultValue = (byte)intResult;
            }

            return resultValue;
        }
    }
}
