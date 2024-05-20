namespace Tubes3_apaKek.Models{
    public class ResultData{
        public Biodata biodata;
        public String algorithm;
        public float Similarity;
        public float execTime;

        public ResultData(){
            // Initialize default values if needed
            this.algorithm = "default algorithm";
            this.Similarity = 0.0f;
            this.execTime = 0.0f;
        }

        // Parameterized constructor
        public ResultData(Biodata biodata, string algorithm, float similarity, float execTime){
            this.biodata = biodata;
            this.algorithm = algorithm;
            this.Similarity = similarity;
            this.execTime = execTime;
        }
    }
}