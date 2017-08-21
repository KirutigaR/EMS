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
        public static decimal MEDICAL_ALLOWANCE = 15000; //1250*12;
        public static decimal CONVEYANCE_ALLOWANCE = 19200; //1600*12;
        public static decimal FOOD_ALLOWANCE = 18000; //1500*12;
        public static decimal PT = 2196; //183*12;

        public static int[] male_leave_type = { 1, 2, 4, 5 };
        public static int[] female_leave_type = { 1, 2, 3, 4, 5 };
        public static int HR = 3;
        public static int Systemrole_Manager = 8;
        public static int Projectrole_Manager = 5;
        public static int Projectrole_TeamLeader = 6;
        public static int Systemrole_TeamLeader = 9;
    }
}