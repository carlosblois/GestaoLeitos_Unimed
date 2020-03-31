using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Security.Principal;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Security;
using System.Configuration;
using GLeitos.Models;
using ExpoFramework.Framework.Utils;
using GLeitos.GLeitosTO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace GLeitos.Controllers
{
    public class ConfiguracaoController : BaseController
    {
        #region "Itens de Check-List"

        public ActionResult ItemCheckList(decimal? id_perfil)
        {
            ItemCheckListView vwItemCheckList = new ItemCheckListView();
            ItemCheckListTO objItemCheckList = new ItemCheckListTO();
            List<ItemCheckListTO> objLstItemCheckList = new List<ItemCheckListTO>();
            try
            {

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];
                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];
                string url = "api/ItemCheckList/items";

                vwItemCheckList.ListaItemCheckList = base.Listar<ItemCheckListTO>(accessToken, _urlBase, url);


                if (id_perfil == null)
                {
                    vwItemCheckList.ItemCheckList = new ItemCheckListTO();
                }
                else
                {

                    List<ItemCheckListTO> lstRetorno = new List<ItemCheckListTO>();

                    accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                    _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                    url = string.Format("api/ItemCheckList/items");

                    objLstItemCheckList = Listar<ItemCheckListTO>(accessToken, _urlBase, url);

                    vwItemCheckList.ItemCheckList = objLstItemCheckList.Where(m => m.id_ItemChecklist == id_perfil.ToString()).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                vwItemCheckList.erro = "Ocorreu um erro ao listar itens de check-list. Detalhe: " + ex.Message;
                //throw ex;

            }
            ViewBag.MenuItemCheckList = "menu-ativo";
            return View("ItemCheckList", vwItemCheckList);
        }

        public ActionResult ExcluirItemCheckList(string id_itemchecklist)
        {
            ItemCheckListView vwItemCheckList = new ItemCheckListView();
            List<ItemCheckListTO> objLstItemCheckList = new List<ItemCheckListTO>();
            try
            {

                if (id_itemchecklist == null)
                {
                    throw new Exception("Identificador do item de check-list é inválido.");
                }
                else
                {

                    List<ItemCheckListTO> lstRetorno = new List<ItemCheckListTO>();

                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                    string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                    string url = string.Format("api/ItemCheckList/items?id_ItemCheckList={0}", id_itemchecklist);

                    if (!Excluir(accessToken, _urlBase, url))
                    {
                        throw new Exception("Erro ao tentar excluir um item check-list.");
                    }

                    url = string.Format("api/ItemCheckList/items");

                    objLstItemCheckList = Listar<ItemCheckListTO>(accessToken, _urlBase, url);

                    vwItemCheckList.ListaItemCheckList = objLstItemCheckList;

                    vwItemCheckList.mensagem = "Item de Check-List excluído com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwItemCheckList.erro = "Erro ao tentar excluir o item de check-list. Erro:" + ex.Message;
            }
            ViewBag.MenuItemCheckList = "menu-ativo";

            return View("ItemCheckList", vwItemCheckList);
        }

        public ActionResult SalvarItemCheckList(ItemCheckListView model)
        {
            ItemCheckListView vwItemCheckList = new ItemCheckListView();
            ItemCheckListTO objItemCheckList = new ItemCheckListTO();
            List<ItemCheckListTO> objLstItemCheckList = new List<ItemCheckListTO>();
            string id_ItemCheckList = "";
            try
            {

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];
                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];
                string url = "api/ItemCheckList/items";

                vwItemCheckList.ListaItemCheckList = base.Listar<ItemCheckListTO>(accessToken, _urlBase, url);

                if (!ModelState.IsValid)
                {
                    return View("ItemCheckList", model);
                }
                else
                {
                    ModelState.Clear();

                    List<ItemCheckListTO> lstRetorno = new List<ItemCheckListTO>();

                    accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                    _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                    url = string.Format("api/ItemCheckList/items");

                    if (string.IsNullOrWhiteSpace(model.ItemCheckList.id_ItemChecklist))
                    {
                        model.ItemCheckList.id_ItemChecklist = "0";
                    }
                    else
                    {
                        id_ItemCheckList = model.ItemCheckList.id_ItemChecklist;
                    }

                    base.Salvar<ItemCheckListTO>(accessToken, _urlBase, url, model.ItemCheckList, ref id_ItemCheckList);

                    url = string.Format("api/ItemCheckList/items");

                    objLstItemCheckList = Listar<ItemCheckListTO>(accessToken, _urlBase, url);

                    vwItemCheckList.ItemCheckList = new ItemCheckListTO();

                    vwItemCheckList.ListaItemCheckList = objLstItemCheckList;

                    vwItemCheckList.mensagem = "Item de Check-List salvo com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwItemCheckList.erro = "Erro ao salvar o Item de Check-List. Erro:" + ex.Message;
                //throw ex;
            }
            ViewBag.MenuItemCheckList = "menu-ativo";
            return View("ItemCheckList", vwItemCheckList);
        }

        #endregion

        #region "Check-Lists"

        public ActionResult ListaCheckList()
        {
            ListaCheckListView vwListaCheckLists = new ListaCheckListView();
            try
            {

                List<CheckListTO> lstRetorno = new List<CheckListTO>();

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                string url = string.Format("api/CheckList/items");

                vwListaCheckLists.ListaCheckList = base.Listar<CheckListTO>(accessToken, _urlBase, url);

            }
            catch (Exception ex)
            {
                vwListaCheckLists.erro = "Ocorreu um erro ao listar check-lists. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuCheckList = "menu-ativo";
            return View("ListaCheckList", vwListaCheckLists);
        }

        public ActionResult CheckList(string id_checklist)
        {
            CheckListView vwCheckList = new CheckListView();
            CheckListTO objCheckList = new CheckListTO();
            ItemCheckListTO objItemCheckList = new ItemCheckListTO();
            List<ItemCheckListTO> objLstItemCheckListSel = new List<ItemCheckListTO>();
            List<ItemCheckListTO> objLstItemCheckList = new List<ItemCheckListTO>();
            ChecklistItemChecklistConsultaTO objChecklistItemChecklistConsulta = new ChecklistItemChecklistConsultaTO();
            List<ChecklistItemChecklistConsultaTO> objLstChecklistItemChecklistConsulta = new List<ChecklistItemChecklistConsultaTO>();
            List<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO> ObjLstChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultas = new List<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO>();
            try
            {
                // ****************************** CARGA DAS COMBOS ****************************************************/
                    vwCheckList.ListaTipoAcomodacao = base.ListarTipoAcomodacao(Session["id_EmpresaSel"].ToString());
                    vwCheckList.ListaTipoAtividadeAcomodacao = base.ListarTipoAtividadeAcomodacao();
                    vwCheckList.ListaTipoSituacaoAcomodacao= base.ListarTipoSituacaoAcomodacao();
                    vwCheckList.ListaItemCheckList = base.ListarItemCheckList();
                // ****************************************************************************************************/


                if (string.IsNullOrWhiteSpace(id_checklist))
                {
                    vwCheckList.CheckList = new CheckListTO();
                }
                else
                {

                    List<CheckListTO> lstRetorno = new List<CheckListTO>();

                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                    string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                    string url = string.Format("api/CheckList/items/checklist/{0}", id_checklist);

                    //base.Consultar<CheckListTO>(accessToken, _urlBase, url, ref objCheckList);
                    objLstChecklistItemChecklistConsulta = Listar<ChecklistItemChecklistConsultaTO>(accessToken, _urlBase, url);

                    foreach (ChecklistItemChecklistConsultaTO item in objLstChecklistItemChecklistConsulta)
                    {
                        objItemCheckList = new ItemCheckListTO();
                        if (!string.IsNullOrWhiteSpace(item.nome_ItemChecklist))
                        {
                            objItemCheckList.id_ItemChecklist = item.id_ItemChecklist;
                            objItemCheckList.nome_ItemChecklist = item.nome_ItemChecklist;
                            vwCheckList.ItensCheckListSel += item.id_ItemChecklist.ToString() + "#";
                            objLstItemCheckListSel.Add(objItemCheckList);
                        }
                    }

                    vwCheckList.ListaItemCheckListSel = objLstItemCheckListSel;

                    objChecklistItemChecklistConsulta = objLstChecklistItemChecklistConsulta.FirstOrDefault();

                    objCheckList.id_Checklist = objChecklistItemChecklistConsulta.id_Checklist;
                    objCheckList.nome_Checklist = objChecklistItemChecklistConsulta.nome_Checklist;


                    vwCheckList.CheckList = objCheckList;

                    ObjLstChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultas = base.ListarChecklistTipoSituacaoTipoAtividadeTipoAcomodacao(Session["id_EmpresaSel"].ToString());

                    vwCheckList.ListaChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItem = ObjLstChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultas.Where(m=>m.id_Checklist==id_checklist).ToList();

                }

            }
            catch (Exception ex)
            {
                vwCheckList.erro = "Ocorreu um erro ao consultar o setor. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuCheckList = "menu-ativo";
            return View("CheckList", vwCheckList);
        }

        public ActionResult ExcluirCheckList(string id_checklist)
        {
            ListaCheckListView vwListaCheckList = new ListaCheckListView();
            List<CheckListTO> objLstCheckList = new List<CheckListTO>();
            try
            {

                if (id_checklist == null)
                {
                    throw new Exception("Identificador do check-list é inválido.");
                }
                else
                {

                    List<CheckListTO> lstRetorno = new List<CheckListTO>();

                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                    string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                    string url = string.Format("api/CheckList/items?id_Empresa={0}&id_CheckList={1}", id_checklist);

                    if (!Excluir(accessToken, _urlBase, url))
                    {
                        throw new Exception("Erro ao tentar excluir setor.");
                    }


                    url = string.Format("api/CheckList/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                    objLstCheckList = Listar<CheckListTO>(accessToken, _urlBase, url);

                    vwListaCheckList.ListaCheckList = objLstCheckList;

                    vwListaCheckList.mensagem = "CheckList excluída com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwListaCheckList.erro = "Erro ao tentar excluir setor. Erro:" + ex.Message;
            }
            ViewBag.MenuCheckList = "menu-ativo";
            return View("ListaCheckList", vwListaCheckList);
        }

        public ActionResult SalvarCheckList(CheckListView model)
        {
            CheckListView vwCheckList = new CheckListView();
            CheckListTO objCheckList = new CheckListTO();
            ChecklistItemChecklistTO objChecklistItemChecklist = new ChecklistItemChecklistTO();
            List<CheckListTO> objLstCheckList = new List<CheckListTO>();
            ItemCheckListTO objItemCheckList = new ItemCheckListTO();
            List<ItemCheckListTO> objLstItemCheckListSel = new List<ItemCheckListTO>();
            List<ItemCheckListTO> objLstItemCheckList = new List<ItemCheckListTO>();
            ChecklistItemChecklistConsultaTO objChecklistItemChecklistConsulta = new ChecklistItemChecklistConsultaTO();

            List<ChecklistItemChecklistConsultaTO> objLstChecklistItemChecklistConsulta = new List<ChecklistItemChecklistConsultaTO>();
            List<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO> ObjLstChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultas = new List<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO>();
            string id_CheckList = "";
            try
            {

                // ****************************** CARGA DAS COMBOS ****************************************************/
                vwCheckList.ListaTipoAcomodacao = base.ListarTipoAcomodacao(Session["id_EmpresaSel"].ToString());
                vwCheckList.ListaTipoAtividadeAcomodacao = base.ListarTipoAtividadeAcomodacao();
                vwCheckList.ListaTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();
                vwCheckList.ListaItemCheckList = base.ListarItemCheckList();
                // ****************************************************************************************************/

                if (!ModelState.IsValid)
                {
                    return View("CheckList", model);
                }
                else
                {
                    ModelState.Clear();

                    List<CheckListTO> lstRetorno = new List<CheckListTO>();

                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                    string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                    //************************ SALVAR O CHECK-LIST ****************************************/
                    string url = string.Format("api/CheckList/items");

                    if (string.IsNullOrWhiteSpace(model.CheckList.id_Checklist))
                    {
                        model.CheckList.id_Checklist = "0";
                    }

                    base.Salvar<CheckListTO>(accessToken, _urlBase, url, model.CheckList, ref id_CheckList);
                    //**************************************************************************************/

                    if (id_CheckList=="")
                    {
                        id_CheckList = model.CheckList.id_Checklist;
                    }

                    url = string.Format("api/CheckList/items/checklist/{0}", id_CheckList);

                    //base.Consultar<CheckListTO>(accessToken, _urlBase, url, ref objCheckList);
                    objLstChecklistItemChecklistConsulta = Listar<ChecklistItemChecklistConsultaTO>(accessToken, _urlBase, url);

                    //************************ SALVAR OS ITENS DO CHECK-LIST ****************************************/

                    foreach (string item in model.ItensCheckListSel.Split('#'))
                    {
                        string id_Relacionamento = "";
                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            objChecklistItemChecklist = new ChecklistItemChecklistTO();
                            objChecklistItemChecklist.id_Checklist = id_CheckList;
                            objChecklistItemChecklist.id_ItemChecklist = item;

                            objChecklistItemChecklistConsulta = new ChecklistItemChecklistConsultaTO();
                            objChecklistItemChecklistConsulta = objLstChecklistItemChecklistConsulta.Where(m => m.id_Checklist == id_CheckList && m.id_ItemChecklist == item).FirstOrDefault();

                            if (objChecklistItemChecklistConsulta == null)
                            {
                                url = string.Format("api/ChecklistItemChecklist/items?Id_Checklist={0}&Id_ItemChecklist={1}", id_CheckList, item);
                                try
                                {
                                    base.Excluir(accessToken, _urlBase, url);
                                }
                                catch (Exception)
                                {
                                    id_Relacionamento = "";
                                }
                                finally
                                {
                                    url = string.Format("api/ChecklistItemChecklist/items");
                                    base.Salvar<ChecklistItemChecklistTO>(accessToken, _urlBase, url, objChecklistItemChecklist, ref id_Relacionamento);
                                }
                            }
                        }
                    }
                    //***********************************************************************************************/

                    ////************************ SALVAR OS RELACIONAMENTOS ATIVIDADES/SITUACAO ****************************************/
                    //url = string.Format("api/ChecklistTipoSituacaoTipoAtividadeTipoAcomodacao/items");



                    ////***************************************************************************************************************/


                    url = string.Format("api/CheckList/items/checklist/{0}", id_CheckList);

                    //base.Consultar<CheckListTO>(accessToken, _urlBase, url, ref objCheckList);
                    objLstChecklistItemChecklistConsulta = Listar<ChecklistItemChecklistConsultaTO>(accessToken, _urlBase, url);

                    foreach (ChecklistItemChecklistConsultaTO item in objLstChecklistItemChecklistConsulta)
                    {
                        objItemCheckList = new ItemCheckListTO();
                        objItemCheckList.id_ItemChecklist = item.id_ItemChecklist;
                        objItemCheckList.nome_ItemChecklist = item.nome_ItemChecklist;
                        vwCheckList.ItensCheckListSel += item.id_ItemChecklist.ToString() + "#";
                        objLstItemCheckListSel.Add(objItemCheckList);
                    }

                    vwCheckList.ListaItemCheckListSel = objLstItemCheckListSel;

                    objChecklistItemChecklistConsulta = objLstChecklistItemChecklistConsulta.FirstOrDefault();

                    objCheckList.id_Checklist = objChecklistItemChecklistConsulta.id_Checklist;
                    objCheckList.nome_Checklist = objChecklistItemChecklistConsulta.nome_Checklist;


                    vwCheckList.CheckList = objCheckList;

                    ObjLstChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultas = base.ListarChecklistTipoSituacaoTipoAtividadeTipoAcomodacao(Session["id_EmpresaSel"].ToString());

                    vwCheckList.ListaChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItem = ObjLstChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultas.Where(m => m.id_Checklist == id_CheckList).ToList();

                    vwCheckList.mensagem = "CheckList salvo com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwCheckList.erro = "Erro ao salvar setor. Erro:" + ex.Message;
                //throw ex;
            }
            ViewBag.MenuCheckList = "menu-ativo";
            return View("CheckList", vwCheckList);
        }


        public ActionResult AdicionaRelacionamentoChecklistGrupo(string id_checklist, string pTipoacomodacao, string pTiposituacaoacomodacao, string pTipoatividadeacomodacao)
        {
            CheckListView vwCheckList = new CheckListView();
            CheckListTO objCheckList = new CheckListTO();
            ChecklistItemChecklistTO objChecklistItemChecklist = new ChecklistItemChecklistTO();
            List<CheckListTO> objLstCheckList = new List<CheckListTO>();
            ItemCheckListTO objItemCheckList = new ItemCheckListTO();
            List<ItemCheckListTO> objLstItemCheckListSel = new List<ItemCheckListTO>();
            List<ItemCheckListTO> objLstItemCheckList = new List<ItemCheckListTO>();
            ChecklistItemChecklistConsultaTO objChecklistItemChecklistConsulta = new ChecklistItemChecklistConsultaTO();
            List<ChecklistItemChecklistConsultaTO> objLstChecklistItemChecklistConsulta = new List<ChecklistItemChecklistConsultaTO>();
            List<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO> ObjLstChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultas = new List<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO>();
            string[] tiposAcomocadacoes;
            string[] tiposSituacoes;
            string[] tiposAtividades;
            try
            {

                // ****************************** CARGA DAS COMBOS **************************************************** /
                vwCheckList.ListaTipoAcomodacao = base.ListarTipoAcomodacao(Session["id_EmpresaSel"].ToString());
                vwCheckList.ListaTipoAtividadeAcomodacao = base.ListarTipoAtividadeAcomodacao();
                vwCheckList.ListaTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();
                vwCheckList.ListaItemCheckList = base.ListarItemCheckList();
                // **************************************************************************************************** /
                // *******************************CONSULTA DADOS DO CHECKLIST ***************************************** /
                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                string url = string.Format("api/CheckList/items/checklist/{0}", id_checklist);

                objLstChecklistItemChecklistConsulta = Listar<ChecklistItemChecklistConsultaTO>(accessToken, _urlBase, url);

                foreach (ChecklistItemChecklistConsultaTO item in objLstChecklistItemChecklistConsulta)
                {
                    objItemCheckList = new ItemCheckListTO();
                    objItemCheckList.id_ItemChecklist = item.id_ItemChecklist;
                    objItemCheckList.nome_ItemChecklist = item.nome_ItemChecklist;
                    vwCheckList.ItensCheckListSel += item.id_ItemChecklist.ToString() + "#";
                    objLstItemCheckListSel.Add(objItemCheckList);
                }

                vwCheckList.ListaItemCheckListSel = objLstItemCheckListSel;

                objChecklistItemChecklistConsulta = objLstChecklistItemChecklistConsulta.FirstOrDefault();

                objCheckList.id_Checklist = objChecklistItemChecklistConsulta.id_Checklist;
                objCheckList.nome_Checklist = objChecklistItemChecklistConsulta.nome_Checklist;

                vwCheckList.CheckList = objCheckList;
                // **************************************************************************************************** /


                if (string.IsNullOrWhiteSpace(id_checklist))
                {
                    return View("CheckList", vwCheckList);
                }
                else
                {
                    ModelState.Clear();

                    //************************ SALVAR O RELACIONADAMENTO ****************************************/
                    tiposAcomocadacoes = pTipoacomodacao.Split(',');
                    tiposSituacoes = pTiposituacaoacomodacao.Split(',');
                    tiposAtividades = pTipoatividadeacomodacao.Split(',');

                    foreach (string itemAco in tiposAcomocadacoes)
                    {
                        foreach (string itemSit in tiposSituacoes)
                        {
                            foreach (string itemAtv in tiposAtividades)
                            {
                                if (!string.IsNullOrWhiteSpace(itemAco) && !string.IsNullOrWhiteSpace(itemSit) && !string.IsNullOrWhiteSpace(itemAtv)) {
                                    if (AdicionaRelacionamentoChecklistTipos(id_checklist, itemAco, itemSit, itemAtv))
                                    {

                                    }
                                }
                            }
                        }

                    }

                    //ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO objChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsulta = new ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO();
                    //string id_Checktstat = "";

                    //objChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsulta.id_CheckTSTAT = "0";
                    //objChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsulta.id_Checklist = id_checklist;
                    //objChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsulta.id_TipoAcomodacao = id_tipoacomodacao;
                    //objChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsulta.id_TipoSituacaoAcomodacao = id_tiposituacaoacomodacao;
                    //objChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsulta.id_TipoAtividadeAcomodacao = id_tipoatividadeacomodacao;
                    //objChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsulta.id_Empresa = Session["id_EmpresaSel"].ToString();

                    //url = string.Format("api/ChecklistTipoSituacaoTipoAtividadeTipoAcomodacao/items");

                    //base.Salvar<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO>(accessToken, _urlBase, url, objChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsulta, ref id_Checktstat);

                    
                    //**************************************************************************************/

                    ObjLstChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultas = base.ListarChecklistTipoSituacaoTipoAtividadeTipoAcomodacao(Session["id_EmpresaSel"].ToString());

                    vwCheckList.ListaChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItem = ObjLstChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultas.Where(m => m.id_Checklist == id_checklist).ToList();

                    vwCheckList.mensagem = "CheckList - Relacionamento salvo com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwCheckList.erro = "Erro ao salvar relacionamento do Check-List. Erro:" + ex.Message;
                //throw ex;
            }
            ViewBag.MenuCheckList = "menu-ativo";
            return View("CheckList", vwCheckList);
        }

        private bool AdicionaRelacionamentoChecklistTipos(string id_checklist, string id_tipoacomodacao, string id_tiposituacaoacomodacao, string id_tipoatividadeacomodacao)
        {
            List<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO> ObjLstChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultas = new List<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO>();
            try
            {
                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                if (string.IsNullOrWhiteSpace(id_checklist))
                {
                    return false;
                }
                else
                {
                    ModelState.Clear();

                    //************************ SALVAR O RELACIONADAMENTO ****************************************/

                    ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO objChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsulta = new ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO();
                    string id_Checktstat = "";

                    objChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsulta.id_CheckTSTAT = "0";
                    objChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsulta.id_Checklist = id_checklist;
                    objChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsulta.id_TipoAcomodacao = id_tipoacomodacao;
                    objChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsulta.id_TipoSituacaoAcomodacao = id_tiposituacaoacomodacao;
                    objChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsulta.id_TipoAtividadeAcomodacao = id_tipoatividadeacomodacao;
                    objChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsulta.id_Empresa = Session["id_EmpresaSel"].ToString();

                    string url = string.Format("api/ChecklistTipoSituacaoTipoAtividadeTipoAcomodacao/items");
                   
                    base.Salvar<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO>(accessToken, _urlBase, url, objChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsulta, ref id_Checktstat);
                    //**************************************************************************************/

                }

            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
            return true;
        }

        public ActionResult ExcluirRelacionamentoChecklistTipos(string id_checklist, string Id_CheckTSTAT)
        {
            CheckListView vwCheckList = new CheckListView();
            CheckListTO objCheckList = new CheckListTO();
            ChecklistItemChecklistTO objChecklistItemChecklist = new ChecklistItemChecklistTO();
            List<CheckListTO> objLstCheckList = new List<CheckListTO>();
            ItemCheckListTO objItemCheckList = new ItemCheckListTO();
            List<ItemCheckListTO> objLstItemCheckListSel = new List<ItemCheckListTO>();
            List<ItemCheckListTO> objLstItemCheckList = new List<ItemCheckListTO>();
            ChecklistItemChecklistConsultaTO objChecklistItemChecklistConsulta = new ChecklistItemChecklistConsultaTO();
            List<ChecklistItemChecklistConsultaTO> objLstChecklistItemChecklistConsulta = new List<ChecklistItemChecklistConsultaTO>();
            List<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO> ObjLstChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultas = new List<ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO>();
            try
            {

                // ****************************** CARGA DAS COMBOS **************************************************** /
                vwCheckList.ListaTipoAcomodacao = base.ListarTipoAcomodacao(Session["id_EmpresaSel"].ToString());
                vwCheckList.ListaTipoAtividadeAcomodacao = base.ListarTipoAtividadeAcomodacao();
                vwCheckList.ListaTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();
                vwCheckList.ListaItemCheckList = base.ListarItemCheckList();
                // **************************************************************************************************** /
                // *******************************CONSULTA DADOS DO CHECKLIST ***************************************** /
                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                string url = string.Format("api/CheckList/items/checklist/{0}", id_checklist);

                objLstChecklistItemChecklistConsulta = Listar<ChecklistItemChecklistConsultaTO>(accessToken, _urlBase, url);

                foreach (ChecklistItemChecklistConsultaTO item in objLstChecklistItemChecklistConsulta)
                {
                    objItemCheckList = new ItemCheckListTO();
                    objItemCheckList.id_ItemChecklist = item.id_ItemChecklist;
                    objItemCheckList.nome_ItemChecklist = item.nome_ItemChecklist;
                    vwCheckList.ItensCheckListSel += item.id_ItemChecklist.ToString() + "#";
                    objLstItemCheckListSel.Add(objItemCheckList);
                }

                vwCheckList.ListaItemCheckListSel = objLstItemCheckListSel;

                objChecklistItemChecklistConsulta = objLstChecklistItemChecklistConsulta.FirstOrDefault();

                objCheckList.id_Checklist = objChecklistItemChecklistConsulta.id_Checklist;
                objCheckList.nome_Checklist = objChecklistItemChecklistConsulta.nome_Checklist;

                vwCheckList.CheckList = objCheckList;
                // **************************************************************************************************** /


                ObjLstChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultas = base.ListarChecklistTipoSituacaoTipoAtividadeTipoAcomodacao(Session["id_EmpresaSel"].ToString());

                vwCheckList.ListaChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItem = ObjLstChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultas.Where(m => m.id_Checklist == id_checklist).ToList();


                if (string.IsNullOrWhiteSpace(id_checklist))
                {
                    return View("CheckList", vwCheckList);
                }
                else
                {
                    ModelState.Clear();

                    //************************ Excluir O RELACIONADAMENTO ****************************************/
                   ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO objChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsulta = new ChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultaTO();

                    url = string.Format("api/ChecklistTipoSituacaoTipoAtividadeTipoAcomodacao/items?Id_CheckTSTAT={0}", Id_CheckTSTAT);

                    base.Excluir(accessToken, _urlBase, url);
                    //**************************************************************************************/

                    ObjLstChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultas = base.ListarChecklistTipoSituacaoTipoAtividadeTipoAcomodacao(Session["id_EmpresaSel"].ToString());

                    vwCheckList.ListaChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItem = ObjLstChecklistTipoSituacaoTipoAtividadeTipoAcomodacaoItemConsultas.Where(m => m.id_Checklist == id_checklist).ToList();

                    vwCheckList.mensagem = "CheckList - Relacionamento excluído com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwCheckList.erro = "Erro ao salvar relacionamento do check-list. Erro:" + ex.Message;
                //throw ex;
            }
            ViewBag.MenuCheckList = "menu-ativo";
            return View("CheckList", vwCheckList);
        }

        #endregion

        #region "SLAs"

        //public ActionResult ListarSLA()
        //{
        //    ListaSLAView vwSLA = new ListaSLAView();
        //    ListaSlaTO objSLA = new ListaSlaTO();
        //    SLAEmpresaTO objSLAEmpresa = new SLAEmpresaTO();
        //    List<SLAEmpresaTO> ListaSLAEmpresa = new List<SLAEmpresaTO>();
        //    try
        //    {

        //        // ****************************** CARGA DAS COMBOS ****************************************************/
        //        vwSLA.ListaTipoAcaoAcomodacao = base.ListarTipoAcaoAcomodacao();
        //        vwSLA.ListaTipoAtividadeAcomodacao = base.ListarTipoAtividadeAcomodacao();
        //        vwSLA.ListaTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();
        //        vwSLA.ListaTipoAcomodacao = base.ListarTipoAcomodacao(Session["id_EmpresaSel"].ToString());
        //        // ****************************************************************************************************/
                
        //    }
        //    catch (Exception ex)
        //    {
        //        vwSLA.erro = "Ocorreu um erro ao listar os SLAs. Detalhe: " + ex.Message;
        //        //throw ex;
        //    }
        //    ViewBag.MenuSLA = "menu-ativo";
        //    return View("ListaSLA", vwSLA);
        //}

        public ActionResult ListarSLA(ListaSLAView model)
        {
            ListaSLAView vwSLA = new ListaSLAView();
            ListaSlaTO objSLA = new ListaSlaTO();
            SLAEmpresaTO objSLAEmpresa = new SLAEmpresaTO();
            List<SLAEmpresaTO> ListaSLAEmpresa = new List<SLAEmpresaTO>();
            try
            {
                
                // ****************************** CARGA DAS COMBOS ****************************************************/
                vwSLA.ListaTipoAcaoAcomodacao = base.ListarTipoAcaoAcomodacao();
                vwSLA.ListaTipoAtividadeAcomodacao = base.ListarTipoAtividadeAcomodacao();
                vwSLA.ListaTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();
                vwSLA.ListaTipoAcomodacao = base.ListarTipoAcomodacao(Session["id_EmpresaSel"].ToString());
                // ****************************************************************************************************/


                if (!ModelState.IsValid)
                {
                    model.ListaTipoAcaoAcomodacao = vwSLA.ListaTipoAcaoAcomodacao;
                    model.ListaTipoAtividadeAcomodacao = vwSLA.ListaTipoAtividadeAcomodacao;
                    model.ListaTipoSituacaoAcomodacao = vwSLA.ListaTipoSituacaoAcomodacao;
                    model.ListaTipoAcomodacao = vwSLA.ListaTipoAcomodacao;
                    model.ListaSLAEmpresa = vwSLA.ListaSLAEmpresa;
                    model.dropDisabled = "";

                    return View("ListaSla", model);
                }
                if (!string.IsNullOrWhiteSpace(model.ListaSla.id_TipoSituacaoAcomodacao) && !string.IsNullOrWhiteSpace(model.ListaSla.id_TipoAtividadeAcomodacao))
                {
                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                    string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                    string url = string.Format("api/SLA/items/listar/empresa/{0}/tiposituacao/{1}/tipoatividade/{2}/tipoAcao/{3}/tipoacomodacao/{4}", Session["id_EmpresaSel"].ToString(), model.ListaSla.id_TipoSituacaoAcomodacao, model.ListaSla.id_TipoAtividadeAcomodacao, model.ListaSla.id_TipoAcaoAcomodacao, model.ListaSla.id_TipoAcomodacao);

                    ListaSLAEmpresa = Listar<SLAEmpresaTO>(accessToken, _urlBase, url);

                    // remove os excluídos.
                    ListaSLAEmpresa = ListaSLAEmpresa.Where(m => m.cod_enabled == "S").ToList();

                    vwSLA.ListaSLAEmpresa = ListaSLAEmpresa;

                    objSLA.id_TipoAcaoAcomodacao = model.ListaSla.id_TipoAcaoAcomodacao;
                    objSLA.id_TipoAtividadeAcomodacao = model.ListaSla.id_TipoAtividadeAcomodacao;
                    objSLA.id_TipoSituacaoAcomodacao = model.ListaSla.id_TipoSituacaoAcomodacao;
                    objSLA.id_TipoAcomodacao = model.ListaSla.id_TipoAcomodacao;
                    
                }

                objSLA.id_Empresa = Session["id_EmpresaSel"].ToString();

                vwSLA.dropDisabled = "";
                vwSLA.ListaSla = objSLA;
                               

            }
            catch (Exception ex)
            {
                vwSLA.erro = "Ocorreu um erro ao listar os SLAs. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuSLA = "menu-ativo";
            return View("ListaSLA", vwSLA);
        }
        public ActionResult VoltarListaSLA(string idTipoSituacaoAcomodacao, string idTipoAtividadeAcomodacao)
        {
            ListaSLAView vwSLA = new ListaSLAView();
            ListaSlaTO objSLA = new ListaSlaTO();
            SLAEmpresaTO objSLAEmpresa = new SLAEmpresaTO();
            List<SLAEmpresaTO> ListaSLAEmpresa = new List<SLAEmpresaTO>();
            try
            {

                // ****************************** CARGA DAS COMBOS ****************************************************/
                vwSLA.ListaTipoAcaoAcomodacao = base.ListarTipoAcaoAcomodacao();
                vwSLA.ListaTipoAtividadeAcomodacao = base.ListarTipoAtividadeAcomodacao();
                vwSLA.ListaTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();
                vwSLA.ListaTipoAcomodacao = base.ListarTipoAcomodacao(Session["id_EmpresaSel"].ToString());
                // ****************************************************************************************************/

                if (!string.IsNullOrWhiteSpace(idTipoSituacaoAcomodacao) && !string.IsNullOrWhiteSpace(idTipoAtividadeAcomodacao))
                {
                    Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                    string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                    string url = string.Format("api/SLA/items/listar/empresa/{0}/tiposituacao/{1}/tipoatividade/{2}/tipoAcao/{3}/tipoacomodacao/{4}", Session["id_EmpresaSel"].ToString(), idTipoSituacaoAcomodacao, idTipoAtividadeAcomodacao,0, 0);

                    ListaSLAEmpresa = Listar<SLAEmpresaTO>(accessToken, _urlBase, url);

                    // remove os excluídos.
                    ListaSLAEmpresa = ListaSLAEmpresa.Where(m => m.cod_enabled == "S").ToList();

                    vwSLA.ListaSLAEmpresa = ListaSLAEmpresa;
                    
                    objSLA.id_TipoAtividadeAcomodacao = idTipoAtividadeAcomodacao;
                    objSLA.id_TipoSituacaoAcomodacao =idTipoSituacaoAcomodacao;                   

                }

                objSLA.id_Empresa = Session["id_EmpresaSel"].ToString();

                vwSLA.dropDisabled = "";
                vwSLA.ListaSla = objSLA;


            }
            catch (Exception ex)
            {
                vwSLA.erro = "Ocorreu um erro ao listar os SLAs. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuSLA = "menu-ativo";
            return View("ListaSLA", vwSLA);
        }
        public ActionResult SLA(string id_sla)
        {
            SLAView vwSLA = new SLAView();
            SlaTO objSLA = new SlaTO();
            SLAEmpresaTO objSLAEmpresa = new SLAEmpresaTO();
            List<SLAEmpresaTO> ListaSLAEmpresa = new List<SLAEmpresaTO>();
            try
            {
                // ****************************** CARGA DAS COMBOS ****************************************************/
                vwSLA.ListaTipoAcaoAcomodacao = base.ListarTipoAcaoAcomodacao();
                vwSLA.ListaTipoAtividadeAcomodacao = base.ListarTipoAtividadeAcomodacao();
                vwSLA.ListaTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();
                vwSLA.ListaTipoAcomodacao = base.ListarTipoAcomodacao(Session["id_EmpresaSel"].ToString());
                // ****************************************************************************************************/

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                string url = string.Format("api/SLA/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                //ListaSLAEmpresa = Listar<SLAEmpresaTO>(accessToken, _urlBase, url);

                //vwSLA.ListaSLAEmpresa = ListaSLAEmpresa;

                if (string.IsNullOrWhiteSpace(id_sla))
                {
                    vwSLA.Sla.id_Empresa = Session["id_EmpresaSel"].ToString();
                    vwSLA.Sla = new SlaTO();
                }
                else
                {

                    accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                    _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                    url = string.Format("api/SLA/items/empresa/{0}/id/{1}", Session["id_EmpresaSel"].ToString(), id_sla);

                    ListaSLAEmpresa = Listar<SLAEmpresaTO>(accessToken, _urlBase, url);

                    objSLAEmpresa = ListaSLAEmpresa.Where(m => m.id_SLA == id_sla).ToList().FirstOrDefault();

                    objSLA.id_SLA = objSLAEmpresa.id_SLA;
                    objSLA.id_TipoAcaoAcomodacao = objSLAEmpresa.id_TipoAcaoAcomodacao;
                    objSLA.id_TipoAtividadeAcomodacao = objSLAEmpresa.id_TipoAtividadeAcomodacao;
                    objSLA.id_TipoSituacaoAcomodacao = objSLAEmpresa.id_TipoSituacaoAcomodacao;
                    objSLA.id_TipoAcomodacao = objSLAEmpresa.id_TipoAcomodacao;
                    objSLA.id_Empresa = objSLAEmpresa.id_Empresa;
                    objSLA.versao_SLA = objSLAEmpresa.versao_SLA;
                    objSLA.tempo_Minutos = objSLAEmpresa.tempo_Minutos;

                    vwSLA.dropDisabled = " disabled ";
                    vwSLA.Sla = objSLA;
                    
                }

            }
            catch (Exception ex)
            {
                vwSLA.erro = "Ocorreu um erro ao consultar o SLA. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuSLA = "menu-ativo";
            return View("SLA", vwSLA);
        }



        public ActionResult SalvarSLA(SLAView model)
        {
            SLAView vwSLA = new SLAView();
            SlaTO objSLA = new SlaTO();
            SLAEmpresaTO objSLAEmpresa = new SLAEmpresaTO();
            List<SLAEmpresaTO> ListaSLAEmpresa = new List<SLAEmpresaTO>();
            string id_Sla = "";
            try
            {

                vwSLA.ListaTipoAcaoAcomodacao = base.ListarTipoAcaoAcomodacao();
                vwSLA.ListaTipoAtividadeAcomodacao = base.ListarTipoAtividadeAcomodacao();
                vwSLA.ListaTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();
                vwSLA.ListaTipoAcomodacao = base.ListarTipoAcomodacao(Session["id_EmpresaSel"].ToString());


                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                string url = "";

                //string url = string.Format("api/SLA/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                //ListaSLAEmpresa = Listar<SLAEmpresaTO>(accessToken, _urlBase, url);

                //vwSLA.ListaSLAEmpresa = ListaSLAEmpresa;

                vwSLA.dropDisabled = "";

                vwSLA.Sla = model.Sla;

                if (!ModelState.IsValid)
                {
                    model.ListaTipoAcaoAcomodacao =  vwSLA.ListaTipoAcaoAcomodacao;
                    model.ListaTipoAtividadeAcomodacao = vwSLA.ListaTipoAtividadeAcomodacao;
                    model.ListaTipoSituacaoAcomodacao = vwSLA.ListaTipoSituacaoAcomodacao;
                    model.ListaTipoAcomodacao = vwSLA.ListaTipoAcomodacao;
                    //model.ListaSLAEmpresa = vwSLA.ListaSLAEmpresa;
                    model.dropDisabled = "";
                   
                    return View("Sla", model);
                }
                else
                {
                    // ****************************** CARGA DAS COMBOS ****************************************************/
                    //vwSLA.ListaTipoAcaoAcomodacao = base.ListarTipoAcaoAcomodacao();
                    //vwSLA.ListaTipoAtividadeAcomodacao = base.ListarTipoAtividadeAcomodacao();
                    //vwSLA.ListaTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();
                    // ****************************************************************************************************/
                                        
                    

                    if (string.IsNullOrWhiteSpace(model.Sla.id_SLA))
                    {
                        model.Sla.id_SLA = "0";
                        url = string.Format("api/SLA/items/empresa/{0}/tiposituacao/{1}/tipoatividade/{2}/tipoacao/{3}/tipoacomodacao/{4}", Session["id_EmpresaSel"].ToString(), model.Sla.id_TipoSituacaoAcomodacao, model.Sla.id_TipoAtividadeAcomodacao, model.Sla.id_TipoAcaoAcomodacao, model.Sla.id_TipoAcomodacao);
                        ListaSLAEmpresa = Listar<SLAEmpresaTO>(accessToken, _urlBase, url);
                        if (ListaSLAEmpresa.Count > 0)
                        {
                            objSLAEmpresa = ListaSLAEmpresa[0];
                            objSLA.id_SLA = objSLAEmpresa.id_SLA;
                            objSLA.id_Empresa = objSLAEmpresa.id_Empresa;
                            objSLA.id_TipoSituacaoAcomodacao = objSLAEmpresa.id_TipoSituacaoAcomodacao;
                            objSLA.id_TipoAtividadeAcomodacao = objSLAEmpresa.id_TipoAtividadeAcomodacao;
                            objSLA.id_TipoAcaoAcomodacao = objSLAEmpresa.id_TipoAcaoAcomodacao;
                            objSLA.id_TipoAcomodacao = objSLAEmpresa.id_TipoAcomodacao;
                            objSLA.tempo_Minutos = model.Sla.tempo_Minutos;
                            objSLA.versao_SLA = (int.Parse(objSLAEmpresa.versao_SLA) + 1).ToString();
                            objSLA.cod_enabled = "S";
                            model.Sla = objSLA;
                        }
                        else
                        {
                            //model.Sla.versao_SLA = (int.Parse(model.Sla.versao_SLA) + 1).ToString();
                            model.Sla.cod_enabled = "S";
                        }

                    }
                    else
                    {
                        model.Sla.versao_SLA = (model.Sla.versao_SLA + 1).ToString();
                        model.Sla.cod_enabled = "S";
                    }


                    url = string.Format("api/SLA/items");

                    if (string.IsNullOrWhiteSpace(model.Sla.id_Empresa))
                    {
                        model.Sla.id_Empresa = Session["id_EmpresaSel"].ToString();
                    }
                    base.Salvar<SlaTO>(accessToken, _urlBase, url, model.Sla, ref id_Sla);

                    ModelState.Clear();

                    //url = string.Format("api/SLA/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                    //ListaSLAEmpresa = Listar<SLAEmpresaTO>(accessToken, _urlBase, url);

                    //vwSLA.ListaSLAEmpresa = ListaSLAEmpresa;

                    vwSLA.dropDisabled = "";

                    //objSLA = new SlaTO();

                    vwSLA.Sla = model.Sla;

                    vwSLA.mensagem = "O SLA foi salvo com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwSLA.erro = "Ocorreu um erro ao salvar os dados do SLA. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuSLA = "menu-ativo";
            return View("SLA", vwSLA);
        }

        public ActionResult ExcluirSLA(string id_sla)
        {
            ListaSLAView vwSLA = new ListaSLAView();
            ListaSlaTO objListaSLA = new ListaSlaTO();
            SlaTO objSLA = new  SlaTO();
            SLAEmpresaTO objSLAEmpresa = new SLAEmpresaTO();
            List<SLAEmpresaTO> ListaSLAEmpresa = new List<SLAEmpresaTO>();
            try
            {
                // ****************************** CARGA DAS COMBOS ****************************************************/
                vwSLA.ListaTipoAcaoAcomodacao = base.ListarTipoAcaoAcomodacao();
                vwSLA.ListaTipoAtividadeAcomodacao = base.ListarTipoAtividadeAcomodacao();
                vwSLA.ListaTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();
                vwSLA.ListaTipoAcomodacao = base.ListarTipoAcomodacao(Session["id_EmpresaSel"].ToString());
                // ****************************************************************************************************/

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                string url = "";

                if (string.IsNullOrWhiteSpace(id_sla))
                {
                    //url = string.Format("api/SLA/items/empresa/{0}", Session["id_EmpresaSel"].ToString());
                    //ListaSLAEmpresa = Listar<SLAEmpresaTO>(accessToken, _urlBase, url);
                    //vwSLA.ListaSLAEmpresa = ListaSLAEmpresa;

                    vwSLA.erro = "O identificador do SLA é inválido.";
                    return View("ListaSla", vwSLA);
                }
                else
                {
                    // ****************************** CARGA DAS COMBOS ****************************************************/
                    //vwSLA.ListaTipoAcaoAcomodacao = base.ListarTipoAcaoAcomodacao();
                    //vwSLA.ListaTipoAtividadeAcomodacao = base.ListarTipoAtividadeAcomodacao();
                    //vwSLA.ListaTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();
                    // ****************************************************************************************************/

                    //url = string.Format("api/SLA/items?id_SLA={0}", id_sla);

                    //base.Excluir(accessToken, _urlBase, url);
                    url = string.Format("api/SLA/items/empresa/{0}/id/{1}", Session["id_EmpresaSel"].ToString(), id_sla);
                    ListaSLAEmpresa = Listar<SLAEmpresaTO>(accessToken, _urlBase, url);

                    objSLAEmpresa = ListaSLAEmpresa.Where(m => m.id_SLA == id_sla).ToList().FirstOrDefault();

                    if (objSLAEmpresa!=null)
                    {
                        
                        objSLA.id_SLA = objSLAEmpresa.id_SLA;
                        objSLA.id_Empresa = objSLAEmpresa.id_Empresa;
                        objSLA.id_TipoSituacaoAcomodacao = objSLAEmpresa.id_TipoSituacaoAcomodacao;
                        objListaSLA.id_TipoSituacaoAcomodacao = objSLAEmpresa.id_TipoSituacaoAcomodacao;
                        objSLA.id_TipoAtividadeAcomodacao = objSLAEmpresa.id_TipoAtividadeAcomodacao;
                        objListaSLA.id_TipoAtividadeAcomodacao = objSLAEmpresa.id_TipoAtividadeAcomodacao;
                        objSLA.id_TipoAcaoAcomodacao = objSLAEmpresa.id_TipoAcaoAcomodacao;
                        objSLA.id_TipoAcomodacao = objSLAEmpresa.id_TipoAcomodacao;
                        objSLA.tempo_Minutos = objSLAEmpresa.tempo_Minutos;
                        objSLA.versao_SLA = (int.Parse(objSLAEmpresa.versao_SLA) + 1).ToString();
                        objSLA.cod_enabled = "N";
                        objListaSLA.cod_enabled = "N";

                    }
                    url = string.Format("api/SLA/items");

                    if (string.IsNullOrWhiteSpace(objSLA.id_Empresa))
                    {
                        objSLA.id_Empresa = Session["id_EmpresaSel"].ToString();
                    }
                    base.Salvar<SlaTO>(accessToken, _urlBase, url, objSLA, ref id_sla);

                    url = string.Format("api/SLA/items/listar/empresa/{0}/tiposituacao/{1}/tipoatividade/{2}/tipoAcao/{3}/tipoacomodacao/{4}", Session["id_EmpresaSel"].ToString(), objSLA.id_TipoSituacaoAcomodacao, objSLA.id_TipoAtividadeAcomodacao, 0, 0);

                    ListaSLAEmpresa = Listar<SLAEmpresaTO>(accessToken, _urlBase, url);

                    ListaSLAEmpresa = ListaSLAEmpresa.Where(m => m.cod_enabled == "S").ToList();

                    //url = string.Format("api/SLA/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                    //ListaSLAEmpresa = Listar<SLAEmpresaTO>(accessToken, _urlBase, url);

                    vwSLA.ListaSLAEmpresa = ListaSLAEmpresa;

                    vwSLA.dropDisabled = "";

                    vwSLA.ListaSla = objListaSLA;

                    vwSLA.mensagem = "O SLA foi excluído com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwSLA.erro = "Ocorreu um erro ao excluir o SLA. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuSLA = "menu-ativo";
            return View("ListaSLA", vwSLA);
        }

        #endregion

        #region "SLAs Sit"

        public ActionResult SLASituacao(string id_sla)
        {
            SLASituacaoView vwSLASituacao = new SLASituacaoView();
            SlaSituacaoTO objSLASituacao = new SlaSituacaoTO();
            SLASituacaoConsultaTO objSLASituacaoConsulta = new SLASituacaoConsultaTO();
            List<SLASituacaoConsultaTO> ListaSLASituacao = new List<SLASituacaoConsultaTO>();
            
            try
            {
                // ****************************** CARGA DAS COMBOS ****************************************************/
                vwSLASituacao.ListaTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();
                vwSLASituacao.ListaTipoAcomodacao = base.ListarTipoAcomodacao(Session["id_EmpresaSel"].ToString());
                // ****************************************************************************************************/

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                string url = string.Format("api/SLASituacao/items");

                ListaSLASituacao = Listar<SLASituacaoConsultaTO>(accessToken, _urlBase, url).Where(m => m.id_Empresa== Session["id_EmpresaSel"].ToString()).ToList();

                vwSLASituacao.ListaSLASituacao = ListaSLASituacao;

                if (string.IsNullOrWhiteSpace(id_sla))
                {
                    vwSLASituacao.SlaSituacao.id_Empresa = Session["id_EmpresaSel"].ToString();
                    vwSLASituacao.SlaSituacao = new SlaSituacaoTO();
                }
                else
                {

                    accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                    _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                    url = string.Format("api/SLASituacao/items");

                    objSLASituacaoConsulta = vwSLASituacao.ListaSLASituacao.Where(m => m.id_SLA == id_sla).ToList().FirstOrDefault();

                    objSLASituacao.id_SLA = objSLASituacaoConsulta.id_SLA;
                    objSLASituacao.id_TipoSituacaoAcomodacao = objSLASituacaoConsulta.id_TipoSituacaoAcomodacao;
                    objSLASituacao.id_TipoAcomodacao = objSLASituacaoConsulta.id_TipoAcomodacao;
                    objSLASituacao.id_Empresa = objSLASituacaoConsulta.id_Empresa;
                    objSLASituacao.versao_SLA = objSLASituacaoConsulta.versao_SLA;
                    objSLASituacao.tempo_Minutos = objSLASituacaoConsulta.tempo_Minutos;

                    vwSLASituacao.dropDisabled = " disabled ";
                    vwSLASituacao.SlaSituacao = objSLASituacao;

                }

            }
            catch (Exception ex)
            {
                vwSLASituacao.erro = "Ocorreu um erro ao consultar o SLASituacao Situação. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuSLASituacao = "menu-ativo";
            return View("SLASituacao", vwSLASituacao);
        }

        public ActionResult SalvarSLASituacao(SLASituacaoView model)
        {
            SLASituacaoView vwSLASituacao = new SLASituacaoView();
            SlaSituacaoTO objSLASituacao = new SlaSituacaoTO();
            SLASituacaoConsultaTO objSLASituacaoConsulta = new SLASituacaoConsultaTO();
            List<SLASituacaoConsultaTO> ListaSLASituacaoConsulta = new List<SLASituacaoConsultaTO>();
            string id_Sla = "";
            try
            {

                // ****************************** CARGA DAS COMBOS ****************************************************/
                vwSLASituacao.ListaTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();
                vwSLASituacao.ListaTipoAcomodacao = base.ListarTipoAcomodacao(Session["id_EmpresaSel"].ToString());
                // ****************************************************************************************************/


                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                string url = string.Format("api/SLASituacao/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                ListaSLASituacaoConsulta = Listar<SLASituacaoConsultaTO>(accessToken, _urlBase, url);

                vwSLASituacao.ListaSLASituacao = ListaSLASituacaoConsulta;

                vwSLASituacao.dropDisabled = "";

                vwSLASituacao.SlaSituacao = model.SlaSituacao;

                if (!ModelState.IsValid)
                {
                    model.ListaTipoSituacaoAcomodacao = vwSLASituacao.ListaTipoSituacaoAcomodacao;
                    model.ListaSLASituacao = vwSLASituacao.ListaSLASituacao;
                    model.ListaTipoAcomodacao = vwSLASituacao.ListaTipoAcomodacao;
                    model.dropDisabled = "";

                    return View("SlaSituacao", model);
                }
                else
                {
                    // ****************************** CARGA DAS COMBOS ****************************************************/
                    //vwSLASituacao.ListaTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();
                    // ****************************************************************************************************/

                    

                    if (string.IsNullOrWhiteSpace(model.SlaSituacao.id_SLA))
                    {
                        model.SlaSituacao.id_SLA= "0";
                        url = string.Format("api/SLASituacao/items/empresa/{0}/tiposituacao/{1}/tipoacomodacao/{2}", Session["id_EmpresaSel"].ToString(), model.SlaSituacao.id_TipoSituacaoAcomodacao,  model.SlaSituacao.id_TipoAcomodacao);
                        ListaSLASituacaoConsulta = Listar<SLASituacaoConsultaTO>(accessToken, _urlBase, url);
                        if (ListaSLASituacaoConsulta.Count > 0)
                        {
                            objSLASituacaoConsulta = ListaSLASituacaoConsulta[0];
                            objSLASituacao.id_SLA = objSLASituacaoConsulta.id_SLA;
                            objSLASituacao.id_Empresa = objSLASituacaoConsulta.id_Empresa;
                            objSLASituacao.id_TipoSituacaoAcomodacao = objSLASituacaoConsulta.id_TipoSituacaoAcomodacao;
                            objSLASituacao.id_TipoAcomodacao = objSLASituacaoConsulta.id_TipoAcomodacao;
                            objSLASituacao.tempo_Minutos = model.SlaSituacao.tempo_Minutos;
                            objSLASituacao.versao_SLA = (int.Parse(objSLASituacaoConsulta.versao_SLA) + 1).ToString();
                            objSLASituacao.cod_enabled = "S";
                            model.SlaSituacao = objSLASituacao;
                        }
                    }
                    else
                    {
                        objSLASituacao.cod_enabled = "S";
                        model.SlaSituacao.versao_SLA = (model.SlaSituacao.versao_SLA + 1).ToString();
                    }

                    url = string.Format("api/SLASituacao/items");

                    if (string.IsNullOrWhiteSpace(model.SlaSituacao.id_Empresa))
                    {
                        model.SlaSituacao.id_Empresa = Session["id_EmpresaSel"].ToString();
                    }
                    base.Salvar<SlaSituacaoTO>(accessToken, _urlBase, url, model.SlaSituacao, ref id_Sla);

                    ModelState.Clear();

                    url = string.Format("api/SLASituacao/items");

                    ListaSLASituacaoConsulta = Listar<SLASituacaoConsultaTO>(accessToken, _urlBase, url).Where(m => m.id_Empresa==Session["id_EmpresaSel"].ToString()).ToList();

                    vwSLASituacao.ListaSLASituacao = ListaSLASituacaoConsulta;

                    vwSLASituacao.dropDisabled = "";

                    objSLASituacao = new SlaSituacaoTO();

                    vwSLASituacao.SlaSituacao = objSLASituacao;

                    vwSLASituacao.mensagem = "O SLA da Situação foi salvo com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwSLASituacao.erro = "Ocorreu um erro ao salvar os dados do SLA da Situação. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuSLASituacao = "menu-ativo";
            return View("SLASituacao", vwSLASituacao);
        }

        public ActionResult ExcluirSLASituacao(string id_sla)
        {
            SLASituacaoView vwSLASituacao = new SLASituacaoView();
            SlaSituacaoTO objSLASituacao = new SlaSituacaoTO();
            SLASituacaoConsultaTO objSLASituacaoConsulta = new SLASituacaoConsultaTO();
            List<SLASituacaoConsultaTO> ListaSLASituacaoConsulta = new List<SLASituacaoConsultaTO>();
            try
            {
                // ****************************** CARGA DAS COMBOS ****************************************************/
                vwSLASituacao.ListaTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();
                vwSLASituacao.ListaTipoAcomodacao = base.ListarTipoAcomodacao(Session["id_EmpresaSel"].ToString());
                // ****************************************************************************************************/

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                string url = "";

                if (string.IsNullOrWhiteSpace(id_sla))
                {
                    url = string.Format("api/SLASituacao/items/empresa/{0}", Session["id_EmpresaSel"].ToString());
                    ListaSLASituacaoConsulta = Listar<SLASituacaoConsultaTO>(accessToken, _urlBase, url);
                    vwSLASituacao.ListaSLASituacao = ListaSLASituacaoConsulta;

                    vwSLASituacao.erro = "O identificador do SLA Situação é inválido.";
                    return View("Sla", vwSLASituacao);
                }
                else
                {
                    // ****************************** CARGA DAS COMBOS ****************************************************/
                    vwSLASituacao.ListaTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();
                    // ****************************************************************************************************/

                    url = string.Format("api/SLASituacao/items/empresa/{0}", Session["id_EmpresaSel"].ToString());

                    ListaSLASituacaoConsulta = Listar<SLASituacaoConsultaTO>(accessToken, _urlBase, url);

                    objSLASituacaoConsulta = ListaSLASituacaoConsulta.Where(m => m.id_SLA == id_sla).ToList().FirstOrDefault();

                    if (objSLASituacaoConsulta!=null)
                    {
                        objSLASituacaoConsulta = ListaSLASituacaoConsulta[0];
                        objSLASituacao.id_SLA = objSLASituacaoConsulta.id_SLA;
                        objSLASituacao.id_Empresa = objSLASituacaoConsulta.id_Empresa;
                        objSLASituacao.id_TipoSituacaoAcomodacao = objSLASituacaoConsulta.id_TipoSituacaoAcomodacao;
                        objSLASituacao.id_TipoAcomodacao = objSLASituacaoConsulta.id_TipoAcomodacao;
                        objSLASituacao.tempo_Minutos = objSLASituacaoConsulta.tempo_Minutos;
                        objSLASituacao.versao_SLA = (int.Parse(objSLASituacaoConsulta.versao_SLA) + 1).ToString();
                        objSLASituacao.cod_enabled = "N";
                    }
                    // url = string.Format("api/SLASituacao/items?id_SLA={0}", id_sla);
                    url = string.Format("api/SLASituacao/items");

                    if (string.IsNullOrWhiteSpace(objSLASituacao.id_Empresa))
                    {
                        objSLASituacao.id_Empresa = Session["id_EmpresaSel"].ToString();
                    }

                    base.Salvar<SlaSituacaoTO>(accessToken, _urlBase, url, objSLASituacao, ref id_sla);

                    url = string.Format("api/SLASituacao/items");

                    ListaSLASituacaoConsulta = Listar<SLASituacaoConsultaTO>(accessToken, _urlBase, url).Where(m =>m.id_Empresa == Session["id_EmpresaSel"].ToString()).ToList();

                    vwSLASituacao.ListaSLASituacao = ListaSLASituacaoConsulta;

                    vwSLASituacao.dropDisabled = "";

                    vwSLASituacao.SlaSituacao = objSLASituacao;

                    vwSLASituacao.mensagem = "O SLA da Situação foi excluído com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwSLASituacao.erro = "Ocorreu um erro ao excluir o SLA da Situação. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuSLASituacaoSituacao = "menu-ativo";
            return View("SLASituacao", vwSLASituacao);
        }

        #endregion

        #region "Fluxo Automatico"

        public ActionResult FluxoAutomatico(string id_tiposituacaoacomodacao, string id_tipoatividadeacomodacao, string id_tipoacaoacomodacao)
        {
            FluxoAutomaticoView vwFluxoAutomatico = new FluxoAutomaticoView();
            FluxoAutomaticoTO objFluxoAutomatico = new FluxoAutomaticoTO();
            FluxoAutomaticoConsultaTO objFluxoAutomaticoConsulta = new FluxoAutomaticoConsultaTO();
            List<FluxoAutomaticoConsultaTO> ListaFluxoAutomatico = new List<FluxoAutomaticoConsultaTO>();
            try
            {
                // ****************************** CARGA DAS COMBOS ****************************************************/
                vwFluxoAutomatico.ListaTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();
                vwFluxoAutomatico.ListaTipoAtividadeAcomodacao = base.ListarTipoAtividadeAcomodacao();
                vwFluxoAutomatico.ListaTipoAcaoAcomodacao = base.ListarTipoAcaoAcomodacao();
                // ****************************************************************************************************/

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                string url = string.Format("api/FluxoAutomaticoAcao/items/fluxoautomatico/empresa/{0}", Session["id_EmpresaSel"].ToString());

                ListaFluxoAutomatico = Listar<FluxoAutomaticoConsultaTO>(accessToken, _urlBase, url);

                vwFluxoAutomatico.ListaFluxoAutomatico = ListaFluxoAutomatico;

                if (string.IsNullOrWhiteSpace(id_tiposituacaoacomodacao) || string.IsNullOrWhiteSpace(id_tipoatividadeacomodacao) || string.IsNullOrWhiteSpace(id_tipoacaoacomodacao))
                {
                    vwFluxoAutomatico.FluxoAutomatico.id_Empresa = Session["id_EmpresaSel"].ToString();
                    vwFluxoAutomatico.FluxoAutomatico = new FluxoAutomaticoTO();
                }
                else
                {

                    accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                    _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                    //url = string.Format("api/FluxoAutomaticoAcao/items/fluxoautomatico/tiposituacao/{0}/tipoatividade/{1}/tipoacao/{2}/empresa/{3}", id_tiposituacaoacomodacao, id_tipoatividadeacomodacao, id_tipoacaoacomodacao, Session["id_EmpresaSel"].ToString());

                    //objFluxoAutomatico = Listar<FluxoAutomaticoTO>(accessToken, _urlBase, url).FirstOrDefault();

                    objFluxoAutomaticoConsulta = ListaFluxoAutomatico.Where(m => m.id_TipoSituacaoAcomodacaoOrigem==id_tiposituacaoacomodacao && m.id_TipoAtividadeAcomodacaoOrigem==id_tipoatividadeacomodacao && m.id_TipoAcaoAcomodacaoOrigem==id_tipoacaoacomodacao).ToList().FirstOrDefault();

                    objFluxoAutomatico.id_TipoSituacaoAcomodacaoOrigem = objFluxoAutomaticoConsulta.id_TipoSituacaoAcomodacaoOrigem;
                    objFluxoAutomatico.id_TipoAtividadeAcomodacaoOrigem = objFluxoAutomaticoConsulta.id_TipoAtividadeAcomodacaoOrigem;
                    objFluxoAutomatico.id_TipoAcaoAcomodacaoOrigem = objFluxoAutomaticoConsulta.id_TipoAcaoAcomodacaoOrigem;
                    objFluxoAutomatico.id_TipoSituacaoAcomodacaoDestino = objFluxoAutomaticoConsulta.id_TipoSituacaoAcomodacaoDestino;
                    objFluxoAutomatico.id_TipoAtividadeAcomodacaoDestino= objFluxoAutomaticoConsulta.id_TipoAtividadeAcomodacaoDestino;

                    vwFluxoAutomatico.dropDisabled = " disabled ";
                    vwFluxoAutomatico.FluxoAutomatico  = objFluxoAutomatico;

                }

            }
            catch (Exception ex)
            {
                vwFluxoAutomatico.erro = "Ocorreu um erro ao consultar o Fluxo Automático. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuFluxoAutomatico = "menu-ativo";
            return View("FluxoAutomatico", vwFluxoAutomatico);
        }

        public ActionResult SalvarFluxoAutomatico(FluxoAutomaticoView model)
        {
            FluxoAutomaticoView vwFluxoAutomatico = new FluxoAutomaticoView();
            FluxoAutomaticoTO objFluxoAutomatico = new FluxoAutomaticoTO();
            FluxoAutomaticoConsultaTO objFluxoAutomaticoConsulta = new FluxoAutomaticoConsultaTO();
            List<FluxoAutomaticoConsultaTO> ListaFluxoAutomaticoConsulta = new List<FluxoAutomaticoConsultaTO>();
            string id_Sla = "";
            try
            {

                vwFluxoAutomatico.ListaTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();
                vwFluxoAutomatico.ListaTipoAtividadeAcomodacao = base.ListarTipoAtividadeAcomodacao();
                vwFluxoAutomatico.ListaTipoAcaoAcomodacao = base.ListarTipoAcaoAcomodacao();

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                string url = string.Format("api/FluxoAutomaticoAcao/items/fluxoautomatico/empresa/{0}", Session["id_EmpresaSel"].ToString());


                if (!ModelState.IsValid)
                {
                    ListaFluxoAutomaticoConsulta = Listar<FluxoAutomaticoConsultaTO>(accessToken, _urlBase, url);

                    vwFluxoAutomatico.ListaFluxoAutomatico = ListaFluxoAutomaticoConsulta;

                    vwFluxoAutomatico.dropDisabled = "";

                    vwFluxoAutomatico.FluxoAutomatico = model.FluxoAutomatico;

                    model.ListaTipoSituacaoAcomodacao = vwFluxoAutomatico.ListaTipoSituacaoAcomodacao;
                    model.ListaTipoAtividadeAcomodacao = vwFluxoAutomatico.ListaTipoAtividadeAcomodacao;
                    model.ListaTipoAcaoAcomodacao = vwFluxoAutomatico.ListaTipoAcaoAcomodacao;
                    model.ListaFluxoAutomatico = vwFluxoAutomatico.ListaFluxoAutomatico;
                    model.dropDisabled = "";

                    return View("FluxoAutomatico", model);
                }
                else
                {
                    ListaFluxoAutomaticoConsulta = Listar<FluxoAutomaticoConsultaTO>(accessToken, _urlBase, url);

                    vwFluxoAutomatico.ListaFluxoAutomatico = ListaFluxoAutomaticoConsulta;

                    objFluxoAutomaticoConsulta = vwFluxoAutomatico.ListaFluxoAutomatico.Where(m => m.id_TipoAcaoAcomodacaoOrigem == model.FluxoAutomatico.id_TipoAcaoAcomodacaoOrigem &&
                                                                                           m.id_TipoAtividadeAcomodacaoOrigem == model.FluxoAutomatico.id_TipoAtividadeAcomodacaoOrigem &&
                                                                                           m.id_TipoSituacaoAcomodacaoOrigem == model.FluxoAutomatico.id_TipoSituacaoAcomodacaoOrigem &&
                                                                                           m.id_TipoSituacaoAcomodacaoDestino == model.FluxoAutomatico.id_TipoSituacaoAcomodacaoDestino &&
                                                                                           m.id_TipoAtividadeAcomodacaoDestino == model.FluxoAutomatico.id_TipoAtividadeAcomodacaoDestino).FirstOrDefault();

                    url = string.Format("api/FluxoAutomaticoAcao/items");

                    if (string.IsNullOrWhiteSpace(model.FluxoAutomatico.id_Empresa))
                    {
                        model.FluxoAutomatico.id_Empresa = Session["id_EmpresaSel"].ToString();
                    }
                    
                    if (objFluxoAutomaticoConsulta != null)
                    {
                        ExcluirFluxoAutomatico(objFluxoAutomaticoConsulta.id_TipoSituacaoAcomodacaoOrigem,
                                                objFluxoAutomaticoConsulta.id_TipoAtividadeAcomodacaoOrigem,
                                                objFluxoAutomaticoConsulta.id_TipoAcaoAcomodacaoOrigem,
                                                objFluxoAutomaticoConsulta.id_TipoSituacaoAcomodacaoDestino,
                                                objFluxoAutomaticoConsulta.id_TipoAtividadeAcomodacaoDestino);
                    }

                    base.Salvar<FluxoAutomaticoTO>(accessToken, _urlBase, url, model.FluxoAutomatico, ref id_Sla);

                    vwFluxoAutomatico.dropDisabled = "";

                    vwFluxoAutomatico.FluxoAutomatico = objFluxoAutomatico;

                    url = string.Format("api/FluxoAutomaticoAcao/items/fluxoautomatico/empresa/{0}", Session["id_EmpresaSel"].ToString());

                    ListaFluxoAutomaticoConsulta = Listar<FluxoAutomaticoConsultaTO>(accessToken, _urlBase, url);

                    vwFluxoAutomatico.ListaFluxoAutomatico = ListaFluxoAutomaticoConsulta;

                    vwFluxoAutomatico.mensagem = "O Fluxo automático foi salvo com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwFluxoAutomatico.erro = "Ocorreu um erro ao salvar os dados do fluxo automático. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuFluxoAutomatico = "menu-ativo";
            return View("FluxoAutomatico", vwFluxoAutomatico);
        }

        public ActionResult ExcluirFluxoAutomatico(string id_tiposituacaoacomodacaoOrigem, string id_tipoatividadeacomodacaoOrigem, string id_tipoacaoacomodacaoOrigem, string id_tiposituacaoacomodacaoDestino, string id_tipoatividadeacomodacaoDestino)
        {
            FluxoAutomaticoView vwFluxoAutomatico = new FluxoAutomaticoView();
            FluxoAutomaticoTO objFluxoAutomatico = new FluxoAutomaticoTO();
            FluxoAutomaticoConsultaTO objFluxoAutomaticoConsulta = new FluxoAutomaticoConsultaTO();
            List<FluxoAutomaticoConsultaTO> ListaFluxoAutomaticoConsulta = new List<FluxoAutomaticoConsultaTO>();
            try
            {
                // ****************************** CARGA DAS COMBOS ****************************************************/
                vwFluxoAutomatico.ListaTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();
                vwFluxoAutomatico.ListaTipoAtividadeAcomodacao = base.ListarTipoAtividadeAcomodacao();
                vwFluxoAutomatico.ListaTipoAcaoAcomodacao = base.ListarTipoAcaoAcomodacao();
                // ****************************************************************************************************/

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                string url = "";

                if (string.IsNullOrWhiteSpace(id_tiposituacaoacomodacaoOrigem) || string.IsNullOrWhiteSpace(id_tipoatividadeacomodacaoOrigem) || string.IsNullOrWhiteSpace(id_tipoacaoacomodacaoOrigem) || string.IsNullOrWhiteSpace(id_tiposituacaoacomodacaoDestino) || string.IsNullOrWhiteSpace(id_tipoatividadeacomodacaoDestino))
                {
                    url = string.Format("api/FluxoAutomatico/items/empresa/{0}", Session["id_EmpresaSel"].ToString());
                    ListaFluxoAutomaticoConsulta = Listar<FluxoAutomaticoConsultaTO>(accessToken, _urlBase, url);
                    vwFluxoAutomatico.ListaFluxoAutomatico = ListaFluxoAutomaticoConsulta;

                    vwFluxoAutomatico.erro = "O identificador do fluxo é inválido.";
                    return View("FluxoAutomatico", vwFluxoAutomatico);
                }
                else
                {
                    // ****************************** CARGA DAS COMBOS ****************************************************/
                    vwFluxoAutomatico.ListaTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();
                    // ****************************************************************************************************/

                    url = string.Format("api/FluxoAutomaticoAcao/items?Id_TipoSituacaoAcomodacaoOrigem={0}&Id_TipoAtividadeAcomodacaoOrigem={1}&Id_TipoAcaoAcomodacaoOrigem={2}&Id_TipoSituacaoAcomodacaoDestino={3}&Id_TipoAtividadeAcomodacaoDestino={4}&IdEmpresa={5}", id_tiposituacaoacomodacaoOrigem, id_tipoatividadeacomodacaoOrigem, id_tipoacaoacomodacaoOrigem, id_tiposituacaoacomodacaoDestino, id_tipoatividadeacomodacaoDestino, Session["id_EmpresaSel"].ToString());

                    base.Excluir(accessToken, _urlBase, url);

                    url = string.Format("api/FluxoAutomaticoAcao/items/fluxoautomatico/empresa/{0}", Session["id_EmpresaSel"].ToString());

                    ListaFluxoAutomaticoConsulta = Listar<FluxoAutomaticoConsultaTO>(accessToken, _urlBase, url);

                    vwFluxoAutomatico.ListaFluxoAutomatico = ListaFluxoAutomaticoConsulta;

                    vwFluxoAutomatico.dropDisabled = "";

                    vwFluxoAutomatico.FluxoAutomatico = objFluxoAutomatico;

                    vwFluxoAutomatico.mensagem = "O Fluxo automático foi excluído com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwFluxoAutomatico.erro = "Ocorreu um erro ao excluir o fluxo automático. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuFluxoAutomatico = "menu-ativo";
            return View("FluxoAutomatico", vwFluxoAutomatico);
        }

        #endregion

        #region "Fluxo Automatico Check"

        public ActionResult FluxoAutomaticoCheck(string id_checklist, string id_tiposituacaoacomodacao, string id_itemchecklist, string id_tipoatividadeacomodacao)
        {
            FluxoAutomaticoCheckView vwFluxoAutomatico = new FluxoAutomaticoCheckView();
            FluxoAutomaticoCheckTO objFluxoAutomatico = new FluxoAutomaticoCheckTO();
            FluxoAutomaticoCheckConsultaTO objFluxoAutomaticoConsulta = new FluxoAutomaticoCheckConsultaTO();
            List<FluxoAutomaticoCheckConsultaTO> ListaFluxoAutomatico = new List<FluxoAutomaticoCheckConsultaTO>();
            try
            {
                // ****************************** CARGA DAS COMBOS ****************************************************/
                vwFluxoAutomatico.ListaTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();
                vwFluxoAutomatico.ListaTipoAtividadeAcomodacao = base.ListarTipoAtividadeAcomodacao();
                vwFluxoAutomatico.ListaCheckList = base.ListarCheckList();
                vwFluxoAutomatico.ListaPermissaoFinalizacaoTotal = base.ListarTipoPermissaoFinalizacaoTotal();
                vwFluxoAutomatico.ListaTipoResposta = base.ListarTipoResposta();
                // ****************************************************************************************************/

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                /// CONFIGURACAO     
                string url = _urlBase + string.Format("api/CheckList/items/checklist/");
                vwFluxoAutomatico.tkn_configuracao = accessToken.access_token;
                vwFluxoAutomatico.url_listaCheckList = url;


                url = string.Format("api/FluxoAutomaticoCheck/items/fluxoautomaticocheck");



                ListaFluxoAutomatico = Listar<FluxoAutomaticoCheckConsultaTO>(accessToken, _urlBase, url);

                vwFluxoAutomatico.ListaFluxoAutomaticoCheck = ListaFluxoAutomatico;

                if (string.IsNullOrWhiteSpace(id_tiposituacaoacomodacao) || string.IsNullOrWhiteSpace(id_tipoatividadeacomodacao) || string.IsNullOrWhiteSpace(id_checklist) || string.IsNullOrWhiteSpace(id_itemchecklist))
                {
                    vwFluxoAutomatico.FluxoAutomaticoCheck = new FluxoAutomaticoCheckTO();
                }
                else
                {

                    accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                    _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                    objFluxoAutomaticoConsulta = ListaFluxoAutomatico.Where(m => m.id_Checklist == id_checklist && m.id_ItemChecklist == id_itemchecklist && m.id_TipoAtividadeAcomodacao == id_tipoatividadeacomodacao && m.id_TipoSituacaoAcomodacao == id_tiposituacaoacomodacao).ToList().FirstOrDefault();


                    url = string.Format("api/CheckList/items/checklist/{0}", objFluxoAutomaticoConsulta.id_Checklist);

                    vwFluxoAutomatico.ListaItemCheckList = Listar<ChecklistItemChecklistConsultaTO>(accessToken, _urlBase, url);

                    objFluxoAutomatico.id_Checklist = objFluxoAutomaticoConsulta.id_Checklist;
                    objFluxoAutomatico.id_ItemChecklist = objFluxoAutomaticoConsulta.id_ItemChecklist;
                    objFluxoAutomatico.id_TipoAtividadeAcomodacao = objFluxoAutomaticoConsulta.id_TipoAtividadeAcomodacao;
                    objFluxoAutomatico.id_TipoSituacaoAcomodacao = objFluxoAutomaticoConsulta.id_TipoSituacaoAcomodacao;
                    objFluxoAutomatico.cod_Resposta = objFluxoAutomaticoConsulta.cod_Resposta;
                    objFluxoAutomatico.cod_PermiteTotal = objFluxoAutomaticoConsulta.cod_PermiteTotal;


                    vwFluxoAutomatico.dropDisabled = " disabled ";
                    vwFluxoAutomatico.FluxoAutomaticoCheck = objFluxoAutomatico;

                    /// CONFIGURACAO                  
                    //vwFluxoAutomatico.tkn_configuracao = accessToken.access_token;
                    //vwFluxoAutomatico.url_listaCheckList = url;

                }

            }
            catch (Exception ex)
            {
                vwFluxoAutomatico.erro = "Ocorreu um erro ao consultar o Fluxo Automático. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuFluxoAutomaticoCheck = "menu-ativo";
            return View("FluxoAutomaticoCheck", vwFluxoAutomatico);
        }

        public ActionResult SalvarFluxoAutomaticoCheck(FluxoAutomaticoCheckView model)
        {
            FluxoAutomaticoCheckView vwFluxoAutomaticoCheck = new FluxoAutomaticoCheckView();
            FluxoAutomaticoCheckTO objFluxoAutomaticoCheck = new FluxoAutomaticoCheckTO();
            FluxoAutomaticoCheckConsultaTO objFluxoAutomaticoCheckConsulta = new FluxoAutomaticoCheckConsultaTO();
            List<FluxoAutomaticoCheckConsultaTO> ListaFluxoAutomaticoCheckConsulta = new List<FluxoAutomaticoCheckConsultaTO>();
            
            string id_Sla = "";
            try
            {

                vwFluxoAutomaticoCheck.ListaTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();
                vwFluxoAutomaticoCheck.ListaTipoAtividadeAcomodacao = base.ListarTipoAtividadeAcomodacao();
                vwFluxoAutomaticoCheck.ListaCheckList = base.ListarCheckList();
                vwFluxoAutomaticoCheck.ListaPermissaoFinalizacaoTotal = base.ListarTipoPermissaoFinalizacaoTotal();
                vwFluxoAutomaticoCheck.ListaTipoResposta = base.ListarTipoResposta();


                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                /// CONFIGURACAO     
                string url = _urlBase + string.Format("api/CheckList/items/checklist/");
                vwFluxoAutomaticoCheck.tkn_configuracao = accessToken.access_token;
                vwFluxoAutomaticoCheck.url_listaCheckList = url;


                 url = string.Format("api/FluxoAutomaticoCheck/items/fluxoautomaticocheck");


                if (!ModelState.IsValid)
                {
                    ListaFluxoAutomaticoCheckConsulta = Listar<FluxoAutomaticoCheckConsultaTO>(accessToken, _urlBase, url);

                    vwFluxoAutomaticoCheck.ListaFluxoAutomaticoCheck = ListaFluxoAutomaticoCheckConsulta;

                    vwFluxoAutomaticoCheck.dropDisabled = "";

                    vwFluxoAutomaticoCheck.FluxoAutomaticoCheck = model.FluxoAutomaticoCheck;

                    model.ListaTipoSituacaoAcomodacao = vwFluxoAutomaticoCheck.ListaTipoSituacaoAcomodacao;
                    model.ListaTipoAtividadeAcomodacao = vwFluxoAutomaticoCheck.ListaTipoAtividadeAcomodacao;
                    model.ListaCheckList = vwFluxoAutomaticoCheck.ListaCheckList;
                    model.ListaFluxoAutomaticoCheck = vwFluxoAutomaticoCheck.ListaFluxoAutomaticoCheck;
                    model.ListaPermissaoFinalizacaoTotal = vwFluxoAutomaticoCheck.ListaPermissaoFinalizacaoTotal;
                    model.ListaTipoResposta = vwFluxoAutomaticoCheck.ListaTipoResposta;

                    if (!string.IsNullOrWhiteSpace(model.FluxoAutomaticoCheck.id_Checklist))
                    {
                        url = string.Format("api/CheckList/items/checklist/{0}", model.FluxoAutomaticoCheck.id_Checklist);
                        model.ListaItemCheckList = Listar<ChecklistItemChecklistConsultaTO>(accessToken, _urlBase, url);
                    }

                    model.dropDisabled = "";

                    return View("FluxoAutomaticoCheck", model);
                }
                else
                {
                    ListaFluxoAutomaticoCheckConsulta = Listar<FluxoAutomaticoCheckConsultaTO>(accessToken, _urlBase, url);

                    vwFluxoAutomaticoCheck.ListaFluxoAutomaticoCheck = ListaFluxoAutomaticoCheckConsulta;

                    objFluxoAutomaticoCheckConsulta = vwFluxoAutomaticoCheck.ListaFluxoAutomaticoCheck.Where(m => m.id_Checklist == model.FluxoAutomaticoCheck.id_Checklist &&
                                                                                                                  m.id_ItemChecklist == model.FluxoAutomaticoCheck.id_ItemChecklist &&
                                                                                                                  m.id_TipoAtividadeAcomodacao == model.FluxoAutomaticoCheck.id_TipoAtividadeAcomodacao  &&
                                                                                                                  m.id_TipoSituacaoAcomodacao == model.FluxoAutomaticoCheck.id_TipoSituacaoAcomodacao).FirstOrDefault();

                    url = string.Format("api/FluxoAutomaticoCheck/items");

                   
                    if (objFluxoAutomaticoCheckConsulta != null)
                    {
                        ExcluirFluxoAutomaticoCheck(objFluxoAutomaticoCheckConsulta.id_Checklist,
                                                objFluxoAutomaticoCheckConsulta.id_ItemChecklist,
                                                objFluxoAutomaticoCheckConsulta.id_TipoAtividadeAcomodacao,
                                                objFluxoAutomaticoCheckConsulta.id_TipoSituacaoAcomodacao);
                    }

                    base.Salvar<FluxoAutomaticoCheckTO>(accessToken, _urlBase, url, model.FluxoAutomaticoCheck, ref id_Sla);

                    vwFluxoAutomaticoCheck.dropDisabled = "";

                    vwFluxoAutomaticoCheck.FluxoAutomaticoCheck = objFluxoAutomaticoCheck;

                    url = _urlBase + string.Format("api/FluxoAutomaticoCheck/items/fluxoautomaticocheck");

                    ListaFluxoAutomaticoCheckConsulta = Listar<FluxoAutomaticoCheckConsultaTO>(accessToken, _urlBase, url);

                    vwFluxoAutomaticoCheck.ListaFluxoAutomaticoCheck = ListaFluxoAutomaticoCheckConsulta;

                    url = string.Format("api/CheckList/items/checklist/{0}", model.FluxoAutomaticoCheck.id_Checklist);

                    vwFluxoAutomaticoCheck.ListaItemCheckList = Listar<ChecklistItemChecklistConsultaTO>(accessToken, _urlBase, url);

                    /// CONFIGURACAO     
                    //url = string.Format("api/CheckList/items/checklist/");
                    //vwFluxoAutomaticoCheck.tkn_configuracao = accessToken.access_token;
                    //vwFluxoAutomaticoCheck.url_listaCheckList = url;

                    vwFluxoAutomaticoCheck.ListaFluxoAutomaticoCheck = ListaFluxoAutomaticoCheckConsulta;

                    vwFluxoAutomaticoCheck.mensagem = "O Fluxo automático foi salvo com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwFluxoAutomaticoCheck.erro = "Ocorreu um erro ao salvar os dados do fluxo automático Check-List. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuFluxoAutomaticoCheck = "menu-ativo";
            return View("FluxoAutomaticoCheck", vwFluxoAutomaticoCheck);
        }

        public ActionResult ExcluirFluxoAutomaticoCheck(string id_checklist, string id_tiposituacaoacomodacao, string id_itemchecklist, string id_tipoatividadeacomodacao)
        {
            FluxoAutomaticoCheckView vwFluxoAutomaticoCheck = new FluxoAutomaticoCheckView();
            FluxoAutomaticoCheckTO objFluxoAutomaticoCheck = new FluxoAutomaticoCheckTO();
            FluxoAutomaticoCheckConsultaTO objFluxoAutomaticoCheckConsulta = new FluxoAutomaticoCheckConsultaTO();
            List<FluxoAutomaticoCheckConsultaTO> ListaFluxoAutomaticoCheckConsulta = new List<FluxoAutomaticoCheckConsultaTO>();
            try
            {
                // ****************************** CARGA DAS COMBOS ****************************************************/
                vwFluxoAutomaticoCheck.ListaTipoSituacaoAcomodacao = base.ListarTipoSituacaoAcomodacao();
                vwFluxoAutomaticoCheck.ListaTipoAtividadeAcomodacao = base.ListarTipoAtividadeAcomodacao();
                vwFluxoAutomaticoCheck.ListaCheckList = base.ListarCheckList();
                vwFluxoAutomaticoCheck.ListaPermissaoFinalizacaoTotal = base.ListarTipoPermissaoFinalizacaoTotal();
                vwFluxoAutomaticoCheck.ListaTipoResposta = base.ListarTipoResposta();
                // ****************************************************************************************************/

                Token accessToken = (Token)System.Web.HttpContext.Current.Application["tokenConfiguracao"];

                string _urlBase = ConfigurationManager.AppSettings["urlModuloConfiguracao"];

                /// CONFIGURACAO     
                string url = _urlBase + string.Format("api/CheckList/items/checklist/");
                vwFluxoAutomaticoCheck.tkn_configuracao = accessToken.access_token;
                vwFluxoAutomaticoCheck.url_listaCheckList = url;

                 url = "";

                if (string.IsNullOrWhiteSpace(id_checklist) || string.IsNullOrWhiteSpace(id_itemchecklist) || string.IsNullOrWhiteSpace(id_tipoatividadeacomodacao) || string.IsNullOrWhiteSpace(id_tiposituacaoacomodacao) )
                {
                    url = string.Format("api/FluxoAutomaticoCheck/items/fluxoautomaticocheck");
                    ListaFluxoAutomaticoCheckConsulta = Listar<FluxoAutomaticoCheckConsultaTO>(accessToken, _urlBase, url);
                    vwFluxoAutomaticoCheck.ListaFluxoAutomaticoCheck = ListaFluxoAutomaticoCheckConsulta;

                    vwFluxoAutomaticoCheck.erro = "O identificador do fluxo é inválido.";
                    return View("FluxoAutomaticoCheck", vwFluxoAutomaticoCheck);
                }
                else
                {

                    url = _urlBase + string.Format("api/FluxoAutomaticoCheck/items?Id_Checklist={0}&Id_TipoSituacaoAcomodacao={1}&Id_ItemChecklist={2}&Id_TipoAtividadeAcomodacao={3}", id_checklist, id_tiposituacaoacomodacao, id_itemchecklist,  id_tipoatividadeacomodacao);

                    base.Excluir(accessToken, _urlBase, url);

                    url = _urlBase + string.Format("api/FluxoAutomaticoCheck/items/fluxoautomaticocheck");

                    ListaFluxoAutomaticoCheckConsulta = Listar<FluxoAutomaticoCheckConsultaTO>(accessToken, _urlBase, url);

                    vwFluxoAutomaticoCheck.ListaFluxoAutomaticoCheck = ListaFluxoAutomaticoCheckConsulta;

                    url = _urlBase + string.Format("api/CheckList/items/checklist/{0}", id_checklist);

                    vwFluxoAutomaticoCheck.ListaItemCheckList = Listar<ChecklistItemChecklistConsultaTO>(accessToken, _urlBase, url);

                    /// CONFIGURACAO                  
                    //vwFluxoAutomaticoCheck.tkn_configuracao = accessToken.access_token;
                    //vwFluxoAutomaticoCheck.url_listaCheckList = url;

                    vwFluxoAutomaticoCheck.dropDisabled = "";

                    vwFluxoAutomaticoCheck.FluxoAutomaticoCheck = objFluxoAutomaticoCheck;

                    vwFluxoAutomaticoCheck.mensagem = "O Fluxo automático Check-List foi excluído com sucesso.";
                }

            }
            catch (Exception ex)
            {
                vwFluxoAutomaticoCheck.erro = "Ocorreu um erro ao excluir o fluxo automático Check-List. Detalhe: " + ex.Message;
                //throw ex;
            }
            ViewBag.MenuFluxoAutomaticoCheck = "menu-ativo";
            return View("FluxoAutomaticoCheck", vwFluxoAutomaticoCheck);
        }

        #endregion
    }
}
