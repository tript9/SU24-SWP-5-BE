using SWPApp.Models;

namespace SWPApp
{
    public class CertificateService
    {
        public string GenerateHtmlContent(Certificate certificate)
        {
            return $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; }}
                        .certificate {{ border: 2px solid black; padding: 20px; text-align: center; }}
                        .header {{ font-size: 24px; font-weight: bold; }}
                        .content {{ margin-top: 20px; }}
                    </style>
                </head>
                <body>
                    <div class='certificate'>
                        <div class='header'>Certificate of Achievement</div>
                        <div class='content'>
                            This is to certify that<br>
                            <b>{certificate.Result.Color}</b><br>
                            has successfully completed the course<br>
                            <b>{certificate.Result.DiamondId}</b><br>
                            on {certificate.IssueDate.ToString("MMMM dd, yyyy")}
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}
