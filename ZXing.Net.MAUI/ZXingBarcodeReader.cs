namespace ZXing.Net.Maui.Readers
{
	public class ZXingBarcodeReader : Readers.IBarcodeReader
	{
		private BarcodeReaderGeneric _zxingReader;
		private BarcodeReaderOptions _options;

		public ZXingBarcodeReader()
		{
			_zxingReader = new BarcodeReaderGeneric();
		}

		public BarcodeReaderOptions Options
		{

			get => _options ??= new BarcodeReaderOptions();
			set
			{
				_options = value ?? new BarcodeReaderOptions();
				_zxingReader.Options.PossibleFormats = _options.Formats.ToZXingList();
				_zxingReader.Options.CharacterSet = _options.EncodingName;
				_zxingReader.AutoRotate = _options.AutoRotate;
				_zxingReader.Options.TryHarder = _options.TryHarder;
				_zxingReader.Options.TryInverted = _options.TryInverted;
				_zxingReader.Options.UseCode39ExtendedMode = _options.UseCode39ExtendedMode;
			}
		}

		public BarcodeResult[] Decode(PixelBufferHolder image)
		{
			var w = (int)image.Size.Width;
			var h = (int)image.Size.Height;

			LuminanceSource ls = default;

#if ANDROID
			ls = new ByteBufferYUVLuminanceSource(image.Data, w, h, 0, 0, w, h);
#elif MACCATALYST || IOS
			ls = new CVPixelBufferBGRA32LuminanceSource(image.Data, w, h);
#elif WINDOWS
			ls = new SoftwareBitmapLuminanceSource(image.Data);
#endif

			if (Options.Multiple)
				return _zxingReader.DecodeMultiple(ls)?.ToBarcodeResults();

			var result = _zxingReader.Decode(ls)?.ToBarcodeResult();

			if (result is not null)
				return [result];

			return null;
		}
	}
}
