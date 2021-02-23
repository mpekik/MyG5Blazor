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
        static Font font6 = new Font("Calibri", 6);
        static Font font8 = new Font("Calibri", 8);
        static Font font10 = new Font("Calibri", 10);
        static Font font12 = new Font("Calibri", 12);
        static Font font14 = new Font("Calibri", 14);

        static Font font8Bold = new Font("Calibri", 8, FontStyle.Bold);
        static Font font10Bold = new Font("Calibri", 10, FontStyle.Bold);
        static Font font12Bold = new Font("Calibri", 12, FontStyle.Bold);
        static Font font14Bold = new Font("Calibri", 14, FontStyle.Bold);

        static Font font6UnderLine = new Font("Calibri", 6, FontStyle.Underline);
        static Font font8UnderLine = new Font("Calibri", 8, FontStyle.Underline);
        static Font font10UnderLine = new Font("Calibri", 10, FontStyle.Underline);
        static Font font12UnderLine = new Font("Calibri", 12, FontStyle.Underline);
        static Font font14UnderLine = new Font("Calibri", 14, FontStyle.Underline);

        static float leading = 4;
        static float lineheight6 = font6.GetHeight() + leading;
        static float lineheight8 = font8.GetHeight() + leading;
        static float lineheight10 = font10.GetHeight() + leading;
        static float lineheight12 = font12.GetHeight() + leading;
        static float lineheight14 = font14.GetHeight() + leading;

        static StringFormat formatLeft = new StringFormat(StringFormatFlags.NoClip);
        static StringFormat formatCenter = new StringFormat(formatLeft);
        static StringFormat formatRight = new StringFormat(formatLeft);

        private static string font_family = string.Empty;
        private static int font_size = 0;
        private static string logoTsel = string.Empty;
        private static string content = string.Empty;
        private static string header = string.Empty;
        private static string footer = string.Empty;
        private static string _logoTsel = string.Empty;

        private static double PIXEL_TO_MM = 0.264583333;

        private static Transaction _trx = new Transaction();
        private static Config _cfg = new Config();
        private static Costumer _cst = new Costumer();

        private static DateTime _dtNow;
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
        public static bool PrintEDCPay(Transaction trx, Costumer cst, Config cfg, string logTsel, ref string p_message)
        {
            bool result = false;

            _dtNow = DateTime.Now;
            _trx = trx;
            _cst = cst;
            _cfg = cfg;
            _logoTsel = logTsel;

            formatCenter.Alignment = StringAlignment.Center;
            formatRight.Alignment = StringAlignment.Far;
            formatLeft.Alignment = StringAlignment.Near;

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
                PrintDocument document = DocumentEDCPay(ref height);

                document.Print();

                int printed_height = (int)(height * PIXEL_TO_MM) + margin_top + margin_bottom;

                // Case khusus
                if (printed_height < 85)
                {
                    printed_height = 85;
                }

                // Stop Paper berkurang
                Paper.Decrease(printed_height, ref _cfg);

                result = true;
            }
            catch (Exception ex)
            {
                p_message = ex.Message;
            }

            return result;
        
        }

        public static PrintDocument DocumentEDCPay(ref int p_height)
        {
            PrintDocument result = null;

            try
            {
                p_height = Process_DocumentEDCPay(null);

                PrinterSettings settings = new PrinterSettings();
                result = new PrintDocument();
                result.DefaultPageSettings = GetPrinterPageInfo(settings.PrinterName);
                //result.DefaultPageSettings.PaperSize = new PaperSize("Custom", 320, p_height);
                result.PrintPage += new PrintPageEventHandler(PrintPage_HandlerEDCPay);

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
        private static int Process_DocumentEDCPay(PrintPageEventArgs e)
        {
            int result = 0;
            string _jenisTrx = string.Empty;
            if (_trx.jenisTrans == "IP")
            {
                _jenisTrx = "ISI PULSA PRA-BAYAR";
            }
            else if (_trx.jenisTrans == "PB")
            {
                _jenisTrx = "BAYAR TAGIHAN HALO";
            }

            float _startX = 20;
            float _startY = leading;
            float Offset = 0;

            SizeF layoutSize = new SizeF(280 - Offset * 2, lineheight14);

            Brush brush = Brushes.Black;
            try
            {
                Font font = new Font(font_family, font_size);
                float fontHeight = font.GetHeight();

                RectangleF layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);
                
                e.Graphics.DrawString(_cfg.terminalDesc
                              , font10, brush, layout, formatCenter);
                Offset = Offset + lineheight10;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString(_cfg.terminalLocation
                              , font10, brush, layout, formatCenter);
                Offset = Offset + lineheight10;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString(_jenisTrx
                              , font10, brush, layout, formatCenter);
                Offset = Offset + lineheight10;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString(_trx.transID
                              , font8, brush, layout, formatCenter);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("_____________________________________"
                              , font8UnderLine, brush, layout, formatCenter);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("DATE : "+_dtNow.ToString("dd-MM-yyyy")
                              , font8, brush, layout, formatLeft);
                
                e.Graphics.DrawString("TIME : "+_dtNow.ToString("HH:mm:ss")
                              , font8, brush, layout, formatRight);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                Offset = Offset + lineheight6;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("TID"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX+50, _startY + Offset), layoutSize);
                e.Graphics.DrawString(": " + _trx.edcTid
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("MID :" + _trx.edcMid
                              , font8, brush, layout, formatRight);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                int countMasked = _trx.edcCardNo.Length - 7;
                string cardMasked = "************" + _trx.edcCardNo.Substring(countMasked);
                string entryCode = string.Empty;
                if (_trx.edcEntryCode == "D")
                {
                    entryCode = "Dip";
                }
                else if (_trx.edcEntryCode == "S")
                {
                    entryCode = "Swipe";
                }
                e.Graphics.DrawString("CARD NO"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX+50, _startY + Offset), layoutSize);
                e.Graphics.DrawString(": " + cardMasked + " (" + entryCode + ")"
                              , font8, brush, layout, formatLeft); ;
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                Offset = Offset + lineheight6;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("SALE"
                              , font12UnderLine, brush, layout, formatCenter);
                Offset = Offset + lineheight12;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("BATCH"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 50, _startY + Offset), layoutSize);
                e.Graphics.DrawString(": " + _trx.edcBatch
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("TRACE NO :" + _trx.edcTrace
                              , font8, brush, layout, formatRight);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("APPR"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 50, _startY + Offset), layoutSize);
                e.Graphics.DrawString(": " + _trx.edcApproval
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("REF NO :" + _trx.edcRefNo
                              , font8, brush, layout, formatRight);
                
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("TOTAL PEMBAYARAN : Rp" + _cst.intTagihan.ToString("N0")
                              , font8, brush, layout, formatLeft);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);
                
                e.Graphics.DrawString("*** PIN VERIFICATION SUCCESS ***"
                              , font8, brush, layout, formatCenter);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("I AGREE TO PAY ABOVE TOTAL AMOUNT"
                              , font8, brush, layout, formatCenter);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("ACCORDING TO CARD ISSUER AGREEMENT"
                              , font8, brush, layout, formatCenter);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("SIMPAN BUKTI STRUK INI"
                              , font8, brush, layout, formatCenter);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                //Offset = Offset + (int)fontHeight + 4;
                result = (int)(_startY + Offset - lineheight10);

                if (e != null)
                {

                }
            }
            catch { }

            return result;
        }
        private static void PrintPage_HandlerEDCPay(object sender, PrintPageEventArgs e)
        {
            Process_DocumentEDCPay(e);
        }

        public static bool PrintEDCVoid(Transaction trx, Costumer cst, Config cfg, string logTsel, ref string p_message)
        {
            bool result = false;

            _dtNow = DateTime.Now;
            _trx = trx;
            _cst = cst;
            _cfg = cfg;
            _logoTsel = logTsel;

            formatCenter.Alignment = StringAlignment.Center;
            formatRight.Alignment = StringAlignment.Far;
            formatLeft.Alignment = StringAlignment.Near;

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
                PrintDocument document = DocumentEDCVoid(ref height);

                document.Print();

                int printed_height = (int)(height * PIXEL_TO_MM) + margin_top + margin_bottom;

                // Case khusus
                if (printed_height < 85)
                {
                    printed_height = 85;
                }

                // Stop Paper berkurang
                Paper.Decrease(printed_height, ref _cfg);

                result = true;
            }
            catch (Exception ex)
            {
                p_message = ex.Message;
            }

            return result;

        }

        public static PrintDocument DocumentEDCVoid(ref int p_height)
        {
            PrintDocument result = null;

            try
            {
                p_height = Process_DocumentEDCVoid(null);

                PrinterSettings settings = new PrinterSettings();
                result = new PrintDocument();
                result.DefaultPageSettings = GetPrinterPageInfo(settings.PrinterName);
                //result.DefaultPageSettings.PaperSize = new PaperSize("Custom", 320, p_height);
                result.PrintPage += new PrintPageEventHandler(PrintPage_HandlerEDCVoid);

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
        private static int Process_DocumentEDCVoid(PrintPageEventArgs e)
        {
            int result = 0;

            string _jenisTrx = string.Empty;

            if(_trx.jenisTrans=="IP")
            {
                _jenisTrx = "ISI PULSA PRA-BAYAR";
            }else if (_trx.jenisTrans=="PB")
            {
                _jenisTrx = "BAYAR TAGIHAN HALO";
            }
            float _startX = 20;
            float _startY = leading;
            float Offset = 0;

            SizeF layoutSize = new SizeF(280 - Offset * 2, lineheight14);

            Brush brush = Brushes.Black;
            try
            {
                Font font = new Font(font_family, font_size);
                float fontHeight = font.GetHeight();

                RectangleF layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString(_cfg.terminalDesc
                              , font10, brush, layout, formatCenter);
                Offset = Offset + lineheight10;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString(_cfg.terminalLocation
                              , font10, brush, layout, formatCenter);
                Offset = Offset + lineheight10;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString(_jenisTrx
                              , font10, brush, layout, formatCenter);
                Offset = Offset + lineheight10;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString(_trx.transID
                              , font8, brush, layout, formatCenter);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("_____________________________________"
                              , font8UnderLine, brush, layout, formatCenter);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("DATE : " + _dtNow.ToString("dd-MM-yyyy")
                              , font8, brush, layout, formatLeft);
                 
                e.Graphics.DrawString("TIME : " + _dtNow.ToString("HH:mm:ss")
                              , font8, brush, layout, formatRight);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);
                
                Offset = Offset + lineheight6;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("TID"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 50, _startY + Offset), layoutSize);
                e.Graphics.DrawString(": " + _trx.edcTid
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("MID : " + _trx.edcMid
                              , font8, brush, layout, formatRight);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                int countMasked = _trx.edcCardNo.Length - 7;
                string cardMasked = "************" + _trx.edcCardNo.Substring(countMasked);
                string entryCode = string.Empty;
                if(_trx.edcEntryCode=="D")
                {
                    entryCode = "Dip";
                }else if(_trx.edcEntryCode=="S")
                {
                    entryCode = "Swipe";
                }
                e.Graphics.DrawString("CARD NO"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 50, _startY + Offset), layoutSize);
                e.Graphics.DrawString(": " + cardMasked + " (" + entryCode + ")"
                              , font8, brush, layout, formatLeft);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("VOID"
                              , font12UnderLine, brush, layout, formatCenter);
                Offset = Offset + lineheight12;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("BATCH"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 50, _startY + Offset), layoutSize);
                e.Graphics.DrawString(": " + _trx.edcBatch
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("TRACE NO :" + _trx.edcTrace
                              , font8, brush, layout, formatRight);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("APPR"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 50, _startY + Offset), layoutSize);
                e.Graphics.DrawString(": " + _trx.edcApproval
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("REF NO :" + _trx.edcRefNo
                              , font8, brush, layout, formatRight);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("TOTAL PENGEMBALIAN DANA : Rp" + _cst.intTagihan.ToString("N0")
                              , font8, brush, layout, formatLeft);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("SIMPAN BUKTI STRUK INI"
                              , font8, brush, layout, formatCenter);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                //Offset = Offset + (int)fontHeight + 4;
                result = (int)(_startY + Offset - lineheight10);

                if (e != null)
                {

                }
            }
            catch { }

            return result;
        }
        private static void PrintPage_HandlerEDCVoid(object sender, PrintPageEventArgs e)
        {
            Process_DocumentEDCVoid(e);
        }


        public static bool PrintHalo(Transaction trx, Costumer cst, Config cfg, string logTsel, ref string p_message)
        {
            bool result = false;

            _dtNow = DateTime.Now;
            _trx = trx;
            _cst = cst;
            _cfg = cfg;
            _logoTsel = logTsel;

            formatCenter.Alignment = StringAlignment.Center;
            formatRight.Alignment = StringAlignment.Far;
            formatLeft.Alignment = StringAlignment.Near;

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
                PrintDocument document = DocumentHalo(ref height);

                document.Print();

                int printed_height = (int)(height * PIXEL_TO_MM) + margin_top + margin_bottom;

                // Case khusus
                if (printed_height < 85)
                {
                    printed_height = 85;
                }

                // Stop Paper berkurang
                Paper.Decrease(printed_height, ref _cfg);

                result = true;
            }
            catch (Exception ex)
            {
                p_message = ex.Message;
            }

            return result;

        }

        public static PrintDocument DocumentHalo(ref int p_height)
        {
            PrintDocument result = null;

            try
            {
                p_height = Process_DocumentHalo(null);

                PrinterSettings settings = new PrinterSettings();
                result = new PrintDocument();
                result.DefaultPageSettings = GetPrinterPageInfo(settings.PrinterName);
                //result.DefaultPageSettings.PaperSize = new PaperSize("Custom", 320, p_height);
                result.PrintPage += new PrintPageEventHandler(PrintPage_HandlerHalo);

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
        private static int Process_DocumentHalo(PrintPageEventArgs e)
        {
            int result = 0;

            string _jenisTrx = string.Empty;
            string _statusTrx = string.Empty;

            if (_trx.jenisTrans == "IP")
            {
                _jenisTrx = "ISI PULSA PRA-BAYAR";
            }
            else if (_trx.jenisTrans == "PB")
            {
                _jenisTrx = "BAYAR TAGIHAN HALO";
            }

            if(_trx.resultPayment=="00")
            {
                _statusTrx = "BERHASIL";
            }else
            {
                _statusTrx = "GAGAL";
            }
            float _startX = 20;
            float _startY = leading;
            float Offset = 0;

            SizeF layoutSize = new SizeF(280 - Offset * 2, lineheight14);

            Brush brush = Brushes.Black;
            try
            {
                Font font = new Font(font_family, font_size);
                float fontHeight = font.GetHeight();

                Image img = Image.FromFile(_logoTsel);
                if (e != null)
                    e.Graphics.DrawImage(img, (e.PageBounds.Width - img.Width) / 2,
                         0,
                         img.Width,
                         img.Height);

                Offset = img.Height + 4;
                Offset = Offset + lineheight8;
                RectangleF layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString(_cfg.terminalDesc
                              , font10, brush, layout, formatCenter);
                Offset = Offset + lineheight10;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString(_cfg.terminalLocation
                              , font10, brush, layout, formatCenter);
                Offset = Offset + lineheight10;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString(_jenisTrx
                              , font10, brush, layout, formatCenter);
                Offset = Offset + lineheight10;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("_____________________________________"
                              , font8UnderLine, brush, layout, formatCenter);
                Offset = Offset + lineheight6;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("DATE : " + _dtNow.ToString("dd-MM-yyyy")
                              , font8, brush, layout, formatLeft);
                 
                e.Graphics.DrawString("TIME : " + _dtNow.ToString("HH:mm:ss")
                              , font8, brush, layout, formatRight);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                Offset = Offset + lineheight6;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("TRANSACTION ID"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX+120, _startY + Offset), layoutSize);
                e.Graphics.DrawString(":"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 130, _startY + Offset), layoutSize);
                e.Graphics.DrawString(_trx.transID
                              , font8, brush, layout, formatLeft);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("MSISDN"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX+120, _startY + Offset), layoutSize);
                e.Graphics.DrawString(":"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 130, _startY + Offset), layoutSize);
                e.Graphics.DrawString(_cst.phoneNumberMasked
                              , font8, brush, layout, formatLeft);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("TERMINAL ID"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX+120, _startY + Offset), layoutSize);
                e.Graphics.DrawString(":"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 130, _startY + Offset), layoutSize);
                e.Graphics.DrawString(_trx.termID
                              , font8, brush, layout, formatLeft);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("METODE BAYAR"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX+120, _startY + Offset), layoutSize);
                e.Graphics.DrawString(":"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 130, _startY + Offset), layoutSize);
                e.Graphics.DrawString(_cst.strMPembayaran
                              , font8, brush, layout, formatLeft);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("NOMINAL TAGIHAN"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX+120, _startY + Offset), layoutSize);
                e.Graphics.DrawString(":"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 130, _startY + Offset), layoutSize);
                e.Graphics.DrawString("Rp " + _cst.intTagihan.ToString("N0")
                              , font8, brush, layout, formatLeft);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("NOMINAL DIBAYARKAN"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX+120, _startY + Offset), layoutSize);
                e.Graphics.DrawString(":"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 130, _startY + Offset), layoutSize);
                e.Graphics.DrawString("Rp " + _cst.intTagihanTerbayar.ToString("N0")
                              , font8, brush, layout, formatLeft);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("STATUS TRANSAKSI"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX+120, _startY + Offset), layoutSize);
                e.Graphics.DrawString(":"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 130, _startY + Offset), layoutSize);
                e.Graphics.DrawString(_statusTrx
                              , font8Bold, brush, layout, formatLeft);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                if (_trx.jenisTrans == "IP")
                {
                    if (_trx.pulsaSerialNumber != string.Empty && _trx.pulsaSerialNumber != "" && _trx.pulsaSerialNumber != null)
                    {
                        e.Graphics.DrawString("SN"
                                      , font8, brush, layout, formatLeft);
                        layout = new RectangleF(new PointF(_startX + 120, _startY + Offset), layoutSize);
                        e.Graphics.DrawString(":"
                                      , font8, brush, layout, formatLeft);
                        layout = new RectangleF(new PointF(_startX + 130, _startY + Offset), layoutSize);
                        e.Graphics.DrawString(_trx.pulsaSerialNumber
                                      , font8Bold, brush, layout, formatLeft);
                        Offset = Offset + lineheight8;
                        layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);
                    }
                }

                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("TERIMA KASIH TELAH MENGGUNAKAN"
                              , font8, brush, layout, formatCenter);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("LAYANAN JASA MYGRAPARI TELKOMSEL"
                              , font8, brush, layout, formatCenter);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("SIMPAN BUKTI STRUK INI"
                              , font8, brush, layout, formatCenter);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                //Offset = Offset + (int)fontHeight + 4;
                result = (int)(_startY + Offset - lineheight10);

                if (e != null)
                {

                }
            }
            catch { }

            return result;
        }
        private static void PrintPage_HandlerHalo(object sender, PrintPageEventArgs e)
        {
            Process_DocumentHalo(e);
        }

        public static bool PrintPSB(Transaction trx, Costumer cst, Config cfg, string logTsel, ref string p_message)
        {
            bool result = false;

            _dtNow = DateTime.Now;
            _trx = trx;
            _cst = cst;
            _cfg = cfg;
            _logoTsel = logTsel;

            formatCenter.Alignment = StringAlignment.Center;
            formatRight.Alignment = StringAlignment.Far;
            formatLeft.Alignment = StringAlignment.Near;

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
                PrintDocument document = DocumentPSB(ref height);

                document.Print();

                int printed_height = (int)(height * PIXEL_TO_MM) + margin_top + margin_bottom;

                // Case khusus
                if (printed_height < 85)
                {
                    printed_height = 85;
                }

                // Stop Paper berkurang
                Paper.Decrease(printed_height, ref _cfg);

                result = true;
            }
            catch (Exception ex)
            {
                p_message = ex.Message;
            }

            return result;

        }

        public static PrintDocument DocumentPSB(ref int p_height)
        {
            PrintDocument result = null;

            try
            {
                p_height = Process_DocumentPSB(null);

                PrinterSettings settings = new PrinterSettings();
                result = new PrintDocument();
                result.DefaultPageSettings = GetPrinterPageInfo(settings.PrinterName);
                //result.DefaultPageSettings.PaperSize = new PaperSize("Custom", 320, p_height);
                result.PrintPage += new PrintPageEventHandler(PrintPage_HandlerPSB);

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
        private static int Process_DocumentPSB(PrintPageEventArgs e)
        {
            int result = 0;

            string _jenisTrx = string.Empty;
            string _statusTrx = string.Empty;

            _jenisTrx = "BERLANGGANAN HALO";

            if (_trx.resultPayment == "00")
            {
                _statusTrx = "BERHASIL";
            }
            else
            {
                _statusTrx = "GAGAL";
            }
            float _startX = 20;
            float _startY = leading;
            float Offset = 0;

            SizeF layoutSize = new SizeF(280 - Offset * 2, lineheight14);

            Brush brush = Brushes.Black;
            try
            {
                Font font = new Font(font_family, font_size);
                float fontHeight = font.GetHeight();

                Image img = Image.FromFile(_logoTsel);
                if (e != null)
                    e.Graphics.DrawImage(img, (e.PageBounds.Width - img.Width) / 2,
                         0,
                         img.Width,
                         img.Height);

                Offset = img.Height + 4;
                Offset = Offset + lineheight8;
                RectangleF layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString(_cfg.terminalDesc
                              , font10, brush, layout, formatCenter);
                Offset = Offset + lineheight10;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString(_cfg.terminalLocation
                              , font10, brush, layout, formatCenter);
                Offset = Offset + lineheight10;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString(_jenisTrx
                              , font10, brush, layout, formatCenter);
                Offset = Offset + lineheight10;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("_____________________________________"
                              , font8UnderLine, brush, layout, formatCenter);
                Offset = Offset + lineheight6;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("DATE : " + _dtNow.ToString("dd-MM-yyyy")
                              , font8, brush, layout, formatLeft);

                e.Graphics.DrawString("TIME : " + _dtNow.ToString("HH:mm:ss")
                              , font8, brush, layout, formatRight);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                Offset = Offset + lineheight6;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("TRANSACTION ID"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 120, _startY + Offset), layoutSize);
                e.Graphics.DrawString(":"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 130, _startY + Offset), layoutSize);
                e.Graphics.DrawString(_trx.transID
                              , font8, brush, layout, formatLeft);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("MSISDN"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 120, _startY + Offset), layoutSize);
                e.Graphics.DrawString(":"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 130, _startY + Offset), layoutSize);
                e.Graphics.DrawString(_trx.psbDetail.selectedNumber
                              , font8, brush, layout, formatLeft);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("TERMINAL ID"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 120, _startY + Offset), layoutSize);
                e.Graphics.DrawString(":"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 130, _startY + Offset), layoutSize);
                e.Graphics.DrawString(_trx.termID
                              , font8, brush, layout, formatLeft);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("METODE BAYAR"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 120, _startY + Offset), layoutSize);
                e.Graphics.DrawString(":"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 130, _startY + Offset), layoutSize);
                e.Graphics.DrawString(_cst.strMPembayaran
                              , font8, brush, layout, formatLeft);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("NOMINAL TAGIHAN"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 120, _startY + Offset), layoutSize);
                e.Graphics.DrawString(":"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 130, _startY + Offset), layoutSize);
                e.Graphics.DrawString("Rp " + _cst.intTagihan.ToString("N0")
                              , font8, brush, layout, formatLeft);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("NOMINAL DIBAYARKAN"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 120, _startY + Offset), layoutSize);
                e.Graphics.DrawString(":"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 130, _startY + Offset), layoutSize);
                e.Graphics.DrawString("Rp " + _cst.intTagihanTerbayar.ToString("N0")
                              , font8, brush, layout, formatLeft);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("Status Registrasi kartuHALO"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 120, _startY + Offset), layoutSize);
                e.Graphics.DrawString(":"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 130, _startY + Offset), layoutSize);
                e.Graphics.DrawString(_statusTrx
                              , font8Bold, brush, layout, formatLeft);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("Status Upfront Payment"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 120, _startY + Offset), layoutSize);
                e.Graphics.DrawString(":"
                              , font8, brush, layout, formatLeft);
                layout = new RectangleF(new PointF(_startX + 130, _startY + Offset), layoutSize);
                e.Graphics.DrawString("Sedang Diproses"
                              , font8Bold, brush, layout, formatLeft);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("TERIMA KASIH TELAH MENGGUNAKAN"
                              , font8, brush, layout, formatCenter);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("LAYANAN JASA MYGRAPARI TELKOMSEL"
                              , font8, brush, layout, formatCenter);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                e.Graphics.DrawString("SIMPAN BUKTI STRUK INI"
                              , font8, brush, layout, formatCenter);
                Offset = Offset + lineheight8;
                layout = new RectangleF(new PointF(_startX, _startY + Offset), layoutSize);

                //Offset = Offset + (int)fontHeight + 4;
                result = (int)(_startY + Offset - lineheight10);

                if (e != null)
                {

                }
            }
            catch { }

            return result;
        }
        private static void PrintPage_HandlerPSB(object sender, PrintPageEventArgs e)
        {
            Process_DocumentPSB(e);
        }
    }
}
