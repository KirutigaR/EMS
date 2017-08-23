﻿using System;
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
        public static decimal MEDICAL_ALLOWANCE = 1250; //15000 per year
        public static decimal CONVEYANCE_ALLOWANCE = 1600; //19200 per year
        public static decimal FOOD_ALLOWANCE = 1500; //18000 per year
        public static decimal PT = 183 ;//2196 per year

        public static int[] male_leave_type = { 1, 2, 4, 5 };
        public static int[] female_leave_type = { 1, 2, 3, 4, 5 };
        public static int HR = 3;
        public static int Systemrole_Manager = 8;
        public static int Projectrole_Manager = 5;
        public static int Projectrole_TeamLeader = 6;
        public static int Systemrole_TeamLeader = 9;
    }
}