using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CExternal
{
    public static class Offsets
    {
        // Offsets.cs
        public static int dwViewAngles = 0x1A29DA0;
        public static int dwLocalPlayerPawn = 0x1825138;
        public static int dwEntityList = 0x19BDE30;

        // client.dll.cs
        public static int m_hPlayerPawn = 0x80C;
        public static int m_iHealth = 0x344;
        public static int m_vOldOrigin = 0x131C;
        public static int m_iTeamNum = 0x3E3;
        public static int m_vecViewOffset = 0xCA8;
        public static int m_lifeState = 0x348;
    }
}
