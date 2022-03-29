using NTW.Panels;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Examples.Data.Collection;
using System.ComponentModel;
using System.Windows.Markup;

namespace Examples.Designers {
    [ContentProperty("LineGroups")]
    [DefaultProperty("LineGroups")]
    public class ElementsPathDesigner : CustomDesigner, IElementArrangeDesigner, IArrangeDesigner, IDrawingPresenter {

        #region Members
        Dictionary<int, string[]> groupsByIndex;
        Dictionary<int, Point> pointsByIndex;
        #endregion

        public ElementsPathDesigner() {
            groupsByIndex = new Dictionary<int, string[]>();
            pointsByIndex = new Dictionary<int, Point>();

            LineGroups = new LineGroupsCollection();
        }

        #region Dependency properties
        public LineGroupsCollection LineGroups {
            get { return (LineGroupsCollection)GetValue(LineGroupsProperty); }
            set { SetValue(LineGroupsProperty, value); }
        }

        public static readonly DependencyProperty LineGroupsProperty =
            DependencyProperty.Register("LineGroups", typeof(LineGroupsCollection), typeof(ElementsPathDesigner), new OptionPropertyMetadata(null, UpdateOptions.Arrange, LineGroupsChanged));

        private static void LineGroupsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ElementsPathDesigner designer) {
                if (e.OldValue is LineGroupsCollection oldC)
                    oldC.OptionCalling -= designer.SetUpdateOption;

                if (e.NewValue is LineGroupsCollection newC)
                    newC.OptionCalling += designer.SetUpdateOption;
                else
                    designer.LineGroups = new LineGroupsCollection();
            }
        }
        #endregion

        #region Attached properties
        public static bool GetNotOnLine(DependencyObject obj) {
            return (bool)obj.GetValue(NotOnLineProperty);
        }

        public static void SetNotOnLine(DependencyObject obj, bool value) {
            obj.SetValue(NotOnLineProperty, value);
        }

        public static readonly DependencyProperty NotOnLineProperty =
            DependencyProperty.RegisterAttached("NotOnLine", typeof(bool), typeof(ElementsPathDesigner), new PropertyMetadata(false));


        public static string GetLineGroupNames(DependencyObject obj) {
            return (string)obj.GetValue(LineGroupNamesProperty);
        }

        public static void SetLineGroupNames(DependencyObject obj, string value) {
            obj.SetValue(LineGroupNamesProperty, value);
        }

        public static readonly DependencyProperty LineGroupNamesProperty =
            DependencyProperty.RegisterAttached("LineGroupNames", typeof(string), typeof(ElementsPathDesigner), new OptionPropertyMetadata("."));
        #endregion

        #region IElementArrangeDesigner
        public void AfterElementArrange(Rect elementRect, Size containerSize, int index, UIElement element, Transform global = null) {

            if (GetNotOnLine(element)) return;

            // calculate the points
            var x = elementRect.X + elementRect.Width / 2;
            var y = elementRect.Y + elementRect.Height / 2;

            groupsByIndex[index] = GetGroupNames(element);
            pointsByIndex[index] = new Point(x, y);
        }

        public void UpdateElementArrage(Rect elementRect, Size containerSize, int index, UIElement element, Transform global = null) {

            if (GetNotOnLine(element)) return;

            var x = elementRect.X + elementRect.Width / 2;
            var y = elementRect.Y + elementRect.Height / 2;

            pointsByIndex[index] = new Point(x, y);

            // calling upate drawing
            backDrawing.Children.Clear();
            backDrawing.Children.Add(LineGroups.GetDrawing(groupsByIndex, pointsByIndex));
        }
        #endregion

        #region IElementArrangeDesigner
        public void BeginElementArrange(Size containerSize, Transform global = null) {
            // clear the path
            groupsByIndex.Clear();
            pointsByIndex.Clear();

            backDrawing.Children.Clear();
        }

        public void EndElementArrange(Size containerSize, Transform global = null) {
            // build the line
            backDrawing.Children.Clear();
            backDrawing.Children.Add(LineGroups.GetDrawing(groupsByIndex, pointsByIndex));
        }
        #endregion

        #region IDrawingPresenter
        private DrawingGroup backDrawing = new DrawingGroup();
        public Drawing BackDrawing => backDrawing;

        public Drawing FrontDrawing { get; }
        #endregion

        protected override void SetUpdateOption(CustomObject sender, UpdateOptions option) {

            if (option == UpdateOptions.ParentUpdate) {
                backDrawing.Children.Clear();
                backDrawing.Children.Add(LineGroups.GetDrawing(groupsByIndex, pointsByIndex));
            } else
                base.SetUpdateOption(sender, option);
        }

        private string[] GetGroupNames(UIElement element) {
            List<string> result = new List<string>();

            string names = ElementsPathDesigner.GetLineGroupNames(element);

            if (names == null)
                result.Add(".");
            else
                result.AddRange(names.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries));

            return result.ToArray();
        }

        protected override Freezable CreateInstanceCore() {
            return new ElementsPathDesigner();
        }
    }
}
