using System;

using ZXing.Net.Maui.Readers;

namespace ZXing.Net.Maui
{
	public class CameraFrameBufferEventArgs : EventArgs
	{
		public readonly PixelBufferHolder Data;

		public CameraFrameBufferEventArgs(PixelBufferHolder pixelBufferHolder) : base()
			=> Data = pixelBufferHolder;
	}
}
