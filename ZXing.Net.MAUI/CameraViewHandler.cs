using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;

using System;

namespace ZXing.Net.Maui
{
	public partial class CameraViewHandler : ViewHandler<ICameraView, NativePlatformCameraPreviewView>
	{
		private CameraManager _cameraManager;

		public static PropertyMapper<ICameraView, CameraViewHandler> CameraViewMapper = new()
		{
			[nameof(ICameraView.IsTorchOn)] = (handler, virtualView) => handler._cameraManager.UpdateTorch(virtualView.IsTorchOn),
			[nameof(ICameraView.CameraLocation)] = (handler, virtualView) => handler._cameraManager.UpdateCameraLocation(virtualView.CameraLocation)
		};

		public static CommandMapper<ICameraView, CameraViewHandler> CameraCommandMapper = new()
		{
			[nameof(ICameraView.Focus)] = MapFocus,
			[nameof(ICameraView.AutoFocus)] = MapAutoFocus,
		};

		public CameraViewHandler() : base(CameraViewMapper)
		{

		}

		public CameraViewHandler(PropertyMapper mapper = null) : base(mapper ?? CameraViewMapper)
		{

		}

		public void Dispose()
			=> _cameraManager?.Dispose();

		protected override NativePlatformCameraPreviewView CreatePlatformView()
		{
			_cameraManager ??= new(MauiContext, VirtualView?.CameraLocation ?? CameraLocation.Rear);

			var result = _cameraManager.CreateNativeView();

			return result;
		}

		protected override async void ConnectHandler(NativePlatformCameraPreviewView nativeView)
		{
			base.ConnectHandler(nativeView);

			if (_cameraManager is not null)
			{
				if (await _cameraManager.CheckPermissions())
					_cameraManager.Connect();

				_cameraManager.FrameReady += CameraManager_FrameReady;
			}
		}

		protected override void DisconnectHandler(NativePlatformCameraPreviewView nativeView)
		{
			if (_cameraManager is not null)
			{
				_cameraManager.FrameReady -= CameraManager_FrameReady;

				_cameraManager.Disconnect();
				_cameraManager.Dispose();
			}

			base.DisconnectHandler(nativeView);
		}

		private void CameraManager_FrameReady(object sender, CameraFrameBufferEventArgs e)
			=> VirtualView?.FrameReady(e);

		public void Focus(Point point)
			=> _cameraManager?.Focus(point);

		public void AutoFocus()
			=> _cameraManager?.AutoFocus();

		public static void MapFocus(CameraViewHandler handler, ICameraView cameraBarcodeReaderView, object parameter)
		{
			if (parameter is not Point point)
				throw new ArgumentException("Invalid parameter", "point");

			handler.Focus(point);
		}

		public static void MapAutoFocus(CameraViewHandler handler, ICameraView cameraBarcodeReaderView, object parameters)
			=> handler.AutoFocus();
	}
}
