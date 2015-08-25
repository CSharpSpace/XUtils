using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
namespace XUtils.Drawing
{
	public static class ImageWater
	{
		public static void AddFont(string Path, string Path_sy, string fontText, string fontFamily, int x, int y)
		{
			Image image = Image.FromFile(Path);
			Graphics graphics = Graphics.FromImage(image);
			graphics.DrawImage(image, 0, 0, image.Width, image.Height);
			Font font = new Font(string.IsNullOrEmpty(fontFamily) ? "Verdana" : fontFamily, 60f);
			Brush brush = new SolidBrush(Color.Green);
			graphics.DrawString(fontText, font, brush, (float)((x < 1) ? x : 35), (float)((y < 1) ? y : 35));
			graphics.Dispose();
			image.Save(Path_sy);
			image.Dispose();
		}
		public static void AddPic(string Path, string Path_syp, string Path_sypf, WaterPosition waterPosition)
		{
			Image image = Image.FromFile(Path);
			Image image2 = Image.FromFile(Path_sypf);
			Graphics graphics = Graphics.FromImage(image);
			int x = 0;
			int y = 0;
			ImageWater.GetPosition(image, image2, out x, out y, waterPosition);
			graphics.DrawImage(image2, new Rectangle(x, y, image2.Width, image2.Height), 0, 0, image2.Width, image2.Height, GraphicsUnit.Pixel);
			graphics.Dispose();
			image.Save(Path_syp);
			image.Dispose();
		}
		public static void AddPic(string filePaths, string waterFile, WaterPosition waterPosition)
		{
			int num = filePaths.LastIndexOf(".");
			string strA = filePaths.Substring(num, filePaths.Length - num);
			if (string.Compare(strA, ".gif", true) == 0)
			{
				return;
			}
			int num2 = 25;
			Image image = null;
			Image image2 = null;
			Graphics graphics = null;
			try
			{
				image = Image.FromFile(filePaths, true);
				image2 = Image.FromFile(waterFile, true);
				graphics = Graphics.FromImage(image);
				int x = 0;
				int y = 0;
				ImageWater.GetPosition(image, image2, out x, out y, waterPosition);
				float[][] array = new float[5][];
				float[][] arg_86_0 = array;
				int arg_86_1 = 0;
				float[] array2 = new float[5];
				array2[0] = 1f;
				arg_86_0[arg_86_1] = array2;
				float[][] arg_9D_0 = array;
				int arg_9D_1 = 1;
				float[] array3 = new float[5];
				array3[1] = 1f;
				arg_9D_0[arg_9D_1] = array3;
				float[][] arg_B4_0 = array;
				int arg_B4_1 = 2;
				float[] array4 = new float[5];
				array4[2] = 1f;
				arg_B4_0[arg_B4_1] = array4;
				float[][] arg_C8_0 = array;
				int arg_C8_1 = 3;
				float[] array5 = new float[5];
				array5[3] = (float)num2;
				arg_C8_0[arg_C8_1] = array5;
				array[4] = new float[]
				{
					0f,
					0f,
					0f,
					0f,
					1f
				};
				float[][] newColorMatrix = array;
				ColorMatrix newColorMatrix2 = new ColorMatrix(newColorMatrix);
				ImageAttributes imageAttributes = new ImageAttributes();
				imageAttributes.SetColorMatrix(newColorMatrix2, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
				graphics.DrawImage(image2, new Rectangle(x, y, image2.Width, image2.Height), 0, 0, image2.Width, image2.Height, GraphicsUnit.Pixel, imageAttributes);
				FileInfo fileInfo = new FileInfo(filePaths);
				ImageFormat format = ImageFormat.Gif;
				string key;
				switch (key = fileInfo.Extension.ToLower())
				{
				case ".jpg":
					format = ImageFormat.Jpeg;
					break;
				case ".gif":
					format = ImageFormat.Gif;
					break;
				case ".png":
					format = ImageFormat.Png;
					break;
				case ".bmp":
					format = ImageFormat.Bmp;
					break;
				case ".tif":
					format = ImageFormat.Tiff;
					break;
				case ".wmf":
					format = ImageFormat.Wmf;
					break;
				case ".ico":
					format = ImageFormat.Icon;
					break;
				}
				MemoryStream memoryStream = new MemoryStream();
				image.Save(memoryStream, format);
				byte[] array6 = memoryStream.ToArray();
				image.Dispose();
				image2.Dispose();
				graphics.Dispose();
				File.Delete(filePaths);
				FileStream fileStream = new FileStream(filePaths, FileMode.Create, FileAccess.Write);
				if (fileStream != null)
				{
					fileStream.Write(array6, 0, array6.Length);
					fileStream.Close();
				}
			}
			finally
			{
				try
				{
					image2.Dispose();
					image.Dispose();
					graphics.Dispose();
				}
				catch
				{
				}
			}
		}
		private static void GetPosition(Image image, Image copyImage, out int x, out int y, WaterPosition waterPosition)
		{
			switch (waterPosition)
			{
			case WaterPosition.TOP:
				x = (image.Width - copyImage.Width) / 2;
				y = 0;
				return;
			case WaterPosition.CENTER:
				x = (image.Width - copyImage.Width) / 2;
				y = (image.Height - copyImage.Height) / 2;
				return;
			case WaterPosition.LEFT:
				x = 0;
				y = (image.Height - copyImage.Height) / 2;
				return;
			case WaterPosition.LEFTUPCORNER:
				x = 0;
				y = 0;
				return;
			case WaterPosition.LEFTBOMCORNER:
				x = 0;
				y = image.Height - copyImage.Height;
				return;
			case WaterPosition.RIGHT:
				x = image.Width - copyImage.Width;
				y = (image.Height - copyImage.Height) / 2;
				return;
			case WaterPosition.RIGHTUPCORNER:
				x = image.Width - copyImage.Width;
				y = 0;
				return;
			case WaterPosition.RIGHTBOMCORNER:
				x = image.Width - copyImage.Width;
				y = image.Height - copyImage.Height;
				return;
			case WaterPosition.BOTTOM:
				x = (image.Width - copyImage.Width) / 2;
				y = image.Height - copyImage.Height;
				return;
			default:
				x = image.Width - copyImage.Width;
				y = image.Height - copyImage.Height;
				return;
			}
		}
	}
}
