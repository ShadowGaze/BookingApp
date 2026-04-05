using System.ComponentModel.DataAnnotations;

namespace BookingApp.Models
{
    public class BookingModel
    {
        public int Id { get; set; }

        public string username { get; set; }

        [Required]
        public string name { get; set; }
        public DateTime BookingDate { get; set; }

        [Required]
        public string startTime { get; set; }

        [Required]
        public string endTime { get; set; }

        public int BookingStatus { get; set; }
        public bool IsBooked { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public string PDFFileNAME { get; set; }
        public byte[] PDFFileDATA { get; set; }
    }
}
