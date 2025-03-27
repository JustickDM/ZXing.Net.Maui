using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;

using System;
using System.Linq;

namespace ZXing.Net.Maui
{
	public partial class CameraBarcodeReaderViewHandler : ViewHandler<ICameraBarcodeReaderView, NativePlatformCameraPreviewView>
	{
		private CameraManager _cameraManager;

		private Readers.IBarcodeReader _barcodeReader;

		public static PropertyMapper<ICameraBarcodeReaderView, CameraBarcodeReaderViewHandler> CameraBarcodeReaderViewMapper = new()
		{
			[nameof(ICameraBarcodeReaderView.Options)] = MapOptions,
			[nameof(ICameraBarcodeReaderView.IsDetecting)] = MapIsDetecting,
			[nameof(ICameraBarcodeReaderView.IsTorchOn)] = (handler, virtualView) => handler._cameraManager.UpdateTorch(virtualView.IsTorchOn),
			[nameof(ICameraBarcodeReaderView.CameraLocation)] = (handler, virtualView) => handler._cameraManager.UpdateCameraLocation(virtualView.CameraLocation)
		};

		public static CommandMapper<ICameraBarcodeReaderView, CameraBarcodeReaderViewHandler> CameraBarcodeReaderCommandMapper = new()
		{
			[nameof(ICameraBarcodeReaderView.Focus)] = MapFocus,
			[nameof(ICameraBarcodeReaderView.AutoFocus)] = MapAutoFocus,
		};

		public CameraBarcodeReaderViewHandler() : base(CameraBarcodeReaderViewMapper, CameraBarcodeReaderCommandMapper)
		{

		}

		public CameraBarcodeReaderViewHandler(PropertyMapper propertyMapper = null, CommandMapper commandMapper = null)
			: base(propertyMapper ?? CameraBarcodeReaderViewMapper, commandMapper ?? CameraBarcodeReaderCommandMapper)
		{

		}

		protected Readers.IBarcodeReader BarcodeReader
			=> _barcodeReader ??= Services.GetService<Readers.IBarcodeReader>();

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
		{
			VirtualView?.FrameReady(e);

			if (VirtualView?.IsDetecting ?? false)
			{
				var barcodes = BarcodeReader.Decode(e.Data);

				if (barcodes?.Any() ?? false)
					VirtualView?.BarcodesDetected(new BarcodeDetectionEventArgs(barcodes));
			}
		}

		public void Focus(Point point)
			=> _cameraManager?.Focus(point);

		public void AutoFocus()
			=> _cameraManager?.AutoFocus();

		public static void MapOptions(CameraBarcodeReaderViewHandler handler, ICameraBarcodeReaderView cameraBarcodeReaderView)
			=> handler.BarcodeReader.Options = cameraBarcodeReaderView.Options;

		public static void MapIsDetecting(CameraBarcodeReaderViewHandler handler, ICameraBarcodeReaderView cameraBarcodeReaderView)
		{

		}

		public static void MapFocus(CameraBarcodeReaderViewHandler handler, ICameraBarcodeReaderView cameraBarcodeReaderView, object parameter)
		{
			if (parameter is not Point point)
				throw new ArgumentException("Invalid parameter", "point");

			handler.Focus(point);
		}

		public static void MapAutoFocus(CameraBarcodeReaderViewHandler handler, ICameraBarcodeReaderView cameraBarcodeReaderView, object parameters)
			=> handler.AutoFocus();
	}
}
