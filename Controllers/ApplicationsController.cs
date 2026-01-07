using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using UniversityApplicationSystem.Data;
using UniversityApplicationSystem.Models;

namespace UniversityApplicationSystem.Controllers
{
    [Authorize]
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ApplicationsController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ============================
        // ADMIN: VIEW ALL APPLICATIONS
        // ============================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applications = await _context.Applications.ToListAsync();
            var universities = await _context.Universities.ToListAsync();

            ViewBag.Universities = universities;

            return View(applications);
        }

        // ============================
        // STUDENT: APPLY TO UNIVERSITY
        // ============================
        public async Task<IActionResult> Apply(int universityId)
        {
            var userId = _userManager.GetUserId(User);

            bool alreadyApplied = await _context.Applications
                .AnyAsync(a => a.UserId == userId && a.UniversityId == universityId);

            if (alreadyApplied)
            {
                return RedirectToAction(nameof(MyApplications));
            }

            var application = new Application
            {
                UserId = userId,
                UniversityId = universityId,
                ApplicationDate = DateTime.Now,
                Status = "Pending"
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyApplications));
        }

        // ============================
        // STUDENT: VIEW OWN APPLICATIONS
        // ============================
        public async Task<IActionResult> MyApplications()
        {
            var userId = _userManager.GetUserId(User);

            var applications = await _context.Applications
                .Where(a => a.UserId == userId)
                .ToListAsync();

            ViewBag.Universities = await _context.Universities.ToListAsync();

            return View(applications);
        }

        // ============================
        // ADMIN: APPLICATION DETAILS
        // ============================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var application = await _context.Applications
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
                return NotFound();

            ViewBag.University = await _context.Universities
                .FirstOrDefaultAsync(u => u.Id == application.UniversityId);

            return View(application);
        }

        // ============================
        // ADMIN: EDIT (ACCEPT / REJECT)
        // ============================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var application = await _context.Applications.FindAsync(id);
            if (application == null)
                return NotFound();

            return View(application);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Application application)
        {
            if (id != application.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(application);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(application);
        }

        // ============================
        // ADMIN: DELETE APPLICATION
        // ============================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var application = await _context.Applications
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
                return NotFound();

            ViewBag.University = await _context.Universities
                .FirstOrDefaultAsync(u => u.Id == application.UniversityId);

            return View(application);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application != null)
            {
                _context.Applications.Remove(application);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
