using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Framework.ObjectHelper;
using Framework.Exceptions;


namespace Framework.Data
{
    public class DataCOM
    {
        private SqlConnection m_Conn = null;
 
        private static string m_Caminho = "";

        public DataCOM(string pCaminho)
        {
            m_Caminho = pCaminho;
            ConectaDb();
        }

        private bool ConectaDb()
        {
            try
            {
                if (m_Conn == null || m_Conn.State == ConnectionState.Closed)
                {
                    m_Conn = new SqlConnection();
                    m_Conn.ConnectionString = m_Caminho;
                    m_Conn.Open();
                }
                return true;
            }

            catch (Exception e)
            {
                throw new ExcecaoDb("ConectaDb", e.Message );
            }
        }

        private bool dbSQLDs(SqlCommand pSQLCommand, ref string pRet, ref string pLinhasAfetadas, ref DataSet pVetRetorno)
        {
            DataSet dsRs = null;
            SqlDataAdapter adpRs = null;
            string lSqlError = "";

            try
            {
                if (m_Conn != null)
                {
                    pSQLCommand.Connection = m_Conn;
                }
                else
                {
                    ConectaDb();
                    pSQLCommand.Connection = m_Conn;
                }

                adpRs = new SqlDataAdapter();

                if (pSQLCommand != null)
                {
                    lSqlError = pSQLCommand.CommandText;

                    pSQLCommand.CommandTimeout = 120;
                    pSQLCommand.CommandType = CommandType.Text;
                    adpRs.SelectCommand = pSQLCommand;
                    dsRs = new System.Data.DataSet("dsGERAL");
                    adpRs.Fill(dsRs, 0, 0, "GERAL");
                    pVetRetorno = dsRs;
                    pLinhasAfetadas = dsRs.Tables["GERAL"].Rows.Count.ToString();
                    if ((pLinhasAfetadas == "0")) pLinhasAfetadas = "-99";

                    return true;
                }
            }
            catch (Exception e)
            {
                throw new ExcecaoDb("dbSQLDs", e.Message);
            }
            finally
            {
                if (adpRs != null)
                {
                    adpRs.Dispose();
                    adpRs = null;
                }
                if (pSQLCommand != null)
                {
                    pSQLCommand.Dispose();
                    pSQLCommand = null;
                }

                if (dsRs != null)
                {
                    dsRs.Dispose();
                    dsRs = null;
                }

                if ((m_Conn != null))
                {
                    m_Conn.Close();
                    m_Conn.Dispose();
                    m_Conn = null;
                }
            }

            return false;
        }

        public void DbSQLObjectList<T>(SqlCommand pSQLCommand, ref List<T> pList)
        {

            string lSqlError = "";
            DataSet pVetRetorno = default(DataSet);

            try
            {
                if (pSQLCommand != null)
                {

                    lSqlError = pSQLCommand.CommandText;

                    string pRet = default(string);
                    string pLinhasAfetadas = default(string);

                    this.dbSQLDs(pSQLCommand, ref pRet, ref pLinhasAfetadas, ref pVetRetorno);

                    if (pVetRetorno != null && pVetRetorno.Tables.Count > 0 && pVetRetorno.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in pVetRetorno.Tables[0].Rows)
                        {
                            pList.Add(Helper.AtribuiValorPropriedadeDataTable<T>(dr, true));
                        }
                    }

                }
                else
                {
                    throw new ArgumentNullException("pSqlCommand");
                }
            }

            catch (Exception e)
            {
                throw new ExcecaoDb("dbSQLObjectList", e.Message + "-->" + lSqlError);
            }
            finally
            {
                if (pSQLCommand != null)
                {
                    pSQLCommand.Dispose();
                    pSQLCommand = null;
                }

                if (pVetRetorno != null)
                {
                    pVetRetorno.Dispose();
                    pVetRetorno = null;
                }
            }
        }

    }
}
