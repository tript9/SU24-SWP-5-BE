using System.ComponentModel.DataAnnotations;

namespace SWPApp.Models
{
    public class Result
    {
        [Key]
        public int ResultId { get; set; }

        // Bước 1: Thông tin chung và định danh
        public int? DiamondId { get; set; }
        public int RequestId { get; set; }
        public string DiamondOrigin { get; set; }

        // Bước 2: Hình dạng và kích thước
        public string Shape { get; set; }
        public string Measurements { get; set; }
        public decimal CaratWeight { get; set; }

        // Bước 3: Đánh giá chất lượng
        public string Color { get; set; }
        public string Clarity { get; set; }
        public string Cut { get; set; }
        public string Proportions { get; set; }
        public string Polish { get; set; }
        public string Symmetry { get; set; }
        public string Fluorescence { get; set; }

        public Request Request { get; set; }
    }
}
