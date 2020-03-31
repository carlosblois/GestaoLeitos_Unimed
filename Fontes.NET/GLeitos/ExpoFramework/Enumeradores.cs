using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Web;
using System.Text;
using Renci.SshNet;

namespace ExpoFramework.Framework
{
    public static class Enumeradores
    {
        public enum funcao
        {
            docstermos = 1,
            unidades = 2,
            jornaiserevistas = 3,
            cursomedico = 4,
            homesite = 5
        }
        public enum tipoArquivo
        {
            pdf = 1,
            imagem = 2
        }

        public enum modulo
        {
            usuario = 1,
            administrativo = 2,
            configuracao = 3,
            operacional = 4
        }
    }
}
