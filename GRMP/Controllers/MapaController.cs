using GRMP.Classes;
using GRMP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProjBancoDados.BancoDados;
using System.Data;

namespace GRMP.Controllers
{
    public class MapaController : Controller
    {
        [HttpGet]
        public IActionResult OsPorLocal(int localId)
        {
            Os os = new Os();

            DataTable dt =
                os.BuscarOSPorLocal(localId);

            var lista = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                int status =
                    Convert.ToInt32(row["status"]);

                lista.Add(new
                {
                    id = Convert.ToInt32(
                        row["idOrdemServico"]
                    ),

                    titulo =
                        row["descricaoServico"]
                        .ToString(),

                    statusTexto =
                        status == 0 ? "Aberto" :
                        status == 1 ? "Em andamento" :
                        status == 2 ? "Concluída" :
                        status == 3 ? "Cancelada" :
                        "Em pausa"
                });
            }

            return Json(lista);
        }

        public IActionResult MapaExibir(int? status)
        {
            string idUsuario = HttpContext.Session.GetString("idUsuario");

            if (string.IsNullOrEmpty(idUsuario))
            {
                return RedirectToAction("LoginExibir", "Login");
            }

            Usuario Us = new Usuario();

            // Busca os dados do usuário logado.

            DataTable dt = Us.BuscarPorID(int.Parse(idUsuario));

            foreach (DataRow dr in dt.Rows)
            {
                int nvAcesso = Convert.ToInt32(dr["nvAcesso"]);

                // Se for nível de acesso 3 ou 2 continua normalmente.

                if (nvAcesso == 3 || nvAcesso == 2)
                {
                    break;
                }

                // Se for nível de acesso 1 redireciona.

                else if (nvAcesso == 1)
                {
                    return RedirectToAction("ListarOSExibir", "Usuario");
                }
                else
                {
                    return RedirectToAction("ListarOSExibir", "Usuario");

                }
            }

            var viewModel = new MapaViewModel();

            Os os = new Os();

            DataTable dtBlocos =
                os.BuscarBlocosComOS(status);

            foreach (DataRow dr in dtBlocos.Rows)
            {
                viewModel.BlocosComOs.Add(
                    dr["nome"].ToString()
                );
            }

            DataTable dtChamados =
                os.BuscarChamadosMapa(status);

            foreach (DataRow dr in dtChamados.Rows)
            {
                int st =
                    Convert.ToInt32(dr["status"]);

                viewModel.Chamados.Add(
                    new ChamadoMapa
                    {
                        Id =
                            Convert.ToInt32(
                                dr["idOrdemServico"]
                            ),

                        Titulo =
                            dr["descricaoServico"]
                            .ToString(),

                        Bloco =
                            dr["bloco"]
                            .ToString(),

                        Status = st,

                        StatusTexto = st switch
                        {
                            0 => "Aberto",
                            1 => "Em andamento",
                            2 => "Concluída",
                            3 => "Cancelada",
                            4 => "Em pausa",
                            _ => "Desconhecido"
                        }
                    }
                );
            }

            viewModel.StatusSelecionado = status;

            return View("MapaExibirView", viewModel);
        }

        public IActionResult BlocoExibir(string id, int? localId)
        {
            ViewBag.Bloco = id;

            var viewModel = new BlocoViewModel { NomeBloco = id, LocalSelecionado = localId };

            Local local = new Local();

            DataTable dtLocais =
                local.BuscarLocaisDoBloco(id);

            foreach (DataRow dr in dtLocais.Rows)
            {
                viewModel.Locais.Add(
                    new LocalBlocoViewModel
                    {
                        Id =
                            Convert.ToInt32(
                                dr["idLocal"]
                            ),

                        Nome =
                            dr["nome"]
                            .ToString()
                    }
                );
            }

            Os os = new Os();

            DataTable dtChamados =
                os.BuscarChamadosDoBloco(
                    id,
                    localId
                );

            foreach (DataRow dr in dtChamados.Rows)
            {
                viewModel.Chamados.Add(
                    new ChamadoBlocoViewModel
                    {
                        Id =
                            Convert.ToInt32(
                                dr["idOrdemServico"]
                            ),

                        Titulo =
                            dr["descricaoServico"]
                            .ToString(),

                        Status =
                            Convert.ToInt32(
                                dr["status"]
                            ),

                        Local =
                            dr["localNome"]
                            .ToString()
                    }
                );
            }

            return View("BlocoExibirView", viewModel);
        }
    }
}