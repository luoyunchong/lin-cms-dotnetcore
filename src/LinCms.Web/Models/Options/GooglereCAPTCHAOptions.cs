using Owl.reCAPTCHA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinCms.Models.Options
{
    public class GooglereCAPTCHAOptions : reCAPTCHAOptions
    {
        public static string RecaptchaSettings = "RecaptchaSettings";

        public string HeaderKey { get; set; } = "Google-RecaptchaToken";
        public bool Enabled { get; set; } = false;
        public string Version { get; set; } = reCAPTCHAConsts.V3;
        public float MinimumScore { get; set; } = 0.9F;
    }
}
