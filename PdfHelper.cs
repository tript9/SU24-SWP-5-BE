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

        // Remove the border code
        // var borderPen = new XPen(XColors.DarkBlue, 2); // Set the border pen width
        double margin = 30; // Set the margin for the border
        // graphics.DrawRectangle(borderPen, margin, margin, page.Width - 2 * margin, page.Height - 2 * margin);

        // Attempt to add a logo at the top
        string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "C:\\Users\\MSI VN\\Downloads", "logo.jpg");
        if (File.Exists(logoPath))
        {
            var logoImage = XImage.FromFile(logoPath);
            graphics.DrawImage(logoImage, page.Width / 2 - 50, margin + 10, 100, 100); // Adjusted position to accommodate border
        }
        else
        {
            graphics.DrawString("Logo Not Found", new XFont("Verdana", 12, XFontStyle.Bold), XBrushes.Red, new XRect(margin + 30, margin + 10, 100, 100), XStringFormats.TopLeft);
        }

        var titleFont = new XFont("Verdana", 24, XFontStyle.Bold);
        var subtitleFont = new XFont("Verdana", 18, XFontStyle.Bold);
        var regularFont = new XFont("Verdana", 14, XFontStyle.Regular);

        // Draw subtitle
        graphics.DrawString("Diamond Certificate", subtitleFont, XBrushes.DarkBlue, new XRect(0, 150, page.Width, 50), XStringFormats.Center);

        // Draw issue date
        graphics.DrawString($"Issued on: {certificate.IssueDate:dd MMM yyyy}", regularFont, XBrushes.Black, new XRect(margin, 220, page.Width - 2 * margin, 30), XStringFormats.TopLeft);

        // Draw diamond details
        graphics.DrawString("Diamond Details:", subtitleFont, XBrushes.DarkBlue, new XRect(margin, 260, page.Width - 2 * margin, 30), XStringFormats.TopLeft);
        var result = certificate.Result;
        graphics.DrawString($"Origin: {result.DiamondOrigin}", regularFont, XBrushes.Black, new XRect(margin, 300, page.Width - 2 * margin, 30), XStringFormats.TopLeft);
        graphics.DrawString($"Shape: {result.Shape}", regularFont, XBrushes.Black, new XRect(margin, 330, page.Width - 2 * margin, 30), XStringFormats.TopLeft);
        graphics.DrawString($"Measurements: {result.Measurements}", regularFont, XBrushes.Black, new XRect(margin, 360, page.Width - 2 * margin, 30), XStringFormats.TopLeft);
        graphics.DrawString($"Carat Weight: {result.CaratWeight}", regularFont, XBrushes.Black, new XRect(margin, 390, page.Width - 2 * margin, 30), XStringFormats.TopLeft);
        graphics.DrawString($"Color: {result.Color}", regularFont, XBrushes.Black, new XRect(margin, 420, page.Width - 2 * margin, 30), XStringFormats.TopLeft);
        graphics.DrawString($"Clarity: {result.Clarity}", regularFont, XBrushes.Black, new XRect(margin, 450, page.Width - 2 * margin, 30), XStringFormats.TopLeft);
        graphics.DrawString($"Cut: {result.Cut}", regularFont, XBrushes.Black, new XRect(margin, 480, page.Width - 2 * margin, 30), XStringFormats.TopLeft);
        graphics.DrawString($"Proportions: {result.Proportions}", regularFont, XBrushes.Black, new XRect(margin, 510, page.Width - 2 * margin, 30), XStringFormats.TopLeft);
        graphics.DrawString($"Polish: {result.Polish}", regularFont, XBrushes.Black, new XRect(margin, 540, page.Width - 2 * margin, 30), XStringFormats.TopLeft);
        graphics.DrawString($"Symmetry: {result.Symmetry}", regularFont, XBrushes.Black, new XRect(margin, 570, page.Width - 2 * margin, 30), XStringFormats.TopLeft);
        graphics.DrawString($"Fluorescence: {result.Fluorescence}", regularFont, XBrushes.Black, new XRect(margin, 600, page.Width - 2 * margin, 30), XStringFormats.TopLeft);

        // Add additional premium package attributes
        if (!string.IsNullOrWhiteSpace(result.Certification))
        {
            graphics.DrawString($"Certification: {result.Certification}", regularFont, XBrushes.Black, new XRect(margin, 630, page.Width - 2 * margin, 30), XStringFormats.TopLeft);
        }
        if (result.Price.HasValue)
        {
            graphics.DrawString($"Price: ${result.Price}", regularFont, XBrushes.Black, new XRect(margin, 660, page.Width - 2 * margin, 30), XStringFormats.TopLeft);
        }
        if (!string.IsNullOrWhiteSpace(result.Comments))
        {
            graphics.DrawString($"Comments: {result.Comments}", regularFont, XBrushes.Black, new XRect(margin, 690, page.Width - 2 * margin, 30), XStringFormats.TopLeft);
        }

        // Add signature area
        graphics.DrawString("Authorized Signature:", regularFont, XBrushes.Black, new XRect(margin, 720, page.Width - 2 * margin, 30), XStringFormats.TopLeft);

        // Add styled signature text
        var signatureFont = new XFont("Script MT Bold", 24, XFontStyle.BoldItalic);
        graphics.DrawString("John Wick", signatureFont, XBrushes.Black, new XRect(200, 750, page.Width - 2 * margin, 30), XStringFormats.TopLeft);

        graphics.DrawLine(new XPen(XColors.Black, 1), 200, 790, page.Width - margin, 790);

        using (MemoryStream ms = new MemoryStream())
        {
            document.Save(ms);
            return ms.ToArray();
        }
    }
}
