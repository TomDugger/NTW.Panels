using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests.Models {
    public class DataItem: Observable {

        public DataItem(string content) {
            this.content = content;
        }

        #region Selected
        private bool isSelected;
        public bool IsSelected {
            get { return isSelected; }
            set { isSelected = value; this.Send(nameof(IsSelected)); }
        }
        #endregion

        #region Content
        private string content;

        public string Content {
            get { return content; }
            set { content = value; this.Send(nameof(Content)); }
        }
        #endregion
    }
}
