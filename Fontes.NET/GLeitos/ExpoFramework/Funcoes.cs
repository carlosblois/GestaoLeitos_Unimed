using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Web;
using System.Text;
using Renci.SshNet;
using System.Runtime.Caching;


namespace ExpoFramework.Framework
{
    public static class Funcoes
    {
        public static DateTime converterParaDateTime(string pData, string pFormato)
        {
            DateTime lRetorno;

            lRetorno = DateTime.ParseExact(pData, pFormato, CultureInfo.InvariantCulture);

            return lRetorno;

        }

        public static string converterDataTimeParaString(DateTime pData, string pFormato)
        {
            string lRetorno;

            lRetorno = pData.ToString(pFormato);

            return lRetorno;

        }

        public static bool validarEmail(string email)
        {
            bool emailValido = false;

            string emailRegex = string.Format("{0}{1}",
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))",
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$");

            try
            {
                emailValido = Regex.IsMatch(
                    email,
                    emailRegex);
            }
            catch (RegexMatchTimeoutException)
            {
                emailValido = false;
            }

            return emailValido;
        }

        public static bool validarUrl(string url)
        {
            string pattern = @"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?$";
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            return reg.IsMatch(url);
            //return true; // alterar a expressao regular para atender os links da perinatal.
        }

        
        public static bool FileUploadSFTP(Stream pUploadFile, string pPathDestino, string pHost, int pPorta, string pNomeUsuario, string pSenha)
        {
            //pHost = "www.perinatal.com.br";
            //pPorta = 27709;
            //pNomeUsuario = "Internalusr";
            //pSenha = "PinkFloyd1!";
            //pUploadFile = @"c:yourfilegoeshere.txt";

            try
            {
                using (var client = new SftpClient(pHost, pPorta, pNomeUsuario, pSenha))
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                        //client.ChangeDirectory(pPathDestino);
                        //using (var fileStream = new FileStream(pUploadFile, FileMode.Open))
                        //{
                        //    client.BufferSize = 4 * 1024;
                        //    client.UploadFile(fileStream   , Path.GetFileName(pUploadFile));
                        //}

                        
                        client.BufferSize = 4 * 1024;
                        client.UploadFile(pUploadFile, pPathDestino);
                        

                    }
                    else
                    {
                        throw new Exception("Não foi possível conectar ao servidor.");
                    }
                    client.Disconnect();
                    client.Dispose();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string FormatarOverFlow(string pTexto, int pTamanho)
        {
            string strRetorno = "";
            if (!string.IsNullOrWhiteSpace(pTexto))
            {
                if (pTexto.Length > pTamanho)
                {
                    strRetorno = pTexto.Substring(0, pTamanho) + "...";
                }
                else
                {
                    strRetorno = pTexto;
                }
            }
            else
            {
                strRetorno = pTexto;
            }

            return strRetorno;
            
        }

        public static bool AcessoAdministrador(string pPerfil, string pCodPerfilAdministrador)
        {
            bool boolAdm = false;

            if (pPerfil == pCodPerfilAdministrador)
            {
                boolAdm = true;
            }
            return boolAdm;
        }

        public static string ToTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }
    }

    public static class InMemoryCache
    {
        public static T GetOrSet<T>(string cacheKey, Func<T> getItemCallback, int pTime) where T : class
        {
            T item = MemoryCache.Default.Get(cacheKey) as T;
            if (item == null)
            {
                item = getItemCallback();
                MemoryCache.Default.Add(cacheKey, item, DateTime.Now.AddMinutes(pTime));
            }
            return item;
        }
    }

   
}
