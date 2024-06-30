using System.ComponentModel.DataAnnotations;

namespace SWPApp.DTO
{
    public class CertificateDTO
    {
        public int CertificateId { get; set; }
        public int ResultId { get; set; }
        public DateTime IssueDate { get; set; }
    }

}
