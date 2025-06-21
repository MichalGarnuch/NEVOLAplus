using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using NEVOLAplus.Data;
using NEVOLAplus.Data.Models.Reservation;

namespace NEVOLAplus.Intranet.Controllers
{
    public class ReservationController : Controller
    {
        private readonly NevolaIntranetContext _context;

        public ReservationController(NevolaIntranetContext context)
        {
            _context = context;
        }

        // GET: Reservation
        public async Task<IActionResult> Index()
        {
            var nevolaIntranetContext = _context.Reservations.Include(r => r.Asset).Include(r => r.Employee);
            return View(await nevolaIntranetContext.ToListAsync());
        }

        // GET: Reservation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Asset)
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservation/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        // POST: Reservation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationId,StartDate,EndDate,Status,AssetId,EmployeeId")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns(reservation.AssetId, reservation.EmployeeId);
            return View(reservation);
        }

        // GET: Reservation/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound();
            PopulateDropdowns(reservation.AssetId, reservation.EmployeeId);
            return View(reservation);
        }

        // POST: Reservation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservationId,StartDate,EndDate,Status,AssetId,EmployeeId")] Reservation reservation)
        {
            if (id != reservation.ReservationId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Reservations.Any(e => e.ReservationId == reservation.ReservationId))
                        return NotFound();
                    throw;
                }
            }
            PopulateDropdowns(reservation.AssetId, reservation.EmployeeId);
            return View(reservation);
        }

        private void PopulateDropdowns(int? selectedAsset = null, int? selectedEmployee = null)
        {
            ViewData["AssetId"] = _context.Assets
                .Select(a => new SelectListItem
                {
                    Value = a.AssetId.ToString(),
                    Text = a.Name,
                    Selected = (a.AssetId == selectedAsset)
                })
                .ToList();

            ViewData["EmployeeId"] = _context.Employees
                .Select(e => new SelectListItem
                {
                    Value = e.EmployeeId.ToString(),
                    Text = e.FirstName + " " + e.LastName,
                    Selected = (e.EmployeeId == selectedEmployee)
                })
                .ToList();
        }


        // GET: Reservation/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Asset)
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
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

            var reservations = await _context.Reservations
                .Where(r => selectedIds.Contains(r.ReservationId))
                .ToListAsync();

            _context.Reservations.RemoveRange(reservations);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Export()
        {
            var reservations = await _context.Reservations
                .Include(r => r.Asset)
                .Include(r => r.Employee)
                .ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Reservations");

            ws.Cells[1, 1].Value = "ReservationId";
            ws.Cells[1, 2].Value = "StartDate";
            ws.Cells[1, 3].Value = "EndDate";
            ws.Cells[1, 4].Value = "Status";
            ws.Cells[1, 5].Value = "AssetId";
            ws.Cells[1, 6].Value = "EmployeeId";

            for (int i = 0; i < reservations.Count; i++)
            {
                var r = reservations[i];
                ws.Cells[i + 2, 1].Value = r.ReservationId;
                ws.Cells[i + 2, 2].Value = r.StartDate.ToString("yyyy-MM-dd");
                ws.Cells[i + 2, 3].Value = r.EndDate.ToString("yyyy-MM-dd");
                ws.Cells[i + 2, 4].Value = r.Status;
                ws.Cells[i + 2, 5].Value = r.AssetId;
                ws.Cells[i + 2, 6].Value = r.EmployeeId;
            }

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;
            var fileName = $"Reservations-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
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

            using var package = new ExcelPackage(stream);
            var ws = package.Workbook.Worksheets.First();
            var rows = ws.Dimension.Rows;

            for (int row = 2; row <= rows; row++)
            {
                var startText = ws.Cells[row, 2].Text;
                var endText = ws.Cells[row, 3].Text;
                var status = ws.Cells[row, 4].Text;
                var assetText = ws.Cells[row, 5].Text; // nazwa assetu
                var employeeText = ws.Cells[row, 6].Text; // email pracownika

                if (string.IsNullOrWhiteSpace(status) || string.IsNullOrWhiteSpace(assetText) || string.IsNullOrWhiteSpace(employeeText))
                    continue;

                DateTime.TryParse(startText, out var start);
                DateTime.TryParse(endText, out var end);

                var asset = await _context.Assets.FirstOrDefaultAsync(a => a.Name == assetText);
                if (asset == null)
                {
                    asset = new NEVOLAplus.Data.Models.Inventory.Asset
                    {
                        Name = assetText,
                        PurchaseDate = DateTime.Now,
                        Cost = 0
                    };
                    _context.Assets.Add(asset);
                    await _context.SaveChangesAsync();
                }

                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == employeeText);
                if (employee == null)
                    continue; // brak pracownika

                var res = new Reservation
                {
                    StartDate = start == default ? DateTime.Now : start,
                    EndDate = end == default ? DateTime.Now : end,
                    Status = status,
                    AssetId = asset.AssetId,
                    EmployeeId = employee.EmployeeId
                };
                _context.Reservations.Add(res);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.ReservationId == id);
        }
    }
}
