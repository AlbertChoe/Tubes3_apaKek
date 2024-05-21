
namespace Tubes3_apaKek.Models
{
    public class Biodata
    {
        public required string NIK { get; set; }
        public required string Nama { get; set; }
        public required string TempatLahir { get; set; }
        public DateTime TanggalLahir { get; set; }
        public required string JenisKelamin { get; set; }
        public required string GolonganDarah { get; set; }
        public required string Alamat { get; set; }
        public required string Agama { get; set; }
        public required string StatusPerkawinan { get; set; }
        public required string Pekerjaan { get; set; }
        public required string Kewarganegaraan { get; set; }

        public override string ToString()
        {
            return $"NIK: {NIK}, Name: {Nama}, Birth Place: {TempatLahir}, Birth Date: {TanggalLahir.ToShortDateString()}, Gender: {JenisKelamin}, Blood Type: {GolonganDarah}, Address: {Alamat}, Religion: {Agama}, Marital Status: {StatusPerkawinan}, Job: {Pekerjaan}, Citizenship: {Kewarganegaraan}";
        }

    }
}

