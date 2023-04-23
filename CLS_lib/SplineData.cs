using System.Runtime.InteropServices;

namespace CLS_lib
{
    public class SplineData
    {
        public RawData rawData;
        public int nGrid;
        public double leftDer;
        public double rightDer;
        public List<SplineDataItem> spline;
        public double integValue;
        public SplineData(RawData rawData, int nGrid, double leftDer, double rightDer) 
        {
            this.rawData = rawData;
            this.nGrid = nGrid;
            this.leftDer = leftDer;
            this.rightDer = rightDer;
            spline = new List<SplineDataItem>();
        }

        [DllImport("DLL_mkl.dll")]
        private static extern void MKLSplines(int nodeQnt, double boundA, double boundB, bool uniform, double[] nodes,
             double[] values, int nGrid, double derivLeft, double derivRight, double[] mes, double[] der1, double[] der2,
             ref double integValue, ref int respCode);
        public void DoSplines()
        {
            int respCode = 0;
            double coord, step;
            double[] mes = new double[nGrid];
            double[] der1 = new double[nGrid];
            double[] der2 = new double[nGrid];
            try {
                MKLSplines(rawData.nodeQnt, rawData.boundA, rawData.boundB, rawData.uniform, rawData.nodes, rawData.values,
                    nGrid, leftDer, rightDer, mes, der1, der2, ref integValue, ref respCode);
                step = (rawData.boundB - rawData.boundA) / (nGrid - 1);
                for (int i = 0; i < nGrid; ++i)
                {
                    coord = rawData.boundA + step * i;
                    SplineDataItem sdi = new(coord, mes[i], der1[i], der2[i]);
                    spline.Add(sdi);
                }
            }
            catch {
                throw;
            }
            return;
        }

    }
}
