using System.ComponentModel;

namespace Examples.Data {
    public struct SegmentSetting {

        public SegmentSetting(SegmentTypes type, bool horizontal, bool toCenter) {
            this.Type = type;
            this.Horizontal = horizontal;
            this.ToCenter = toCenter;
        }

        [DefaultValue(SegmentTypes.Line)]
        public SegmentTypes Type { get; }

        public bool ToCenter { get; }

        public bool Horizontal { get; }


        public override string ToString() {

            switch (this.Type) {
                case SegmentTypes.QuadraticBezier:
                    return $"{this.Type}, To center = {this.ToCenter}";
                case SegmentTypes.Bezier:
                    return $"{this.Type}, Horizontal = {this.Horizontal}";
                default:
                    return this.Type.ToString();
            }
        }


        public static SegmentSetting Line => new SegmentSetting();

        public static SegmentSetting ToCenterQuadraticBezier => new SegmentSetting(SegmentTypes.QuadraticBezier, false, true);
        public static SegmentSetting FromCenterQuadraticBezier => new SegmentSetting(SegmentTypes.QuadraticBezier, false, false);

        public static SegmentSetting VerticalBezier => new SegmentSetting(SegmentTypes.Bezier, false, false);
        public static SegmentSetting HorizontalBesier => new SegmentSetting(SegmentTypes.Bezier, true, false);
    }
}
