﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShawInterviewExercise.Web.Models
{
    public static class ImageHelper
    {
        public static System.Web.Mvc.MvcHtmlString Image(this HtmlHelper helper, string src, string altText, string height)
        {
            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", src);
            builder.MergeAttribute("alt", altText);

            if ( Convert.ToInt32(height) > 0 )
                builder.MergeAttribute("height", height);
            
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }
    }
}