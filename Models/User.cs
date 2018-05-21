using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class User : BaseEntity
    {
        private List<Planner> _planners;
        // These must match your columns in your table database!
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Planner> Planners { get => _planners; set => _planners = value; }
        public User()
        {
            Planners = new List<Planner>();
        }
    }
}
