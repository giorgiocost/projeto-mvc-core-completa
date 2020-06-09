using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DevIO.App.ViewModels;
using DevIO.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace DevIO.App.Controllers
{
   public class ProdutosController : BaseController
   {
       private readonly IProdutoRepository _produtoRepository;
       private readonly IFornecedorRepository _fornecedorRepository;
       private readonly IMapper _mapper;

       public ProdutosController(
           IProdutoRepository produtoRepository,
           IFornecedorRepository fornecedorRepository,
           IMapper mapper)
       {
           _produtoRepository = produtoRepository;
           _fornecedorRepository = fornecedorRepository;
           _mapper = mapper;
       }

       // GET: Produtos
       public async Task<IActionResult> Index()
       {
           return View(_mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterProdutosFornecedores()));
       }

       // GET: Produtos/Details/5
       public async Task<IActionResult> Details(Guid id)
       {
           var produtoViewModel = await ObterProduto(id);

           if (produtoViewModel == null)
           {
               return NotFound();
           }

           return View(produtoViewModel);
       }

       // GET: Produtos/Create
       public IActionResult Create()
       {
           ViewData["FornecedorId"] = new SelectList(_context.Set<FornecedorViewModel>(), "Id", "Documento");
           return View();
       }

       // POST: Produtos/Create
       // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
       // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
       [HttpPost]
       [ValidateAntiForgeryToken]
       public async Task<IActionResult> Create([Bind("Id,FornecedorId,Nome,Descricao,Imagem,Valor,Ativo")] ProdutoViewModel produtoViewModel)
       {
           if (ModelState.IsValid)
           {
               produtoViewModel.Id = Guid.NewGuid();
               _context.Add(produtoViewModel);
               await _context.SaveChangesAsync();
               return RedirectToAction(nameof(Index));
           }
           ViewData["FornecedorId"] = new SelectList(_context.Set<FornecedorViewModel>(), "Id", "Documento", produtoViewModel.FornecedorId);
           return View(produtoViewModel);
       }

       // GET: Produtos/Edit/5
       public async Task<IActionResult> Edit(Guid? id)
       {
           if (id == null)
           {
               return NotFound();
           }

           var produtoViewModel = await _context.ProdutoViewModel.FindAsync(id);
           if (produtoViewModel == null)
           {
               return NotFound();
           }
           ViewData["FornecedorId"] = new SelectList(_context.Set<FornecedorViewModel>(), "Id", "Documento", produtoViewModel.FornecedorId);
           return View(produtoViewModel);
       }

       // POST: Produtos/Edit/5
       // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
       // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
       [HttpPost]
       [ValidateAntiForgeryToken]
       public async Task<IActionResult> Edit(Guid id, [Bind("Id,FornecedorId,Nome,Descricao,Imagem,Valor,Ativo")] ProdutoViewModel produtoViewModel)
       {
           if (id != produtoViewModel.Id)
           {
               return NotFound();
           }

           if (ModelState.IsValid)
           {
               try
               {
                   _context.Update(produtoViewModel);
                   await _context.SaveChangesAsync();
               }
               catch (DbUpdateConcurrencyException)
               {
                   if (!ProdutoViewModelExists(produtoViewModel.Id))
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
           ViewData["FornecedorId"] = new SelectList(_context.Set<FornecedorViewModel>(), "Id", "Documento", produtoViewModel.FornecedorId);
           return View(produtoViewModel);
       }

       // GET: Produtos/Delete/5
       public async Task<IActionResult> Delete(Guid? id)
       {
           if (id == null)
           {
               return NotFound();
           }

           var produtoViewModel = await _context.ProdutoViewModel
               .Include(p => p.Fornecedor)
               .FirstOrDefaultAsync(m => m.Id == id);
           if (produtoViewModel == null)
           {
               return NotFound();
           }

           return View(produtoViewModel);
       }

       // POST: Produtos/Delete/5
       [HttpPost, ActionName("Delete")]
       [ValidateAntiForgeryToken]
       public async Task<IActionResult> DeleteConfirmed(Guid id)
       {
           var produtoViewModel = await _context.ProdutoViewModel.FindAsync(id);
           _context.ProdutoViewModel.Remove(produtoViewModel);
           await _context.SaveChangesAsync();
           return RedirectToAction(nameof(Index));
       }

       private bool ProdutoViewModelExists(Guid id)
       {
           return _context.ProdutoViewModel.Any(e => e.Id == id);
       }

       private async Task<ProdutoViewModel> ObterProduto(Guid id)
       {
           var produto = _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor(id));
           produto.Fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());
           return produto;
       }
   }

}
