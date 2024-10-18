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
    public class ExModeloEmailParamController : Controller
    {
        private readonly Contexto _context;

        public ExModeloEmailParamController(Contexto context)
        {
            _context = context;
        }

        // GET: ExModeloEmailParam
        public async Task<IActionResult> Index()
        {
            return View(await _context.ModelExModeloEmailParam.ToListAsync());
        }

        // GET: ExModeloEmailParam/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exModeloEmailParamModel = await _context.ModelExModeloEmailParam
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exModeloEmailParamModel == null)
            {
                return NotFound();
            }

            return View(exModeloEmailParamModel);
        }

        // GET: ExModeloEmailParam/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ExModeloEmailParam/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DescricaoModelo,Destinatarios,ConteudoEmail,Assunto")] ExModeloEmailParamModel exModeloEmailParamModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exModeloEmailParamModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(exModeloEmailParamModel);
        }

        // GET: ExModeloEmailParam/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exModeloEmailParamModel = await _context.ModelExModeloEmailParam.FindAsync(id);
            if (exModeloEmailParamModel == null)
            {
                return NotFound();
            }
            return View(exModeloEmailParamModel);
        }

        // POST: ExModeloEmailParam/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,DescricaoModelo,Destinatarios,ConteudoEmail,Assunto")] ExModeloEmailParamModel exModeloEmailParamModel)
        {
            if (id != exModeloEmailParamModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exModeloEmailParamModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExModeloEmailParamModelExists(exModeloEmailParamModel.Id))
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
            return View(exModeloEmailParamModel);
        }

        // GET: ExModeloEmailParam/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exModeloEmailParamModel = await _context.ModelExModeloEmailParam
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exModeloEmailParamModel == null)
            {
                return NotFound();
            }

            return View(exModeloEmailParamModel);
        }

        // POST: ExModeloEmailParam/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var exModeloEmailParamModel = await _context.ModelExModeloEmailParam.FindAsync(id);
            if (exModeloEmailParamModel != null)
            {
                _context.ModelExModeloEmailParam.Remove(exModeloEmailParamModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExModeloEmailParamModelExists(long id)
        {
            return _context.ModelExModeloEmailParam.Any(e => e.Id == id);
        }
    }
}
