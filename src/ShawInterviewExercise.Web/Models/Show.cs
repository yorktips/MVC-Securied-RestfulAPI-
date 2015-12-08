using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShawInterviewExercise.Web.Models
{

    public partial class Show
    {
        private DateTime _ShowDate = DateTime.MinValue;
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required"), StringLength(24, MinimumLength = 3)]
        public string Name { get; set; }


        public string Description { get; set; }

        [Required(ErrorMessage = "Description is required"), StringLength(255, MinimumLength = 3)]
        public string Title { get; set; }
        public string ImageGuid { get; set; }
        public byte[] Image { get; set; }

        [DisplayName("Image File")]
        public string ImageFile { get; set; }
        public string VideoUrl { get; set; }
        public bool Enabled { get; set; }

        [DataType(DataType.Date), Display(Name = "Effective Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ShowDate
        {
            get
            {
                return (_ShowDate == DateTime.MinValue) ? DateTime.Now : _ShowDate;
            }
            set { _ShowDate = value; }
        }

        public string Memo { get; set; }
        public System.DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }

    public partial class TblShow
    {
        private DateTime _ShowDate = DateTime.MinValue;

        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required"), StringLength(24, MinimumLength = 3)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Title is required"), StringLength(255, MinimumLength = 3)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Image is required"), StringLength(255, MinimumLength = 3)]
        public string FileLocation { get; set; }

        public byte[] ImageData { get; set; }
        public HttpPostedFileBase Image { get; set; }
        public string Location { get; set; }
        public string VideoUrl { get; set; }
        public bool Enabled { get; set; }

        [DataType(DataType.Date), Display(Name = "Effective Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ShowDate
        {
            get
            {
                return (_ShowDate == DateTime.MinValue) ? DateTime.Now : _ShowDate;
            }
            set { _ShowDate = value; }
        }

        public string Memo { get; set; }
    }
}
