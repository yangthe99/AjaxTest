﻿namespace AjaxTest.Models
{
    public class userDTO
    {
        public string? userName {  get; set; }
        public string? userEmail {  get; set; }
        public int? userAge {  get; set; }
        public IFormFile? userPhoto { get; set; }
        public string userPassword { get; set; }
        public string userPasswordConfirm { get; set; }


    }
}
