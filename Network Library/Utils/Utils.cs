using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Utils
{
    private static Utils instance;

    public static Utils Instance
    {
        get
        {
            if (instance == null)
                instance = new Utils();
            return instance;
        }
    }

    public bool IsValidIPAddress(string ip)
    {
        IPAddress address;
        if (IPAddress.TryParse(ip, out address))
            return true;

        return false;
    }
}
