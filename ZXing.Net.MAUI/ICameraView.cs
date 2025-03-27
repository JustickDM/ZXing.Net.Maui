using Microsoft.Maui;
using Microsoft.Maui.Graphics;

namespace ZXing.Net.Maui
{
	public interface ICameraFrameAnalyzer
	{
		void FrameReady(CameraFrameBufferEventArgs args);
	}

	public interface ICameraView : IView, ICameraFrameAnalyzer
	{
		CameraLocation CameraLocation { get; set; }

		//CameraMode Mode { get; set; }

		bool IsTorchOn { get; set; }

		void AutoFocus();

		void Focus(Point point);
	}
}
