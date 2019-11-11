using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace MovieReview.Models
{
    public class Review
    {
        // auto-implemented properties need to match the columns in your table
        // the [Key] attribute is used to mark the Model property being used for your table's Primary Key
        [Key]
        public int ReviewId { get; set; }
        public int MovieId { get; set; }
        // MySQL VARCHAR and TEXT types can be represeted by a string
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string Description { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public Movie ReviewMovie { get; set; }
        public User ReviewUser { get; set; }



    }
}