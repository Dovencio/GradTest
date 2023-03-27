using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vortice.Direct2D1;
using Vortice.DirectWrite;
using Vortice.Mathematics;

namespace GradTest
{
    public partial class UserControl1 : PictureBox // using picturebox to double buffer
    {
        private IDWriteTextFormat textFormat;
        private ID2D1HwndRenderTarget hwndRenderTarget;
        private IDWriteFactory writeFactory;
        private ID2D1Factory direct2DFactory;

        public UserControl1()
        {
            InitializeComponent();
            RegenerateDx();
        }

        private void RegenerateDx()
        {
            direct2DFactory?.Dispose();
            writeFactory?.Dispose();
            textFormat?.Dispose();
            hwndRenderTarget?.Dispose();
            writeFactory = DWrite.DWriteCreateFactory<IDWriteFactory>(Vortice.DirectWrite.FactoryType.Isolated);
            direct2DFactory = D2D1.D2D1CreateFactory<ID2D1Factory>(Vortice.Direct2D1.FactoryType.MultiThreaded);
            textFormat = writeFactory.CreateTextFormat(Font.Name, Font.Size);
            textFormat.TextAlignment = TextAlignment.Leading;
            textFormat.ParagraphAlignment = ParagraphAlignment.Near;
            textFormat.SetLineSpacing(LineSpacingMethod.Default, 0, 0);
            RenderTargetProperties renderTargetProperties = new RenderTargetProperties()
            {
                DpiX = DeviceDpi,
                DpiY = DeviceDpi,
                Type = RenderTargetType.Hardware,
                Usage = RenderTargetUsage.None,
            };
            HwndRenderTargetProperties hwndRenderTargetProperties = new()
            {
                Hwnd = Handle,
                PixelSize = new Size(Right - Left, Bottom - Top),
                PresentOptions = PresentOptions.None
            };
            hwndRenderTarget = direct2DFactory.CreateHwndRenderTarget(renderTargetProperties, hwndRenderTargetProperties);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RegenerateDx(); // Needs to resize if size changes
        }

        public void Render()
        {
            OnPaint(new(CreateGraphics(), new(0, 0, Size.Width, Size.Height)));
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            GradientStop[] stops = new GradientStop[]
            {
                new(0, new(100, 100, 100)),
                new(1f, new(255, 255, 255))
            };
            ID2D1GradientStopCollection stopCollection = hwndRenderTarget.CreateGradientStopCollection(stops, ExtendMode.Wrap);
            ID2D1LinearGradientBrush brush = hwndRenderTarget.CreateLinearGradientBrush(new(new(0, 0), new(100, 100)), stopCollection);
            hwndRenderTarget.BeginDraw();
            hwndRenderTarget.Clear(new(BackColor.R, BackColor.G, BackColor.B, BackColor.A));
            hwndRenderTarget.FillRectangle(new(pe.ClipRectangle.X, pe.ClipRectangle.Y, pe.ClipRectangle.Width, pe.ClipRectangle.Height), brush);
            hwndRenderTarget.EndDraw();
            stopCollection.Dispose();
            brush.Dispose();
        }
    }
}
