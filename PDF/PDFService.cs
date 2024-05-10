using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static System.Environment;

namespace ArmsFW.Services.PDF
{
    public class PDFService
    {
        public static async Task<PDFFile> Load(string fileSource, string tempFolder = "")
        {
            var pdfFile = new PDFFile();
            Exception e = null;
            try
            {
                //Assincronamente, recupera o conteudo das paginas do arquivo
                var dicDocs = await Task.Run<List<PDFDoc>>(()=> {

                    List<PDFDoc> listDocs = new List<PDFDoc>();

                    try
                    {
                        tempFolder = $@"{GetTempFolder(tempFolder)}\{Path.GetFileNameWithoutExtension(fileSource)}";

                        //Tenta recuperar inforações do cache
                        if (Directory.Exists(tempFolder))
                        {
                            var docInfo = new DocInfo(filename: $@"{tempFolder}\001.TXT", PageNumber: 1);

                            docInfo.FileSource = fileSource;

                            var doc = new PDFDoc(docInfo, false);

                            listDocs.Add(doc);
                        }
                        else
                        {
                            return iTextService.GetPDFDocFromPages(fileSource, tempFolder);
                        }
                    }
                    catch (Exception ex)
                    {
                        e = ex;
                    }

                    return listDocs;

                });

                if (e!=null)
                {
                    throw e;
                }

                if (dicDocs?.Count > 0) pdfFile.Docs.AddRange(dicDocs);

                pdfFile.FilePath = fileSource;

                pdfFile.StatusLoadResult = new LoadResult(true, "O Documento é um PDF valido e foi carregado com sucesso");
            }
            catch (Exception ex)
            {
                pdfFile.StatusLoadResult = new LoadResult(false, $"Ocorreu uma falha inesperada no carregamento do documento. Detalhe {ex.Message}");
                pdfFile.StatusLoadResult.Exceptions.Add(new Exception("Falha ao carregar o arquivo PDF", ex));
            }        

            return pdfFile;
        }

        private static string GetTempFolder(string tempFolder = "")
        {
            try
            {
                if (string.IsNullOrEmpty(tempFolder)) tempFolder = GetEnvironmentVariable("temp") + "_pdf";

                if (!System.IO.Directory.Exists(tempFolder)) System.IO.Directory.CreateDirectory(tempFolder);
            }
            catch 
            {
            }

            return tempFolder;
        }
    }
}
