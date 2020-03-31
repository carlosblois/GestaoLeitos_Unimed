using System;
using System.IO;
using System.Threading;

namespace ErrorHandler
{
    public class TrataErro
    {

        #region "Variaveis"

        private string m_AppPath;

        #endregion

        #region "Metodos"

        public TrataErro()
        {
            m_AppPath = AppContext.BaseDirectory + "\\LOGERROS" ;
        }
        private void GravaErroTexto(string pMSG)
        {

            EventWaitHandle waitHandle = null;
            try
            {
                waitHandle = new EventWaitHandle(true, EventResetMode.AutoReset, "LOG_WRITE_15484868418");
                string linhaMSG = null;

                System.IO.Directory.CreateDirectory(m_AppPath);

                waitHandle.WaitOne();
                using (StreamWriter sw = new StreamWriter(m_AppPath + "\\\\LogErro_" + System.DateTime.Now.ToString("yyyyMMdd") + ".txt", true))
                {
                    linhaMSG = "Data :" + System.DateTime.Now.ToShortDateString() + " Hora : " + System.DateTime.Now.ToShortTimeString() + " Msg Erro - > " + pMSG;
                    sw.WriteLine(linhaMSG);
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (waitHandle != null) waitHandle.Set();
            }

        }


        private void RegistraErro(string pMsg)
        {
           GravaErroTexto(pMsg);      
        }

        public  void Main(Exception pErro)
        {
            RegistraErro(pErro.Message);     
        }

        public void Main(string pErro)
        {
            RegistraErro(pErro);
        }

        #endregion

    }
}
