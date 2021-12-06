
namespace Example.Model {
    public sealed class Item: NotifyProperty {

        public Item(int value, string group) {
            this.Value = value;
            this.group = group;
        }

        private bool isSelected;
        public bool IsSelected {
            get { return isSelected; }
            set { isSelected = value; this.Send(nameof(isSelected)); }
        }

        private int val;
        public int Value {
            get { return val; }
            set { val = value; this.Send(nameof(Value)); }
        }

        private string group;
        public string Group {
            get { return group; }
        }
    }
}
