using SWPApp.Models;

namespace SWPApp
{
    public class SealingService
    {
        private readonly DiamondAssesmentSystemDBContext _context;

        public SealingService(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        public void CreateSealingRecords()
        {
            var today = DateTime.Today;

            var certificates = _context.Certificates
                .Where(c => c.IssueDate.AddDays(10) == today)
                .ToList();

            foreach (var certificate in certificates)
            {
                var sealingRecord = new SealingRecord
                {
                    RequestId = certificate.ResultId, // Assuming ResultId relates to RequestId
                    SealDate = today,
                    // Assuming Result has a Request property
                    Request = _context.Requests.FirstOrDefault(r => r.RequestId == certificate.ResultId)
                };

                _context.SealingRecords.Add(sealingRecord);
            }

            _context.SaveChanges();
        }
    }
}
