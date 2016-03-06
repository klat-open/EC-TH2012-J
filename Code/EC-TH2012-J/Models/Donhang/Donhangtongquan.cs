using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EC_TH2012_J.Models
{
    public class Donhangtongquan
    {
        public string address { get; set; }
        [Required(ErrorMessage = "Vui lòng cung cấp số điện thoại", AllowEmptyStrings = false)]
        [RegularExpression(@"(09)\d{8}|(01)\d{9}", ErrorMessage = "Entered phone format is not valid.")]
        [Display(Name = "Điện thoại liên lạc")]
        [DataType(DataType.PhoneNumber)]
        public string phoneNumber { get; set; }
        public string buyer { get; set; }
        public string seller { get; set; }
        public string Note { get; set; }
    }
}