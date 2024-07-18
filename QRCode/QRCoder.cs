using ArmsFW.Services.Extensions;
using ArmsFW.Services.Logging;
using ArmsFW.Services.Shared;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsFW.Tools
{
    //Creditos :
    //Balta > https://balta.io/blog/aspnet-qrcode#:~:text=Gerando%20o%20QRCode,QRCodeGenerator()%3B%20var%20qrCodeData%20%3D%20qrGenerator.
    //Macoratti > https://www.macoratti.net/20/01/aspc_qrcode1.htm
    public static class QrCode
    {
        public static async Task<Result<QrResultado>> Gerar(string texto)
        {
            try
            {
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(texto, QRCodeGenerator.ECCLevel.Q);

                //Codigo que funciona na versão 1.4 para net50. Nao funciona para net6 em diante
                //var qrCode = new QRCode(qrCodeData);
                //var qrCodeImage = qrCode.GetGraphic(10);
                //return ResultBase<QrResultado>.Sucesso("Código QR gerado com sucesso", new QrResultado { Imagem = qrCodeImage, Bytes = ConverterParaBytes(qrCodeImage) });

                var qrCode = new BitmapByteQRCode(qrCodeData);
                var qrCodeImage = qrCode.GetGraphic(10);
                return ResultBase<QrResultado>.Sucesso("Código QR gerado com sucesso", new QrResultado { Bytes = qrCodeImage });

            }
            catch (Exception ex)
            {
                await ex.Logar();
                return ResultBase<QrResultado>.Erro($"Falha ao gerar o QrCode. Erro : {ex.Message}");
            }
        }

        private static byte[] ConverterParaBytes(Image img)
        {
            try
            {
                using var stream = new MemoryStream();
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
            catch 
            {
            }
            return new byte[0];
        }
    }

    public class QrResultado
    {
        public Bitmap Imagem { get; set; }
        public byte[] Bytes { get; set; }
        public string Base64 => Bytes.ToBase64();
    }
}
