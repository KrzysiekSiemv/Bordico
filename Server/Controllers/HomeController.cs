// using System.Diagnostics;
// using Microsoft.AspNetCore.Mvc;
// using Bordico.Server.Models;
// using Server.Database;
// using Microsoft.AspNetCore.Mvc.Filters;
// using Server.Helpers;
// using Microsoft.EntityFrameworkCore;
// using System.Threading.Tasks;
// using Bordico.Server.ViewModel;

// namespace Server.Controllers;

// public class HomeController : Controller
// {
//     private readonly ILogger<HomeController> _logger;
//     private readonly BordicoContext _context;
//     public HomeController(ILogger<HomeController> logger, BordicoContext context)
//     {
//         _logger = logger;
//         _context = context;
//     }

//     public async Task<IActionResult> Index()
//     {
//         User user = SessionHelper.GetLoggedInUser(HttpContext) ?? new User { id_user = 0 };
//         int loggedUser = user.id_user;

//         if (user.id_user == 0)
//             return RedirectToAction("Login", "Form");

//         List<Conversations> _conversations = [];

//         List<Conversation> conversations = await _context.conversations.Where(c => c.id_first_user == loggedUser || c.id_second_user == loggedUser).Select(
//             c => new Conversation
//             {
//                 id_conversation = c.id_conversation,
//                 id_first_user = c.id_first_user,
//                 id_second_user = c.id_second_user
//             }
//         ).ToListAsync();

//         if (conversations.Count > 0)
//         {
//             foreach (var conversation in conversations)
//             {
//                 int userId = conversation.id_first_user == loggedUser ? conversation.id_second_user : conversation.id_first_user;
//                 User anotherUser = await _context.users.Where(u => u.id_user == userId).Select(u => new User { nickname = u.nickname }).FirstOrDefaultAsync() ?? new User { nickname = "NotFound" }; 

//                 _conversations.Add(new Conversations
//                 {
//                     ConversationId = conversation.id_conversation,
//                     UserId = userId,
//                     UserName = anotherUser.nickname ?? "NotFound",
//                     Messages = []
//                 });
//             }
//         }

//         return View(new MainPageViewModel
//         {
//             UserData = user,
//             LoggedUserId = loggedUser,
//             ConversationList = _conversations
//         });
//     }

//     public IActionResult Privacy()
//     {
//         return View();
//     }

//     public IActionResult Registration()
//     {
//         return RedirectToAction("Register", "Form");
//     }

//     public IActionResult Login()
//     {
//         return RedirectToAction("Login", "Form");
//     }

//     [HttpGet]
//     public async Task<IActionResult> Profile(int id_user)
//     {
//         var currentUser = SessionHelper.GetLoggedInUser(HttpContext);
//         if (currentUser == null)
//             return RedirectToAction("Login");

//         User user = await _context.users.Where(u => u.id_user == id_user).Select(
//             u => new User
//             {
//                 id_user = u.id_user,
//                 nickname = u.nickname,
//                 username = u.username,
//                 email_address = u.email_address,
//                 description = u.description 
//             }
//         ).FirstOrDefaultAsync() ?? new User { id_user = 0 };
//         return View(user);
//     }

//     public async Task<IActionResult> AllUsers()
//     {
//         var currentUser = SessionHelper.GetLoggedInUser(HttpContext);
//         if (currentUser == null)
//             return RedirectToAction("Login", "Form");

//         List<User> users = await _context.users.Select(
//             u => new User
//             {
//                 id_user = u.id_user,
//                 nickname = u.nickname
//             }
//         ).ToListAsync();

//         return View(users);
//     }

//     public override void OnActionExecuting(ActionExecutingContext context)
//     {
//         var user = SessionHelper.GetLoggedInUser(HttpContext);
//         ViewBag.CurrentUser = user;
//         base.OnActionExecuting(context);
//     }

//     [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//     public IActionResult Error()
//     {
//         return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//     }
// }
