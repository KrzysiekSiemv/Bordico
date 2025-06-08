using System.Text;
using System.Text.Json;
using Server.Database;

namespace Server.Helpers;

public static class SessionHelper
{
    public static User? GetLoggedInUser(HttpContext context)
    {
        var sessionData = context.Session.Get("UserData");
        if (sessionData == null) return null;

        return JsonSerializer.Deserialize<User>(Encoding.UTF8.GetString(sessionData));
    }
}
