using ClosedXML.Excel;
using RestaurentBookingWebsite.DbModels;

namespace RestaurentBookingWebsite.Services
{
    public class ExcelFileHandling
    {
        
        public MemoryStream CreateExcelFile(List<Booking> bookings)
        {
           
            var workbook = new XLWorkbook();
            
            IXLWorksheet worksheet = workbook.Worksheets.Add("Bookings");

            worksheet.Cell(1, 1).Value = "BookingId"; 
            worksheet.Cell(1, 2).Value = "CustomerId";
            worksheet.Cell(1, 3).Value = "BookingDate";
            worksheet.Cell(1, 4).Value = "SlotTime"; 
            worksheet.Cell(1, 5).Value = "Status"; 
            worksheet.Cell(1, 6).Value = "CreationTime";
            //worksheet.Cell(1, 7).Value = "FirstName";
            //worksheet.Cell(1, 8).Value = "LastName";
          
            int row = 2;
            
            foreach (var book in bookings)
            {
                worksheet.Cell(row, 1).Value = book.BookingId;
                worksheet.Cell(row, 2).Value = book.CustomerId;
                worksheet.Cell(row, 3).Value = book.BookingDate;
                worksheet.Cell(row, 4).Value = book.SlotTime;
                worksheet.Cell(row, 5).Value = book.Status;
                worksheet.Cell(row, 6).Value = book.CreationTime;
                //worksheet.Cell(row, 7).Value = book.Customer.FirstName;
                //worksheet.Cell(row, 8).Value = book.Customer.LastName;
                row++; 
            }
            
            var stream = new MemoryStream();
            
            workbook.SaveAs(stream);
           
            stream.Position = 0;
            return stream;
        }
    }
}
