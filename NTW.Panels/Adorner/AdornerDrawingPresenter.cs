using System.Linq;
using System.Windows.Documents;
using System.Windows.Media;

namespace NTW.Panels {
    /// <summary>
    /// Simple front drawing presenter
    /// </summary>
    internal class AdornerDrawingPresenter: Adorner {

        public AdornerDrawingPresenter(CustomPanel adornedElement)
            : base(adornedElement) {

            this.IsHitTestVisible = false;
        }

        private CustomPanel Owner => this.AdornedElement as CustomPanel;

        protected override void OnRender(DrawingContext dc) {
            if (this.Owner.ItemsLocator is IDrawingPresenter dp && dp.FrontDrawing != null)
                dc.DrawDrawing(dp.FrontDrawing);

            foreach (IDrawingPresenter drawingPresenter in this.Owner.Handlers.Where(x => x.IsActive && x is IDrawingPresenter idp && idp.FrontDrawing != null))
                dc.DrawDrawing(drawingPresenter.FrontDrawing);
        }

        /// <summary>
        /// Begin to refresh front drawing layer
        /// </summary>
        public void Refresh() {
            this.InvalidateVisual();
        }
    }
}
