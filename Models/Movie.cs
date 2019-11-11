using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace MovieReview.Models
{
    public class Movie
    {
        // auto-implemented properties need to match the columns in your table
        // the [Key] attribute is used to mark the Model property being used for your table's Primary Key
        [Key]
        public int MovieId { get; set; }
        // MySQL VARCHAR and TEXT types can be represeted by a string
        public int UserId { get; set; }
        [Required]
        [MinLength(2)]

        public string Title { get; set; }
        [Required]
        [MinLength(2)]
        public string Actor1 { get; set; }
        [Required]
        [MinLength(2)]
        public string Actor2 { get; set; }
        public string Actor3 { get; set; }
        [Required]
        public string Director { get; set; }

        [Required]
        public int Year { get; set; }
        [Required]
        public string Category { get; set; }


        [Required]
        [MinLength(5)]
        public string Description { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public User Createdby { get; set; }
        public List<Review> MovieReview { get; set; }


    }
}