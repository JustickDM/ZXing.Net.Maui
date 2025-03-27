using System;

namespace ZXing.Net.Maui
{
	public class BarcodeDetectionEventArgs : EventArgs
	{
		public BarcodeResult[] Results { get; private set; }

		public BarcodeDetectionEventArgs(BarcodeResult[] results) : base()
		{
			Results = results;
		}
	}
}
