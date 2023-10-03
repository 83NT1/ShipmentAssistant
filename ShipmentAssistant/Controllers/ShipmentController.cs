using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Org.BouncyCastle.Utilities;
using ShipmentAssistant.Models;
using System.IO;
using System.Text;

namespace ShipmentAssistant.Controllers
{
    [Authorize]
    public class ShipmentController : Controller
    {
        private readonly HistoryContext _context;

        public ShipmentController(HistoryContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            byte[]? bytes = HttpContext.Session.Get("FormatError");
            string msg = bytes == null ? null : Encoding.UTF8.GetString(bytes);

            return View("Index",msg);
        }

        [HttpGet]
        public IActionResult LoadAssistant()
        {
            byte[]? bytes = HttpContext.Session.Get("items");

            if(bytes == null)
            {         
                byte[] bytesMsg = Encoding.UTF8.GetBytes("Brak załadowanych danych");
                HttpContext.Session.Set("FormatError", bytesMsg);
                return Redirect(nameof(Index));
            } 
            else
            {
                List<Item> items = JsonConvert.DeserializeObject<List<Item>>(Encoding.UTF8.GetString(bytes));

                return View("Assistant", items);
            }
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Assistant(IFormFile file, bool join = true)
        {
            if(file == null)
            {
                return Redirect(nameof(Index));
            }

            try
            {

                var inputstream = file.OpenReadStream();

                XSSFWorkbook workbook = new XSSFWorkbook(inputstream);

                ISheet sheet = workbook.GetSheetAt(0);

                List<Item> items = new List<Item>();

                int index = 1;
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);

                    string code = row.GetCell(1).CellType == CellType.Numeric ? row.GetCell(1).NumericCellValue.ToString() : row.GetCell(1).StringCellValue;
                    string name = row.GetCell(2).StringCellValue;
                    int units = (int)row.GetCell(3).NumericCellValue;

                    if (join)
                    {
                        var existingItem = items.FirstOrDefault(f => f.Code == code);

                        if (existingItem == null)
                        {
                            items.Add(new Item(index++, code, name, units));
                        }
                        else
                        {
                            existingItem.Units += units;
                        }
                    }
                    else
                    {
                        items.Add(new Item(index++, code, name, units));
                    }
                }

                HttpContext.Session.Set("items", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(items)));
                HttpContext.Session.Remove("FormatError");
                return View("Assistant", items);
            }
            catch
            {
                byte[] bytes = Encoding.UTF8.GetBytes("Plik jest w nieodpowiednim formacie. Pamiętaj o wierszu nagłówkowym.");
                HttpContext.Session.Set("FormatError", bytes);
                return Redirect(nameof(Index));
            }
        }

        [HttpPost]
        public IActionResult SaveAssistant(string submit, List<Item> items)
        {
            if (submit == "raport")
            {
                XSSFWorkbook workbook = new XSSFWorkbook();

                ISheet spreadSheet = workbook.CreateSheet("Arkusz1");

                IRow header = spreadSheet.CreateRow(0);
                header.CreateCell(0).SetCellValue("lp");
                header.CreateCell(1).SetCellValue("kod");
                header.CreateCell(2).SetCellValue("nazwa");
                header.CreateCell(3).SetCellValue("szt");
                header.CreateCell(4).SetCellValue("przyjęto");
                header.CreateCell(5).SetCellValue("brakujace");
                header.CreateCell(6).SetCellValue("uwagi");

                for (int i = 0; i < items.Count; i++)
                {
                    IRow row = spreadSheet.CreateRow(i + 1);
                    ICell cell0 = row.CreateCell(0);
                    cell0.SetCellValue(items[i].Index);
                    ICell cell1 = row.CreateCell(1);
                    cell1.SetCellValue(items[i].Code);
                    ICell cell2 = row.CreateCell(2);
                    cell2.SetCellValue(items[i].Name);
                    ICell cell3 = row.CreateCell(3);
                    cell3.SetCellValue(items[i].Units);
                    ICell cell4 = row.CreateCell(4);
                    cell4.SetCellValue(items[i].UnitsRecived);
                    ICell cell5 = row.CreateCell(5);
                    cell5.SetCellValue(items[i].Units - items[i].UnitsRecived);
                    ICell cell6 = row.CreateCell(6);
                    cell6.SetCellValue(items[i].Info);


                    if (items[i].UnitsRecived > 0)
                    {
                        _context.HistoryLogs.Add(new HistoryLog(DateTime.Now, User.Identity.Name, items[i].Code, items[i].UnitsRecived));
                    }
                }

                _context.SaveChanges();

                byte[] fileContents = null;

                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    fileContents = memoryStream.ToArray();
                }

                HttpContext.Session.Set("items", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(items)));

                return File(fileContents, "application/octet-stream", $"raport.xlsx");
            }
            else if(submit == "save")
            {
                HttpContext.Session.Set("items", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(items)));
                return View("Assistant", items);
            }
            else
            {
                HttpContext.Session.Remove("items");
                return Redirect(nameof(Index));
            }
        }
    }   
}
