using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TrueTypeFont
{
    public class CustomPanel : VirtualizingPanel
    {
        public CustomPanel() : base() { }
        private List<Visual> visuals = new List<Visual>();
        private DrawingVisual drawingVisual = null;
        protected override int VisualChildrenCount => visuals.Count;
        public int VisualsCount => VisualChildrenCount;
        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }
        public void AddVisual(Visual visual)
        {
            visuals.Add(visual);
            base.AddVisualChild(visual);
            base.AddLogicalChild(visual);
        }
        public void DeleteVisual(Visual visual)
        {
            this.visuals.Remove(visual);
            base.RemoveLogicalChild(visual);
            base.RemoveVisualChild(visual);
        }
        public void ClearVisuals()
        {
            foreach (Visual visual in visuals)
            {
                base.RemoveLogicalChild(visual);
                base.RemoveVisualChild(visual);
            }
            visuals.Clear();
        }
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            this.Background = Brushes.White;
        }
        public DrawingContext RenderOpen()
        {
            drawingVisual = new DrawingVisual();
            this.AddVisual(drawingVisual);
            return drawingVisual.RenderOpen();
        }
        public double PixelsPerDip => VisualTreeHelper.GetDpi(drawingVisual).PixelsPerDip;
        public double PixelsPerInchX => VisualTreeHelper.GetDpi(drawingVisual).PixelsPerInchX;
        public double PixelsPerInchY => VisualTreeHelper.GetDpi(drawingVisual).PixelsPerInchY;
        public DrawingVisual GetVisual(Point point)
        {
            return VisualTreeHelper.HitTest(this, point)?.VisualHit as DrawingVisual;
        }
    }
}
