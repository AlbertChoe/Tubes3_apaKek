
namespace Tubes3_apaKek.Models
{
    public class Biodata
    {
        public string NIK { get; set; }
        public string Nama { get; set; }
        public string TempatLahir { get; set; }
        public DateTime TanggalLahir { get; set; }
        public string JenisKelamin { get; set; }
        public string GolonganDarah { get; set; }
        public string Alamat { get; set; }
        public string Agama { get; set; }
        public string StatusPerkawinan { get; set; }
        public string Pekerjaan { get; set; }
        public string Kewarganegaraan { get; set; }

        public override string ToString()
        {
            return $"NIK: {NIK}, Name: {Nama}, Birth Place: {TempatLahir}, Birth Date: {TanggalLahir.ToShortDateString()}, Gender: {JenisKelamin}, Blood Type: {GolonganDarah}, Address: {Alamat}, Religion: {Agama}, Marital Status: {StatusPerkawinan}, Job: {Pekerjaan}, Citizenship: {Kewarganegaraan}";
        }

    }
}

