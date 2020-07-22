namespace SpecDrill
{
    public class PdfPage : WebPage
    {
        private string titlePattern;

        public PdfPage() : this(string.Empty) { }
        public PdfPage(string titlePattern) : base(titlePattern)
        {
            this.titlePattern = titlePattern;
        }

        public override string Text
        {
            get
            {
                return Browser.GetPdfText();
            }
        }
    }
}
