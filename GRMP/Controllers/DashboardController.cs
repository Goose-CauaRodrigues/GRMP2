using GRMP.Classes;
using Microsoft.AspNetCore.Mvc;

namespace GRMP.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            Relatorio relatorio = new Relatorio();

            var dados = relatorio.SelecionarFiltro(
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );

            return View(dados);
        }
    }
}
