using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Operacional.API.TO
{
    public class ConsultarAcessoAtividadeEmpresaPerfilTO
    {
        string m_sql;
        #region Atributos

        private int m_Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade;
        private int m_Id_Perfil;
        private string m_Nome_Perfil;
        private int m_Id_Empresa;
        private int m_Id_TipoSituacaoAcomodacao;
        private string m_Nome_TipoSituacaoAcomodacao;
        private int m_Id_TipoAtividadeAcomodacao;
        private string m_Nome_TipoAtividadeAcomodacao;
        private string m_Cod_Tipo;

        #endregion

        #region Propriedades

        public int Id_Empresa { get => m_Id_Empresa; set => m_Id_Empresa = value; }
        public int Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade { get => m_Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade; set => m_Id_AcessoEmpresaPerfilTipoSituacaoTipoAtividade = value; }
        public int Id_Perfil { get => m_Id_Perfil; set => m_Id_Perfil = value; }
        public string Nome_Perfil { get => m_Nome_Perfil; set => m_Nome_Perfil = value; }
        public int Id_TipoSituacaoAcomodacao { get => m_Id_TipoSituacaoAcomodacao; set => m_Id_TipoSituacaoAcomodacao = value; }
        public string Nome_TipoSituacaoAcomodacao { get => m_Nome_TipoSituacaoAcomodacao; set => m_Nome_TipoSituacaoAcomodacao = value; }
        public int Id_TipoAtividadeAcomodacao { get => m_Id_TipoAtividadeAcomodacao; set => m_Id_TipoAtividadeAcomodacao = value; }
        public string Nome_TipoAtividadeAcomodacao { get => m_Nome_TipoAtividadeAcomodacao; set => m_Nome_TipoAtividadeAcomodacao = value; }
        public string Cod_Tipo { get => m_Cod_Tipo; set => m_Cod_Tipo = value; }

        #endregion

        #region Construtor

        public ConsultarAcessoAtividadeEmpresaPerfilTO()
        {
            Nome_Perfil = string.Empty;
            Nome_TipoSituacaoAcomodacao = string.Empty;
            Nome_TipoAtividadeAcomodacao = string.Empty;
            Cod_Tipo = string.Empty;

        }

        #endregion

       
    }
}
