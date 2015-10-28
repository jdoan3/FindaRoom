using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FindaRoom.Models
{
    public class FbInfo
    {
        public FbInfo()
        {
            this.friendsList = new List<friends>();
            this.mutualFriendsList = new List<mutualFriends>();
            this.educationList = new List<education>();
            this.workHistoryList = new List<workHistory>();
            this.albumsList = new List<albums>();
        }
        [Key]
        public int id { get; set; }
        public string fbId { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string link { get; set; }
        public string gender { get; set; }
        public string imageURL { get; set; }
        public string locale { get; set; }
        public string aboutMe { get; set; }
        public string birthday { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
        public List<mutualFriends> mutualFriendsList { get; set; }
        public List<friends> friendsList { get; set; }
        public List<education> educationList { get; set; }
        public List<workHistory> workHistoryList { get; set; }
        public List<albums> albumsList { get; set; }
    }
    public class mutualFriends
    {
        [Key]
        public int mutualfriend_id { get; set; }
        public string id { get; set; }
        public string name { get; set; }
    }
    public class friends
    {
        [Key]
        public int friend_id { get; set; }
        public string id { get; set; }
        public string name { get; set; }
    }
    public class education
    {
        [Key]
        public int education_id { get; set; }
        public string id { get; set;}
        public string name { get; set; }
        public string year { get; set; }
    }
    public class workHistory
    {
        [Key]
        public int workHistory_id { get; set; }
        public string id { get; set; }
        public string name {get; set;}
    }
    public class albums
    {
        [Key]
        public int albums_id { get; set; }
        public int friend_id { get; set; }
        public string id { get; set; }
        public string name { get; set; }
    
    }
}