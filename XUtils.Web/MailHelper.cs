using System;
using System.Net;
using System.Net.Mail;
namespace XUtils.Web
{
	public class MailHelper
	{
		private string serverAddress;
		private string serverUserName;
		private string serverPassWord;
		private bool async;
		private bool isBodyHtml;
		private string subject;
		private string body;
		public string ServerAddress
		{
			get
			{
				return this.serverAddress;
			}
			set
			{
				this.serverAddress = value;
			}
		}
		public string ServerUserName
		{
			get
			{
				return this.serverUserName;
			}
			set
			{
				this.serverUserName = value;
			}
		}
		public string ServerPassWord
		{
			get
			{
				return this.serverPassWord;
			}
			set
			{
				this.serverPassWord = value;
			}
		}
		public bool Async
		{
			get
			{
				return this.async;
			}
			set
			{
				this.async = value;
			}
		}
		public bool IsBodyHtml
		{
			get
			{
				return this.isBodyHtml;
			}
			set
			{
				this.isBodyHtml = value;
			}
		}
		public string Subject
		{
			get
			{
				return this.subject;
			}
			set
			{
				this.subject = value;
			}
		}
		public string Body
		{
			get
			{
				return this.body;
			}
			set
			{
				this.body = value;
			}
		}
		public bool Send(string[] ToAddress, string FromAddress)
		{
			MailMessage mailMessage = new MailMessage();
			for (int i = 0; i < ToAddress.Length; i++)
			{
				string addresses = ToAddress[i];
				mailMessage.To.Add(addresses);
			}
			if (!string.IsNullOrEmpty(FromAddress))
			{
				mailMessage.From = new MailAddress(FromAddress);
			}
			return this.Send(mailMessage);
		}
		public bool Send(string ToAddress, string FromAddress)
		{
			MailMessage mailMessage = new MailMessage();
			mailMessage.To.Add(new MailAddress(ToAddress));
			if (!string.IsNullOrEmpty(FromAddress))
			{
				mailMessage.From = new MailAddress(FromAddress);
			}
			else
			{
				mailMessage.From = new MailAddress(ToAddress);
			}
			return this.Send(mailMessage);
		}
		private bool Send(MailMessage mm)
		{
			mm.Subject = this.Subject;
			mm.Body = this.Body;
			mm.IsBodyHtml = this.IsBodyHtml;
			mm.Priority = MailPriority.High;
			SmtpClient smtpClient = new SmtpClient();
			smtpClient.Host = this.ServerAddress;
			smtpClient.Credentials = new NetworkCredential
			{
				UserName = this.ServerUserName,
				Password = this.ServerPassWord
			};
			bool result;
			try
			{
				if (!this.Async)
				{
					smtpClient.Send(mm);
				}
				else
				{
					smtpClient.SendAsync(mm, mm);
				}
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}
	}
}
