using CExternal;
using Swed64;
using System.Numerics;
using System.Runtime.InteropServices;

//Memory för CS2
Swed swed = new("cs2");

// Call för Client.dll bas
IntPtr client = swed.GetModuleBase("client.dll");

// init ImGui and overlay
Renderer renderer = new Renderer();
renderer.Start().Wait();

// get entity list & handling
List<Entity> entities = new List<Entity>(); // alla spelare
Entity localPlayer = new Entity(); // Local Spelare

// consts
const int HOTKEY = 0x06; // value för hotkey Microsoft DOCS (Virtual Key codes)

// Aimbot Loop
while (true)
{
    entities.Clear();
    Console.Clear();

    IntPtr entityList = swed.ReadPointer(client, Offsets.dwEntityList);

    IntPtr listEntry = swed.ReadPointer(entityList, 0x10);
    
    // Uppdaterar localplayer information
    localPlayer.pawnAdress = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn);
    localPlayer.team = swed.ReadInt(localPlayer.pawnAdress, Offsets.m_iTeamNum);
    localPlayer.origin = swed.ReadVec(localPlayer.pawnAdress, Offsets.m_vOldOrigin);
    localPlayer.view = swed.ReadVec(localPlayer.pawnAdress, Offsets.m_vecViewOffset);
        
    for (int i = 0; i < 64; i++)
    {
        if (listEntry == IntPtr.Zero)
            continue;

        IntPtr currentController = swed.ReadPointer(listEntry, i * 0x78);

        if (currentController == IntPtr.Zero)
            continue;

        int pawnHandle = swed.ReadInt(currentController, Offsets.m_hPlayerPawn);

        if (pawnHandle == 0)
            continue;

        IntPtr listEntry2 = swed.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);

        IntPtr currentPawn = swed.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));

        if (currentPawn == localPlayer.pawnAdress)
              continue;

        int health = swed.ReadInt(currentPawn, Offsets.m_iHealth);
        int team = swed.ReadInt(currentPawn, Offsets.m_iTeamNum);
        uint lifeState = swed.ReadUInt(currentPawn, Offsets.m_lifeState);

        if (lifeState != 256)
            continue;
        if (team == localPlayer.team && renderer.aimOnTeam)
            continue;

        Entity entity = new Entity();

        entity.pawnAdress = currentPawn;
        entity.controllerAddress = currentController;
        entity.health = health;
        entity.lifeState = lifeState;
        entity.origin = swed.ReadVec(currentPawn, Offsets.m_vOldOrigin);
        entity.view = swed.ReadVec(currentPawn, Offsets.m_vecViewOffset);
        entity.distance = Vector3.Distance(entity.origin, localPlayer.origin);

        entities.Add(entity);

        Console.ForegroundColor = ConsoleColor.Green;

        if (team != localPlayer.team)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }

        Console.WriteLine($"{entity.health}hp, distance: {(int)(entity.distance) / 100}m");

        Console.ResetColor();
    }

    entities = entities.OrderBy(o => o.distance).ToList();

    if (entities.Count > 0 && GetAsyncKeyState(HOTKEY) <0 && renderer.aimbot)
    {
        Vector3 playerView = Vector3.Add(localPlayer.origin, localPlayer.view);
        Vector3 entityView = Vector3.Add(entities[0].origin, entities[0].view);

        Vector2 newAngles = Calculate.calculateAngles(playerView, entityView);
        Vector3 NewAnglesVec3 = new Vector3(newAngles.Y, newAngles.X, 0.0f);

        swed.WriteVec(client, Offsets.dwViewAngles, NewAnglesVec3);
    }

    Thread.Sleep(20);
}

// Imports
[DllImport("user32.dll")]
static extern short GetAsyncKeyState(int vKey);