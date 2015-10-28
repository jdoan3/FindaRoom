using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FindaRoom.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public int idString { get; set; }
        [Display(Name = "From:")]
        public string UserSender { get; set; }
        public string SenderId { get; set; }
        [Display(Name = "To:")]
        public string UserReceiver { get; set; }
        public string ReceiverId { get; set; }
        public string MessageBody { get; set; }
        public string Subject { get; set; }
        public DateTime SentAt { get; set; }
        public bool Unread { get; set; }
    }

}