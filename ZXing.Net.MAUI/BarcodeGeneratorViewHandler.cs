using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;

using System.Linq;

namespace ZXing.Net.Maui
{
	public partial class BarcodeGeneratorViewHandler : ViewHandler<IBarcodeGeneratorView, NativePlatformImageView>
	{
		private Size _desiredSize;
		private BarcodeWriter _writer;
		private NativePlatformImageView _imageView;

		public static PropertyMapper<IBarcodeGeneratorView, BarcodeGeneratorViewHandler> BarcodeGeneratorViewMapper = new()
		{
			[nameof(IBarcodeGeneratorView.Format)] = MapUpdateBarcode,
			[nameof(IBarcodeGeneratorView.Value)] = MapUpdateBarcode,
			[nameof(IBarcodeGeneratorView.ForegroundColor)] = MapUpdateBarcode,
			[nameof(IBarcodeGeneratorView.BackgroundColor)] = MapUpdateBarcode,
			[nameof(IBarcodeGeneratorView.Margin)] = MapUpdateBarcode,
		};

		public BarcodeGeneratorViewHandler() : base(BarcodeGeneratorViewMapper)
		{
			_writer = new BarcodeWriter();
		}

		public BarcodeGeneratorViewHandler(PropertyMapper mapper = null) : base(mapper ?? BarcodeGeneratorViewMapper)
		{
			_writer = new BarcodeWriter();
		}

		public override void PlatformArrange(Rect rect)
		{
			base.PlatformArrange(rect);

			// Don't update if it's the same size, otherwise we could infinite loop
			if (_desiredSize.Width == rect.Width && _desiredSize.Height == rect.Height)
				return;

			_desiredSize = rect.Size;

			UpdateBarcode();
		}

		protected override NativePlatformImageView CreatePlatformView()
		{
#if IOS || MACCATALYST
			_imageView ??= new UIKit.UIImageView { BackgroundColor = UIKit.UIColor.Clear };
#elif ANDROID
			_imageView = new NativePlatformImageView(Context);
			_imageView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif WINDOWS
			_imageView = new NativePlatformImageView();
#endif
			return _imageView;
		}

		protected override void ConnectHandler(NativePlatformImageView nativeView)
		{
			base.ConnectHandler(nativeView);

			UpdateBarcode();
		}

		void UpdateBarcode()
		{
			_writer.Format = VirtualView.Format.ToZXingList().FirstOrDefault();
			_writer.Options.Width = (int)_desiredSize.Width;
			_writer.Options.Height = (int)_desiredSize.Height;
			_writer.Options.Margin = VirtualView.BarcodeMargin;
			_writer.ForegroundColor = VirtualView.ForegroundColor;
			_writer.BackgroundColor = VirtualView.BackgroundColor;
			_writer.Options.Hints[EncodeHintType.CHARACTER_SET] = VirtualView.EncodingName;

			NativePlatformImage image = null;

			if (!string.IsNullOrEmpty(VirtualView.Value))
				image = _writer?.Write(VirtualView.Value);

#if IOS || MACCATALYST
			_imageView.Image = image;
#elif ANDROID
			_imageView?.SetImageBitmap(image);
#elif WINDOWS
			_imageView.Source = image;
#endif
		}

		public static void MapUpdateBarcode(BarcodeGeneratorViewHandler handler, IBarcodeGeneratorView barcodeGeneratorView)
			=> handler.UpdateBarcode();
	}
}
