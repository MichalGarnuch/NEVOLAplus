using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using NEVOLAplus.Data;
using NEVOLAplus.Data.Models.HR;

namespace NEVOLAplus.Intranet.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly NevolaIntranetContext _context;

        public EmployeeController(NevolaIntranetContext context)
        {
            _context = context;
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            var nevolaIntranetContext = _context.Employees.Include(e => e.Position);
            return View(await nevolaIntranetContext.ToListAsync());
        }

        // GET: Employee/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Position)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            PopulatePositions();
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,FirstName,LastName,Email,Phone,PositionId")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulatePositions(employee.PositionId);
            return View(employee);
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();
            PopulatePositions(employee.PositionId);
            return View(employee);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,FirstName,LastName,Email,Phone,PositionId")] Employee employee)
        {
            if (id != employee.EmployeeId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Employees.Any(e => e.EmployeeId == employee.EmployeeId))
                        return NotFound();
                    throw;
                }
            }
            PopulatePositions(employee.PositionId);
            return View(employee);
        }

        private void PopulatePositions(int? selectedId = null)
        {
            ViewData["PositionId"] = _context.Positions
                .Select(p => new SelectListItem
                {
                    Value = p.PositionId.ToString(),
                    Text = p.Name,
                    Selected = (p.PositionId == selectedId)
                })
                .ToList();
        }

        // GET: Employee/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Position)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSelected(int[] selectedIds)
        {
            if (selectedIds == null || selectedIds.Length == 0)
                return RedirectToAction(nameof(Index));

            var employees = await _context.Employees
                .Where(e => selectedIds.Contains(e.EmployeeId))
                .ToListAsync();

            _context.Employees.RemoveRange(employees);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Export()
        {
            var employees = await _context.Employees
                .Include(e => e.Position)
                .ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new OfficeOpenXml.ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Employees");

            ws.Cells[1, 1].Value = "EmployeeId";
            ws.Cells[1, 2].Value = "FirstName";
            ws.Cells[1, 3].Value = "LastName";
            ws.Cells[1, 4].Value = "Email";
            ws.Cells[1, 5].Value = "Phone";
            ws.Cells[1, 6].Value = "PositionId";

            for (int i = 0; i < employees.Count; i++)
            {
                var e = employees[i];
                ws.Cells[i + 2, 1].Value = e.EmployeeId;
                ws.Cells[i + 2, 2].Value = e.FirstName;
                ws.Cells[i + 2, 3].Value = e.LastName;
                ws.Cells[i + 2, 4].Value = e.Email;
                ws.Cells[i + 2, 5].Value = e.Phone;
                ws.Cells[i + 2, 6].Value = e.PositionId;
            }

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;
            var fileName = $"Employees-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return RedirectToAction(nameof(Index));

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new OfficeOpenXml.ExcelPackage(stream);
            var ws = package.Workbook.Worksheets.First();
            var rows = ws.Dimension.Rows;

            for (int row = 2; row <= rows; row++)
            {
                var first = ws.Cells[row, 2].Text;
                var last = ws.Cells[row, 3].Text;
                var email = ws.Cells[row, 4].Text;
                var phone = ws.Cells[row, 5].Text;
                var positionText = ws.Cells[row, 6].Text; // tu może być NAZWA stanowiska, a nie ID!

                if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(last) || string.IsNullOrWhiteSpace(positionText))
                    continue;

                // Szukamy stanowiska po nazwie:
                var position = await _context.Positions.FirstOrDefaultAsync(p => p.Name == positionText);
                if (position == null)
                {
                    position = new NEVOLAplus.Data.Models.HR.Position
                    {
                        Name = positionText
                    };
                    _context.Positions.Add(position);
                    await _context.SaveChangesAsync(); // żeby mieć już pozycję w bazie!
                }

                // Tworzymy nowego pracownika
                var emp = new Employee
                {
                    FirstName = first,
                    LastName = last,
                    Email = string.IsNullOrWhiteSpace(email) ? null : email,
                    Phone = string.IsNullOrWhiteSpace(phone) ? null : phone,
                    PositionId = position.PositionId
                };
                _context.Employees.Add(emp);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
