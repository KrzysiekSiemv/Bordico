using System.Diagnostics;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Server.Controllers;
using Server.Database;

public class FormController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public FormController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("Register")]
    public IActionResult Register()
    {
        return View(new User());
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(User user)
    {
        if (!ModelState.IsValid)
            return View(new User());

        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.PostAsJsonAsync("http://localhost:5274/api/Auth/register", user);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Zarejestrowano pomyślnie";
            return RedirectToAction("Login");
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Wystąpił błąd: {error}");

            return View(user);
        }
    }

    [HttpGet("Login")]
    public IActionResult Login()
    {
        return View(new LoginModel());
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromForm] LoginModel loginRequest)
    {
        if (!ModelState.IsValid)
            return View(new LoginModel());

        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.PostAsJsonAsync("http://localhost:5274/Api/Auth/Login", loginRequest);

        if (response.IsSuccessStatusCode)
        {
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (loginResponse == null) return NotFound();

            HttpContext.Session.SetString("Token", loginResponse.Token);

            return RedirectToAction("Index", "Home");
        }
        else
        {
            TempData["Error"] = "Niepoprawne dane do logowania!";
            Console.WriteLine(TempData["Error"]);
            return View(loginRequest);
        }
    }
}
public class LoginResponse()
{
    public string Token { get; set; } = "";
}