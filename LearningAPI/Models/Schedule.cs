﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace LearningAPI.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        
        [Required, MinLength(6), MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required, MinLength(6), MaxLength(100)]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set;}

        // Foreign key property
        public int UserId { get; set; }
        // Navigation property to represent the one-to-many relationship
        public User? User { get; set; }
    }
}
