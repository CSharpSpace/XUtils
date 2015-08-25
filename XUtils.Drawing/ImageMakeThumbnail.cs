using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
namespace XUtils.Drawing
{
	public static class ImageMakeThumbnail
	{
		public static void Generate(string originalImagePath, string thumbnailPath, int width, int height, ImageMakeThumbnailMode mode, IsDeleteOriginalImage isDelete)
		{
			Image image = Image.FromFile(originalImagePath);
			int num = width;
			int num2 = height;
			int x = 0;
			int y = 0;
			int num3 = image.Width;
			int num4 = image.Height;
			switch (mode)
			{
			case ImageMakeThumbnailMode.W:
				num2 = image.Height * width / image.Width;
				break;
			case ImageMakeThumbnailMode.H:
				num = image.Width * height / image.Height;
				break;
			case ImageMakeThumbnailMode.CUT:
				if ((double)image.Width / (double)image.Height > (double)num / (double)num2)
				{
					num4 = image.Height;
					num3 = image.Height * num / num2;
					y = 0;
					x = (image.Width - num3) / 2;
				}
				else
				{
					num3 = image.Width;
					num4 = image.Width * height / num;
					x = 0;
					y = (image.Height - num4) / 2;
				}
				break;
			}
			Image image2 = new Bitmap(num, num2);
			Graphics graphics = Graphics.FromImage(image2);
			graphics.InterpolationMode = InterpolationMode.High;
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.Clear(Color.Transparent);
			graphics.DrawImage(image, new Rectangle(0, 0, num, num2), new Rectangle(x, y, num3, num4), GraphicsUnit.Pixel);
			try
			{
				image2.Save(thumbnailPath, ImageFormat.Jpeg);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				image.Dispose();
				image2.Dispose();
				graphics.Dispose();
				if (isDelete == IsDeleteOriginalImage.YES)
				{
					FileInfo fileInfo = new FileInfo(originalImagePath);
					if (fileInfo.Exists)
					{
						fileInfo.Delete();
					}
				}
			}
		}
	}
}
