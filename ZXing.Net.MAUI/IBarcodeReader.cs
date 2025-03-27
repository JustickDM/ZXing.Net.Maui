namespace ZXing.Net.Maui.Readers
{
	public interface IBarcodeReader
	{
		BarcodeReaderOptions Options { get; set; }

		BarcodeResult[] Decode(PixelBufferHolder image);
	}
}
