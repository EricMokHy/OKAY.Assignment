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

namespace OKAY.Assignment.MVC.Areas.Transactions.Controllers
{
    [Area("Transactions")]
    public class TransactionsController : Controller
    {
        private readonly ITransactionReporsitory _repo;

        public TransactionsController(ITransactionReporsitory reporsitory)
        {
            _repo = reporsitory;
        }

        // GET: Transactions/Transactions
        public async Task<IActionResult> Index()
        {
            return View(await _repo.FindAsync("", 1, 100, "", ""));
        }

        // GET: Transactions/Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactionViewModel = await _repo.FindByIdAsync(id.Value);
            if (transactionViewModel == null)
            {
                return NotFound();
            }

            return View(transactionViewModel);
        }

        // GET: Transactions/Transactions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Transactions/Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,userId,propertyId,userName,propertyName,TransactionDate")] TransactionViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _repo.CreateAsync(model.propertyId);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Transactions/Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactionViewModel = await _repo.FindByIdAsync(id.Value);
            if (transactionViewModel == null)
            {
                return NotFound();
            }
            return View(transactionViewModel);
        }

        // POST: Transactions/Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,userId,propertyId,userName,propertyName,TransactionDate")] TransactionViewModel model)
        {
            if (id != model.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _repo.UpdateAsync(model.id, model.propertyId, model.userId, model.TransactionDate);
                }
                catch (Exception)
                {
                        return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Transactions/Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactionViewModel = await _repo.FindByIdAsync(id.Value);
            if (transactionViewModel == null)
            {
                return NotFound();
            }

            return View(transactionViewModel);
        }

        // POST: Transactions/Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
