using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Utility
{
    public class Constants
    {
        public static int[] MALE_LEAVE_TYPES = { 1, 2, 4, 5 };
        public static int[] FEMALE_LEAVE_TYPES = { 1, 2, 3, 4, 5 };
        public static int ROLE_HR = 3;
        public static int ROLE_MANAGER = 5;
        public static int LEAVE_STATUS_APPROVED = 2;
        public static int LEAVE_STATUS_REJECTED = 3;
        public static int LEAVE_STATUS_PENDING = 1;
        public static int LEAVE_STATUS_CANCELLED = 4;
        public static decimal MEDICAL_ALLOWANCE = 1250; //15000 per year
        public static decimal CONVEYANCE_ALLOWANCE = 1600; //19200 per year
        public static decimal FOOD_ALLOWANCE = 1500; //18000 per year
        public static decimal PT = 0 ;//2196 per year -183 per month
        public static int PROJECT_BENCH_ID = 1;

        public static int SYSTEMROLE_MANAGER = 8;
        public static int PROJECTROLE_MANAGER = 5;
        public static int PROJECTROLE_TEAMLEAD = 6;
        public static int PROJECTROLE_TEAMMEMBER = 7;
        public static int SYSTEMROLE_TEAMLEAD = 9;
    }
}