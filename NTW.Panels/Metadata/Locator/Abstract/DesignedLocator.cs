using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace NTW.Panels {
    /// <summary>
    /// Simple Design locator
    /// </summary>
    public abstract class DesignedLocator : CustomLocator, IDesign {

        public DesignedLocator() {
            this.Designers = new DesignersCollection();
        }

        #region IDesign
        /// <summary>
        /// Special extension collection. Locator uses this collection as attached properties
        /// </summary>
        public DesignersCollection Designers {
            get { return (DesignersCollection)GetValue(DesignersProperty); }
            set { SetValue(DesignersProperty, value); }
        }

        public static readonly DependencyProperty DesignersProperty =
            DependencyProperty.Register("Designers", typeof(DesignersCollection), typeof(DesignedLocator), new PropertyMetadata(null, DesignersChanged));


        private static void DesignersChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {

            if (sender is DesignedLocator locator) {
                if (e.OldValue is DesignersCollection o)
                    o.OptionCalling -= locator.UpdateOptionCalled;

                if (e.NewValue is DesignersCollection n)
                    n.OptionCalling += locator.UpdateOptionCalled;
            }
        }


        protected virtual void UpdateOptionCalled(CustomObject sender, UpdateOptions option) {
            if (option != UpdateOptions.ParentArrange && option != UpdateOptions.ParentMeasure)
                this.SetUpdateOption(sender, option);

            // needs to do other calculation, because it is parent object of CustomObject
        }


        public virtual T GetDesigner<T>()
            where T : IDesigner {
            return (T)Designers.Where(x => x is T).Cast<IDesigner>().FirstOrDefault();
        }

        public virtual IEnumerable<T> GetDesigners<T>()
            where T : IDesigner {
            return Designers.Where(x => x is T).Cast<T>();
        }

        public virtual void Execute<T>(IEnumerable<T> designers, Action<T> action)
            where T : IDesigner {

            if (designers == null || action == null) return;

            foreach (T designer in designers)
                action(designer);
        }

        public virtual void ExecuteFor<T>(Action<T> action)
            where T : IDesigner {

            if (action == null) return;

            foreach (T designer in GetDesigners<T>())
                action(designer);
        }
        #endregion
    }
}
