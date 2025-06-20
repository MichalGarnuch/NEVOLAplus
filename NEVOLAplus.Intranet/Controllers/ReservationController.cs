using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.ReservationId == id);
        }
    }
}
