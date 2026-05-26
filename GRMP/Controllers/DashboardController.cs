using GRMP.Classes;
using Microsoft.AspNetCore.Mvc;
using ProjBancoDados.BancoDados;
using System.Data;

namespace GRMP.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            string idUsuario = HttpContext.Session.GetString("idUsuario");

            if (string.IsNullOrEmpty(idUsuario))
            {
                return RedirectToAction("Login", "Login");
            }

            Usuario Us = new Usuario();

            // Busca dados do usuário logado
            DataTable dt = Us.BuscarPorID(int.Parse(idUsuario));

            foreach (DataRow dr in dt.Rows)
            {
                int nvAcesso = Convert.ToInt32(dr["nvAcesso"]);

                // Se for nível 3 -> continua normalmente
                if (nvAcesso == 3)
                {
                    break;
                }
                // Se for nível 1 -> redireciona
                else if (nvAcesso == 2)
                {
                    return RedirectToAction("Index", "Mapa");


                }
                else
                {
                    return RedirectToAction("InicioExibir", "Usuario");

                }
            }
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
