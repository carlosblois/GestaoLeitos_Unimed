using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Data.SqlClient;
using System.Data;
namespace Framework.ObjectHelper
{
    public class Helper
    {
        #region Members

        //Armazena as propriedades em cache
        private static IDictionary<string, Dictionary<string, PropertyInfo>> m_CachePropriedades = new Dictionary<string, Dictionary<string, PropertyInfo>>();

        // Tratamento de lock no cache
        private static ReaderWriterLockSlim m_CachePropriedadesLock = new ReaderWriterLockSlim();

        #endregion

        #region metodos publicos

        public static Object RetornaValorPropriedade<T>(string property, T pOut, bool propertyNotFoundException = false)
        {
            Dictionary<string, PropertyInfo> props = GetCachedProperties<T>();

            PropertyInfo propTO = props[property.ToUpper()];

            if (propTO != null)
                return RetornarValorObjeto(propTO, pOut);
            else
                if (propertyNotFoundException)
                throw new Exception("Propriedade não localizada");
            else
                return null;
        }

        public static T AtribuiValorPropriedade<T, Z>(Z pTO, T pDef)
        {

            //' Pega as propriedades
            Dictionary<string, PropertyInfo> propsDef = GetCachedProperties<T>();
            Dictionary<string, PropertyInfo> propsTO = GetCachedProperties<Z>();

            string[] matchKeys = propsDef.Keys.Intersect(propsTO.Keys).ToArray();

            for (int i = 0; i < matchKeys.Length; i++)
            {
                PropertyInfo propDef = propsDef[matchKeys[i]];
                PropertyInfo propTO = propsTO[matchKeys[i]];
                if (propDef.PropertyType.Equals(propTO.PropertyType))
                {
                    typeof(T).InvokeMember(propDef.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetProperty, null, pDef, new Object[] { RetornarValorObjeto(propTO, pTO) });
                }
            }

            return pDef;

        }

        public static T AtribuiValorPropriedadeReader<T>(SqlDataReader pReader, ref List<string> pReaderColumn)
        {
            Type lType = typeof(T);
            return (T)AtribuiValorPropriedadeReaderByType(lType, pReader, ref pReaderColumn);
        }

        public static T AtribuiValorPropriedadeDataTable<T>(DataRow pDataRow, bool compatibilityMode = false)
        {

            // Cria Objeto
            T lObjeto = Activator.CreateInstance<T>();

            //' Pega as propriedades
            Dictionary<string, PropertyInfo> props = GetCachedProperties<T>();

            // Carrega dados do reader nas propriedades
            List<string> lListaColuna = VerificaColuna(ref pDataRow);

            string[] matchKeys = props.Keys.Intersect(lListaColuna).ToArray();

            for (int i = 0; i < matchKeys.Length; i++)
            {
                PropertyInfo propTO = props[matchKeys[i]];
                typeof(T).InvokeMember(propTO.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetProperty, null, lObjeto, new Object[] { RetornarValorReader(pDataRow[propTO.Name], propTO, compatibilityMode) });
            }

            return lObjeto;

        }

        public static object AtribuiValorPropriedadeReaderByType(Type pType, SqlDataReader pReader, ref List<string> lListaColuna)
        {

            // Cria Objeto
            object lObjeto = Activator.CreateInstance(pType, true);

            // Pega as propriedades
            Dictionary<string, PropertyInfo> props = GetCachedPropertiesByType(pType);

            // Carrega dados do reader nas propriedades

            if (lListaColuna == null || lListaColuna.Count == 0)
                lListaColuna = VerificaColuna(pReader);

            string[] matchKeys = props.Keys.Intersect(lListaColuna).ToArray();

            for (int i = 0; i < matchKeys.Length; i++)
            {
                PropertyInfo propTO = props[matchKeys[i]];
                pType.InvokeMember(propTO.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetProperty, null, lObjeto, new Object[] { RetornarValorReader(pReader[propTO.Name], propTO) });
            }

            return lObjeto;

        }

        public static object AtribuiValorPropriedadeDataTableByType(Type pType, DataRow pDataRow, bool compatibilityMode = false)
        {

            // Cria Objeto
            object lObjeto = Activator.CreateInstance(pType, true);

            // Pega as propriedades
            Dictionary<string, PropertyInfo> props = GetCachedPropertiesByType(pType);

            // Carrega dados do reader nas propriedades
            List<string> lListaColuna = VerificaColuna(ref pDataRow);

            string[] matchKeys = props.Keys.Intersect(lListaColuna).ToArray();

            for (int i = 0; i < matchKeys.Length; i++)
            {
                PropertyInfo propTO = props[matchKeys[i]];
                pType.InvokeMember(propTO.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetProperty, null, lObjeto, new Object[] { RetornarValorReader(pDataRow[propTO.Name], propTO, compatibilityMode) });
            }

            return lObjeto;

        }

        public static void AtribuiValorPropriedade<T>(string property, T pOut, object pValor)
        {
            Dictionary<string, PropertyInfo> props = GetCachedProperties<T>();
            PropertyInfo propTO = props[property.ToUpper()];
            if (propTO != null)
                AtribuiValorObjeto(propTO, pOut, pValor);
        }

        #endregion


        #region metodos privados de apoio

        private static object RetornarValorObjeto<T>(PropertyInfo info, T pObj)
        {
            if ((!object.ReferenceEquals(info, null)))
            {
                return info.GetValue(pObj, null);
            }
            else
            {
                return null;
            }
        }
        private static void AtribuiValorObjeto<T>(PropertyInfo info, T pObj, object pValor)
        {
            if ((!object.ReferenceEquals(info, null)))
                info.SetValue(pObj, pValor, null);
        }

        private static Dictionary<string, PropertyInfo> GetCachedProperties<T>()
        {

            PropertyInfo[] props = new PropertyInfo[-1 + 1];
            Dictionary<string, PropertyInfo> dicProps = new Dictionary<string, PropertyInfo>();

            if (m_CachePropriedadesLock.TryEnterUpgradeableReadLock(200))
            {
                try
                {
                    if (!m_CachePropriedades.TryGetValue(typeof(T).FullName, out dicProps))
                    {
                        props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        dicProps = props.ToDictionary(item => item.Name.Trim().ToUpper(), item => item);


                        if (m_CachePropriedadesLock.TryEnterWriteLock(200))
                        {
                            try
                            {
                                m_CachePropriedades.Add(typeof(T).FullName, dicProps);
                            }
                            finally
                            {
                                m_CachePropriedadesLock.ExitWriteLock();
                            }
                        }
                    }
                }
                finally
                {
                    m_CachePropriedadesLock.ExitUpgradeableReadLock();
                }
                return dicProps;
            }
            else
            {
                props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                return props.ToDictionary(item => item.Name.Trim().ToUpper(), item => item);
            }

        }

        private static List<string> VerificaColuna(SqlDataReader pReader)
        {

            List<string> lListaColuna = new List<string>();
            System.Data.DataTable lSchema = pReader.GetSchemaTable();

            for (int i = 0; i <= lSchema.Rows.Count - 1; i++)
            {
                lListaColuna.Add(lSchema.Rows[i]["ColumnName"].ToString().ToUpper());
            }

            return lListaColuna;

        }


        private static List<string> VerificaColuna(ref DataRow pDataRow)
        {

            List<string> lListaColuna = new List<string>();
            DataTable pDataTable = pDataRow.Table;

            for (int i = 0; i < pDataTable.Columns.Count; i++)
            {
                lListaColuna.Add(pDataTable.Columns[i].ColumnName.ToString().ToUpper());
            }

            return lListaColuna;

        }

        private static object RetornarValorReader(object value, PropertyInfo prop, bool compatibilityMode = false)
        {
            if ((!object.ReferenceEquals(value, DBNull.Value)))
            {
                if (!compatibilityMode)
                    return value;
                if (Util.Utilitarios.isNumberic(value))
                {
                    if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                        return Convert.ToInt32(value);
                    if (prop.PropertyType == typeof(long) || prop.PropertyType == typeof(long?))
                        return Convert.ToInt64(value);
                    if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?))
                        return Convert.ToDecimal(value);
                    if (prop.PropertyType == typeof(short) || prop.PropertyType == typeof(short?))
                        return Convert.ToInt16(value);
                    if (prop.PropertyType == typeof(byte) || prop.PropertyType == typeof(byte?))
                        return Convert.ToByte(value);
                    if (prop.PropertyType == typeof(double) || prop.PropertyType == typeof(double?))
                        return Convert.ToDouble(value);
                }
                return value;
            }
            else
            {
                return null;
            }
        }

        private static Dictionary<string, PropertyInfo> GetCachedPropertiesByType(Type pType)
        {

            PropertyInfo[] props = new PropertyInfo[-1 + 1];
            Dictionary<string, PropertyInfo> dicProps = new Dictionary<string, PropertyInfo>();

            if (m_CachePropriedadesLock.TryEnterUpgradeableReadLock(200))
            {
                try
                {
                    if (!m_CachePropriedades.TryGetValue(pType.FullName, out dicProps))
                    {
                        props = pType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        dicProps = props.ToDictionary(item => item.Name.Trim().ToUpper(), item => item);

                        if (m_CachePropriedadesLock.TryEnterWriteLock(200))
                        {
                            try
                            {
                                m_CachePropriedades.Add(pType.FullName, dicProps);
                            }
                            finally
                            {
                                m_CachePropriedadesLock.ExitWriteLock();
                            }
                        }
                    }
                }
                finally
                {
                    m_CachePropriedadesLock.ExitUpgradeableReadLock();
                }
                return dicProps;
            }
            else
            {
                props = pType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                return props.ToDictionary(item => item.Name.Trim().ToUpper(), item => item);
            }

        }

        #endregion

    }
}
