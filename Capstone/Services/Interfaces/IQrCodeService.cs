namespace Capstone.Services.Interfaces
{
    public interface IQrCodeService
    {
        public byte[] GenerateQRCode(string text);

    }
}
