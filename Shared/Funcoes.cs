using ArmsFW.Services.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace ArmsFW.Services.PDF
{
    public static class PDFTextExtensions
    {
        public static bool ContemTexto(this string texto, string contemTexto)
        {
            return texto.Replace(" ", "").ToUpper().Contains(contemTexto.ToUpper().Replace(" ", ""));
        }

        // Verifica se todas as tags est�o no arquivo
        private static void validaTags(string[] vTags, string[] vLinhas, int AteLinha = 0)
        {
            int intCountTag = 0;
            int intLinha;
            string linha;
            if ((AteLinha == 0))
            {
                AteLinha = vLinhas.Length;
            }

            for (intLinha = 0; (intLinha <= AteLinha); intLinha++)
            {

                linha = vLinhas.GetValue(intLinha).ToString();

                foreach (var tag in vTags)
                {
                    if (linha.Contains(tag))
                    {
                        intCountTag = (intCountTag + 1);
                    }
                }
            }
            return;
        }

        public static dynamic pegarValor(this string strString, int inicial = 0, int tamanho = 0, dynamic retornarCasoErro = null, bool tratarComoTexto = false)
        {
            dynamic ret;
            try
            {
                if ((tamanho == 0))
                {
                    ret = strString.Substring(inicial).Trim();
                }
                else
                {
                    if ((strString.Length
                                < (inicial + tamanho)))
                    {
                        tamanho = (strString.Length - inicial);
                    }

                    ret = strString.Substring(inicial, tamanho).Trim();
                }

                if (tratarComoTexto)
                {
                    return ret;
                }

                if (ret.ToString().Contains("-"))
                {
                    if (((string)ret).Replace("-", "").IsNumeric())
                    {
                        ret = (double.Parse(((string)ret).Replace("-", "")) * -1);
                    }

                }
                else if (((retornarCasoErro == 0)
                            && !(retornarCasoErro == null)))
                {
                    if (((string)ret).IsNumeric())
                    {
                        ret = double.Parse(ret);
                    }
                    else
                    {
                        ret = retornarCasoErro;
                    }

                }

            }
            catch {
                ret = retornarCasoErro??"";
            }

            return ret;
        }

        /// <summary>
        /// Atalho para pegar o texto ate a quantidade de caracteres iniciando no caractere 0
        /// </summary>
        /// <param name="strString"></param>
        /// <param name="tamanho"></param>
        /// <param name="retornarCasoErro"></param>
        /// <returns></returns>
        public static string pegarPrimeiroTexto(this string strString, int tamanho = 0, dynamic retornarCasoErro = null) => pegarTexto(strString, 0, tamanho, retornarCasoErro);
        /// <summary>
        /// Retorna uma parte de um texto com base na posição inicial e o tamanho final
        /// </summary>
        /// <param name="strString"></param>
        /// <param name="inicial"></param>
        /// <param name="tamanho"></param>
        /// <param name="retornarCasoErro"></param>
        /// <returns></returns>
        public static string pegarTexto(this string strString, int inicial = 0, int tamanho = 0, dynamic retornarCasoErro = null)
        {
            dynamic ret;

            try
            {
                if ((tamanho == 0))
                {
                    ret = strString.Substring(inicial).Trim();
                }
                else
                {
                    if ((strString.Length
                                < (inicial + tamanho)))
                    {
                        tamanho = (strString.Length - inicial);
                    }

                    ret = strString.Substring(inicial, tamanho).Trim();
                }

                if (ret.ToString().Contains("-"))
                {
                    if (((string)ret).Replace("-", "").IsNumeric())
                    {
                        ret = (double.Parse(((string)ret).Replace("-", "")) * -1);
                    }

                }
                else if (((retornarCasoErro == "0")
                            && !(retornarCasoErro == null)))
                {
                    if (((string)ret).IsNumeric())
                    {
                        ret = double.Parse(ret);
                    }
                    else
                    {
                        ret = retornarCasoErro;
                    }

                }
            }
            catch
            {
                ret = retornarCasoErro ?? "";
            }

            return ret;
        }

        public static string pegarUltimoTexto(this string strString, int tamanho = 0) {

            var _textoInvertido = InverterString(strString.TrimEnd());

            var texto_quebrado = _textoInvertido.Substring(0, tamanho).Trim();

            return InverterString(texto_quebrado);
        }

        public static int Posicao(this string str, string valorProcura )
        {
            try
            {
                return str.IndexOf(valorProcura);
            }
            catch
            {
                return 0;
            }
        }


        public static object FormatarValor(this string trimLine)
        {
            dynamic retValue;
            retValue = trimLine.Substring(131, (trimLine.Length - 131)).Replace("R$", "").Trim();
            if (retValue.ToString().Contains("-"))
            {
                retValue = (double.Parse(retValue.ToString().Replace("-", "")) * -1);
            }
            else
            {
                retValue = double.Parse(retValue);
            }

            return retValue;
        }


        public static string PegarUltimoNumeros(this string pText )
        {
            string _numero = "";
            try
            {
                var _textoInvertido = InverterString(pText.TrimEnd());
                //Ajusta a string em caso de valor negativo
                if (_textoInvertido.IndexOf("-")>0)
                {
                    _textoInvertido = _textoInvertido.Substring(0, _textoInvertido.IndexOf("-")+1);
                    _numero = _textoInvertido.Replace(" ", "");
                }
                else
                {
                    _numero = _textoInvertido.Substring(0, _textoInvertido.IndexOf(" "));
                }
                _numero = InverterString(_numero);
            }
            catch (Exception)
            {
                return _numero;
            }
            return _numero;
        }

        /// <summary>
        /// Retorna o texto invertido
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string InverterString(this string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }


        public static string PegarNumero(this string strTrataTexto, int lngStart)
        {
            var info = PegarNumeroInfo(strTrataTexto, lngStart);
            return info.Valor;
        }


        public static string PegarNumero2(this string strTrataTexto, int lngStart)
        {
            string strValor = "";
            bool achou_numero = false;
            var arrChars = strTrataTexto.Substring(lngStart).ToCharArray();
            foreach (char caract in arrChars)
            {
                lngStart = (lngStart + 1);

                if (
                       char.GetUnicodeCategory(caract) == System.Globalization.UnicodeCategory.DecimalDigitNumber                    
                    || char.GetUnicodeCategory(caract) == System.Globalization.UnicodeCategory.OtherPunctuation
                    || char.GetUnicodeCategory(caract) == System.Globalization.UnicodeCategory.DashPunctuation
                 )
                {
                    if ((char.GetUnicodeCategory(caract) == System.Globalization.UnicodeCategory.DashPunctuation))
                    {
                        var caractere = (char)strTrataTexto.Substring((lngStart + 1)).Substring(0, 1).ToCharArray().GetValue(0);

                        if ((char.GetUnicodeCategory(caractere) == System.Globalization.UnicodeCategory.DecimalDigitNumber))
                        {
                            strValor = (strValor + string.Concat(caract));
                            achou_numero = true;
                        }
                    }
                    else
                    {
                        strValor = (strValor + string.Concat(caract));
                        achou_numero = true;
                    }
                }
                else
                {
                    if (achou_numero)
                    {
                        break;
                    }
                }

            }

            strValor = strValor.Trim();
            return strValor;
        }

        public static CampoTexto PegarNumeroInfo(this string strTrataTexto, int lngStart)
        {
            string strValor = "";
            bool achou_numero = false;
            var arrChars = strTrataTexto.Substring(lngStart).ToCharArray();
            foreach (char caract in arrChars)
            {
                lngStart = (lngStart + 1);

                if (
                       char.GetUnicodeCategory(caract) == System.Globalization.UnicodeCategory.DecimalDigitNumber
                    || char.GetUnicodeCategory(caract) == System.Globalization.UnicodeCategory.OtherPunctuation
                    || char.GetUnicodeCategory(caract) == System.Globalization.UnicodeCategory.DashPunctuation
                 )
                {
                    if ((char.GetUnicodeCategory(caract) == System.Globalization.UnicodeCategory.DashPunctuation))
                    {
                        var caractere = (char)strTrataTexto.Substring((lngStart + 1)).Substring(0, 1).ToCharArray().GetValue(0);

                        if ((char.GetUnicodeCategory(caractere) == System.Globalization.UnicodeCategory.DecimalDigitNumber))
                        {
                            strValor = (strValor + string.Concat(caract));
                            strValor = strValor.Trim();
                            achou_numero = true;
                        }
                    }
                    else
                    {
                       strValor = (strValor + string.Concat(caract));
                       strValor = strValor.Trim();
                       achou_numero = true;
                    }
                }
                else
                {
                    if (achou_numero)
                    {
                        break;
                    }
                }

            }
            return new CampoTexto() { Valor = strValor.Trim(), ProximoCaract = lngStart } ;
        }

        public static string PegarTextoEntreEspaco(this string strTrataTexto, ref int lngStart)
    {
        string strValor = "";
        bool achou_numero = false;
        int newStart;

        if ((strTrataTexto.Length <= lngStart))
        {
            lngStart = strTrataTexto.Length;
        }

        newStart = lngStart;
        foreach (char caract in strTrataTexto.Substring(lngStart).ToCharArray())
        {
            if (caract.ToString()!= " ")
            {
                break;
            }

            lngStart = (lngStart + 1);
        }

        foreach (char caract in strTrataTexto.Substring(lngStart).ToCharArray())
        {
            lngStart = (lngStart + 1);
            if ((char.GetUnicodeCategory(caract) == System.Globalization.UnicodeCategory.SpaceSeparator))
            {
                break;
            }

            strValor = (strValor + string.Concat(caract));
            if (achou_numero)
            {
                break;
            }
        }
        strValor = strValor.Trim();
        return strValor;
        }
        public static long PegarUltimaLinha(string[] vLinhas, params string[] vTextoFinal)
        {
            int intEncontrado = 0;
            try
            {
                for (var iLinha = 0; (iLinha
                            <= (vLinhas.Length - 1)); iLinha++)
                {
                    intEncontrado = 0;
                    foreach (var vText in vTextoFinal)
                    {
                        if (vLinhas.GetValue(iLinha).ToString().Trim().Contains(vText))
                        {
                            intEncontrado = (intEncontrado + 1);
                        }

                    }

                    if ((intEncontrado == vTextoFinal.Length))
                    {
                        return iLinha;
                    }
                }
                return vLinhas.Length;
            }
            catch 
            {
                return vLinhas.Length;
            }

        }

        static List<string> _erros;
        static List<string> Erros
        {
            get
            {
                if ((_erros == null))
                {
                    _erros = new List<string>();
                }
                return _erros;
            }
            set
            {
                _erros = value;
            }
        }

        public static void addLog(string pContext, int codigoRetorno, string pMensagem)
        {
            if ((_erros == null))
            {
                _erros = new List<string>();
            }
            _erros.Add((pContext + ("|" + (codigoRetorno + ("|" + pMensagem)))));
        }

        static void ClearLog()
        {
            Erros.Clear();
        }

        static System.Xml.XmlElement eleInfo;
        static System.Xml.XmlElement xRoot = null;
        static dynamic xParent;
        


        public static void SaveClassToXML(object instancia, string strOutputFile)
        {
            System.Xml.Serialization.XmlSerializer Serializer;
            FileStream DataFile;
            try
            {
                if ((strOutputFile == ""))
                {
                    strOutputFile = (Environment.GetEnvironmentVariable("temp") + ("\\"+ (instancia.GetType().Name + ".XML")));
                }

                Serializer = new System.Xml.Serialization.XmlSerializer(instancia.GetType());
                DataFile = new FileStream(strOutputFile, FileMode.Create, FileAccess.Write, FileShare.None);
                Serializer.Serialize(DataFile, instancia);
                DataFile.Close();
                instancia = null;
            }
            catch
            {
            }

        }

        private static void AddToXmlOutput(dynamic doc, dynamic cCampos, dynamic strOutputFile, dynamic parent)
        {
            xParent = doc.CreateElement(parent);
            // Adiciona no elemento raiz
            xRoot.AppendChild(xParent);
            // Cria um elemento para cada campo/value
            foreach (var value in cCampos)
            {
                eleInfo = xParent.AppendChild(doc.CreateElement(value.Split("|")[0].ToString));
                switch (value.Split("|")[1].ToString)
                {
                    case "DT":
                        eleInfo.InnerText = DateTime.Parse(value.Split("|")[2].ToString);
                        break;
                    case "NUM":
                        eleInfo.InnerText = double.Parse(value.Split("|")[2].ToString);
                        break;
                    default:
                        eleInfo.InnerText = value.Split("|")[2].ToString;
                        break;
                }
            }

            doc.Save(strOutputFile);
            cCampos.Clear();
        }

    }
}
