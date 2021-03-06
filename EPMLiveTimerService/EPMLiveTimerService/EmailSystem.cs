﻿using System;
using System.Net.Mail;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using EpmCoreFunctions = EPMLiveCore.CoreFunctions;

namespace TimerService
{
    internal class EmailSystem
    {
        private const string NameKey = "{ToUser_Name}";
        private const string EmailKey = "{ToUser_Email}";
        private const string UsernameKey = "{ToUser_Username}";

        public static void SendFullEmail(string body, string subject, bool hideFrom, SPUser fromUser, SPUser toUser)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentNullException(nameof(subject));
            }

            if (fromUser == null)
            {
                throw new ArgumentNullException(nameof(fromUser));
            }

            if (toUser == null)
            {
                throw new ArgumentNullException(nameof(toUser));
            }

            var spWebAdmin = SPAdministrationWebApplication.Local;
            var sMailSvr = spWebAdmin.OutboundMailServiceInstance.Server.Address;

            using (var mailMsg = new MailMessage())
            {
                if (hideFrom)
                {
                    mailMsg.From = new MailAddress(spWebAdmin.OutboundMailSenderAddress);
                }
                else
                {
                    mailMsg.From = fromUser.Email == string.Empty
                        ? new MailAddress(spWebAdmin.OutboundMailSenderAddress, fromUser.Name)
                        : new MailAddress(fromUser.Email, fromUser.Name);
                }

                body = body.Replace(NameKey, toUser.Name)
                    .Replace(EmailKey, toUser.Email)
                    .Replace(UsernameKey, EpmCoreFunctions.GetJustUsername(toUser.LoginName));

                subject = subject.Replace(NameKey, toUser.Name)
                    .Replace(EmailKey, toUser.Email)
                    .Replace(UsernameKey, EpmCoreFunctions.GetJustUsername(toUser.LoginName));

                mailMsg.To.Add(new MailAddress(toUser.Email));
                mailMsg.Subject = subject;
                mailMsg.Body = body;
                mailMsg.IsBodyHtml = true;
                
                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Host = sMailSvr;
                    smtpClient.Send(mailMsg);
                }
            }
        }
    }
}
