using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OKAY.Assignment.MVC.Data;
using OKAY.Assignment.MVC.Models;
using OKAY.Assignment.MVC.Services;

namespace OKAY.Assignment.MVC.Areas.Properties.Controllers
{
    [Area("Properties")]
    public class PropertyController : Controller
    {
        private readonly IPropertyRepository _repo;

        public PropertyController(IPropertyRepository repo)
        {
            _repo = repo;
        }

        // GET: Properties/Property
        public async Task<IActionResult> Index()
        {
            return View(await _repo.FindAsync("", 1, 100, "", ""));
        }

        // GET: Properties/Property/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyViewModel = await _repo.FindByIdAsync(id.Value);
            if (propertyViewModel == null)
            {
                return NotFound();
            }

            return View(propertyViewModel);
        }

        // GET: Properties/Property/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Properties/Property/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,bedroom,isAvailable,leasePrice,owner,createdDate,updatedDate")] PropertyViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _repo.CreateAsync(model.name, model.bedroom, model.isAvailable, model.leasePrice);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Properties/Property/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyViewModel = await _repo.FindByIdAsync(id.Value);
            if (propertyViewModel == null)
            {
                return NotFound();
            }
            return View(propertyViewModel);
        }

        // POST: Properties/Property/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,bedroom,isAvailable,leasePrice,owner,createdDate,updatedDate")] PropertyViewModel model)
        {
            if (id != model.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _repo.UpdateAsync(model.id, model.name, model.bedroom, model.isAvailable, model.leasePrice);
                }
                catch (Exception)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Properties/Property/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyViewModel = await _repo.FindByIdAsync(id.Value);
            if (propertyViewModel == null)
            {
                return NotFound();
            }

            return View(propertyViewModel);
        }

        // POST: Properties/Property/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> PropertyViewModelExists(int id)
        {
            return (await _repo.FindByIdAsync(id)) != null;
        }
    }
}
