﻿using System.ComponentModel.DataAnnotations;

namespace ShradhaGeneralBookStore.Models.ViewModel
{
    public class AddAuthorViewModel
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }
    }
}
