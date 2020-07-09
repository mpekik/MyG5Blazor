using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Printing;

namespace MyG5Blazor.Data
{
    public class Printer
    {
        private static string font_family = string.Empty;
        private static int font_size = 0;
        private static string logoTsel = string.Empty;
        private static string content = string.Empty;
        private static string header = string.Empty;
        private static string footer = string.Empty;

        private static double PIXEL_TO_MM = 0.264583333;

        public static PrintDocument Document(string _header, string _content, string _logoTsel, string _footer
                                                , ref int p_height)
        {
            PrintDocument result = null;

            try
            {
                logoTsel = _logoTsel;
                content = _content;
                header = _header;
                footer = _footer;
                p_height = Process_Document(null);

                PrinterSettings settings = new PrinterSettings();
                result = new PrintDocument();
                result.DefaultPageSettings = GetPrinterPageInfo(settings.PrinterName);
                //result.DefaultPageSettings.PaperSize = new PaperSize("Custom", 320, p_height);
                result.PrintPage += new PrintPageEventHandler(PrintPage_Handler);

                try
                {
                    result.PrinterSettings.PrinterName = settings.PrinterName; // get DefaultPrinter from Windows
                }
                catch { }
            }
            catch
            {
                result = null;
            }

            return result;
        }
        public static PrintDocument Document2(string _header, string _content, string _logoTsel, string _footer
                                                , ref int p_height)
        {
            PrintDocument result = null;

            try
            {
                logoTsel = _logoTsel;
                content = _content;
                header = _header;
                footer = _footer;
                p_height = Process_Document2(null);

                PrinterSettings settings = new PrinterSettings();
                result = new PrintDocument();
                result.DefaultPageSettings = GetPrinterPageInfo(settings.PrinterName);
                //result.DefaultPageSettings.PaperSize = new PaperSize("Custom", 320, p_height);
                result.PrintPage += new PrintPageEventHandler(PrintPage_Handler2);

                try
                {
                    result.PrinterSettings.PrinterName = settings.PrinterName; // get DefaultPrinter from Windows
                }
                catch { }
            }
            catch
            {
                result = null;
            }

            return result;
        }
        private static void PrintPage_Handler2(object sender, PrintPageEventArgs e)
        {
            Process_Document2(e);
        }
        public static PageSettings GetPrinterPageInfo(String printerName)
        {
            PrinterSettings settings;

            // printer by its name 
            settings = new PrinterSettings();

            settings.PrinterName = printerName;

            return settings.DefaultPageSettings;
        }


        private static void PrintPage_Handler(object sender, PrintPageEventArgs e)
        {
            Process_Document(e);
        }

        private static int Process_Document2(PrintPageEventArgs e)
        {
            int result = 0;
            Font font6 = new Font("Calibri", 6);
            Font font8 = new Font("Calibri", 8);
            Font font10 = new Font("Calibri", 10);
            Font font12 = new Font("Calibri", 12);
            Font font14 = new Font("Calibri", 14);

            Font font8Bold = new Font("Calibri", 8, FontStyle.Bold);
            Font font10Bold = new Font("Calibri", 10, FontStyle.Bold);
            Font font12Bold = new Font("Calibri", 12, FontStyle.Bold);
            Font font14Bold = new Font("Calibri", 14, FontStyle.Bold);

            float leading = 4;
            float lineheight6 = font6.GetHeight() + leading;
            float lineheight8 = font8.GetHeight() + leading;
            float lineheight10 = font10.GetHeight() + leading;
            float lineheight12 = font12.GetHeight() + leading;
            float lineheight14 = font14.GetHeight() + leading;

            float _startX = 20;
            float _startY = leading;
            float Offset = 0;

            StringFormat formatLeft = new StringFormat(StringFormatFlags.NoClip);
            StringFormat formatCenter = new StringFormat(formatLeft);
            StringFormat formatRight = new StringFormat(formatLeft);

            formatCenter.Alignment = StringAlignment.Center;
            formatRight.Alignment = StringAlignment.Far;
            formatLeft.Alignment = StringAlignment.Near;

            SizeF layoutSize = new SizeF(280 - Offset * 2, lineheight14);

            Brush brush = Brushes.Black;
            try
            {
                Font font = new Font(font_family, font_size);
                float fontHeight = font.GetHeight();

                Image img = Image.FromFile(logoTsel);
                if (e != null)
                    e.Graphics.DrawImage(img, (e.PageBounds.Width - img.Width) / 2,
                         0,
                         img.Width,
                         img.Height);

                Offset = img.Height + 4;
                Offset = Offset + lineheight8;
                RectangleF layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                string[] linesHeader = header.Split(new[] { '\n' }, StringSplitOptions.None);

                for (int i = 0; i < linesHeader.Length; i++)
                {
                    if (e != null)
                    {
                        e.Graphics.DrawString(linesHeader[i]
                                      , font10, brush, layout, formatCenter);
                        Offset = Offset + lineheight10;
                        layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);
                    }
                }
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);
                string[] linesContent = content.Split(new[] { '\n' }, StringSplitOptions.None);

                for (int i = 0; i < linesContent.Length; i++)
                {
                    if (e != null)
                    {
                        
                            if (linesContent[i].Contains('\t'))
                            {
                                string[] splitLines = linesContent[i].Split(new[] { '\t' }, StringSplitOptions.None);
                                e.Graphics.DrawString(splitLines[0]
                                          , font8, brush, layout, formatLeft);
                                e.Graphics.DrawString(splitLines[1]
                                          , font8, brush, layout, formatRight);
                                Offset = Offset + lineheight8;
                            }
                            else
                            {
                                if (linesContent[i].Contains("TOTAL"))
                                {
                                    e.Graphics.DrawString(linesContent[i]
                                              , font10Bold, brush, layout, formatLeft);
                                    Offset = Offset + lineheight10;
                                }
                                else
                                {
                                    e.Graphics.DrawString(linesContent[i]
                                                  , font8, brush, layout, formatLeft);
                                    Offset = Offset + lineheight8;
                                }
                            }
                        
                        layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);
                    }
                }
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);
                string[] linesFooter = footer.Split(new[] { '\n' }, StringSplitOptions.None);

                for (int i = 0; i < linesFooter.Length; i++)
                {
                    if (e != null)
                    {
                        if (linesFooter[i].Contains("***"))
                        {
                            e.Graphics.DrawString(linesFooter[i]
                                          , font8Bold, brush, layout, formatCenter);
                            Offset = Offset + lineheight8;
                            layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);
                        }
                        else
                        {
                            e.Graphics.DrawString(linesFooter[i]
                                          , font8, brush, layout, formatCenter);
                            Offset = Offset + lineheight8;
                            layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);
                        }
                    }
                }
                //Offset = Offset + (int)fontHeight + 4;
                result = (int)(_startY + Offset - lineheight10);

                if (e != null)
                {

                }
            }
            catch { }

            return result;
        }
        private static int Process_Document(PrintPageEventArgs e)
        {
            int result = 0;
            Font font6 = new Font("Calibri", 6);
            Font font8 = new Font("Calibri", 8);
            Font font10 = new Font("Calibri", 10);
            Font font12 = new Font("Calibri", 12);
            Font font14 = new Font("Calibri", 14);

            Font font8Bold = new Font("Calibri", 8, FontStyle.Bold);
            Font font10Bold = new Font("Calibri", 10, FontStyle.Bold);
            Font font12Bold = new Font("Calibri", 12, FontStyle.Bold);
            Font font14Bold = new Font("Calibri", 14,FontStyle.Bold);

            float leading = 4;
            float lineheight6 = font6.GetHeight() + leading;
            float lineheight8 = font8.GetHeight() + leading;
            float lineheight10 = font10.GetHeight() + leading;
            float lineheight12 = font12.GetHeight() + leading;
            float lineheight14 = font14.GetHeight() + leading;

            float _startX = 20;
            float _startY = leading;
            float Offset = 0;

            StringFormat formatLeft = new StringFormat(StringFormatFlags.NoClip);
            StringFormat formatCenter = new StringFormat(formatLeft);
            StringFormat formatRight = new StringFormat(formatLeft);

            formatCenter.Alignment = StringAlignment.Center;
            formatRight.Alignment = StringAlignment.Far;
            formatLeft.Alignment = StringAlignment.Near;

            SizeF layoutSize = new SizeF(280 - Offset * 2, lineheight14);

            Brush brush = Brushes.Black;
            try
            {
                Font font = new Font(font_family, font_size);
                float fontHeight = font.GetHeight();

                Image img = Image.FromFile(logoTsel);
                if(e!=null)
                e.Graphics.DrawImage(img, (e.PageBounds.Width - img.Width) / 2,
                     0,
                     img.Width,
                     img.Height);

                Offset = img.Height + 4;
                Offset = Offset + lineheight8;
                RectangleF layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                string[] linesHeader = header.Split(new[] { '\n' }, StringSplitOptions.None);

                for (int i = 0;i<linesHeader.Length;i++)
                {
                    if (e != null)
                    {
                            e.Graphics.DrawString(linesHeader[i]
                                          , font10, brush, layout, formatCenter);
                            Offset = Offset + lineheight10;
                        layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);
                    }
                }
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);
                string[] linesContent = content.Split(new[] { '\n' }, StringSplitOptions.None);

                for (int i = 0; i < linesContent.Length; i++)
                {
                    if (e != null)
                    {
                        if (i == 0)
                        {
                            e.Graphics.DrawString(linesContent[i]
                                          , font12Bold, brush, layout, formatLeft);
                            Offset = Offset + lineheight12;
                        }
                        else
                        {
                            if (linesContent[i].Contains('\t'))
                            {
                                string[] splitLines = linesContent[i].Split(new[] { '\t' }, StringSplitOptions.None);
                                e.Graphics.DrawString(splitLines[0]
                                          , font8, brush, layout, formatLeft);
                                e.Graphics.DrawString(splitLines[1]
                                          , font8, brush, layout, formatRight);
                                Offset = Offset + lineheight8;
                            }
                            else
                            {
                                if (linesContent[i].Contains("TOTAL"))
                                {
                                    e.Graphics.DrawString(linesContent[i]
                                              , font10Bold, brush, layout, formatLeft);
                                    Offset = Offset + lineheight10;
                                }
                                else 
                                { 
                                    e.Graphics.DrawString(linesContent[i]
                                                  , font8, brush, layout, formatLeft);
                                    Offset = Offset + lineheight8;
                                }
                            }
                        }
                        layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);
                    }
                }
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);
                string[] linesFooter = footer.Split(new[] { '\n' }, StringSplitOptions.None);

                for (int i = 0; i < linesFooter.Length; i++)
                {
                    if (e != null)
                    {
                        if(linesFooter[i].Contains("***"))
                        {
                            e.Graphics.DrawString(linesFooter[i]
                                          , font8Bold, brush, layout, formatCenter);
                            Offset = Offset + lineheight8;
                            layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);
                        }
                        else
                        {
                            e.Graphics.DrawString(linesFooter[i]
                                          , font8, brush, layout, formatCenter);
                            Offset = Offset + lineheight8;
                            layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);
                        }  
                    }
                }
                //Offset = Offset + (int)fontHeight + 4;
                result = (int)(_startY + Offset - lineheight10);

                if (e != null)
                {

                }
            }
            catch { }

            return result;
        }

        public static bool Print(Config p_config,string _header, string _content, string _logoTsel,string _footer, ref string p_message)
        {
            bool result = false;

            p_message = string.Empty;
            font_family = "Arial";
            font_size = 8;

            try
            {
                int margin_top = 5;
                int margin_bottom = 5;
                int margin_left = 5;
                int margin_right = 5;
                int height = 0;
                Margins margin = new Margins(margin_left, margin_right, margin_top, margin_bottom);
                PrintDocument document = Document(_header, _content, _logoTsel, _footer, ref height);

                document.Print();

                int printed_height = (int)(height * PIXEL_TO_MM) + margin_top + margin_bottom;

                // Case khusus
                if (printed_height < 85)
                {
                    printed_height = 85;
                }

                // Stop Paper berkurang
                Paper.Decrease(printed_height, ref p_config);

                result = true;
            }
            catch (Exception ex)
            {
                p_message = ex.Message;
            }

            return result;
        }
        public static bool Print2(Config p_config, string _header, string _content, string _logoTsel, string _footer, ref string p_message)
        {
            bool result = false;

            p_message = string.Empty;
            font_family = "Arial";
            font_size = 8;

            try
            {
                int margin_top = 5;
                int margin_bottom = 5;
                int margin_left = 5;
                int margin_right = 5;
                int height = 0;
                Margins margin = new Margins(margin_left, margin_right, margin_top, margin_bottom);
                PrintDocument document = Document2(_header, _content, _logoTsel, _footer, ref height);

                document.Print();

                int printed_height = (int)(height * PIXEL_TO_MM) + margin_top + margin_bottom;

                // Case khusus
                if (printed_height < 85)
                {
                    printed_height = 85;
                }

                // Stop Paper berkurang
                Paper.Decrease(printed_height, ref p_config);

                result = true;
            }
            catch (Exception ex)
            {
                p_message = ex.Message;
            }

            return result;
        }
    }
}
