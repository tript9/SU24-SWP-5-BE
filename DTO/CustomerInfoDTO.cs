namespace SWPApp.DTO
{
    public class CustomerInfoDTO
    {
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? IDCard { get; set; }
        public string? Address { get; set; }
        public bool Status { get; set; }
    }
}
