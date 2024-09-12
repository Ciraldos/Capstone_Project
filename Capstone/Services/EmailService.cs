using Capstone.Models;
using Capstone.Services.Interfaces;
using System.Net;
using System.Net.Mail;

public class EmailService : IEmailService
{
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly bool _enableSsl;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailService(IConfiguration configuration)
    {
        _smtpHost = configuration["SmtpSettings:Host"]!;
        _smtpPort = int.Parse(configuration["SmtpSettings:Port"]!);
        _smtpUsername = configuration["SmtpSettings:Username"]!;
        _smtpPassword = configuration["SmtpSettings:Password"]!;
        _enableSsl = bool.Parse(configuration["SmtpSettings:EnableSsl"]!);
        _fromEmail = configuration["SmtpSettings:FromEmail"]!;
        _fromName = configuration["SmtpSettings:FromName"]!;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, List<Ticket> tickets)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_fromEmail, _fromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);

        foreach (var ticket in tickets)
        {
            // Aggiungi i QR code come allegati
            var qrCodeAttachment = new Attachment(new MemoryStream(ticket.QRCodeImage), $"{ticket.NumTicket}.png", "image/png");
            mailMessage.Attachments.Add(qrCodeAttachment);
        }

        var smtpClient = new SmtpClient(_smtpHost)
        {
            Port = _smtpPort,
            Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
            EnableSsl = _enableSsl
        };

        await smtpClient.SendMailAsync(mailMessage);
    }

}
