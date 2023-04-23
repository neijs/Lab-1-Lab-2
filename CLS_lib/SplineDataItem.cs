namespace CLS_lib
{
    public struct SplineDataItem
    {
        public double coord { get; set; }
        public double value {get; set;}
        public double fDer {get; set;}
        public double sDer {get; set;}

        public SplineDataItem (double coord, double value, double fDer, double sDer)
        {
            this.coord = coord;
            this.value = value;
            this.fDer = fDer;
            this.sDer = sDer;
        }

        public string ToString(string format)
        {
            string info = $"(coord: {coord.ToString(format)}, " +
                $"value: {value.ToString(format)}, " +
                $"d/dx: {fDer.ToString(format)}, " +
                $"d2/dx2: {sDer.ToString(format)},)";
            return info;
        }
        
        public override string ToString()
        {
            string info = $" (coord: {coord:f2}, " +
                $"value: {value:f2}, " +
                $"d/dx: {fDer:f2}, " +
                $"d2/dx2: {sDer:f2})";
            return info;
        }
        
    }
}
