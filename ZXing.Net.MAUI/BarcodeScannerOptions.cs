namespace ZXing.Net.Maui
{
	public record BarcodeReaderOptions
	{
		public BarcodeFormat Formats { get; init; } = BarcodeFormats.All;

		public string EncodingName { get; init; } = "UTF8";

		public bool AutoRotate { get; init; }

		public bool TryHarder { get; init; }

		public bool TryInverted { get; init; }

		public bool Multiple { get; init; }

		public bool UseCode39ExtendedMode { get; init; }
	}
}
