using System;
using System.Collections.Generic;
using System.Text;

namespace com.ocwebservice.model
{
	[Serializable]
	public class ValidationConstants
	{
		public static String ZipRegEx
		{
			get { return @"(\d{5}(-\d{4})?)"; }
        }
        public static String PostalCodeRegEx
        {
            get { return @"([A-Z][0-9][A-Z]( )?[0-9][A-Z][0-9])"; }
        }
        public static String ZipPostalRegEx
        {
            get { return ZipRegEx + "|" + PostalCodeRegEx; }
        }

		public static String BasicPhoneRegEx
		{
			get { return @"\d{3}-\d{3}-\d{4}"; }
        }

        public static String BasicPhoneFormat
        {
            get { return @"000-000-0000"; }
        }

		public static String EmailRegEx
		{
			get { return @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"; }
		}

		public static String EmailLengthRegEx
		{
			get { return MaxLenRegex(EmailMaxLength); }
		}

		public static int EmailMaxLength
		{
			get { return 255; }
        }

        public static String NameLengthRegEx
        {
            get { return MaxLenRegex(NameMaxLength); }
        }

        public static int NameMaxLength
        {
            get { return 64; }
        }

        public static String CompanyNameLengthRegEx
        {
            get { return MaxLenRegex(CompanyNameMaxLength); }
        }

        public static int CompanyNameMaxLength
        {
            get { return 64; }
        }

        public static String CommentLengthRegEx
        {
            get { return MaxLenRegex(CommentMaxLength); }
        }

        public static int CommentMaxLength
        {
            get { return 4000; }
        }

		public static String ValidationReplacementToken
		{
			get { return "{#}"; }
		}

		public static String NumericRegEx
		{
			get { return @"[0-9]*"; }
		}

		public static String MaxLenRegex(int maxLength)
		{
			return @"[\S\s]{0," + maxLength + "}";
		}
	}
}
