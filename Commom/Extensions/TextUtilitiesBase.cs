using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ArmsFW.Utilities.Text
{
    public static class TextUtilities
    {
        public static string TrataUnicode64258(this string texto)
        {
            string textoSaida="";
            foreach (char c in texto.ToCharArray())
            {
                //Caso encontre esse codigo char 64258, troca pelas letras "fl"
                if ((int)c == 64258)
                {
                    textoSaida += "fl";
                }
                else
                {
                    textoSaida += c;
                }
                
            }

            return textoSaida;
        }

        public static List<Palavra> ExtrairPalavrasRegex(this string texto)
        {
            List<Palavra> Palavras = new List<Palavra>();

            var mPalavras = Regex.Matches(texto, @"\S+", RegexOptions.IgnoreCase);

            foreach (Match m in mPalavras)
            {
                var p = new Palavra(m.Value, m.Index) { Index = Palavras.Count + 1 };
                Palavras.Add(p);
            }

            return Palavras;
        }

        public static List<Palavra> ExtrairPalavras(this string texto)
        {
            List<Palavra> Palavras = new List<Palavra>();
            var caracteres = texto.ToCharArray();
            string palavra = "";
            int posicaoInicio = 0;

            for (int i = 0; i < caracteres.Length; i++)
            {
                if (char.IsSeparator(caracteres[i]))
                {

                    if (!string.IsNullOrEmpty(palavra))
                    {
                        var p = new Palavra(palavra, posicaoInicio) { Index = Palavras.Count + 1 };
                        Palavras.Add(p);
                    }

                    palavra = "";
                    posicaoInicio = i + 1;
                }
                else
                {
                    if (((int)caracteres[i]) != 10 && ((int)caracteres[i]) != 13 && ((int)caracteres[i]) != 8226)
                    {
                        palavra += caracteres[i];
                    }


                }
            }
            if (!string.IsNullOrEmpty(palavra))
            {
                var p = new Palavra(palavra, posicaoInicio) { Index = Palavras.Count + 1 };
                Palavras.Add(p);
            }
            return Palavras;
        }

        public static List<Palavra> Correspondentes(this List<Palavra> listaEntrada, List<Palavra> listaComparacao)
        {
            try
            {
              return  listaEntrada.Intersect(listaComparacao, new PalavraComparer()).ToList();
            }
            catch (System.Exception)
            {
                return new List<Palavra>();
            }
        }

        public static List<Palavra> NaoCorrespondentes(this List<Palavra> listaEntrada, List<Palavra> listaComparacao)
        {
            try
            {
                return listaEntrada.Except(listaComparacao, new PalavraComparer()).ToList();
            }
            catch (System.Exception)
            {
                return new List<Palavra>();
            }
        }


        public static List<Palavra> CompararTexto(this List<Palavra> listaEntrada, string pesquisarNoTexto)
        {
            List<Palavra> Palavras = new List<Palavra>();
            List<Palavra> PalavrasNaoEncontradas = new List<Palavra>();

            var palavrasDeEntrada = listaEntrada;
            int inicio = 0;
            var separador = @"(\s+|.)";
            var tamanhoDoTexto = pesquisarNoTexto.Length;

            foreach (var palavra in palavrasDeEntrada)
            {
                var textoDePesquisa = pesquisarNoTexto.Substring(inicio, (tamanhoDoTexto - inicio));

                var textoInput = palavra.Texto.Replace("+", "[plus]").Replace(" ", separador).Replace("[plus]", "\\+") + separador;

                var mPalavras = Regex.Match(textoDePesquisa, textoInput, RegexOptions.IgnoreCase);

                if (mPalavras.Success)
                {
                    inicio += mPalavras.Index + mPalavras.Value.Length;
                    Palavras.Add(palavra);
                }
                else
                {
                    PalavrasNaoEncontradas.Add(palavra);
                }
            }
            return Palavras;

        }
    }

    public class Palavra
    {
        public Palavra(string palavra, int inicio)
        {
            this.Texto = palavra;
            this.PosicaoInicial = inicio;
        }
        public string Texto { get; set; }
        public int PosicaoInicial { get; set; }
        public int PosicaoFinal => PosicaoInicial + Tamanho;
        public int Tamanho => Texto.Length;
        public int Index { get; set; }
        public override string ToString() => $"{Index} | {Texto} | {PosicaoInicial} - {PosicaoFinal} ({Tamanho})";
    }

    public class PalavraComparer : IEqualityComparer<Palavra>
    {
        public bool Equals(Palavra x, Palavra y)
        {
            if (string.Equals(x.Texto.Trim(), y.Texto.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(Palavra obj)
        {
            return obj.Texto.GetHashCode();
        }
    }

    public class TextAnaliser
    {
        public static TextAnaliser CompararPalavrasEntreTexto(List<Palavra> textoDeEntrada, string textoComparacao) {
            var analiser = new TextAnaliser(textoDeEntrada, textoComparacao);
            return analiser.Analisar();
        }

        public List<Palavra> PalavrasDeEntrada { get; private set; } = new List<Palavra>();

        public TextAnaliser(List<Palavra> textoDeEntrada)
        {
            PalavrasDeEntrada = textoDeEntrada;
        }

        public TextAnaliser(List<Palavra> textoDeEntrada, string textoComparacao)
        {
            PalavrasDeEntrada = textoDeEntrada;
            TextoDeComparacao = textoComparacao;
        }

        public TextAnaliser(string textoDeEntrada)
        {
            TextoDeEntrada = textoDeEntrada;
        }

        public TextAnaliser(string textoDeEntrada, string textoComparacao)
        {
            TextoDeEntrada = textoDeEntrada;
            TextoDeComparacao = textoComparacao;
        }
        public List<Palavra> PalavrasEncontradas { get; private set; } = new List<Palavra>();
        public List<Palavra> PalavrasNaoEncontradas { get; private set; } = new List<Palavra>();
        public List<Palavra> PalavrasEncontradasNoTextoComparado { get; private set; } = new List<Palavra>();

        public string TextoDeEntrada { get; set; }
        public string TextoDeComparacao { get; set; }
        public decimal TaxaDeAcerto => Convert.ToDecimal(PalavrasEncontradas.Count) / Convert.ToDecimal(PalavrasDeEntrada.Count);

        public override string ToString() => $"Total de Palavras : {PalavrasDeEntrada.Count} / Encontradas : {PalavrasEncontradas.Count} / Nao Encontradas : {PalavrasNaoEncontradas.Count} / Taxa de Acerto : {TaxaDeAcerto }";
        public TextAnaliser Analisar()
        {
            List<Palavra> _Encontradas = new List<Palavra>();
            List<Palavra> _EncontradasNoTextoComparado = new List<Palavra>();
            List<Palavra> _NaoEncontradas = new List<Palavra>();

            //Trata um caractere UNICODE 64258 (esse codigo caractere utiliza as letras F e L minusculas)
            this.TextoDeComparacao = this.TextoDeComparacao.TrataUnicode64258();

            var palavrasDeEntrada = this.PalavrasDeEntrada;
            int inicio = 0;
         
            var tamanhoDoTexto = this.TextoDeComparacao.Length;

            var str = new String((char)32, this.TextoDeComparacao.Length);
            string textoDePesquisa = "";

            foreach (var palavra in palavrasDeEntrada)
            {
                if (inicio>0)
                {

                    textoDePesquisa = $"{str.Substring(0, inicio)}{this.TextoDeComparacao.Substring(inicio, (tamanhoDoTexto - inicio))}";
                }
                else
                {
                    textoDePesquisa = $"{this.TextoDeComparacao.Substring(inicio, (tamanhoDoTexto - inicio))}";
                }
                

                var textoInput = palavra.Texto.Replace("+", "[plus]").Replace("[plus]", "\\+");

                //if (palavra.Index == 12)
                //{
                //    System.Diagnostics.Debugger.Break();
                //}

                textoDePesquisa = textoDePesquisa.Replace((char)(64258), (char)102);

                var mPalavras = Regex.Match(textoDePesquisa, textoInput, RegexOptions.IgnoreCase);

                if (mPalavras.Success)
                {
                    _Encontradas.Add(palavra);
                    inicio = mPalavras.Index + mPalavras.Value.Length;
                    _EncontradasNoTextoComparado.Add(new Palavra(mPalavras.Value, mPalavras.Index));
                }
                else
                {
                    _NaoEncontradas.Add(palavra);
                }
            }

            this.PalavrasEncontradas = _Encontradas;
            this.PalavrasNaoEncontradas = _NaoEncontradas;
            this.PalavrasEncontradasNoTextoComparado = _EncontradasNoTextoComparado;
            
            return this;
        }
    }
}