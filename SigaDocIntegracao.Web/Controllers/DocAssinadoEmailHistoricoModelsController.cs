using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SigaDocIntegracao.Web.Models.ModuloEmail;
using SigaDocIntegracao.Web.Persistence;

namespace SigaDocIntegracao.Web.Controllers
{
    public class DocAssinadoEmailHistoricoModelsController : Controller
    {
        private readonly Contexto _context;

        public DocAssinadoEmailHistoricoModelsController(Contexto context)
        {
            _context = context;
        }

        // GET: DocAssinadoEmailHistoricoModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.DocAssinadoEmailHistorico.ToListAsync());
        }

        // GET: DocAssinadoEmailHistoricoModels/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var docAssinadoEmailHistoricoModel = await _context.DocAssinadoEmailHistorico
                .FirstOrDefaultAsync(m => m.Id == id);
            if (docAssinadoEmailHistoricoModel == null)
            {
                return NotFound();
            }

            return View(docAssinadoEmailHistoricoModel);
        }

        // GET: DocAssinadoEmailHistoricoModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DocAssinadoEmailHistoricoModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DescricaoModelo,enviado")] DocAssinadoEmailHistoricoModel docAssinadoEmailHistoricoModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(docAssinadoEmailHistoricoModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(docAssinadoEmailHistoricoModel);
        }

        // GET: DocAssinadoEmailHistoricoModels/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var docAssinadoEmailHistoricoModel = await _context.DocAssinadoEmailHistorico.FindAsync(id);
            if (docAssinadoEmailHistoricoModel == null)
            {
                return NotFound();
            }
            return View(docAssinadoEmailHistoricoModel);
        }

        // POST: DocAssinadoEmailHistoricoModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,DescricaoModelo,enviado")] DocAssinadoEmailHistoricoModel docAssinadoEmailHistoricoModel)
        {
            if (id != docAssinadoEmailHistoricoModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(docAssinadoEmailHistoricoModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocAssinadoEmailHistoricoModelExists(docAssinadoEmailHistoricoModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(docAssinadoEmailHistoricoModel);
        }

        // GET: DocAssinadoEmailHistoricoModels/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var docAssinadoEmailHistoricoModel = await _context.DocAssinadoEmailHistorico
                .FirstOrDefaultAsync(m => m.Id == id);
            if (docAssinadoEmailHistoricoModel == null)
            {
                return NotFound();
            }

            return View(docAssinadoEmailHistoricoModel);
        }

        // POST: DocAssinadoEmailHistoricoModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var docAssinadoEmailHistoricoModel = await _context.DocAssinadoEmailHistorico.FindAsync(id);
            if (docAssinadoEmailHistoricoModel != null)
            {
                _context.DocAssinadoEmailHistorico.Remove(docAssinadoEmailHistoricoModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocAssinadoEmailHistoricoModelExists(long id)
        {
            return _context.DocAssinadoEmailHistorico.Any(e => e.Id == id);
        }
    }
}
