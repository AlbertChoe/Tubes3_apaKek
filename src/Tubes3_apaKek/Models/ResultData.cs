namespace Tubes3_apaKek.Models{
    public class ResultData{
        public Biodata biodata;
        public String algorithm;
        public double Similarity;
        public float execTime;
        public string fingerprintImagePath;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public ResultData(){
            this.algorithm = "default algorithm";
            this.Similarity = 0.0f;
            this.execTime = 0.0f;
            this.fingerprintImagePath = "";
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        public ResultData(Biodata biodata, string algorithm, double similarity, float execTime, string fingerprintImagePath){
            this.biodata = biodata;
            this.algorithm = algorithm;
            this.Similarity = similarity;
            this.execTime = execTime;
            this.fingerprintImagePath = fingerprintImagePath;
        }
    }
}