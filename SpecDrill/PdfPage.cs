namespace SpecDrill
{
    public class PdfPage : WebPage
    {
        public PdfPage() : this(string.Empty) { }
        public PdfPage(string titlePattern) : base(titlePattern) { }

        public override string Text
        {
            get
            {
                return Browser.GetPdfText();
            }
        }
    }
}
