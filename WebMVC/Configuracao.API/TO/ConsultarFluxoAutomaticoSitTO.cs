using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Configuracao.API.TO
{
    public class ConsultarFluxoAutomaticoSitTO
    {
        string m_sql;
        #region Atributos

        private int m_Id_TipoAtividadeAcomodacaoDestino;
        private int m_Id_TipoSituacaoAcomodacaoDestino;
        private int m_Id_TipoSituacaoAcomodacaoOrigem;

        private int m_Id_Empresa;
        private string m_Nome_TipoSituacaoAcomodacaoDestino;
        private string m_Nome_TipoAtividadeAcomodacaoDestino;
        private string m_Nome_TipoSituacaoAcomodacaoOrigem;


        #endregion

        #region Propriedades

        public int Id_TipoSituacaoAcomodacaoOrigem
        {
            get
            {
                return this.m_Id_TipoSituacaoAcomodacaoOrigem;
            }
            set
            {
                this.m_Id_TipoSituacaoAcomodacaoOrigem = value;
            }
        }
        public int Id_TipoAtividadeAcomodacaoDestino
        {
            get
            {
                return this.m_Id_TipoAtividadeAcomodacaoDestino;
            }
            set
            {
                this.m_Id_TipoAtividadeAcomodacaoDestino = value;
            }
        }
        public int Id_TipoSituacaoAcomodacaoDestino
        {
            get
            {
                return this.m_Id_TipoSituacaoAcomodacaoDestino;
            }
            set
            {
                this.m_Id_TipoSituacaoAcomodacaoDestino = value;
            }
        }
        public int Id_Empresa
        {
            get
            {
                return this.m_Id_Empresa;
            }
            set
            {
                this.m_Id_Empresa = value;
            }
        }
        public string Nome_TipoSituacaoAcomodacaoOrigem
        {
            get
            {
                return this.m_Nome_TipoSituacaoAcomodacaoOrigem;
            }
            set
            {
                this.m_Nome_TipoSituacaoAcomodacaoOrigem = value;
            }
        }
        public string Nome_TipoSituacaoAcomodacaoDestino
        {
            get
            {
                return this.m_Nome_TipoSituacaoAcomodacaoDestino;
            }
            set
            {
                this.m_Nome_TipoSituacaoAcomodacaoDestino = value;
            }
        }
        public string Nome_TipoAtividadeAcomodacaoDestino
        {
            get
            {
                return this.m_Nome_TipoAtividadeAcomodacaoDestino;
            }
            set
            {
                this.m_Nome_TipoAtividadeAcomodacaoDestino = value;
            }
        }

        #endregion

        #region Construtor

        public ConsultarFluxoAutomaticoSitTO()
        {
            m_Nome_TipoSituacaoAcomodacaoDestino = string.Empty;
            m_Nome_TipoAtividadeAcomodacaoDestino = string.Empty;
            m_Nome_TipoSituacaoAcomodacaoOrigem = string.Empty;
        }

        #endregion

        #region SQL
        public void ConsultarFluxoAutomaticoSitTOCommand(int IdTipoSituacaoAcomodacaoOrigem,
                                                        int IdEmpresa,
                                                        string connection,
                                                        ref List<ConsultarFluxoAutomaticoSitTO> l_ListFluxoTO)
        {

            m_sql = " SELECT  TA.Nome_TipoAtividadeAcomodacao AS Nome_TipoAtividadeAcomodacaoDestino, TS.Nome_TipoSituacaoAcomodacao AS Nome_TipoSituacaoAcomodacaoDestino,  ";
            m_sql += "        TSA.Nome_TipoSituacaoAcomodacao AS Nome_TipoSituacaoAcomodacaoOrigem, F.Id_TipoSituacaoAcomodacaoOrigem, F.Id_TipoSituacaoAcomodacaoDestino, F.Id_TipoAtividadeAcomodacaoDestino, F.Id_Empresa ";
            m_sql += " FROM   FluxoAutomaticoTipoSituacaoTransicao AS F INNER JOIN ";
            m_sql += "        TipoAtividadeAcomodacao AS TA ON F.Id_TipoAtividadeAcomodacaoDestino = TA.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += "        TipoSituacaoAcomodacao AS TS ON F.Id_TipoSituacaoAcomodacaoDestino = TS.Id_TipoSituacaoAcomodacao INNER JOIN  ";
            m_sql += "        TipoSituacaoAcomodacao AS TSA ON F.Id_TipoSituacaoAcomodacaoOrigem = TSA.Id_TipoSituacaoAcomodacao ";
            m_sql += " WHERE (Id_TipoSituacaoAcomodacaoOrigem = @Id_TipoSituacaoAcomodacao) ";
            m_sql += " AND (Id_Empresa = @Id_Empresa) ";


            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_TipoSituacaoAcomodacao", IdTipoSituacaoAcomodacaoOrigem);
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListFluxoTO);
            Dados = null;
        }

        public void ConsultarFluxoAutomaticoSitPorEmpresaTOCommand(
                                                int IdEmpresa,
                                                string connection,
                                                ref List<ConsultarFluxoAutomaticoSitTO> l_ListFluxoTO)
        {
            m_sql = " SELECT  TAD.Nome_TipoAtividadeAcomodacao AS Nome_TipoAtividadeAcomodacaoDestino, TSD.Nome_TipoSituacaoAcomodacao AS Nome_TipoSituacaoAcomodacaoDestino,  ";
            m_sql += "        TSO.Nome_TipoSituacaoAcomodacao AS Nome_TipoSituacaoAcomodacaoOrigem, F.Id_TipoSituacaoAcomodacaoOrigem, F.Id_TipoSituacaoAcomodacaoDestino, F.Id_TipoAtividadeAcomodacaoDestino, F.Id_Empresa ";
            m_sql += " FROM            FluxoAutomaticoTipoSituacaoTransicao AS F INNER JOIN ";
            m_sql += "               TipoSituacaoAcomodacao AS TSD ON F.Id_TipoSituacaoAcomodacaoDestino = TSD.Id_TipoSituacaoAcomodacao INNER JOIN ";
            m_sql += "               TipoAtividadeAcomodacao AS TAD ON F.Id_TipoAtividadeAcomodacaoDestino = TAD.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += "               TipoSituacaoAcomodacao AS TSO ON F.Id_TipoSituacaoAcomodacaoOrigem = TSO.Id_TipoSituacaoAcomodacao ";

            m_sql += " WHERE(Id_Empresa = @Id_Empresa) ";


            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListFluxoTO);
            Dados = null;
        }

        #endregion
    }
}
