using Antlr4.Runtime.Tree;
using MJC.model;
using Newtonsoft.Json.Linq;
using Sentry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace MJC.common
{
    public class OrderPrint
    {
        int iCellHeight = 0; //Used to get/set the datagridview cell height
        int iTotalWidth = 0; //
        int iRow = 0;//Used as counter
        bool bFirstPage = false; //Used to check whether we are printing first page
        bool bNewPage = false;// Used to check whether we are printing a new page
        int iHeaderHeight = 0; //Used for the header height
        StringFormat strFormat; //Used to format the grid rows.
        ArrayList arrColumnLefts = new ArrayList();//Used to save left coordinates of columns
        ArrayList arrColumnWidths = new ArrayList();//Used to save column widths
        private PrintDocument _printDocument = new PrintDocument();
        private int paperWidth = 827;
        private int paperHeight = 1070;
        private PrintSoldToInfo printSoldToInfo = new PrintSoldToInfo();
        private PrintShipToInfo printShipToInfo = new PrintShipToInfo();
        private PrintOrderInfo printOrderInfo = new PrintOrderInfo();
        private List<PrintOrderItemInfo> printOrderItemInfoList = new List<PrintOrderItemInfo>();
        private PrintInvoiceModel printInvoiceModelObj = new PrintInvoiceModel();
        private int orderStatus = 0;
        private string subTotal = "0.00";
        private string taxValue = "0.00";
        private string laborValue = "0.00";
        private string coreValue = "0.00";
        private string totalSale = "0.00";

        public OrderPrint(int orderId, int orderStatus, string subTotal, string taxValue, string laborValue, string coreValue, string totalSale)
        {
            //printOrderItemList = new List<PrintOrderItem>();
            //for (int i = 0; i < 10; i++)
            //{
            //    printOrderItemList.Add(new PrintOrderItem { Qty = 1, PartNo = "6.5-3-1371KK\nSPI COMP 181SY 11.87Lx3.Ox16SPL", List = 401.31, Net = 324.87, Labor = 145.00, Extended = 324.12 });
            //}

            this.orderStatus = orderStatus;
            this.subTotal = subTotal;
            this.taxValue = taxValue;
            this.laborValue = laborValue;
            this.coreValue = coreValue;
            this.totalSale = totalSale;

            printSoldToInfo = printInvoiceModelObj.GetSoldToInfo(orderId);
            printShipToInfo = printInvoiceModelObj.GetPrintShipToInfo(orderId);
            printOrderInfo = printInvoiceModelObj.GetPrintOrderInfo(orderId);
            printOrderItemInfoList = printInvoiceModelObj.GetPrintOrderItemInfo(orderId);

            int orderItemCount = printOrderItemInfoList.Count;
            while( orderItemCount < 10 ) {
                printOrderItemInfoList.Add(new PrintOrderItemInfo());
                orderItemCount += 1;
            }

            _printDocument.DefaultPageSettings.PaperSize = new PaperSize("A4", this.paperWidth, this.paperHeight);
            _printDocument.PrintPage += new PrintPageEventHandler(_printDocument_PrintPage);
            _printDocument.BeginPrint += new PrintEventHandler(_printDocument_BeginPrint);
        }

        public void PrintForm()
        {
            //Open the print dialog
            //PrintDialog printDialog = new PrintDialog();
            //printDialog.Document = _printDocument;
            //printDialog.UseEXDialog = true;

            ////Get the document
            //if (DialogResult.OK == printDialog.ShowDialog())
            //{
            //    _printDocument.DocumentName = "Test Page Print";
            //    _printDocument.Print();
            //}

            //Open the print preview dialog
            PrintPreviewDialog objPPdialog = new PrintPreviewDialog();
            objPPdialog.Width = 520;
            objPPdialog.Height = 671;
            objPPdialog.Document = _printDocument;
            objPPdialog.ShowDialog();
        }

        private int m_index = 0;

        //private List<PrintOrderItem> printOrderItemList;

        private bool termsOfServiceItem = false;

        private void _printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                int iLeftMargin = 20;
                int iTopMargin = 30;
                //Whether more pages have to print or not
                bool bMorePagesToPrint = false;
                StringFormat strFormat = new StringFormat();
                strFormat.Alignment = StringAlignment.Center;
                int titleHeight = 0;
                int addressHeight = 0;
                int soldToHeight = 0;
                int orderItemColumnY = 0;
                int orderItemContentY = 0;
                Font _fontInvoice = new System.Drawing.Font("Segoe UI", 30F, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                List<PrintItem> printOrderItemContentList = new List<PrintItem>();
                int index = 0;
                Font _fontOrderItem = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                if(printOrderItemInfoList.Count > 0)
                {
                    Type printOrderItemType = printOrderItemInfoList[0].GetType();
                    PropertyInfo[] orderItemProperties = printOrderItemType.GetProperties();
                    int previousHeight = 0;
                    int orderItemTotalHeight = 0;

                    foreach (PrintOrderItemInfo printOrderItem in printOrderItemInfoList)
                    {
                        foreach (PropertyInfo property in orderItemProperties)
                        {
                            string propertyName = property.Name;
                            object propertyValue = property.GetValue(printOrderItem);

                            int width = 0;
                            int height = 18;
                            int marginLeft = 0;
                            int marginTop = 0;
                            switch (propertyName)
                            {
                                case "Qty":
                                    width = 100;
                                    break;
                                case "PartNo":
                                    width = 500;
                                    marginLeft = 100;
                                    break;
                                case "List":
                                    width = 150;
                                    marginLeft = 500;
                                    break;
                                case "Net":
                                    width = 150;
                                    marginLeft = 150;
                                    break;
                                case "Core":
                                    width = 150;
                                    marginLeft = 150;
                                    break;
                                case "Labor":
                                    width = 150;
                                    marginLeft = 150;
                                    break;
                                case "Extended":
                                    marginLeft = 150;
                                    width = 200;
                                    break;
                            }

                            int orderItemContentHeight = (int)e.Graphics.MeasureString(printOrderItem.PartNo, _fontOrderItem, 500).Height;
                            if (orderItemContentHeight == 0)
                                orderItemContentHeight = 34;
                            if (index % 7 == 0)
                            {
                                marginTop += previousHeight;
                                orderItemTotalHeight += orderItemContentHeight;
                            }

                            previousHeight = orderItemContentHeight;
                            string? value = null;
                            value = propertyValue?.ToString();
                            
                            printOrderItemContentList.Add(new PrintItem { value = value, width = width, height = orderItemContentHeight, marginLeft = marginLeft, marginTop = marginTop });

                            index += 1;
                        }
                    }
                }

                if (bNewPage)
                {
                    int titleX = 250;
                    int titleY = iTopMargin;

                    string title = Session.SettingsModelObj.Settings.businessName;

                    e.Graphics.DrawString(title,
                                    _fontInvoice, Brushes.Black,
                                    new RectangleF(titleX, titleY,
                                    320, this.paperHeight), strFormat);
                    titleHeight = (int)e.Graphics.MeasureString(title, _fontInvoice, 320).Height;

                    
                    string address = $"{Session.SettingsModelObj.Settings.address1} {Session.SettingsModelObj.Settings.address2}, {Session.SettingsModelObj.Settings.city}, {Session.SettingsModelObj.Settings.state}, {Session.SettingsModelObj.Settings.postalCode} {Session.SettingsModelObj.Settings.phone}, FAX {Session.SettingsModelObj.Settings.fax}";
                    int addressX = 250;
                    int addressY = iTopMargin + titleHeight - 13;
                    _fontInvoice = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                    e.Graphics.DrawString(address,
                                   _fontInvoice, Brushes.Black,
                                   new RectangleF(addressX, addressY,
                                   320, this.paperHeight), strFormat);
                    addressHeight = (int)e.Graphics.MeasureString(address, _fontInvoice, 320).Height;

                    // invoice header
                    string invoiceState = "PICKING SLIP";
                    switch (orderStatus)
                    {
                        case 1:
                            invoiceState = "Quote";
                            break;
                        case 2:
                            invoiceState = "Picking Slip";
                            break;
                        case 3:
                            invoiceState = "Invoice";
                            break;
                        case 4:
                            invoiceState = "Historical Invoice";
                            break;
                    }
                    int invoiceHeaderX = this.paperWidth - iLeftMargin - 240;
                    int invoiceHeaderY = iTopMargin + 20;
                    _fontInvoice = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                    e.Graphics.DrawString(invoiceState,
                                            _fontInvoice,
                                            new SolidBrush(Color.Black),
                                            new RectangleF(invoiceHeaderX, invoiceHeaderY,
                                            240, 40), strFormat);

                    _fontInvoice = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                    List<PrintItem> invoiceNoList = new List<PrintItem>();
                    PrintItem invoiceNoLabel = new PrintItem { value = "Invoice No,", width = 100, height = 18, marginTop = 0, marginLeft = 0 };
                    PrintItem invoiceNoValue = new PrintItem { value = printOrderInfo.invoiceNumber, width = 140, height = 18, marginTop = 0, marginLeft = 80 };
                    PrintItem pageNoLabel = new PrintItem { value = "Page No,", width = 100, height = 18, marginTop = 18, marginLeft = 0 };
                    PrintItem pageNoValue = new PrintItem { value = "1", width = 140, height = 18, marginTop = 18, marginLeft = 80 };

                    invoiceNoList.Add(invoiceNoLabel);
                    invoiceNoList.Add(invoiceNoValue);
                    invoiceNoList.Add(pageNoLabel);
                    invoiceNoList.Add(pageNoValue);

                    foreach (PrintItem invoiceNo in invoiceNoList)
                    {
                        int invoiceX = this.paperWidth - iLeftMargin - 240 + invoiceNo.marginLeft;
                        int invoiceY = iTopMargin + invoiceNo.marginTop + 20 + 40;
                        int invoiceNoWidth = invoiceNo.width;
                        int invoiceNoHeight = invoiceNo.height;
                        strFormat.Alignment = StringAlignment.Near;
                        strFormat.LineAlignment = StringAlignment.Center;

                        e.Graphics.FillRectangle(new SolidBrush(Color.LightGray),
                                        new Rectangle(invoiceX, invoiceY,
                                        invoiceNoWidth, invoiceNoHeight));

                        e.Graphics.DrawRectangle(Pens.Black,
                            new Rectangle(invoiceX, invoiceY, invoiceNoWidth, invoiceNoHeight));

                        e.Graphics.DrawString(invoiceNo.value,
                            _fontInvoice,
                            new SolidBrush(Color.Black),
                            new RectangleF(invoiceX, invoiceY,
                            invoiceNoWidth, invoiceNoHeight), strFormat);
                    }

                    string soldTo = "SOLD TO";
                    int soldToX = iLeftMargin;
                    int soldToY = iTopMargin + titleHeight + addressHeight - 13;
                    _fontInvoice = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    int soldToWidth = (int)e.Graphics.MeasureString(soldTo, _fontInvoice, 14).Width;
                    soldToHeight = (int)e.Graphics.MeasureString(soldTo, _fontInvoice, 14).Height;

                    e.Graphics.DrawString(soldTo,
                                _fontInvoice, Brushes.Black,
                                new RectangleF(soldToX, soldToY,
                                14, soldToHeight), strFormat);

                    string soldToInfo = printSoldToInfo.customerName + "\n" + printSoldToInfo.address1 + "\n" + printSoldToInfo.address2 + " \n" + printSoldToInfo.city + " " + printSoldToInfo.state + " " + printSoldToInfo.zipcode + " \n" + printSoldToInfo.businessPhone;

                    _fontInvoice = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    int soldToContentX = iLeftMargin + soldToWidth + 16;
                    int soldToContentY = iTopMargin + titleHeight + addressHeight - 13 + 2;

                    e.Graphics.DrawString(soldToInfo,
                               _fontInvoice, Brushes.Black,
                               new RectangleF(soldToContentX, soldToContentY,
                               200, soldToHeight - 4), strFormat);

                    string shipTo = "SHIP TO";
                    int shipToX = 450;
                    int shipToY = iTopMargin + titleHeight + addressHeight - 13;
                    _fontInvoice = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    int shipToWidth = (int)e.Graphics.MeasureString(shipTo, _fontInvoice, 14).Width;
                    int shipToHeight = (int)e.Graphics.MeasureString(shipTo, _fontInvoice, 14).Height;

                    e.Graphics.DrawString(shipTo,
                                _fontInvoice, Brushes.Black,
                                new RectangleF(shipToX, shipToY,
                                14, soldToHeight), strFormat);

                    string shipToInfo = printShipToInfo.name + "\n" + printShipToInfo.address1 + "\n" + printShipToInfo.address2 + " \n" + printShipToInfo.city + " " + printShipToInfo.state + " " + printShipToInfo.zipcode + " \n" + printShipToInfo.businessPhone;

                    _fontInvoice = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    int shipToContentX = 450 + shipToWidth + 16;
                    int shipToContentY = iTopMargin + titleHeight + addressHeight - 13 + 2;

                    e.Graphics.DrawString(shipToInfo,
                               _fontInvoice, Brushes.Black,
                               new RectangleF(shipToContentX, shipToContentY,
                               200, shipToHeight - 4), strFormat);

                    // Print OrderItem
                    List<PrintItem> printOrderColumnList = new List<PrintItem>();
                    printOrderColumnList.Add(new PrintItem { value = "ACCT#", width = 200, height = 18, marginLeft = 0, marginTop = 0 });
                    printOrderColumnList.Add(new PrintItem { value = "DATE", width = 200, height = 18, marginLeft = 200, marginTop = 0 });
                    printOrderColumnList.Add(new PrintItem { value = "P.O.NUMBER", width = 300, height = 18, marginLeft = 200, marginTop = 0 });
                    printOrderColumnList.Add(new PrintItem { value = "SHIP VIA", width = 200, height = 18, marginLeft = 300, marginTop = 0 });
                    printOrderColumnList.Add(new PrintItem { value = "TERMS", width = 300, height = 18, marginLeft = 200, marginTop = 0 });

                    int orderColumnX = iLeftMargin + 50;
                    int orderColumnY = 0;
                    strFormat = new StringFormat();
                    strFormat.Alignment = StringAlignment.Center;
                    foreach (PrintItem order in printOrderColumnList)
                    {
                        orderColumnY = iTopMargin + titleHeight + addressHeight + soldToHeight;
                        double orderTmpWidth = (double)order.width / 1200;
                        int orderWidth = (int)(orderTmpWidth * (this.paperWidth - 2 * (iLeftMargin + 50)));
                        int orderHeight = order.height;

                        e.Graphics.FillRectangle(new SolidBrush(Color.LightGray),
                                        new Rectangle(orderColumnX, orderColumnY,
                                        orderWidth, orderHeight));

                        e.Graphics.DrawRectangle(Pens.Black,
                            new Rectangle(orderColumnX, orderColumnY, orderWidth, orderHeight));

                        e.Graphics.DrawString(order.value,
                            _fontInvoice,
                            new SolidBrush(Color.Black),
                            new RectangleF(orderColumnX, orderColumnY,
                            orderWidth, orderHeight), strFormat);
                        orderColumnX = orderColumnX + orderWidth;
                    }

                    List<PrintItem> printOrderColumnContentList = new List<PrintItem>();

                    DateTime? date = printOrderInfo.date;
                    string dateStr = "";
                    dateStr = date?.ToString("MM-dd-yy") ?? string.Empty;

                    PrintOrder printOrder = new PrintOrder { AcctNumber = printOrderInfo.customerName, Date = dateStr, PONumber = printOrderInfo.poNumber, ShipVia = printOrderInfo.shipVia, Terms = printOrderInfo.terms };

                    Type printOrderType = printOrder.GetType();
                    PropertyInfo[] properties = printOrderType.GetProperties();

                    foreach (PropertyInfo property in properties)
                    {
                        string propertyName = property.Name;
                        object propertyValue = property.GetValue(printOrder);

                        int orderWidth = 0;
                        int orderHeight = 18;
                        int orderMarginLeft = 0;
                        int orderMarginTop = 0;
                        switch (propertyName)
                        {
                            case "AcctNumber":
                                orderWidth = 200;
                                break;
                            case "Date":
                                orderWidth = 200;
                                break;
                            case "PONumber":
                                orderWidth = 300;
                                break;
                            case "ShipVia":
                                orderWidth = 200;
                                break;
                            case "Terms":
                                orderWidth = 300;
                                break;
                        }

                        printOrderColumnContentList.Add(new PrintItem { value = propertyValue?.ToString(), width = orderWidth, height = orderHeight, marginLeft = orderMarginLeft, marginTop = orderMarginTop });
                    }

                    int orderContentX = iLeftMargin + 50;
                    int orderContentY = iTopMargin + titleHeight + addressHeight + soldToHeight + 18;
                    foreach (PrintItem order in printOrderColumnContentList)
                    {
                        double orderTmpWidth = (double)order.width / 1200;
                        int orderWidth = (int)(orderTmpWidth * (this.paperWidth - 2 * (iLeftMargin + 50)));
                        int orderHeight = order.height;

                        e.Graphics.DrawRectangle(Pens.Black,
                            new Rectangle(orderContentX, orderContentY, orderWidth, orderHeight));

                        e.Graphics.DrawString(order.value,
                            _fontInvoice,
                            new SolidBrush(Color.Black),
                            new RectangleF(orderContentX, orderContentY,
                            orderWidth, orderHeight), strFormat);
                        orderContentX = orderContentX + orderWidth;
                    }

                    // Print Order Item Header
                    List<PrintItem> printOrderItemColumnList = new List<PrintItem>();
                    printOrderItemColumnList.Add(new PrintItem { value = "Qty", width = 100, height = 18, marginLeft = 0, marginTop = 0 });
                    printOrderItemColumnList.Add(new PrintItem { value = "Part No.", width = 500, height = 18, marginLeft = 200, marginTop = 0 });
                    printOrderItemColumnList.Add(new PrintItem { value = "List", width = 150, height = 18, marginLeft = 200, marginTop = 0 });
                    printOrderItemColumnList.Add(new PrintItem { value = "Net", width = 150, height = 18, marginLeft = 300, marginTop = 0 });
                    printOrderItemColumnList.Add(new PrintItem { value = "Core", width = 150, height = 18, marginLeft = 200, marginTop = 0 });
                    printOrderItemColumnList.Add(new PrintItem { value = "Labor", width = 150, height = 18, marginLeft = 200, marginTop = 0 });
                    printOrderItemColumnList.Add(new PrintItem { value = "Extended", width = 200, height = 18, marginLeft = 200, marginTop = 0 });

                    int orderItemColumnX = iLeftMargin;
                    orderItemColumnY = iTopMargin + titleHeight + addressHeight + soldToHeight + 75;
                    iTopMargin = orderItemColumnY;
                     strFormat = new StringFormat();
                    strFormat.Alignment = StringAlignment.Near;
                    strFormat.LineAlignment = StringAlignment.Center;
                    index = 0;
                    foreach (PrintItem orderItemRow in printOrderItemColumnList)
                    {
                        double orderItemTmpWidth = (double)orderItemRow.width / 1400;
                        int width = (int)(orderItemTmpWidth * (this.paperWidth - 2 * iLeftMargin));
                        int height = orderItemRow.height;

                        e.Graphics.FillRectangle(new SolidBrush(Color.LightGray),
                                        new Rectangle(orderItemColumnX, orderItemColumnY,
                                        width, height));

                        e.Graphics.DrawRectangle(Pens.Black,
                            new Rectangle(orderItemColumnX, orderItemColumnY, width, height));

                        e.Graphics.DrawString(orderItemRow.value,
                            _fontInvoice,
                            new SolidBrush(Color.Black),
                            new RectangleF(orderItemColumnX, orderItemColumnY,
                            width, height), strFormat);
                        orderItemColumnX = orderItemColumnX + width;
                        if(index % 7 == 0) 
                            iTopMargin += height;
                        index++;
                    }
                }

                int orderItemContentX = iLeftMargin;
                orderItemContentY = iTopMargin;
                
                for(; m_index < printOrderItemContentList.Count; m_index++)
                {
                    PrintItem orderItem = printOrderItemContentList[m_index];
                    if (m_index % 7 == 0)
                    {
                        iTopMargin += orderItem.height;
                    }

                    if (iTopMargin + 18 >= this.paperHeight - 30)
                    {
                        bFirstPage = false;
                        bMorePagesToPrint = true;
                        break;
                    }
                    else
                    {
                        // Order Item Content
                        strFormat.Alignment = StringAlignment.Center;
                        strFormat.LineAlignment = StringAlignment.Center;
                       
                        string value = orderItem.value;
                        double tmpWidth = (double)orderItem.width / 1400;
                        int orderItemWidth = (int)(tmpWidth * (this.paperWidth - 2 * iLeftMargin));

                        int orderItemHeight = orderItem.height;
                        double tmpMarginLeft = (double)orderItem.marginLeft / 1400;
                        int marginLeft = (int)(tmpMarginLeft * (this.paperWidth - 2 * iLeftMargin));
                        int marginTop = orderItem.marginTop;

                        if (m_index % 7 == 0)
                        {
                            orderItemContentX = iLeftMargin;
                        }
                        orderItemContentX = orderItemContentX + marginLeft;
                        orderItemContentY = orderItemContentY + marginTop;

                        if ((m_index / 7) % 2 == 1)
                        {
                            e.Graphics.FillRectangle(new SolidBrush(Color.LightGray),
                                            new Rectangle(orderItemContentX, orderItemContentY,
                                            orderItemWidth, orderItemHeight));
                        }

                        e.Graphics.DrawRectangle(Pens.Black,
                            new Rectangle(orderItemContentX, orderItemContentY, orderItemWidth, orderItemHeight));

                        e.Graphics.DrawString(value,
                            _fontOrderItem,
                            new SolidBrush(Color.Black),
                            new RectangleF(orderItemContentX, orderItemContentY,
                            orderItemWidth, orderItemHeight), strFormat);

                        bNewPage = false;
                    }
                }

                if(m_index == printOrderItemContentList.Count)
                {
                    int orderItemCount = printOrderItemContentList.Count;
                    //iTopMargin += printOrderItemContentList[orderItemCount - 1].height;
                    int marginBottom = this.paperHeight - 30 - iTopMargin;

                    if (!termsOfServiceItem)
                    {
                        if (marginBottom < 160)
                        {
                            bFirstPage = false;
                            bMorePagesToPrint = true;
                        }
                        else
                        {
                            // Footer
                            string desc = Session.SettingsModelObj.Settings.businessDescription;
                            strFormat.Alignment = StringAlignment.Center;
                            Font _fontDesc = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                            int descX = iLeftMargin + 20;
                            int descY = iTopMargin;
                            int descHeight = (int)e.Graphics.MeasureString(desc, _fontDesc, this.paperWidth - 2 * iLeftMargin).Height;
                            iTopMargin += descHeight;

                            e.Graphics.DrawString(desc,
                                    _fontDesc,
                                    new SolidBrush(Color.Black),
                                    new RectangleF(descX, descY,
                                    this.paperWidth - 2 * iLeftMargin, descHeight), strFormat);

                            Font _fontTermsHeader = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            string termsHeader = "Received in Good Condition and Agree to Terms";

                            strFormat.Alignment = StringAlignment.Near;
                            int termsHeaderX = iLeftMargin + 16;
                            int termsHeaderY = iTopMargin + 64;
                            int termsHeaderHeight = (int)e.Graphics.MeasureString(termsHeader, _fontTermsHeader, this.paperWidth - 2 * iLeftMargin).Height;
                            iTopMargin += termsHeaderHeight;

                            e.Graphics.DrawString(termsHeader,
                                    _fontTermsHeader,
                                    new SolidBrush(Color.Black),
                                    new RectangleF(termsHeaderX, termsHeaderY,
                                    this.paperWidth - 2 * iLeftMargin, termsHeaderHeight), strFormat);

                            // Result total
                            List<PrintItem> resultList = new List<PrintItem>();
                            PrintItem subTotal = new PrintItem { value = "SUBTOTAL", width = 120, height = 18, marginTop = 0, marginLeft = 0 };
                            PrintItem subTotalValue = new PrintItem { value = this.subTotal, width = 140, height = 18, marginTop = 0, marginLeft = 120 };
                            PrintItem salesTax = new PrintItem { value = "SALES TAX", width = 120, height = 18, marginTop = 18, marginLeft = 0 };
                            PrintItem salesTaxValue = new PrintItem { value = this.taxValue, width = 140, height = 18, marginTop = 18, marginLeft = 120 };
                            PrintItem core = new PrintItem { value = "CORE", width = 120, height = 18, marginTop = 18, marginLeft = 0 };
                            PrintItem coreValue = new PrintItem { value = this.coreValue, width = 140, height = 18, marginTop = 18, marginLeft = 120 };
                            PrintItem labor = new PrintItem { value = "LABOR", width = 120, height = 18, marginTop = 18, marginLeft = 0 };
                            PrintItem laborValue = new PrintItem { value = this.laborValue, width = 140, height = 18, marginTop = 18, marginLeft = 120 };

                            PrintItem total = new PrintItem { value = "TOTAL", width = 120, height = 18, marginTop = 18, marginLeft = 0 };
                            PrintItem totalValue = new PrintItem { value = this.totalSale, width = 140, height = 18, marginTop = 18, marginLeft = 120 };

                            resultList.Add(subTotal);
                            resultList.Add(subTotalValue);
                            resultList.Add(salesTax);
                            resultList.Add(salesTaxValue);
                            resultList.Add(core);
                            resultList.Add(coreValue);
                            resultList.Add(labor);
                            resultList.Add(laborValue);
                            resultList.Add(total);
                            resultList.Add(totalValue);

                            int resultX = this.paperWidth - iLeftMargin - 260;
                            int resultY = iTopMargin;
                            Font _fontResult = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                            index = 0;
                            foreach (PrintItem result in resultList)
                            {
                                string value = result.value;
                                int width = result.width;
                                int height = result.height;

                                if (index % 2 == 0)
                                {
                                    if (index >= 8)
                                        resultY += 18;
                                    resultY += result.marginTop;
                                    resultX = this.paperWidth - iLeftMargin - 260;
                                    iTopMargin += resultY;
                                }

                                resultX += result.marginLeft;

                                strFormat.Alignment = StringAlignment.Near;
                                strFormat.LineAlignment = StringAlignment.Center;

                                e.Graphics.DrawString(result.value,
                                    _fontResult,
                                    new SolidBrush(Color.Black),
                                    new RectangleF(resultX, resultY,
                                    width, height), strFormat);
                                index++;
                            }

                            termsOfServiceItem = true;
                            iTopMargin = resultY;
                        }
                    }
                    if(termsOfServiceItem) {
                        // Terms of Service
                        marginBottom = this.paperHeight - 30 - iTopMargin;

                        if (marginBottom < 160)
                        {
                            bFirstPage = false;
                            bMorePagesToPrint = true;
                        }
                        else
                        {

                            string termsOfService = Session.SettingsModelObj.Settings.businessTermsOfService;
                            // "PRICES SUBJECT TO CHANGE WITHOUT NOTICE: \nRETURNED GOODS: This shlip must accompany all claims. Goods ordered special, not carried in stock, are not returnable for credit. A 25% charge to cover handling will be made on all returned goods. Parts not claimed within 35 days will be disposed of or sold.\nTERMS: Net 10th of the Month. 1 1/2% per month, 18% per annim will be charged on past due accounts after 30 days.\nWARRANTY: All products, excluding Hi Performance or Heavy Duty Clutches, of our manufacture are under warranty against defective materials or workmanship for a period of 90 days. We will assume no liability beyond the repair or replacement of such parts that may be proven defective. Failure to resurface the flywheel MAY void all warranties.\n \nPROCESSED BY: DR";

                            Font _fontTOS = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                            int termsOfServiceX = iLeftMargin;
                            int tosHeight = (int)e.Graphics.MeasureString(termsOfService, _fontTOS, this.paperWidth - 2 * iLeftMargin).Height;
                            int termsOfServiceY = iTopMargin + 18;

                            e.Graphics.FillRectangle(new SolidBrush(Color.LightGray),
                                             new Rectangle(termsOfServiceX, termsOfServiceY,
                                             this.paperWidth - 2 * iLeftMargin, tosHeight));

                            e.Graphics.DrawRectangle(Pens.Black,
                                new Rectangle(termsOfServiceX, termsOfServiceY, this.paperWidth - 2 * iLeftMargin, tosHeight));

                            e.Graphics.DrawString(termsOfService,
                                _fontTOS,
                                new SolidBrush(Color.Black),
                                new RectangleF(termsOfServiceX, termsOfServiceY,
                                this.paperWidth - 2 * iLeftMargin, tosHeight), strFormat);
                        }
                    }
                }

                // Current Time
                DateTime currentDate = DateTime.Now;
                string currentTime = currentDate.ToString("hh:mm tt");
                Font _fontCurrentTime = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                int currentTimeHeight = (int)e.Graphics.MeasureString(currentTime, _fontCurrentTime, this.paperWidth - 2 * iLeftMargin).Height;
                int currentTimeX = iLeftMargin;
                int currentTimeY = this.paperHeight - 30;
                strFormat.Alignment = StringAlignment.Near;

                e.Graphics.DrawString(currentTime,
                  _fontCurrentTime,
                  new SolidBrush(Color.Black),
                  new RectangleF(currentTimeX, currentTimeY,
                  this.paperWidth - 2 * iLeftMargin, currentTimeHeight), strFormat);

                // Footer Name
                string footerName = Session.SettingsModelObj.Settings.businessFooter; 
                // "CLUTCH & DRIVERSHAFT SPECIALISTS";

                Font _fontFooterName = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                int footerNameHeight = (int)e.Graphics.MeasureString(footerName, _fontFooterName, this.paperWidth - 2 * iLeftMargin).Height;
                int footerNameX = iLeftMargin;
                int footernameY = this.paperHeight - 30;
                strFormat.Alignment = StringAlignment.Center;

                e.Graphics.DrawString(footerName,
                  _fontFooterName,
                  new SolidBrush(Color.Black),
                  new RectangleF(footerNameX, footernameY,
                  this.paperWidth - 2 * iLeftMargin, footerNameHeight), strFormat);

                //If more lines exist, print another page.
                if (bMorePagesToPrint)
                    e.HasMorePages = true;
                else
                    e.HasMorePages = false;
            }
            catch (Exception exc)
            {
                Sentry.SentrySdk.CaptureException(exc);
                if (exc.Message.Contains("KEY"))
                {
                    Messages.ShowError("There was a problem updating the SKU.");
                }
                else
                {
                    Messages.ShowError(exc.Message);
                }
            }
        }

        private void _printDocument_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            try
            {
                strFormat = new StringFormat();
                strFormat.Alignment = StringAlignment.Center;
                strFormat.LineAlignment = StringAlignment.Center;
                strFormat.Trimming = StringTrimming.EllipsisCharacter;

                arrColumnLefts.Clear();
                arrColumnWidths.Clear();
                iCellHeight = 0;
                iRow = 0;
                bFirstPage = true;
                bNewPage = true;
            }
            catch (Exception exc)
            {
                Sentry.SentrySdk.CaptureException(exc);
                if (exc.Message.Contains("KEY"))
                {
                    Messages.ShowError("There was a problem updating the SKU.");
                }
                else
                {
                    Messages.ShowError(exc.Message);
                } 
            }
        }
    }
}
