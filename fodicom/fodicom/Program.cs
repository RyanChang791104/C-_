using Dicom;
using Dicom.Imaging;
using Dicom.Imaging.Render;
using Dicom.IO;
using Dicom.IO.Buffer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Ocr;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Foundation;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;
using System.IO;
namespace fodicom
{
    class Program
    {
        private static Language ocrLanguage = new Language("zh-Hant-TW");
      
        static void Main(string[] args)
        {
            var DCM_Files = Directory.GetFiles(@"C:\Users\ryan.chang\Desktop\","*.dcm");
            Stopwatch st = new Stopwatch();
            List<string> path_list = new List<string>();
            

            Parallel.ForEach(DCM_Files, new ParallelOptions { MaxDegreeOfParallelism = 50 }, (path, loopState) => 
            {
                st.Start();
                DCM(path);
                st.Stop();
                Console.WriteLine("dcm {0} takes {1} ms \n", path, st.ElapsedMilliseconds);
            });


         

            Console.WriteLine("Process Done");
            Console.ReadKey();


            //DicomPixelData dcmPixelData = DicomPixelData.Create(dcmDataSet);
            //byte[] pixels2 = dcmPixelData.GetFrame(0).Data;
            // raw data
            //IPixelData pixelData = Create(dcmPixelData, 0);
            //dicomiage.RenderImage().AsClonedBitmap().Save(@"C:\Users\ryan.chang\Desktop\test.jpg");
            //byte[] pixels3 = GetPixels(temp);

            //applyMasksOnBytes(pixels3, new List<int[]> { new int[] { 5, 6, 39, 21 }, new int[] { 56, 6, 26, 18 }, new int[] { 89, 6, 44, 18 }, new int[] { 5, 930, 96, 17 } },
            // int.Parse(dicomProcess.GetDicomTagValueByDicomDataset(dcmDataSet, DicomTag.Rows)),
            // int.Parse(dicomProcess.GetDicomTagValueByDicomDataset(dcmDataSet, DicomTag.Columns)), pixelData.Components);
           
            //MemoryByteBuffer buffer = new MemoryByteBuffer(pixels3);
            //dcmPixelData = DicomPixelData.Create(dcmDataSet, true);
            //dcmPixelData.AddFrame(buffer);
            //DicomFile dcmFile_ = new DicomFile(dcmDataSet);
            // 由 Bitmap 轉成 byte[] 再轉成 IBuffer
            //dcmFile.Save($@"C:\Users\ryan.chang\Desktop\{DateTime.Now.ToString("HHmmssfff")}.dcm");
            //dcmFile.Save($@"C:\Users\ryan.chang\Desktop\test.dcm");

            //byte[] pixels2 = dcmPixelData.GetFrame(0).Data;
            //var buffer1 = dcmPixelData.GetFrame(0);
            //buffer1 = PixelDataConverter.InterleavedToPlanar24(buffer1);
            //IPixelData pixelData = PixelDataFactory.Create(dcmPixelData, 0);
            //if (pixelData.Components == 2)
            //{
            //    ushort[] pixels = { 0x00 };
            //    pixels = ((GrayscalePixelDataU16)pixelData).Data;
            //    pixels2 = new byte[2 * pixels.Length];
            //    Buffer.BlockCopy(pixels, 0, pixels2, 0, 2 * pixels.Length);
            //}
            //else if (pixelData.Components == 3)
            //{
            //    pixels2 = ((ColorPixelData24)pixelData).Data;
            //}
            //else
            //{
            //    pixels2 = ((GrayscalePixelDataU8)pixelData).Data;
            //}
            //change
            //for (int i = 0; i < pixels.Length / 2; i++)
            //{
            //    pixels[2 * i] = 255;
            //}
            //dcmPixelData = DicomPixelData.Create(dcmDataSet, true);
            //int bytes = int.Parse(dicomProcess.GetDicomTagValueByDicomDataset(dcmDataSet, DicomTag.BitsAllocated)) / 8;
            //if ((int.Parse(dicomProcess.GetDicomTagValueByDicomDataset(dcmDataSet, DicomTag.BitsAllocated)) % 8) > 0)
            //    bytes++;

            //int bytesPerPixel = bytes * int.Parse(dicomProcess.GetDicomTagValueByDicomDataset(dcmDataSet, DicomTag.SamplesPerPixel));
        }
        private static void DCM(string path)
        {
            DicomFile dcmFile = DicomFile.Open(path);
            DicomProcess dicomProcess = new DicomProcess();
            DicomDataset dcmDataSet = dcmFile.Dataset;
            DicomImage dicomiage = new DicomImage(dcmDataSet);
            Bitmap temp = dicomiage.RenderImage().AsClonedBitmap();
            OCR(temp);
        }
        static int i = 0;
        private static async void OCR(Bitmap temp)
        {

            int width = temp.Width;
            int height = temp.Height;
            BitmapData bmpData = temp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, temp.PixelFormat);
            IntPtr ptr = bmpData.Scan0; // Get memory pointer of image
            int numBytes = temp.Width * temp.Height * 4; // Rgba, times 4.
            byte[] stream = new byte[numBytes];
            Marshal.Copy(ptr, stream, 0, numBytes);
            temp.UnlockBits(bmpData);
            IBuffer buf = stream.AsBuffer();
            OcrEngine ocrEngine = OcrEngine.TryCreateFromLanguage(ocrLanguage); //select zh-Hant-TW
            SoftwareBitmap swBitmap = SoftwareBitmap.CreateCopyFromBuffer(buf, BitmapPixelFormat.Bgra8, width, height);//transfer IBuffer to SoftwareBitmap
            var ocrResult = await ocrEngine.RecognizeAsync(swBitmap);//Get the Ocr result
            string result = "";
            foreach (var line in ocrResult.Lines)
            {
                foreach (var word in line.Words)
                {
                    result += word.Text;
                    //Console.WriteLine(word.Text + "\n");
                    //Console.WriteLine("Result " + word.Text);
                    //Console.WriteLine("X: " + word.BoundingRect.X);
                    //Console.WriteLine("Y: " + word.BoundingRect.Y);
                    //Console.WriteLine("Width: " + word.BoundingRect.Width);
                    //Console.WriteLine("Height: " + word.BoundingRect.Height);
                    int X = Convert.ToInt16(word.BoundingRect.X);
                    int Y = Convert.ToInt16(word.BoundingRect.Y);
                    int Width = Convert.ToInt16(word.BoundingRect.Width);
                    int Height = Convert.ToInt16(word.BoundingRect.Height);
                    Draw_Rectangle(temp,X, Y, Width, Height);

                }
            }
            temp.Save(@"C:\Users\ryan.chang\Desktop\" + Guid.NewGuid() + ".jpg");
            i++;
            Console.WriteLine(result +"\n");
            Console.ReadKey();
        }
       
        private static void Draw_Rectangle(Bitmap temp,int X, int Y, int Width, int Height)
        {
            using (Graphics graphics = Graphics.FromImage(temp))
            {
                using (Pen myBrush = new Pen(Color.Red))
                {
                    myBrush.Width = 1;
                    graphics.DrawRectangle(myBrush, new Rectangle(X, Y, Width, Height));
                }
            }
        }

        public static IPixelData Create(DicomPixelData pixelData, int frame)
        {
            PhotometricInterpretation pi = pixelData.PhotometricInterpretation;

            if (pi == null)
            {
                // generally ACR-NEMA
                var samples = pixelData.SamplesPerPixel;
                if (samples == 0 || samples == 1)
                {
                    pi = pixelData.Dataset.Contains(DicomTag.RedPaletteColorLookupTableData)
                        ? PhotometricInterpretation.PaletteColor
                        : PhotometricInterpretation.Monochrome2;
                }
                else
                {
                    // assume, probably incorrectly, that the image is RGB
                    pi = PhotometricInterpretation.Rgb;
                }
            }

            if (pixelData.BitsStored == 1)
            {
                if (pixelData.Dataset.GetSingleValue<DicomUID>(DicomTag.SOPClassUID)
                    == DicomUID.MultiFrameSingleBitSecondaryCaptureImageStorage)
                    // Multi-frame Single Bit Secondary Capture is stored LSB -> MSB
                    return new SingleBitPixelData(
                        pixelData.Width,
                        pixelData.Height,
                        PixelDataConverter.ReverseBits(pixelData.GetFrame(frame)));
                else
                    // Need sample images to verify that this is correct
                    return new SingleBitPixelData(pixelData.Width, pixelData.Height, pixelData.GetFrame(frame));
            }
            else if (pi == PhotometricInterpretation.Monochrome1 || pi == PhotometricInterpretation.Monochrome2
                     || pi == PhotometricInterpretation.PaletteColor)
            {
                if (pixelData.BitsAllocated == 8 && pixelData.HighBit == 7 && pixelData.BitsStored == 8)
                    return new GrayscalePixelDataU8(pixelData.Width, pixelData.Height, pixelData.GetFrame(frame));
                else if (pixelData.BitsAllocated <= 16)
                {
                    if (pixelData.PixelRepresentation == PixelRepresentation.Signed)
                        return new GrayscalePixelDataS16(
                            pixelData.Width,
                            pixelData.Height,
                            pixelData.BitDepth,
                            pixelData.GetFrame(frame));
                    else
                        return new GrayscalePixelDataU16(
                            pixelData.Width,
                            pixelData.Height,
                            pixelData.BitDepth,
                            pixelData.GetFrame(frame));
                }
                else if (pixelData.BitsAllocated <= 32)
                {
                    if (pixelData.PixelRepresentation == PixelRepresentation.Signed)
                        return new GrayscalePixelDataS32(
                            pixelData.Width,
                            pixelData.Height,
                            pixelData.BitDepth,
                            pixelData.GetFrame(frame));
                    else
                        return new GrayscalePixelDataU32(
                            pixelData.Width,
                            pixelData.Height,
                            pixelData.BitDepth,
                            pixelData.GetFrame(frame));
                }
                else
                    throw new DicomImagingException(
                        "Unsupported pixel data value for bits stored: {0}",
                        pixelData.BitsStored);
            }
            else if (pi == PhotometricInterpretation.Rgb || pi == PhotometricInterpretation.YbrFull
                     || pi == PhotometricInterpretation.YbrFull422 || pi == PhotometricInterpretation.YbrPartial422)
            {
                var buffer = pixelData.GetFrame(frame);

                if (pixelData.PlanarConfiguration == PlanarConfiguration.Planar)
                    buffer = PixelDataConverter.PlanarToInterleaved24(buffer);

                if (pi == PhotometricInterpretation.YbrFull) buffer = PixelDataConverter.YbrFullToRgb(buffer);
                else if (pi == PhotometricInterpretation.YbrFull422) buffer = YbrFull422ToRgb(buffer, pixelData.Width);
                else if (pi == PhotometricInterpretation.YbrPartial422) buffer = PixelDataConverter.YbrPartial422ToRgb(buffer, pixelData.Width);

                return new ColorPixelData24(pixelData.Width, pixelData.Height, buffer);
            }
            else if (pi == PhotometricInterpretation.YbrFull422)
            {
                var buffer = pixelData.GetFrame(frame);
                if (pixelData.PlanarConfiguration == PlanarConfiguration.Planar) throw new DicomImagingException("Unsupported planar configuration for YBR_FULL_422");
                return new ColorPixelData24(pixelData.Width, pixelData.Height, buffer);
            }
            else
            {
                throw new DicomImagingException(
                    "Unsupported pixel data photometric interpretation: {0}",
                    pi.Value);
            }
        }

        private static byte ToByte(double x)
        {
            return (byte)(x < 0.0 ? 0.0 : x > 255.0 ? 255.0 : x);
        }

        /// <summary>
        /// Convert YBR_FULL_422 photometric interpretation pixels to RGB.
        /// </summary>
        /// <param name="data">Array of YBR_FULL_422 photometric interpretation pixels.</param>
        /// <param name="width">Image width.</param>
        /// <returns>Array of pixel data in RGB photometric interpretation.</returns>
        public static IByteBuffer YbrFull422ToRgb(IByteBuffer data, int width)
        {
            var oldPixels = data.Data;
            var newPixels = new byte[oldPixels.Length / 4 * 2 * 3];

            unchecked
            {
                for (int n = 0, p = 0, col = 0; n < oldPixels.Length-4;)
                {
                    int y1 = oldPixels[n++];
                    int y2 = oldPixels[n++];
                    int cb = oldPixels[n++];
                    int cr = oldPixels[n++];

                    newPixels[p++] = ToByte(y1 + 1.4020 * (cr - 128) + 0.5);
                    newPixels[p++] = ToByte(y1 - 0.3441 * (cb - 128) - 0.7141 * (cr - 128) + 0.5);
                    newPixels[p++] = ToByte(y1 + 1.7720 * (cb - 128) + 0.5);

                    if (++col == width)
                    {
                        // Issue #471: for uneven width images (i.e. when col equals width after first of two pixels), 
                        // ignore last pixel in each row.
                        col = 0;
                        continue;
                    }

                    newPixels[p++] = ToByte(y2 + 1.4020 * (cr - 128) + 0.5);
                    newPixels[p++] = ToByte(y2 - 0.3441 * (cb - 128) - 0.7141 * (cr - 128) + 0.5);
                    newPixels[p++] = ToByte(y2 + 1.7720 * (cb - 128) + 0.5);

                    if (++col == width) col = 0;
                }
            }

            return new MemoryByteBuffer(newPixels);
        }

        public static byte[] GetPixels(Bitmap bitmap)
        {
            byte[] bytes = new byte[bitmap.Width * bitmap.Height * 3];
            int wide = bitmap.Width;
            int i = 0;
            int height = bitmap.Height;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < wide; x++)
                {
                    var srcColor = bitmap.GetPixel(x, y);//bytes[i] = (byte)(srcColor.R * .299 + srcColor.G * .587 + srcColor.B * .114);
                    bytes[i] = srcColor.R;
                    i++;
                    bytes[i] = srcColor.G;
                    i++;
                    bytes[i] = srcColor.B;
                    i++;
                }
            }
            return bytes;
        }

        public static void ImportImage()
        {
            Bitmap bitmap = new Bitmap(@"C:\Users\ryan.chang\Desktop\output\testabc.bmp");            //bitmap = GetValidImage(bitmap);


            byte[] pixels = GetPixels(bitmap);
            MemoryByteBuffer buffer = new MemoryByteBuffer(pixels);
            DicomDataset dataset = new DicomDataset();            //FillDataset(dataset);
            dataset.Add(DicomTag.PhotometricInterpretation, PhotometricInterpretation.Rgb.Value);
            dataset.Add(DicomTag.Rows, (ushort)bitmap.Height);
            dataset.Add(DicomTag.Columns, (ushort)bitmap.Width);
            dataset.Add(DicomTag.BitsAllocated, (ushort)8);
            dataset.Add(DicomTag.SOPClassUID, "1.2.840.10008.5.1.4.1.1.7");
            dataset.Add(DicomTag.StudyInstanceUID, "1.2.840.10008.5.1.4.1.1.2.20181120090837121311");
            dataset.Add(DicomTag.SeriesInstanceUID, "1.2.840.10008.5.1.4.1.1.2.20181120090837121312");
            dataset.Add(DicomTag.SOPInstanceUID, "1.2.840.10008.5.1.4.1.1.2.20181120090837121314");
            dataset.Add(DicomTag.TransferSyntaxUID, "1.2.840.10008.1.2.4.70");

            //dataset.Add(DicomTag.SpecificCharacterSet, "ISO_IR 192");
            //dataset.Add(new DicomPersonName(DicomTag.PatientName,Encoding.UTF8,"王曉明"));
            DicomPixelData pixelData = DicomPixelData.Create(dataset, true);
            pixelData.BitsStored = 8;            //pixelData.BitsAllocated = 8;
            pixelData.SamplesPerPixel = 3;
            pixelData.HighBit = 7;
            pixelData.PixelRepresentation = 0;
            pixelData.PlanarConfiguration = 0;
            pixelData.AddFrame(buffer);

            DicomFile dicomfile = new DicomFile(dataset);
            dicomfile.Save(@"C:\Users\ryan.chang\Desktop\output\dicomfile.dcm");
        }
        /**
         * 在影像 byte[] 上加上 masks
         */
        private static void applyMasksOnBytes(byte[] bytes, List<int[]> masks, int w, int h, int bytesPerPixel)
        {
            byte zero = (byte)255;
            for (int m = 0; m < masks.Count; m++)
            {
                int x1 = Math.Max(0, masks[m][0]);
                int y1 = Math.Max(0, masks[m][1]);
                int x2 = Math.Min(w - 1, masks[m][2]);
                int y2 = Math.Min(h - 1, masks[m][3]);
                for (int i = y1; i <= y2; i++)
                    for (int j = x1; j <= x2; j++)
                        for (int k = 0; k < bytesPerPixel; k++)
                            bytes[(i * h + j) * bytesPerPixel + k] = zero; 
            }
        }
    }
}

