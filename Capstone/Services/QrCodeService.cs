using Capstone.Services.Interfaces;
using QRCoder;

public class QRCodeService : IQrCodeService
{
    public byte[] GenerateQRCode(string text)
    {
        // Creazione dell'oggetto QRCodeGenerator
        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        {
            // Creazione dei dati del QR Code con il testo fornito
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q))
            {
                // Creazione dell'oggetto PngByteQRCode a partire dai dati del QR Code
                using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                {
                    // Generazione dell'immagine QR Code come array di byte
                    return qrCode.GetGraphic(20);
                }
            }
        }
    }
}