using Framework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Configuracao.API.TO
{
    public class ConsultarFluxoAutomaticoTO
    {
        string m_sql;
        #region Atributos
         
        private int m_Id_TipoAtividadeAcomodacaoDestino;
        private int m_Id_TipoSituacaoAcomodacaoDestino;
        private int m_Id_TipoAtividadeAcomodacaoOrigem;
        private int m_Id_TipoSituacaoAcomodacaoOrigem;

        private int m_Id_Empresa;
        private string m_Nome_TipoSituacaoAcomodacaoDestino;
        private string m_Nome_TipoAtividadeAcomodacaoDestino;
        private string m_Nome_TipoSituacaoAcomodacaoOrigem;
        private string m_Nome_TipoAtividadeAcomodacaoOrigem;

        private string m_Nome_TipoAcaoAcomodacao;
        private string m_Nome_Status;
        private int m_Id_TipoAcaoAcomodacaoOrigem;



        #endregion

        #region Propriedades

        public int Id_TipoAtividadeAcomodacaoOrigem
        {
            get
            {
                return this.m_Id_TipoAtividadeAcomodacaoOrigem;
            }
            set
            {
                this.m_Id_TipoAtividadeAcomodacaoOrigem = value;
            }
        }
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
        public string Nome_TipoAtividadeAcomodacaoOrigem
        {
            get
            {
                return this.m_Nome_TipoAtividadeAcomodacaoOrigem;
            }
            set
            {
                this.m_Nome_TipoAtividadeAcomodacaoOrigem = value;
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
        public string Nome_TipoAcaoAcomodacao
        {
            get
            {
                return this.m_Nome_TipoAcaoAcomodacao;
            }
            set
            {
                this.m_Nome_TipoAcaoAcomodacao = value;
            }
        }
        public string Nome_Status
        {
            get
            {
                return this.m_Nome_Status;
            }
            set
            {
                this.m_Nome_Status = value;
            }
        }
        public int Id_TipoAcaoAcomodacaoOrigem
        {
            get
            {
                return this.m_Id_TipoAcaoAcomodacaoOrigem;
            }
            set
            {
                this.m_Id_TipoAcaoAcomodacaoOrigem = value;
            }
        }
        #endregion

        #region Construtor

        public ConsultarFluxoAutomaticoTO()
        {
            m_Nome_TipoSituacaoAcomodacaoDestino = string.Empty;
            m_Nome_TipoAtividadeAcomodacaoDestino = string.Empty;
            m_Nome_TipoSituacaoAcomodacaoOrigem = string.Empty;
            m_Nome_TipoAtividadeAcomodacaoOrigem = string.Empty;
            m_Nome_TipoAcaoAcomodacao = string.Empty;
            m_Nome_Status = string.Empty;
        }

        #endregion

        #region SQL
        public void ConsultarFluxoAutomaticoSitTOCommand(int IdTipoSituacaoAcomodacaoOrigem,
                                                    int IdTipoAtividadeAcomodacaoOrigem,
                                                    int IdTipoAcaoAcomodacaoOrigem,
                                                    int IdEmpresa,
                                                    string connection,
                                                    ref List<ConsultarFluxoAutomaticoTO> l_ListFluxoTO)
        {

            m_sql = "  SELECT F.Id_TipoAtividadeAcomodacaoDestino, F.Id_TipoSituacaoAcomodacaoDestino, TA.Nome_TipoAtividadeAcomodacao AS Nome_TipoAtividadeAcomodacaoDestino , TS.Nome_TipoSituacaoAcomodacao AS Nome_TipoSituacaoAcomodacaoDestino  ";
            m_sql += " FROM   FluxoAutomaticoTipoSituacao_TipoAtividade AS F INNER JOIN ";
            m_sql += "        TipoSituacao_TipoAtividadeAcomodacao AS TSTA ON F.Id_TipoSituacaoAcomodacaoDestino = TSTA.Id_TipoSituacaoAcomodacao AND F.Id_TipoAtividadeAcomodacaoDestino = TSTA.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += "        TipoAtividadeAcomodacao AS TA ON TSTA.Id_TipoAtividadeAcomodacao = TA.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += "        TipoSituacaoAcomodacao AS TS ON TSTA.Id_TipoSituacaoAcomodacao = TS.Id_TipoSituacaoAcomodacao ";
            m_sql += " WHERE (Id_TipoAcaoAcomodacaoOrigem = @Id_TipoAcaoAcomodacaoOrigem) ";
            m_sql += "  AND (Id_TipoAtividadeAcomodacaoOrigem = @Id_TipoAtividadeAcomodacaoOrigem)  ";
            m_sql += "  AND (Id_TipoSituacaoAcomodacaoOrigem = @Id_TipoSituacaoAcomodacaoOrigem) ";
            m_sql += "  AND (Id_Empresa = @Id_Empresa) ";
            m_sql += "  AND (Id_TipoAtividadeAcomodacaoDestino IS NULL) ";


            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_TipoAcaoAcomodacaoOrigem", IdTipoAcaoAcomodacaoOrigem);
            command.Parameters.AddWithValue("Id_TipoAtividadeAcomodacaoOrigem", IdTipoAtividadeAcomodacaoOrigem);
            command.Parameters.AddWithValue("Id_TipoSituacaoAcomodacaoOrigem", IdTipoSituacaoAcomodacaoOrigem);
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListFluxoTO);
            Dados = null;
        }
        public void ConsultarFluxoAutomaticoTOCommand(int IdTipoSituacaoAcomodacaoOrigem, 
                                                        int IdTipoAtividadeAcomodacaoOrigem,
                                                        int IdTipoAcaoAcomodacaoOrigem, 
                                                        int IdEmpresa,
                                                        string connection, 
                                                        ref List<ConsultarFluxoAutomaticoTO> l_ListFluxoTO)
        {

            m_sql = "  SELECT F.Id_TipoAtividadeAcomodacaoDestino, F.Id_TipoSituacaoAcomodacaoDestino, TA.Nome_TipoAtividadeAcomodacao AS Nome_TipoAtividadeAcomodacaoDestino , TS.Nome_TipoSituacaoAcomodacao AS Nome_TipoSituacaoAcomodacaoDestino  ";
            m_sql += " FROM   FluxoAutomaticoTipoSituacao_TipoAtividade AS F INNER JOIN ";
            m_sql += "        TipoSituacao_TipoAtividadeAcomodacao AS TSTA ON F.Id_TipoSituacaoAcomodacaoDestino = TSTA.Id_TipoSituacaoAcomodacao AND F.Id_TipoAtividadeAcomodacaoDestino = TSTA.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += "        TipoAtividadeAcomodacao AS TA ON TSTA.Id_TipoAtividadeAcomodacao = TA.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += "        TipoSituacaoAcomodacao AS TS ON TSTA.Id_TipoSituacaoAcomodacao = TS.Id_TipoSituacaoAcomodacao ";
            m_sql += " WHERE (Id_TipoAcaoAcomodacaoOrigem = @Id_TipoAcaoAcomodacaoOrigem) ";
            m_sql += "  AND (Id_TipoAtividadeAcomodacaoOrigem = @Id_TipoAtividadeAcomodacaoOrigem)  ";
            m_sql += "  AND (Id_TipoSituacaoAcomodacaoOrigem = @Id_TipoSituacaoAcomodacaoOrigem) ";
            m_sql += "  AND (Id_Empresa = @Id_Empresa) ";
            m_sql += "  AND (Id_TipoAtividadeAcomodacaoDestino IS NOT NULL) ";


            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = m_sql;
            command.Parameters.AddWithValue("Id_TipoAcaoAcomodacaoOrigem", IdTipoAcaoAcomodacaoOrigem);
            command.Parameters.AddWithValue("Id_TipoAtividadeAcomodacaoOrigem", IdTipoAtividadeAcomodacaoOrigem);
            command.Parameters.AddWithValue("Id_TipoSituacaoAcomodacaoOrigem", IdTipoSituacaoAcomodacaoOrigem);
            command.Parameters.AddWithValue("Id_Empresa", IdEmpresa);

            DataCOM Dados = new DataCOM(connection);
            Dados.DbSQLObjectList(command, ref l_ListFluxoTO);
            Dados = null;
        }

        public void ConsultarFluxoAutomaticoPorEmpresaTOCommand(
                                                int IdEmpresa,
                                                string connection,
                                                ref List<ConsultarFluxoAutomaticoTO> l_ListFluxoTO)
        {

            m_sql = "   SELECT F.Id_TipoSituacaoAcomodacaoOrigem, F.Id_TipoAtividadeAcomodacaoOrigem, F.Id_TipoAcaoAcomodacaoOrigem, F.Id_TipoAtividadeAcomodacaoDestino, F.Id_TipoSituacaoAcomodacaoDestino, F.Id_Empresa,  ";
            m_sql += "         TSO.Nome_TipoSituacaoAcomodacao AS Nome_TipoSituacaoAcomodacaoOrigem, TAO.Nome_TipoAtividadeAcomodacao AS Nome_TipoAtividadeAcomodacaoOrigem, TAA.Nome_TipoAcaoAcomodacao, TAA.Nome_Status,  ";
            m_sql += "         TSD.Nome_TipoSituacaoAcomodacao AS Nome_TipoSituacaoAcomodacaoDestino, TAD.Nome_TipoAtividadeAcomodacao AS Nome_TipoAtividadeAcomodacaoDestino ";
            m_sql += "FROM     FluxoAutomaticoTipoSituacao_TipoAtividade AS F INNER JOIN ";
            m_sql += "         TipoSituacao_TipoAtividadeAcomodacao AS TTO ON F.Id_TipoSituacaoAcomodacaoOrigem = TTO.Id_TipoSituacaoAcomodacao AND F.Id_TipoAtividadeAcomodacaoOrigem = TTO.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += "         TipoAtividadeAcomodacao AS TAO ON TTO.Id_TipoAtividadeAcomodacao = TAO.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += "         TipoSituacaoAcomodacao AS TSO ON TTO.Id_TipoSituacaoAcomodacao = TSO.Id_TipoSituacaoAcomodacao INNER JOIN ";
            m_sql += "         TipoAcaoAcomodacao AS TAA ON F.Id_TipoAcaoAcomodacaoOrigem = TAA.Id_TipoAcaoAcomodacao INNER JOIN ";
            m_sql += "         TipoSituacao_TipoAtividadeAcomodacao AS TTD ON F.Id_TipoSituacaoAcomodacaoDestino = TTD.Id_TipoSituacaoAcomodacao AND F.Id_TipoAtividadeAcomodacaoDestino = TTD.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += "         TipoAtividadeAcomodacao AS TAD ON TTD.Id_TipoAtividadeAcomodacao = TAD.Id_TipoAtividadeAcomodacao INNER JOIN ";
            m_sql += "         TipoSituacaoAcomodacao AS TSD ON TTD.Id_TipoSituacaoAcomodacao = TSD.Id_TipoSituacaoAcomodacao ";

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
