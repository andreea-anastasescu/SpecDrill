﻿using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecDrill.Secondary.Adapters.WebDriver.Extensions
{
    internal static class PdfPageExtensions
    {
        public static IEnumerable<PdfPage> Pages(this PdfDocument pdfDoc)
        {
            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                yield return pdfDoc.GetPage(i);
            }
        }
        public static string GetText(this PdfPage page) => PdfTextExtractor.GetTextFromPage(page);
    }
}