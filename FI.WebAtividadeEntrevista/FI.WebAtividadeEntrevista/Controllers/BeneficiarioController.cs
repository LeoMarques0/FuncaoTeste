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
    public class BeneficiarioController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public JsonResult Incluir(Beneficiario beneficiario)
        {
            List<Beneficiario> beneficiarios = (List<Beneficiario>)Session["beneficiarios"];

            Beneficiario beneficiarioComCPF = null;
            if (beneficiarios.Count > 0)
                beneficiarioComCPF = beneficiarios.Where(b => b.CPF == beneficiario.CPF).FirstOrDefault();
            if (beneficiarioComCPF != null)
            {
                string erro = "CPF já registrado no sistema";
                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erro));
            }

            beneficiarios.Add(beneficiario);
            Session["beneficiarios"] = beneficiarios;

            return Json(beneficiarios);
        }

        [HttpPost]
        public JsonResult Alterar(Beneficiario beneficiario, int index)
        {
            List<Beneficiario> beneficiarios = (List<Beneficiario>)Session["beneficiarios"];
            beneficiario.Id = beneficiarios[index].Id;
            beneficiarios[index] = beneficiario;
            Session["beneficiarios"] = beneficiarios;

            return Json(beneficiarios);
        }

        [HttpDelete]
        public JsonResult Deletar(int index)
        {
            List<Beneficiario> beneficiarios = (List<Beneficiario>)Session["beneficiarios"];
            List<Beneficiario> beneficiariosDeletados = (List<Beneficiario>)Session["beneficiariosDeletados"];

            if(beneficiarios[index].Id != 0)
                beneficiariosDeletados.Add(beneficiarios[index]);
            beneficiarios.RemoveAt(index);

            Session["beneficiarios"] = beneficiarios;
            Session["beneficiariosDeletados"] = beneficiariosDeletados;

            return Json(beneficiarios);
        }

        [HttpPost]
        public JsonResult BeneficiarioList()
        {
            List<Beneficiario> beneficiarios = (List<Beneficiario>)Session["beneficiarios"];
            return Json(beneficiarios);
        }
    }
}