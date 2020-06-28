using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using System.Collections.Generic;

public class SimpleEmailSender
{
    private static string userEmail;
    public static EmailSettings emailSettings = new EmailSettings { STMPClient = "smtp.gmail.com", SMTPPort = 587, UserName = "dennydenii@gmail.com", UserPass = "cmxkvmntsfqpguxc" };
    public struct EmailSettings
    {
        public string STMPClient;
        public int SMTPPort;
        public string UserName;
        public string UserPass;
    }

    public static void Send(List<string> attachedFiles, Action<object, AsyncCompletedEventArgs> callback)
    {
        try
        {
            SmtpClient mailServer = new SmtpClient(emailSettings.STMPClient, emailSettings.SMTPPort);
            mailServer.EnableSsl = true;
            mailServer.Credentials = new NetworkCredential(emailSettings.UserName, emailSettings.UserPass) as ICredentialsByHost;
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };

            MailMessage msg = new MailMessage(emailSettings.UserName, "dennydenii@gmail.com");
            msg.Subject = "Dados do jogo";
            if(userEmail == null)
                msg.Body = "userEmail";
            else
                msg.Body = userEmail;
            foreach (var attachFile in attachedFiles)
            {
                if (attachFile != null && !attachFile.Equals(""))
                    if (File.Exists(attachFile))
                        msg.Attachments.Add(new Attachment(attachFile));
            }
            mailServer.SendCompleted += new SendCompletedEventHandler(callback);
            mailServer.SendAsync(msg, "");

            Debug.Log("SimpleEmail: Sending Email.");
        }
        catch (Exception ex)
        {
            Debug.LogWarning("SimpleEmail: " + ex);
            callback("", new AsyncCompletedEventArgs(ex, true, ""));
        }
    }

    public static void SetEmail(string email)
    {
        userEmail = email;
    }
    public static string GetEmail(){
        return userEmail;
    }
}

