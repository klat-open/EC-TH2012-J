using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using EC_TH2012_J.Models;
using EC_TH2012_J.Models.B2B;
using System.IO;
using PagedList;
using PagedList.Mvc;

namespace EC_TH2012_J.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        public bool Kiemtrataikhoan(string username, string pass)
        {
            var user = UserManager.Find(username, pass);
            if (user == null)
                return false;
            else
                return true;
        }
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("Login");
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    ManagerObiect.getIntance().userName = model.UserName;
                    if(UserManager.GetRoles(user.Id).FirstOrDefault() == "Nhà cung cấp")
                    {
                        return RedirectToLocal("/Auction/index");
                    }
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return PartialView("Register");
        }

        [AllowAnonymous]
        public ActionResult RegisterB2B()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterB2B(Register2B2ViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.UserName, Email = model.Email, Avatar = "noavatar.jpg" };

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Nhà cung cấp");
                    NhaCungCapModel ncc = new NhaCungCapModel();
                    ncc.ThemNCC(model, user.Id);
                    await SignInAsync(user, isPersistent: false);
                    ManagerObiect.getIntance().userName = model.UserName;
                    return RedirectToLocal("/Auction/index");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.UserName, PhoneNumber = model.DienThoai, Email = model.Email, DiaChi = model.DiaChi, HoTen = model.HoTen, Avatar = "noavatar.jpg" };

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Khách hàng");
                    //UserManager.AddToRole(user.Id, "Nhà cung cấp");
                    await SignInAsync(user, isPersistent: false);
                    ManagerObiect.getIntance().userName = model.UserName;
                    SendMailConfirm(user.Id);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private void SendMailConfirm(string p)
        {
            UserModel us = new UserModel();
            us.SendMailConfirm(p, Url.Action("ConfirmMail", "Account", new { id = p }, this.Request.Url.Scheme));
            ////http://account/ConfirmMail/dbaf5ac1-9ac3-4fbb-a5a0-891ea80cd0e7
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Mật khẩu của bạn đã được đổi."
                : message == ManageMessageId.SetPasswordSuccess ? "Mật khẩu của bạn đã được thiết lập."
                : message == ManageMessageId.RemoveLoginSuccess ? "Kết nối tài khoản khác đã được loại bỏ."
                : message == ManageMessageId.Error ? "Lỗi xử lý."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Khách hàng");
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }
        public ActionResult LogOut()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
        [AllowAnonymous]
        public ActionResult Authentication(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult EditInfo()
        {
            UserModel user = new UserModel();
            EditInfoModel info = new EditInfoModel(user.FindById(User.Identity.GetUserId()));
            return View(info);
        }

        public ActionResult EditNCCInfo()
        {
            NhaCungCapModel ncc = new NhaCungCapModel();
            EditInfo2B2ViewModel info = new EditInfo2B2ViewModel(ncc.FindByNetUser(User.Identity.GetUserId()));
            return View(info);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditNCCInfo([Bind(Include = "MaNCC,TenNCC,DiaChi,SDT_NCC,Email")] EditInfo2B2ViewModel info)
        {
            if (ModelState.IsValid)
            {
                NhaCungCapModel ncc = new NhaCungCapModel();
                ncc.UpdateInfo(info);
                ViewBag.StatusMessage = "Cập nhật thông tin thành công";
            }
            return View(info);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditInfo([Bind(Include = "Email,DienThoai,CMND,HoTen,NgaySinh,GioiTinh,DiaChi")] EditInfoModel info)
        {
            if (ModelState.IsValid)
            {
                UserModel user = new UserModel();
                user.UpdateInfo(info, User.Identity.GetUserId());
                info.Avatar = user.FindById(User.Identity.GetUserId()).Avatar;
                ViewBag.StatusMessage = "Cập nhật thông tin thành công";
            }
            return View(info);
        }

        // This action handles the form POST and the upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAvatar(HttpPostedFileBase file)
        {
            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                var name = Path.GetExtension(file.FileName);
                // extract only the filename
                if (!Path.GetExtension(file.FileName).Equals(".jpg"))
                {
                    HttpStatusCodeResult status = new HttpStatusCodeResult(400);
                    return status;
                }
                // store the file inside ~/App_Data/uploads folder
                var path = Path.Combine(Server.MapPath("~/images/avatars"), User.Identity.GetUserId() + ".jpg");
                file.SaveAs(path);
                UserModel user = new UserModel();
                user.UpdateImage(User.Identity.GetUserId());
            }
            // redirect back to the index action to show the form once again
            return RedirectToAction("EditInfo");
        }

        [AllowAnonymous]
        public ActionResult ConfirmMail(string id)
        {
            UserModel us = new UserModel();
            if (us.ConfirmMail(id))
                return RedirectToAction("ThongBao", "Account", new { mesage = "Đã xác nhận mail thành công, bạn có thể đăng nhập và sử dụng" });
            return RedirectToAction("ThongBao", "Account", new { mesage = "Có lỗi trong quá trình xác nhận mail" });
        }

        [AllowAnonymous]
        public ActionResult ThongBao(string mesage)
        {
            ViewBag.mesage = mesage;
            return View();
        }


        //Phần back-end. Bao gồm thêm user mới, chỉnh quyền user, xem  thông tin user
        [AuthLog(Roles = "Quản trị viên")]
        public ActionResult Index()
        {
            UserModel us = new UserModel();
            ViewBag.Roleseach = new SelectList(us.GetAllRole(), "Id", "Name");
            ViewBag.Role = new SelectList(us.GetAllRole(), "Name", "Name");
            return View();
        }

        [AuthLog(Roles = "Quản trị viên")]
        [HttpPost]
        public ActionResult PhanQuyen(List<string> lstu, string quyen)
        {
            UserModel u = new UserModel();
            foreach (var item in lstu)
            {
                if (!item.Equals(User.Identity.GetUserId()))
                {
                    u.deleleallrole(item);
                    UserManager.AddToRole(item, quyen);
                }         
            }
            return TimUser(null, null, null, null, null, null);
        }

        [AuthLog(Roles = "Quản trị viên")]
        public ActionResult UserDetail(string id)
        {
            UserModel usm = new UserModel();
            return PartialView("UserDetail", usm.FindById(id));
        }

        [AuthLog(Roles = "Quản trị viên")]
        public ActionResult TimUser(string key, string email, string hoten, string phone, string quyen, int? page)
        {
            UserModel spm = new UserModel();
            ViewBag.key = key;
            ViewBag.email = email;
            ViewBag.hoten = hoten;
            ViewBag.phone = phone;
            ViewBag.quyen = quyen;
            return PhanTrangUser(spm.SearchUser(key, email, hoten, phone, quyen), page, null);
        }

        [AuthLog(Roles = "Quản trị viên")]
        public ActionResult PhanTrangUser(IQueryable<AspNetUser> lst, int? page, int? pagesize)
        {
            int pageSize = (pagesize ?? 10);
            int pageNumber = (page ?? 1);
            return PartialView("UserList", lst.OrderBy(m => m.HoTen).ToPagedList(pageNumber, pageSize));
        }

    }
}