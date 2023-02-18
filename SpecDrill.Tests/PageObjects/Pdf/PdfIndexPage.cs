using SpecDrill;
using SpecDrill.Secondary.Ports.AutomationFramework;

#nullable disable

namespace SomeTests.PageObjects.Pdf
{
    public class PdfIndexPage : WebPage
    {
        [Find(By.LinkText, "View Pdf")]
        public IWindowElement<MyPdfPage> LnkViewPdf;
    }
}
