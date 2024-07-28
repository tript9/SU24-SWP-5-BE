using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.IO;
using SWPApp.Models;

public static class PdfHelper
{
    public static byte[] CreateCertificatePdf(Certificate certificate)
    {
        var document = new PdfDocument();
        var page = document.AddPage();
        var graphics = XGraphics.FromPdfPage(page);

        // Draw a border around the page
        var borderPen = new XPen(XColors.DarkBlue, 2);
        graphics.DrawRectangle(borderPen, 20, 20, page.Width - 40, page.Height - 40);

        // Attempt to add a logo at the top
        string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "C:\\Users\\MSI VN\\Downloads", "logo.jpg");
        if (File.Exists(logoPath))
        {
            var logoImage = XImage.FromFile(logoPath);
            graphics.DrawImage(logoImage, page.Width / 2 - 50, 30, 100, 100);
        }
        else
        {
            graphics.DrawString("Logo Not Found", new XFont("Verdana", 12, XFontStyle.Bold), XBrushes.Red, new XRect(50, 30, 100, 100), XStringFormats.TopLeft);
        }

        var titleFont = new XFont("Verdana", 24, XFontStyle.Bold);
        var subtitleFont = new XFont("Verdana", 18, XFontStyle.Bold);
        var regularFont = new XFont("Verdana", 14, XFontStyle.Regular);

        // Draw subtitle
        graphics.DrawString("Diamond Certificate", subtitleFont, XBrushes.DarkBlue, new XRect(0, 180, page.Width, 50), XStringFormats.Center);

        // Draw issue date
        graphics.DrawString($"Issued on: {certificate.IssueDate:dd MMM yyyy}", regularFont, XBrushes.Black, new XRect(50, 240, page.Width - 100, 30), XStringFormats.TopLeft);

        // Draw diamond details
        graphics.DrawString("Diamond Details:", subtitleFont, XBrushes.DarkBlue, new XRect(50, 280, page.Width - 100, 30), XStringFormats.TopLeft);
        var result = certificate.Result;
        graphics.DrawString($"Origin: {result.DiamondOrigin}", regularFont, XBrushes.Black, new XRect(50, 320, page.Width - 100, 30), XStringFormats.TopLeft);
        graphics.DrawString($"Shape: {result.Shape}", regularFont, XBrushes.Black, new XRect(50, 350, page.Width - 100, 30), XStringFormats.TopLeft);
        graphics.DrawString($"Measurements: {result.Measurements}", regularFont, XBrushes.Black, new XRect(50, 380, page.Width - 100, 30), XStringFormats.TopLeft);
        graphics.DrawString($"Carat Weight: {result.CaratWeight}", regularFont, XBrushes.Black, new XRect(50, 410, page.Width - 100, 30), XStringFormats.TopLeft);
        graphics.DrawString($"Color: {result.Color}", regularFont, XBrushes.Black, new XRect(50, 440, page.Width - 100, 30), XStringFormats.TopLeft);
        graphics.DrawString($"Clarity: {result.Clarity}", regularFont, XBrushes.Black, new XRect(50, 470, page.Width - 100, 30), XStringFormats.TopLeft);
        graphics.DrawString($"Cut: {result.Cut}", regularFont, XBrushes.Black, new XRect(50, 500, page.Width - 100, 30), XStringFormats.TopLeft);
        graphics.DrawString($"Proportions: {result.Proportions}", regularFont, XBrushes.Black, new XRect(50, 530, page.Width - 100, 30), XStringFormats.TopLeft);
        graphics.DrawString($"Polish: {result.Polish}", regularFont, XBrushes.Black, new XRect(50, 560, page.Width - 100, 30), XStringFormats.TopLeft);
        graphics.DrawString($"Symmetry: {result.Symmetry}", regularFont, XBrushes.Black, new XRect(50, 590, page.Width - 100, 30), XStringFormats.TopLeft);
        graphics.DrawString($"Fluorescence: {result.Fluorescence}", regularFont, XBrushes.Black, new XRect(50, 620, page.Width - 100, 30), XStringFormats.TopLeft);

        // Add additional premium package attributes
        if (!string.IsNullOrWhiteSpace(result.Certification))
        {
            graphics.DrawString($"Certification: {result.Certification}", regularFont, XBrushes.Black, new XRect(50, 650, page.Width - 100, 30), XStringFormats.TopLeft);
        }
        if (result.Price.HasValue)
        {
            graphics.DrawString($"Price: ${result.Price}", regularFont, XBrushes.Black, new XRect(50, 680, page.Width - 100, 30), XStringFormats.TopLeft);
        }
        if (!string.IsNullOrWhiteSpace(result.Comments))
        {
            graphics.DrawString($"Comments: {result.Comments}", regularFont, XBrushes.Black, new XRect(50, 710, page.Width - 100, 30), XStringFormats.TopLeft);
        }

        // Add signature area
        graphics.DrawString("Authorized Signature:", regularFont, XBrushes.Black, new XRect(50, 740, page.Width - 100, 30), XStringFormats.TopLeft);
        graphics.DrawLine(borderPen, 200, 770, 500, 770);

        using (MemoryStream ms = new MemoryStream())
        {
            document.Save(ms);
            return ms.ToArray();
        }
    }
}
