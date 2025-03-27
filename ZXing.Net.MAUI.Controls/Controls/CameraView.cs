using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

using System;

namespace ZXing.Net.Maui.Controls
{
	public partial class CameraView : View, ICameraView
	{
		public event EventHandler<CameraFrameBufferEventArgs> FrameReady;

		public static readonly BindableProperty IsTorchOnProperty =
			BindableProperty.Create(nameof(IsTorchOn), typeof(bool), typeof(CameraView), defaultValue: true);

		public static readonly BindableProperty CameraLocationProperty =
			BindableProperty.Create(nameof(CameraLocation), typeof(CameraLocation), typeof(CameraView), defaultValue: CameraLocation.Rear);

		public bool IsTorchOn
		{
			get => (bool)GetValue(IsTorchOnProperty);
			set => SetValue(IsTorchOnProperty, value);
		}

		public CameraLocation CameraLocation
		{
			get => (CameraLocation)GetValue(CameraLocationProperty);
			set => SetValue(CameraLocationProperty, value);
		}

		public CameraViewHandler StrongHandler
			=> Handler as CameraViewHandler;

		public CameraView()
		{
			Unloaded += (s, e) => Cleanup();
		}

		void ICameraFrameAnalyzer.FrameReady(CameraFrameBufferEventArgs e)
			=> FrameReady?.Invoke(this, e);

		public void AutoFocus()
			=> StrongHandler?.Invoke(nameof(AutoFocus), null);

		public void Focus(Point point)
			=> StrongHandler?.Invoke(nameof(Focus), point);

		private void Cleanup()
			=> Handler?.DisconnectHandler();
	}
}
