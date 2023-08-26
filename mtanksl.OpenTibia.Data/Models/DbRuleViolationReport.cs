﻿using System;
using System.ComponentModel.DataAnnotations;

namespace OpenTibia.Data.Models
{
    public class DbRuleViolationReport
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string Comment { get; set; }

        [StringLength(255)]
        public string Translation { get; set; }

        public DateTime CreationDate { get; set; }


        public DbPlayer Player { get; set; }
    }
}