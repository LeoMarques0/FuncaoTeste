using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            Session["beneficiarios"] = new List<Beneficiario>();
            Session["beneficiariosDeletados"] = new List<Beneficiario>();
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente bo = new BoCliente();
            
            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else if(bo.VerificarExistencia(model.CPF, model.Id))
            {
                string erro = "CPF já registrado no sistema";
                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erro));
            }
            else
            {
                
                model.Id = bo.Incluir(new Cliente()
                {                    
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = model.CPF
                });

                List<Beneficiario> beneficiarios = (List<Beneficiario>)Session["beneficiarios"];
                BoBeneficiario boBeneficiario = new BoBeneficiario();
                foreach (Beneficiario beneficiario in beneficiarios)
                {
                    beneficiario.Id = boBeneficiario.Incluir(new Beneficiario()
                    {
                        CPF = beneficiario.CPF,
                        Nome = beneficiario.Nome,
                        IdCliente = model.Id
                    });
                }

                return Json("Cadastro efetuado com sucesso");
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente bo = new BoCliente();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else if (bo.VerificarExistencia(model.CPF, model.Id))
            {
                string erro = "CPF já registrado no sistema";
                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erro));
            }
            else
            {
                bo.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = model.CPF
                });

                List<Beneficiario> beneficiarios = (List<Beneficiario>)Session["beneficiarios"];
                List<Beneficiario> beneficiariosDeletados = (List<Beneficiario>)Session["beneficiariosDeletados"];
                BoBeneficiario boBeneficiario = new BoBeneficiario();
                if (beneficiarios.Count > 0)
                {
                    List<Beneficiario> beneficiariosNovos = beneficiarios.Where(b => b.Id == 0).ToList();
                    List<Beneficiario> beneficiariosAntigos = beneficiarios.Where(b => b.Id != 0).ToList();

                    foreach (Beneficiario beneficiario in beneficiariosNovos)
                    {
                        beneficiario.Id = boBeneficiario.Incluir(new Beneficiario()
                        {
                            CPF = beneficiario.CPF,
                            Nome = beneficiario.Nome,
                            IdCliente = model.Id
                        });
                    }

                    foreach (Beneficiario beneficiario in beneficiariosAntigos)
                    {
                        boBeneficiario.Alterar(new Beneficiario()
                        {
                            Id = beneficiario.Id,
                            CPF = beneficiario.CPF,
                            Nome = beneficiario.Nome,
                            IdCliente = model.Id
                        });
                    }
                }

                foreach (Beneficiario beneficiario in beneficiariosDeletados)
                {
                    boBeneficiario.Excluir(beneficiario.Id);
                }

                return Json("Cadastro alterado com sucesso");
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            Models.ClienteModel model = null;

            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone,
                    CPF = cliente.CPF
                };


            }
            Session["beneficiarios"] = new List<Beneficiario>();
            List<Beneficiario> beneficiarios = new BoBeneficiario().ConsultarPorIDCliente(id);
            Session["beneficiarios"] = beneficiarios;
            Session["beneficiariosDeletados"] = new List<Beneficiario>();

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}