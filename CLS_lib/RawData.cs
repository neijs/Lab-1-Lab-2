using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
namespace CLS_lib
{
    public delegate double FRaw(double x);
    public enum FRawEnum { 
        Linear, RandomInit, Polynomial3
    }
    [Serializable]
    public class RawData
    {
        [JsonProperty]
        public double boundA {get; set;}
        [JsonProperty]
        public double boundB {get; set;}
        [JsonProperty]
        public int nodeQnt {get; set;}
        [JsonProperty]
        public bool uniform {get; set;}
        [JsonIgnore]
        public FRaw? initFunc {get; set;}
        [JsonProperty]
        public double[] nodes {get; set;}
        [JsonProperty]
        public double[] values {get; set;}
        public RawData(double boundA, double boundB, int nodeQnt, bool uniform, FRaw initFunc)
        {
            this.boundA = boundA;
            this.boundB = boundB;
            this.nodeQnt = nodeQnt;
            this.uniform = uniform;
            this.initFunc = initFunc;
            nodes = new double[nodeQnt];
            values = new double[nodeQnt];
            initRawData();
        }
        private void initRawData() {
            Random random = new();
            if (uniform) {
                double step = (boundB - boundA)/(nodeQnt - 1);
                for (int i = 0; i < nodeQnt; ++i) {
                    nodes[i] = boundA + step * i;
                }
            } else {
                int randQnt = random.Next(1, nodeQnt);
                for (int i = 0; i < randQnt; ++i) {
                    double t = random.NextDouble();
                    nodes[i] = boundA*t + (1 - t)*boundB; 
                }
                double step = (boundB - boundA)/(nodeQnt - randQnt - 1);
                for (int i = randQnt; i < nodeQnt; i++) {
                    nodes[i] = boundA + step*(i - randQnt);
                }
                Array.Sort(nodes);
            }
            for (int i = 0; i < nodeQnt; ++i) {
                values[i] = initFunc!(nodes[i]);
            }
        }
        [JsonConstructor]
        public RawData(double boundA, double boundB, int nodeQnt, bool uniform, double[] nodes, double[] values)
        {
            this.boundA = boundA;
            this.boundB = boundB;
            this.nodeQnt = nodeQnt;
            this.uniform = uniform;
            
            this.nodes = new double[nodeQnt];
            this.values = new double[nodeQnt];
            for (int i = 0; i < nodeQnt; ++i)
            {
                this.nodes[i] = nodes[i];
                this.values[i] = values[i];
            }
        }
        public static double Linear(double x)
        {
            return x;
        }
        public static double RandomInit(double x)
        {
            Random randGen = new();
            return randGen.NextDouble();
        }
        public static double Polynomial3(double x)
        {
            return x * (x - 1) * (x - 2) + 2;
        }
        public void Save(string filename)
        {
            FileStream? fs = null;
            try
            {
                var jsonText = JsonConvert.SerializeObject(this, Formatting.Indented);
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
                File.WriteAllText(filename, jsonText);
            }
            catch
            {
                throw;
            }
            finally
            {
                fs?.Close();
            }
        }
        public static void Load(string filename, out RawData rawData)
        {
            FileStream? fs = null;
            try
            {
                var jsonText = File.ReadAllText(filename);
                rawData = JsonConvert.DeserializeObject<RawData>(jsonText)!;  
            }
            catch
            {
                throw; 
            }
            finally
            {
                fs?.Close();
            }
        }
    }
}