﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MimeKit;


namespace PerfumeStore.Domain.Models
{
  public class Message
  {
    public List<MailboxAddress> To { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }

    public IFormFileCollection Attachments { get; set; }

    public Message(IEnumerable<string> to, string subject, string content, IFormFileCollection attachments, string recipientName)
    {
      To = new List<MailboxAddress>();

      To.AddRange(to.Select(x => new MailboxAddress(recipientName, x)));
      Subject = subject;
      Content = content;
      Attachments = attachments;
    }
  }
}