using ArmsFW.Services.Logging;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ArmsFW.Services.Extensions
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Recupera um Stream a partir de um arry
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Stream GetStream(this byte[] bytes)
        {
            return new MemoryStream(bytes);
        }

        public static void SalvarNoDisco(this byte[] bytes, string localDestino)
        {
            try
            {
                bytes.GetStream().SaveToDiskAsync(localDestino, true);
            }
            catch (Exception ex)
            {
                LogServices.Logar($"SalvarNoDisco() - {ex.Message}");
            }
        }


        public static string ToBase64(this byte[] bytes)
        {
            try
            {
                return Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                return $"Erro ToBase64String() > {ex.Message}";
            }
        }

        public static byte[] GetBytes(this string base64)
        {
            try
            {
                if (base64.StartsWith("data:")) base64 = base64.Split(",".ToCharArray())[1] ?? "";
                if (base64.EndsWith(";")) base64 = base64.Substring(0, base64.Length - 1);

                byte[] conteudoBytes = Convert.FromBase64String(base64);

                return conteudoBytes;
            }
            catch 
            {
                return null;
            }
        }

        public static byte[] GetBytes(this Stream str)
        {
            try
            {
                return new BinaryReader(str).ReadBytes((int)str.Length);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Compara dois conteudos Bytes Array e verifica se os mesmos são iguais
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="bytesCompare">Array a ser comparado</param>
        /// <returns></returns>
        /// <remarks>Esse metodo utiliza a comparação bit a bit</remarks>
        public static bool ByteIsEqual(this byte[] bytes, byte[] bytesCompare)
        {
            try
            {
                int length = bytes.Length;
                if (length != bytesCompare.Length) { return false; }

                if (bytes.SequenceEqual(bytesCompare))
                {
                    return true;
                }

                for (int i = 0; i < length; i++)
                {
                    if (bytes[i] != bytesCompare[i])
                    {
                        return false;
                    }

                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Equals(this byte[] bytes, byte[] bytesCompare) => StructuralComparisons.StructuralEqualityComparer.Equals(bytes, bytesCompare);

        public static byte[] SaveToDiskAsync(this Stream strOrigem, string destinationFileName, bool forceReplace = true)
        {
            try
            {
                if (string.IsNullOrEmpty(destinationFileName)) { destinationFileName = Path.GetTempFileName(); }

                if (File.Exists(destinationFileName))
                {
                    if (forceReplace)
                    {
                        File.Delete(destinationFileName);
                    }
                    else
                    {
                        return null;
                    }
                }

                Directory.CreateDirectory(Path.GetDirectoryName(destinationFileName));

                using (var streamDestino = new FileStream(destinationFileName, FileMode.Create))
                {
                    //Salva no Disco
                    strOrigem.CopyTo(streamDestino);

                    strOrigem.Close();
                    streamDestino.Close();

                    return File.ReadAllBytes(destinationFileName);
                }

            }
            catch (System.Exception)
            {
                return new byte[0];
            }
        }
        ///
    }
}