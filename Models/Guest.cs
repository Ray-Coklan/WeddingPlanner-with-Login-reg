using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class Guest : BaseEntity
    {
        public int GuestId {get;set;}
        public User user {get;set;} 
        public Planner planner {get;set;}
        public int UserId {get;set;}
        public int WeddingId {get;set;}
    }
}