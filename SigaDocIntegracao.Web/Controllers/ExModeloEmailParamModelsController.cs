using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SigaDocIntegracao.Web.Models.ModuloEmail;
using SigaDocIntegracao.Web.Persistence;
using SigaDocIntegracao.Web.Service;
using static System.Formats.Asn1.AsnWriter;

namespace SigaDocIntegracao.Web.Controllers
{
    public class ExModeloEmailParamModelsController : Controller
    {
        private readonly Contexto _context;

        public ExModeloEmailParamModelsController(Contexto context)
        {
            _context = context;
        }

        // GET: ExModeloEmailParamModels
        public async Task<IActionResult> Index()
        {
            var orderedList = await _context.ModelExModeloEmailParam
                                            .OrderByDescending(e => e.DataCriacao)
                                            .ToListAsync();

            return View(orderedList);
        }

        // GET: ExModeloEmailParamModels/Details/5
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

        // GET: ExModeloEmailParamModels/Create
        public IActionResult Create()
        {
            var emailService = new EmailService(_context);
            List<ExModeloModel> modelos = emailService.GetModelos();

            // Verifique se a lista não é nula e se contém elementos
            if (modelos == null || !modelos.Any())
            {
                // Se necessário, trate a situação quando não houver modelos
                modelos = new List<ExModeloModel>();
            }

            // Ordenar a lista de modelos por NomeModelo em ordem alfabética
            var modelosOrdenados = modelos.OrderBy(m => m.NomeModelo).ToList();

            // Passar a lista ordenada para a view
            ViewBag.Modelos = new SelectList(modelosOrdenados, "CodigoSigaDoc", "NomeModelo");

            return View();
        }

        // POST: ExModeloEmailParamModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdSigaDoc,DescricaoModelo,Destinatarios,ConteudoEmail,Assunto,DataInicio,Ativo,NomeNot")] ExModeloEmailParamModel exModeloEmailParamModel)
        {
            if (ModelState.IsValid)
            {
                // Garantir que as datas sejam convertidas para UTC
                exModeloEmailParamModel.DataInicio = DateTime.SpecifyKind(exModeloEmailParamModel.DataInicio, DateTimeKind.Utc);
                exModeloEmailParamModel.DataCriacao = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

                _context.Add(exModeloEmailParamModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Se o modelo não for válido, você deve obter a lista de modelos novamente
            var emailService = new EmailService(_context);
            List<ExModeloModel> modelos = emailService.GetModelos();

            // Verifique se a lista não é nula e se contém elementos
            if (modelos == null || !modelos.Any())
            {
                modelos = new List<ExModeloModel>();
            }

            // Ordenar a lista de modelos por NomeModelo em ordem alfabética
            var modelosOrdenados = modelos.OrderBy(m => m.NomeModelo).ToList();

            // Passar a lista ordenada para a view
            ViewBag.Modelos = new SelectList(modelosOrdenados, "CodigoSigaDoc", "NomeModelo");

            return View(exModeloEmailParamModel);
        }

        // GET: ExModeloEmailParamModels/Edit/5
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

        // POST: ExModeloEmailParamModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,IdSigaDoc,DescricaoModelo,Destinatarios,ConteudoEmail,Assunto,DataInicio,Ativo,NomeNot")] ExModeloEmailParamModel exModeloEmailParamModel)
        {
            var emailService = new EmailService(_context);
            List<ExModeloModel> modelos = emailService.GetModelos();
            exModeloEmailParamModel.DataInicio = DateTime.SpecifyKind(exModeloEmailParamModel.DataInicio, DateTimeKind.Utc);
            exModeloEmailParamModel.DataUltimoProcessamento = DateTime.SpecifyKind(exModeloEmailParamModel.DataUltimoProcessamento, DateTimeKind.Utc);


            // Verifique se a lista não é nula e se contém elementos
            if (modelos == null || !modelos.Any())
            {
                // Se necessário, trate a situação quando não houver modelos
                modelos = new List<ExModeloModel>();
            }

            // Ordenar a lista de modelos por NomeModelo em ordem alfabética
            var modelosOrdenados = modelos.OrderBy(m => m.NomeModelo).ToList();

            // Passar a lista ordenada para a view
            ViewBag.Modelos = new SelectList(modelosOrdenados, "CodigoSigaDoc", "NomeModelo");
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

        // GET: ExModeloEmailParamModels/Delete/5
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

        // POST: ExModeloEmailParamModels/Delete/5
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
