using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using UnityEngine;

namespace TeleopReachy
{
    public class IPUtils
    {
        //from https://learn.microsoft.com/en-us/dotnet/api/system.net.ipaddress.tryparse?view=net-8.0
        public static bool IsIPValid(string ipAddress)
        {
            try
            {
                if (ipAddress == "localhost" || ipAddress == Robot.VIRTUAL_ROBOT_IP)
                    return true;
                // Create an instance of IPAddress for the specified address string (in
                // dotted-quad, or colon-hexadecimal notation).
                IPAddress address = IPAddress.Parse(ipAddress);
                return true;
            }

            catch (ArgumentNullException e)
            {
                Debug.LogError("IP is invalid : " + e.Message);
                return false;
            }

            catch (FormatException e)
            {
                Debug.LogError("IP is invalid : " + e.Message);
                return false;
            }

            catch (Exception e)
            {
                Debug.LogError("IP is invalid : " + e.Message);
                return false;
            }
        }
    }
}
