using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace NTW.Panels {
    /// <summary>
    /// Designers collection (is freezable Collection)
    /// </summary>
    public class DesignersCollection: FreezableCollection<CustomDesigner>, IDrawingPresenter {

        public DesignersCollection() : base(new List<CustomDesigner>()) {
            transformGroup = new TransformGroup();

            ((INotifyCollectionChanged)this).CollectionChanged += MouseHandlerCollection_CollectionChanged;
        }

        private void MouseHandlerCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    foreach (var designer in e.NewItems?.Cast<CustomDesigner>()) {
                        if (designer is ITransformDesigner transformDesigner)
                            AddWithSorting(transformDesigner);

                        if (designer is IDrawingPresenter presenter)
                            AddDrawing(presenter);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var designer in e.OldItems?.Cast<CustomDesigner>()) {
                        if (designer is ITransformDesigner transformDesigner)
                            transformGroup.Children.Remove(transformDesigner.GetTransform());

                        if (designer is IDrawingPresenter presenter) {
                            if (presenter.BackDrawing != null)
                                backDrawing.Children.Remove(presenter.BackDrawing);

                            if (presenter.FrontDrawing != null)
                                frontDrawing.Children.Remove(presenter.FrontDrawing);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    transformGroup.Children.Clear();
                    backDrawing.Children.Clear();
                    frontDrawing.Children.Clear();
                    break;
            }
        }

        private TransformGroup transformGroup;
        public Transform Transformation => transformGroup;

        #region IDrawingPresenter
        private DrawingGroup backDrawing = new DrawingGroup();
        public Drawing BackDrawing => backDrawing;

        private DrawingGroup frontDrawing = new DrawingGroup();
        public Drawing FrontDrawing => frontDrawing; 
        #endregion


        private void AddWithSorting(ITransformDesigner transform) {

            if (transform.GetTransform() is TranslateTransform translate)
                transformGroup.Children.Add(translate);
            else if (transform.GetTransform() is ScaleTransform scale)
                transformGroup.Children.Insert(0, scale);
            else if (transform.GetTransform() is TransformGroup group)
                transformGroup.Children.Insert(0, group);
        }

        private void AddDrawing(IDrawingPresenter presenter) {
            if (presenter.BackDrawing != null)
                backDrawing.Children.Add(presenter.BackDrawing);

            if (presenter.FrontDrawing != null)
                frontDrawing.Children.Add(presenter.FrontDrawing);
        }
    }
}
